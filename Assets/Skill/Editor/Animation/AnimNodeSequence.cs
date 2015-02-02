using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;
using UnityEngine;

namespace Skill.Editor.Animation
{
    public class AnimNodeSequenceData : AnimNodeData
    {

        public AnimNodeSequenceData()
            : base("AnimNodeSequence")
        {
            this.Speed = 1;
            this.WrapMode = WrapMode.Default;
            this.IsPublic = false;
            this.UseTreeProfile = true;
            this.Sync = false;
            base.Inputs = new ConnectorData[0];
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Sequence; } }

        public string AnimationName { get; set; }

        public bool UseTreeProfile { get; set; }
        public bool Sync { get; set; }

        /// <summary> Speed at which the animation will be played back.Default is 1.0 </summary>        
        public float Speed { get; set; }

        public WrapMode WrapMode { get; set; }



        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement data = new XmlElement("SequenceData");

            data.SetAttributeValue("AnimationName", string.IsNullOrEmpty(this.AnimationName) ? "" : this.AnimationName);
            data.SetAttributeValue("Speed", this.Speed);
            data.SetAttributeValue("WrapMode", (int)this.WrapMode);
            data.SetAttributeValue("UseTreeProfile", this.UseTreeProfile);
            data.SetAttributeValue("Sync", this.Sync);


            e.AppendChild(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["SequenceData"];
            if (data != null)
            {
                this.AnimationName = data.GetAttributeValueAsString("AnimationName", "");
                this.Speed = data.GetAttributeValueAsFloat("Speed", 1);
                this.WrapMode = (WrapMode)data.GetAttributeValueAsInt("WrapMode", (int)WrapMode.Default);
                this.UseTreeProfile = data.GetAttributeValueAsBoolean("UseTreeProfile", true);
                this.Sync = data.GetAttributeValueAsBoolean("Sync", false);
            }
            base.ReadAttributes(e);
        }

        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeSequenceData sequence = other as AnimNodeSequenceData;
            this.AnimationName = sequence.AnimationName;
            this.Speed = sequence.Speed;
            this.WrapMode = sequence.WrapMode;
            this.UseTreeProfile = sequence.UseTreeProfile;
            this.Sync = sequence.Sync;
        }

        public override AnimNodeData Clone()
        {
            AnimNodeSequenceData node = new AnimNodeSequenceData();
            node.CopyFrom(this);
            return node;
        }
    }
}
