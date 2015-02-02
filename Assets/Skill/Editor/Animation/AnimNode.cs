using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public enum AnimNodeType
    {
        None,
        Sequence,
        Override,
        BlendByIndex,
        Blend1D,
        Blend2D,
        Additive,
        Root,        
    }

    public abstract class AnimNodeData : IXmlElementSerializable
    {

        public AnimNodeData(string name)
        {
            this.Name = name;
            IsPublic = false;
        }

        public abstract AnimNodeType NodeType { get; }

        /// <summary> Whether code generator create a public property for this node</summary>
        public bool IsPublic { get; set; }

        /// <summary> Id of Animation Node </summary>
        public int Id { get; set; }

        /// <summary> Name animation node</summary>
        public string Name { get; set; }

        /// <summary> If true code generator create an method and hook it to BecameRelevant event </summary>                
        public bool BecameRelevant { get; set; }

        /// <summary> If true code generator create an method and hook it to CeaseRelevant event </summary>                
        public bool CeaseRelevant { get; set; }

        /// <summary> User comment for this Animation Node </summary>                        
        public string Comment { get; set; }

        /// <summary> Canvas.Left </summary>        
        public float X { get; set; }

        /// <summary> Canvas.Top </summary>        
        public float Y { get; set; }

        /// <summary> input slots </summary>
        public ConnectorData[] Inputs { get; set; }

        #region Save
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XmlElement e) { }

        /// <summary>
        /// Create a XmlElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XmlElement ToXmlElement()
        {
            XmlElement node = new XmlElement("AnimNode");
            node.SetAttributeValue("Name", Name);
            node.SetAttributeValue("Id", Id);
            node.SetAttributeValue("IsPublic", this.IsPublic);
            node.SetAttributeValue("NodeType", NodeType.ToString());

            if (this.Inputs != null)
            {
                XmlElement inputs = new XmlElement("Inputs");
                inputs.SetAttributeValue("Count", this.Inputs.Length);
                foreach (var item in this.Inputs)
                {
                    inputs.AppendChild(item.ToXmlElement());
                }
                node.AppendChild(inputs);
            }


            XmlElement ui = new XmlElement("UI");
            ui.SetAttributeValue("X", X);
            ui.SetAttributeValue("Y", Y);
            node.AppendChild(ui);

            XmlElement events = new XmlElement("Events");
            events.SetAttributeValue("CeaseRelevant", CeaseRelevant);
            events.SetAttributeValue("BecameRelevant", BecameRelevant);
            node.AppendChild(events);
            if (!string.IsNullOrEmpty(Comment))
            {
                XmlElement comment = new XmlElement("Comment");
                comment.Value = Comment;
                node.AppendChild(comment);
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
        protected virtual void ReadAttributes(XmlElement e) { }

        /// <summary>
        /// Load Animation Node data
        /// </summary>
        /// <param name="e">contains Animation Node data</param>
        public void Load(XmlElement e)
        {
            Name = e.GetAttributeValueAsString("Name", Name);
            Id = e.GetAttributeValueAsInt("Id", -1);
            this.IsPublic = e.GetAttributeValueAsBoolean("IsPublic", this.IsPublic);

            XmlElement inputs = e["Inputs"];
            if (inputs != null)
            {
                int count = inputs.GetAttributeValueAsInt("Count", 0);
                this.Inputs = new ConnectorData[count];
                int i = 0;
                foreach (var element in inputs)
                {
                    if (element.Name == "Input")
                    {
                        this.Inputs[i] = new ConnectorData();
                        this.Inputs[i].Load(element);
                        i++;
                    }
                }
            }

            XmlElement ui = e["UI"];
            if (ui != null)
            {
                X = ui.GetAttributeValueAsFloat("X", 0);
                Y = ui.GetAttributeValueAsFloat("Y", 0);
            }

            XmlElement events = e["Events"];
            if (events != null)
            {
                var ceaseRelevant = events.GetAttribute("CeaseRelevant");
                var becameRelevant = events.GetAttribute("BecameRelevant");

                if (ceaseRelevant != null) CeaseRelevant = bool.Parse(ceaseRelevant);
                if (becameRelevant != null) BecameRelevant = bool.Parse(becameRelevant);
            }

            XmlElement comment = e["Comment"];
            if (comment != null)
            {
                Comment = comment.Value;
            }
            ReadAttributes(e);// allow subclass to read additional data            
        }
        #endregion

        public abstract AnimNodeData Clone();
        public virtual void CopyFrom(AnimNodeData other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Id = other.Id;
            this.BecameRelevant = other.BecameRelevant;
            this.CeaseRelevant = other.CeaseRelevant;
            this.Comment = other.Comment;
            this.Name = (other.Name == null ? "" : other.Name);

            if (other.Inputs != null)
            {
                this.Inputs = new ConnectorData[other.Inputs.Length];
                for (int i = 0; i < other.Inputs.Length; i++)
                {
                    this.Inputs[i] = other.Inputs[i].Clone();
                }
            }
            else
                this.Inputs = new ConnectorData[0];
        }


        public UnityEngine.Texture2D GetIcon()
        {
            switch (NodeType)
            {
                case AnimNodeType.Sequence:
                    return Skill.Editor.Resources.UITextures.ATree.Sequence;
                case AnimNodeType.Override:
                    return Skill.Editor.Resources.UITextures.ATree.Override;
                case AnimNodeType.BlendByIndex:
                    return Skill.Editor.Resources.UITextures.ATree.ByIndex;
                case AnimNodeType.Blend1D:
                    return Skill.Editor.Resources.UITextures.ATree.OneD;
                case AnimNodeType.Blend2D:
                    return Skill.Editor.Resources.UITextures.ATree.TwoD;
                case AnimNodeType.Additive:
                    return Skill.Editor.Resources.UITextures.ATree.Additive;
                case AnimNodeType.Root:
                    return Skill.Editor.Resources.UITextures.ATree.Root;
                //case AnimNodeType.SubTree:
                //    return Skill.Editor.Resources.UITextures.ATree.SubTree;
            }
            return Skill.Editor.Resources.UITextures.ATree.Blank;
        }
    }

    public abstract class AnimNodeBlendBaseData: AnimNodeData
    {
        public bool NewLayer { get; set; }        

        public AnimNodeBlendBaseData(string name)
            : base(name)
        {
        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("NewLayer", NewLayer);            
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {

            this.NewLayer = e.GetAttributeValueAsBoolean("NewLayer", false);            
            base.ReadAttributes(e);
        }

        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeBlendBaseData s = other as AnimNodeBlendBaseData;
            this.NewLayer = s.NewLayer;
        }
    }
}
