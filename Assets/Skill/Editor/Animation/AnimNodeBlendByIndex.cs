using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimNodeBlendByIndexData : AnimNodeBlendBaseData
    {
        public AnimNodeBlendByIndexData()
            : base("AnimNodeBlendByIndex")
        {
            this.BlendTime = 0.3f;
            base.Inputs = new ConnectorData[2];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Index0" };
            base.Inputs[1] = new ConnectorData() { Index = 1, Name = "Index1" };
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByIndex; } }

        /// <summary> Blend Time of animation node.</summary>
        public float BlendTime { get; set; }

        public string Parameter { get; set; }

        /// <summary> name of enumerator based on input names.</summary>
        public string Enum { get; set; }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement data = new XmlElement("BlendByIndexData");
            data.SetAttributeValue("BlendTime", this.BlendTime);
            data.SetAttributeValue("Enum", this.Enum);
            if (this.Parameter == null) this.Parameter = string.Empty;
            data.SetAttributeValue("Parameter", this.Parameter);
            e.AppendChild(data);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement data = e["BlendByIndexData"];
            if (data != null)
            {
                this.BlendTime = data.GetAttributeValueAsFloat("BlendTime", 0.3f);
                this.Enum = data.GetAttributeValueAsString("Enum", string.Empty);
                this.Parameter = data.GetAttributeValueAsString("Parameter", string.Empty);
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            this.BlendTime = ((AnimNodeBlendByIndexData)other).BlendTime;
            this.Enum = (string)((AnimNodeBlendByIndexData)other).Enum.Clone();
            this.Parameter = (string)((AnimNodeBlendByIndexData)other).Parameter.Clone();
            base.CopyFrom(other);
        }

        public override AnimNodeData Clone()
        {
            AnimNodeBlendByIndexData node = new AnimNodeBlendByIndexData();
            node.CopyFrom(this);
            return node;
        }
    }
}
