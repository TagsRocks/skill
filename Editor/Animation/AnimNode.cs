using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using Skill.Editor.Diagram;
using System.Windows.Controls;

namespace Skill.Editor.Animation
{
    public enum AnimNodeType
    {
        None,
        Sequence,
        Override,
        BlendByIndex,
        BlendBySpeed,
        BlendByPosture,
        BlendByIdle,
        Blend4Directional,
        AimOffset,
        AdditiveBlending,
        Random,
        Root,
    }

    public abstract class AnimNode : IXElement
    {

        public AnimNode(string name)
        {
            this.Name = name;
            this.BlendTime = 0.3f;
            IsPublic = true;
        }

        public abstract AnimNodeType NodeType { get; }

        /// <summary> Whether code generator create a public property for this node</summary>
        public bool IsPublic { get; set; }

        /// <summary> Id of Animation Node </summary>
        public int Id { get; set; }

        /// <summary> Name animation node</summary>
        public string Name { get; set; }

        /// <summary> Blend Time of animation node. (not used by for AnimNodeSequence)</summary>
        public float BlendTime { get; set; }

        /// <summary> If true code generator create an method and hook it to BecameRelevant event </summary>        
        [Category("Events")]
        public bool BecameRelevant { get; set; }

        /// <summary> If true code generator create an method and hook it to CeaseRelevant event </summary>        
        [Category("Events")]
        public bool CeaseRelevant { get; set; }

        /// <summary> User comment for this Animation Node </summary>                        
        public string Comment { get; set; }

        /// <summary> Canvas.Left </summary>        
        public double X { get; set; }

        /// <summary> Canvas.Top </summary>        
        public double Y { get; set; }


        #region Save
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        /// <summary>
        /// Create a XElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement node = new XElement(NodeType.ToString());
            node.SetAttributeValue("Name", Name);
            node.SetAttributeValue("Id", Id);
            node.SetAttributeValue("BlendTime", this.BlendTime);
            node.SetAttributeValue("IsPublic", this.IsPublic);

            XElement ui = new XElement("UI");
            ui.SetAttributeValue("X", X);
            ui.SetAttributeValue("Y", Y);
            node.Add(ui);

            XElement events = new XElement("Events");
            events.SetAttributeValue("CeaseRelevant", CeaseRelevant);
            events.SetAttributeValue("BecameRelevant", BecameRelevant);
            node.Add(events);
            if (!string.IsNullOrEmpty(Comment))
            {
                XElement comment = new XElement("Comment");
                comment.SetValue(Comment);
                node.Add(comment);
            }
            WriteAttributes(node); // allow subclass to add additional data
            return node;
        }
        #endregion

        #region Load

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains Animation Node data</param>
        protected virtual void ReadAttributes(XElement e) { }

        /// <summary>
        /// Load Animation Node data
        /// </summary>
        /// <param name="e">contains Animation Node data</param>
        public void Load(XElement e)
        {
            Name = e.GetAttributeValueAsString("Name", Name);
            Id = e.GetAttributeValueAsInt("Id", -1);
            this.BlendTime = e.GetAttributeValueAsFloat("BlendTime", 0.3f);
            this.IsPublic = e.GetAttributeValueAsBoolean("IsPublic", this.IsPublic);

            XElement ui = e.FindChildByName("UI");
            if (ui != null)
            {
                X = ui.GetAttributeValueAsDouble("X", 0);
                Y = ui.GetAttributeValueAsDouble("Y", 0);
            }

            XElement events = e.FindChildByName("Events");
            if (events != null)
            {
                var ceaseRelevant = events.Attribute("CeaseRelevant");
                var becameRelevant = events.Attribute("BecameRelevant");

                if (ceaseRelevant != null) CeaseRelevant = bool.Parse(ceaseRelevant.Value);
                if (becameRelevant != null) BecameRelevant = bool.Parse(becameRelevant.Value);
            }

            XElement comment = e.FindChildByName("Comment");
            if (comment != null)
            {
                Comment = comment.Value;
            }
            ReadAttributes(e);// allow subclass to read additional data            
        }
        #endregion

        public abstract AnimNode Clone();

        public virtual void CopyFrom(AnimNode other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Id = other.Id;
            this.BecameRelevant = other.BecameRelevant;
            this.BlendTime = other.BlendTime;
            this.CeaseRelevant = other.CeaseRelevant;
            this.Comment = other.Comment;
            this.Name = (other.Name == null ? "" : other.Name);
        }
    }

    public abstract class AnimNodeProperties
    {
        [Browsable(false)]
        public AnimNodeViewModel ViewModel { get; private set; }

        public AnimNodeProperties(AnimNodeViewModel vm)
        {
            ViewModel = vm;
        }

        [Description("Name animation node and variable in code generation")]
        public virtual string Name
        {
            get { return ViewModel.Model.Name; }
            set
            {
                if (ViewModel.Model.Name != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, ViewModel.Model.Name));
                    }
                    ViewModel.Model.Name = value;
                    ViewModel.Header = value;

                }
            }
        }


        [Description("Blend Time of animation node.")]
        public virtual float BlendTime
        {
            get { return ViewModel.Model.BlendTime; }
            set
            {
                if (ViewModel.Model.BlendTime != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendTime", value, ViewModel.Model.BlendTime));
                    }
                    ViewModel.Model.BlendTime = value;
                }
            }
        }


        [Description("If true code generator create an method and hook it to BecameRelevant event")]
        [Category("Events")]
        public bool BecameRelevant
        {
            get { return ViewModel.Model.BecameRelevant; }
            set
            {
                if (ViewModel.Model.BecameRelevant != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BecameRelevant", value, ViewModel.Model.BecameRelevant));
                    }
                    ViewModel.Model.BecameRelevant = value;
                }
            }
        }

        [Description("If true code generator create an method and hook it to CeaseRelevant event")]
        [Category("Events")]
        public bool CeaseRelevant
        {
            get { return ViewModel.Model.CeaseRelevant; }
            set
            {
                if (ViewModel.Model.CeaseRelevant = value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "CeaseRelevant", value, ViewModel.Model.CeaseRelevant));
                    }
                    ViewModel.Model.CeaseRelevant = value;
                }
            }
        }

        [Description("User comment for this Animation Node")]
        public string Comment
        {
            get { return ViewModel.Model.Comment; }
            set
            {
                if (ViewModel.Model.Comment != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, ViewModel.Model.Comment));
                    }
                    ViewModel.Model.Comment = value;
                }
            }
        }


        private AnimationTreeEditor _Editor;
        [Browsable(false)]
        public AnimationTreeEditor Editor
        {
            get
            {
                if (_Editor == null)
                {
                    AnimationTreeCanvas canvas = AnimationTreeCanvas.GetDiagramCanvas(ViewModel) as AnimationTreeCanvas;
                    if (canvas != null)
                    {
                        _Editor = canvas.Editor;
                    }
                }
                return _Editor;
            }
        }

    }

    public class AnimNodeViewModel : DragableContent
    {

        #region Properties

        public virtual Connector Out { get { return null; } }

        public AnimNodeProperties Properties { get; protected set; }

        public AnimNode Model { get; private set; }

        public virtual void CommiteChangesToModel()
        {
            Model.X = DiagramCanvas.GetLeft(this);
            Model.Y = DiagramCanvas.GetTop(this);
        }
        #endregion

        #region Constructor
        public AnimNodeViewModel() // for designer
        {

        }
        public AnimNodeViewModel(AnimNode model)
        {
            this.Model = model;
        }
        public override void EndInit()
        {
            base.EndInit();
            if (Model != null)
                this.Header = Model.Name;
        }
        #endregion

        public override string ToString()
        {
            if (Model != null)
                return Model.Name;
            else
                return base.ToString();
        }
    }

}
