using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlend2DData : AnimNodeBlendBaseData
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Blend2D; } }

        public Vector2Data[] Thresholds { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }

        public AnimNodeBlend2DData()
            : base("Blend2D")
        {
            base.Inputs = new ConnectorData[2];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Input1" };
            base.Inputs[1] = new ConnectorData() { Index = 1, Name = "Input2" };
        }

        protected override void WriteAttributes(XmlElement e)
        {

            XmlElement data = new XmlElement("Blend2DData");

            if (this.Parameter1 == null) this.Parameter1 = string.Empty;
            if (this.Parameter2 == null) this.Parameter2 = string.Empty;
            data.SetAttributeValue("Parameter1", this.Parameter1);
            data.SetAttributeValue("Parameter2", this.Parameter2);

            XmlElement constraints = new XmlElement("Thresholds");
            constraints.SetAttributeValue("Count", Thresholds.Length);
            foreach (var item in Thresholds)
            {
                XmlElement f = item.ToXmlElement();
                f.Value = item.ToString();
                constraints.AppendChild(f);
            }
            data.AppendChild(constraints);
            e.AppendChild(data);

            base.WriteAttributes(e);
        }
        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["Blend2DData"];
            if (data != null)
            {
                this.Parameter1 = data.GetAttributeValueAsString("Parameter1", string.Empty);
                this.Parameter2 = data.GetAttributeValueAsString("Parameter2", string.Empty);

                XmlElement constraints = data["Thresholds"];
                if (constraints != null)
                {
                    int count = constraints.GetAttributeValueAsInt("Count", 0);
                    Thresholds = new Vector2Data[count];
                    int i = 0;
                    foreach (var element in constraints)
                    {
                        Thresholds[i] = new Vector2Data();
                        Thresholds[i].Load(element);
                        i++;
                    }
                }
            }
            base.ReadAttributes(e);
        }
        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeBlend2DData s = other as AnimNodeBlend2DData;
            this.Thresholds = (Vector2Data[])s.Thresholds.Clone();
            this.Parameter1 = s.Parameter1;
            this.Parameter2 = s.Parameter2;
        }
        public override AnimNodeData Clone()
        {
            AnimNodeBlend2DData node = new AnimNodeBlend2DData();
            node.CopyFrom(this);
            return node;
        }
    }
}
