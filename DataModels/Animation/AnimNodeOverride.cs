using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeOverride : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Override; } }

        public float OverridePeriod { get; set; }

        public AnimNodeOverride()
            : base("AnimNodeOverride")
        {
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Normal" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Override" };
        }

        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("OverrideData");
            data.SetAttributeValue("OverridePeriod", this.OverridePeriod);
            e.Add(data);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("OverrideData");
            if (data != null)
            {
                this.OverridePeriod = data.GetAttributeValueAsFloat("OverridePeriod", 0.0f);
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeOverride s = other as AnimNodeOverride;
            this.OverridePeriod = s.OverridePeriod;
        }


        public override AnimNode Clone()
        {
            AnimNodeOverride node = new AnimNodeOverride();
            node.CopyFrom(this);
            return node;
        }
    }
}
