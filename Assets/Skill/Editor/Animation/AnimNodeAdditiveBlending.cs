using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimNodeAdditiveBlendingData : AnimNodeBlendBaseData
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Additive; } }
        public string Parameter { get; set; }

        public AnimNodeAdditiveBlendingData()
            : base("AnimNodeAdditiveBlending")
        {
            base.Inputs = new ConnectorData[2];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Base" };
            base.Inputs[1] = new ConnectorData() { Index = 1, Name = "Additive" };
        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement data = new XmlElement("AdditiveData");
            if (this.Parameter == null) this.Parameter = string.Empty;
            data.SetAttributeValue("Parameter", this.Parameter);
            e.AppendChild(data);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["AdditiveData"];
            if (data != null)
                this.Parameter = data.GetAttributeValueAsString("Parameter", string.Empty);
            base.ReadAttributes(e);
        }


        public override AnimNodeData Clone()
        {
            AnimNodeAdditiveBlendingData node = new AnimNodeAdditiveBlendingData();
            node.CopyFrom(this);            
            return node;
        }

        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeAdditiveBlendingData s = other as AnimNodeAdditiveBlendingData;
            this.Parameter = s.Parameter;            
        }        
    }
}
