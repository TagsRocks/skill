using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimNodeOverrideData : AnimNodeBlendBaseData
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Override; } }
        public string Parameter { get; set; }

        public AnimNodeOverrideData()
            : base("AnimNodeOverride")
        {
            base.Inputs = new ConnectorData[2];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Base" };
            base.Inputs[1] = new ConnectorData() { Index = 1, Name = "Override" };
        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement data = new XmlElement("OverrideData");
            if (this.Parameter == null) this.Parameter = string.Empty;
            data.SetAttributeValue("Parameter", this.Parameter);
            e.AppendChild(data);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["OverrideData"];
            if (data != null)            
                this.Parameter = data.GetAttributeValueAsString("Parameter", string.Empty);            
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeOverrideData s = other as AnimNodeOverrideData;
            this.Parameter = s.Parameter;
        }


        public override AnimNodeData Clone()
        {
            AnimNodeOverrideData node = new AnimNodeOverrideData();
            node.CopyFrom(this);
            return node;
        }
    }
}
