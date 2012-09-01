using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public enum WrapMode
    {
        Default = 0,
        Once = 1,
        Loop = 2,
        PingPong = 4,
        ClampForever = 8,
    }

    public class AnimNodeSequence : AnimNode
    {

        public AnimNodeSequence()
            : base("AnimNodeSequence")
        {
            this.Speed = 1;
            this.WrapMode = WrapMode.Default;
            this.IsPublic = false;
            this.UseTreeProfile = true;
            this.Synchronize = false;
            this.RootMotion = new RootMotionState();

            base.Inputs = new Connector[1];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Null" };
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Sequence; } }

        public string AnimationName { get; set; }

        public string[] MixingTransforms { get; set; }

        public bool UseTreeProfile { get; set; }

        public bool Synchronize { get; set; }

        /// <summary> Speed at which the animation will be played back.Default is 1.0 </summary>        
        public float Speed { get; set; }

        /// <summary> Length of Animation </summary>        
        public float Length { get; set; }

        public WrapMode WrapMode { get; set; }

        public RootMotionState RootMotion { get; set; }

        protected override void WriteAttributes(XElement e)
        {
            XElement data = new XElement("SequenceData");

            data.SetAttributeValue("AnimationName", string.IsNullOrEmpty(this.AnimationName) ? "" : this.AnimationName);
            data.SetAttributeValue("Speed", this.Speed);
            data.SetAttributeValue("Length", this.Length);
            data.SetAttributeValue("WrapMode", (int)this.WrapMode);
            data.SetAttributeValue("UseTreeProfile", this.UseTreeProfile);
            data.SetAttributeValue("Synchronize", this.Synchronize);

            if (MixingTransforms != null && MixingTransforms.Length > 0)
            {
                XElement mixingTransforms = new XElement("MixingTransforms");
                mixingTransforms.SetAttributeValue("Count", MixingTransforms.Length);
                foreach (var item in MixingTransforms)
                {
                    XElement transform = new XElement("Transform");
                    transform.Value = item;
                    mixingTransforms.Add(transform);
                }

                data.Add(mixingTransforms);
            }

            data.Add(RootMotion.ToXElement());

            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("SequenceData");
            if (data != null)
            {
                this.AnimationName = data.GetAttributeValueAsString("AnimationName", "");
                this.Speed = data.GetAttributeValueAsFloat("Speed", 1);
                this.Length = data.GetAttributeValueAsFloat("Length", 0);
                this.WrapMode = (WrapMode)data.GetAttributeValueAsInt("WrapMode", (int)WrapMode.Default);
                this.UseTreeProfile = data.GetAttributeValueAsBoolean("UseTreeProfile", true);
                this.Synchronize = data.GetAttributeValueAsBoolean("Synchronize", false);

                XElement mixingTransforms = data.FindChildByName("MixingTransforms");
                if (mixingTransforms != null)
                {
                    int count = mixingTransforms.GetAttributeValueAsInt("Count", 0);
                    int i = 0;
                    MixingTransforms = new string[count];
                    foreach (var transform in mixingTransforms.Elements())
                    {
                        MixingTransforms[i++] = transform.Value;
                    }
                }

                XElement rootmotion = data.FindChildByName(RootMotionState.ElementName);
                if (rootmotion != null)
                    this.RootMotion.Load(rootmotion);

            }
            base.ReadAttributes(e);
        }

        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeSequence sequence = other as AnimNodeSequence;
            this.AnimationName = sequence.AnimationName;
            this.Speed = sequence.Speed;
            this.Length = sequence.Length;
            this.WrapMode = sequence.WrapMode;
            this.UseTreeProfile = sequence.UseTreeProfile;
        }

        public override AnimNode Clone()
        {
            AnimNodeSequence node = new AnimNodeSequence();
            node.CopyFrom(this);
            return node;
        }
    }
}
