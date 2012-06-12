using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
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
        Root
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
        public bool BecameRelevant { get; set; }

        /// <summary> If true code generator create an method and hook it to CeaseRelevant event </summary>                
        public bool CeaseRelevant { get; set; }

        /// <summary> User comment for this Animation Node </summary>                        
        public string Comment { get; set; }

        /// <summary> Canvas.Left </summary>        
        public double X { get; set; }

        /// <summary> Canvas.Top </summary>        
        public double Y { get; set; }

        /// <summary> input slots </summary>
        public Connector[] Inputs { get; set; }

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

            if (this.Inputs != null)
            {
                XElement inputs = new XElement("Inputs");
                inputs.SetAttributeValue("Count", this.Inputs.Length);
                foreach (var item in this.Inputs)
                {
                    inputs.Add(item.ToXElement());
                }
                node.Add(inputs);
            }


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

            XElement inputs = e.FindChildByName("Inputs");
            if (inputs != null)
            {
                int count = inputs.GetAttributeValueAsInt("Count", 0);
                this.Inputs = new Connector[count];
                int i = 0;
                foreach (var element in inputs.Elements().Where(p => p.Name == "Input"))
                {
                    this.Inputs[i] = new Connector();
                    this.Inputs[i].Load(element);
                    i++;
                }
            }

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

            if (other.Inputs != null)
            {
                this.Inputs = new Connector[other.Inputs.Length];
                for (int i = 0; i < other.Inputs.Length; i++)
                {
                    this.Inputs[i] = other.Inputs[i].Clone();
                }
            }
            else
                this.Inputs = new Connector[0];
        }
    }
}
