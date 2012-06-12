using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeBlendBySpeed : AnimNode
    {
        public AnimNodeBlendBySpeed()
            : base("AnimNodeBlendBySpeed")
        {
            Constraints = new float[] { 0, 1, 2 };
            this.BlendUpTime = BlendTime;
            this.BlendDownTime = BlendTime;
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Input1" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Input2" };
        }


        public override AnimNodeType NodeType { get { return AnimNodeType.BlendBySpeed; } }

        public float BlendUpTime { get; set; }

        public float BlendDownTime { get; set; }

        public float BlendUpDelay { get; set; }

        public float BlendDownDelay { get; set; }

        // Where abouts in the constraint bounds should the blend start blending down.
        public float BlendDownPercent { get; set; }

        public float[] Constraints { get; set; }

        protected override void WriteAttributes(XElement e)
        {

            XElement data = new XElement("BlendBySpeedData");
            data.SetAttributeValue("BlendUpTime", this.BlendUpTime);
            data.SetAttributeValue("BlendDownTime", this.BlendDownTime);
            data.SetAttributeValue("BlendUpDelay", this.BlendUpDelay);
            data.SetAttributeValue("BlendDownDelay", this.BlendDownDelay);
            data.SetAttributeValue("BlendDownPercent", this.BlendDownPercent);

            XElement constraints = new XElement("Constraints");
            constraints.SetAttributeValue("Count", Constraints.Length);
            foreach (var item in Constraints)
            {
                XElement f = new XElement("Float");
                f.Value = item.ToString();
                constraints.Add(f);
            }
            data.Add(constraints);
            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("BlendBySpeedData");
            if (data != null)
            {
                this.BlendUpTime = data.GetAttributeValueAsFloat("BlendUpTime", 0.3f);
                this.BlendDownTime = data.GetAttributeValueAsFloat("BlendDownTime", 0.3f);
                this.BlendUpDelay = data.GetAttributeValueAsFloat("BlendUpDelay", 0.0f);
                this.BlendDownDelay = data.GetAttributeValueAsFloat("BlendDownDelay", 0.0f);
                this.BlendDownPercent = data.GetAttributeValueAsFloat("BlendDownPercent", 0.0f);

                XElement constraints = data.FindChildByName("Constraints");
                if (constraints != null)
                {
                    int count = constraints.GetAttributeValueAsInt("Count", 0);
                    Constraints = new float[count];
                    int i = 0;
                    foreach (var element in constraints.Elements().Where(p => p.Name == "Float"))
                    {
                        Constraints[i] = float.Parse(element.Value);
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
            AnimNodeBlendBySpeed s = other as AnimNodeBlendBySpeed;
            this.BlendDownDelay = s.BlendDownDelay;
            this.BlendDownTime = s.BlendDownTime;
            this.BlendUpDelay = s.BlendUpDelay;
            this.BlendUpTime = s.BlendUpTime;
            this.BlendDownPercent = s.BlendDownPercent;
            this.Constraints = (float[])s.Constraints.Clone();
        }

        public override AnimNode Clone()
        {
            AnimNodeBlendBySpeed node = new AnimNodeBlendBySpeed();
            node.CopyFrom(this);
            return node;
        }
    }

}
