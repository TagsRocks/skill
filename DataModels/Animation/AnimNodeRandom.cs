using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeRandom : AnimNode
    {
        public AnimNodeRandom()
            : base("AnimNodeRandom")
        {
            Chances = new float[] { 1.0f, 1.0f };
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Input1" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Input2" };
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Random; } }

        public float[] Chances { get;  set; }

        protected override void WriteAttributes(XElement e)
        {

            XElement data = new XElement("RandomData");

            XElement chances = new XElement("Chances");
            chances.SetAttributeValue("Count", Chances.Length);
            foreach (var item in Chances)
            {
                XElement f = new XElement("Float");
                f.Value = item.ToString();
                chances.Add(f);
            }
            data.Add(chances);
            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("RandomData");
            if (data != null)
            {
                XElement chances = data.FindChildByName("Chances");
                if (chances != null)
                {
                    int count = chances.GetAttributeValueAsInt("Count", 0);
                    Chances = new float[count];
                    int i = 0;
                    foreach (var element in chances.Elements().Where(p => p.Name == "Float"))
                    {
                        Chances[i] = float.Parse(element.Value);
                        i++;
                    }
                }
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeRandom s = other as AnimNodeRandom;
            this.Chances = (float[])s.Chances.Clone();
        }

        public override AnimNode Clone()
        {
            AnimNodeRandom node = new AnimNodeRandom();
            node.CopyFrom(this);
            return node;
        }
    }
}
