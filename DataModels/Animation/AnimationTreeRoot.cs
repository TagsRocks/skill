using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimationTreeRoot : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Root; } }

        public AnimationTreeRoot()
            : base("AnimationTree")
        {
            base.Inputs = new Connector[1];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Animation" };   
        }        

        public override AnimNode Clone()
        {
            AnimationTreeRoot node = new AnimationTreeRoot();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
        }        
    }
}
