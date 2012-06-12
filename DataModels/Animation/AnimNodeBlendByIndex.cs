using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimNodeBlendByIndex : AnimNode
    {
        public AnimNodeBlendByIndex()
            : base("AnimNodeBlendByIndex")
        {
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Index0" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Index1" };            
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByIndex; } }

        protected override void WriteAttributes(XElement e)
        {
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            //AnimNodeBlendByIndex s = other as AnimNodeBlendByIndex;            
        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByIndex node = new AnimNodeBlendByIndex();
            node.CopyFrom(this);
            return node;
        }
    }
}
