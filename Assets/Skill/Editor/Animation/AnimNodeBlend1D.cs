using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlend1DData : AnimNodeBlendBaseData
    {
        public AnimNodeBlend1DData()
            : base("Blend1D")
        {
            Thresholds = new float[] { 0, 1 };            
            base.Inputs = new ConnectorData[2];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Input1" };
            base.Inputs[1] = new ConnectorData() { Index = 1, Name = "Input2" };
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Blend1D; } }                
        public float[] Thresholds { get; set; }
        public string Parameter { get; set; }
        protected override void WriteAttributes(XmlElement e)
        {

            XmlElement data = new XmlElement("Blend1DData");

            if (this.Parameter == null) this.Parameter = string.Empty;
            data.SetAttributeValue("Parameter", this.Parameter);

            XmlElement thresholds = new XmlElement("Thresholds");
            thresholds.SetAttributeValue("Count", Thresholds.Length);
            foreach (var item in Thresholds)
            {
                XmlElement f = new XmlElement("Float");
                f.Value = item.ToString();
                thresholds.AppendChild(f);
            }
            data.AppendChild(thresholds);
            e.AppendChild(data);

            base.WriteAttributes(e);
        }
        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["Blend1DData"];
            if (data != null)
            {
                this.Parameter = data.GetAttributeValueAsString("Parameter", string.Empty);
                XmlElement constraints = data["Thresholds"];
                if (constraints != null)
                {
                    int count = constraints.GetAttributeValueAsInt("Count", 0);
                    Thresholds = new float[count];
                    int i = 0;
                    foreach (var element in constraints)
                    {
                        if (element.Name == "Float")
                        {
                            Thresholds[i] = float.Parse(element.Value);
                            i++;
                        }
                    }
                }

            }
            base.ReadAttributes(e);
        }
        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeBlend1DData s = other as AnimNodeBlend1DData;
            this.Thresholds = (float[])s.Thresholds.Clone();
            this.Parameter = s.Parameter;
        }
        public override AnimNodeData Clone()
        {
            AnimNodeBlend1DData node = new AnimNodeBlend1DData();
            node.CopyFrom(this);
            return node;
        }
    }

}
