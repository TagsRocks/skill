using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimationTreeRootData : AnimNodeData
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Root; } }

        public AnimationTreeRootData()
            : base("AnimationTree")
        {
            base.Inputs = new ConnectorData[1];
            base.Inputs[0] = new ConnectorData() { Index = 0, Name = "Animation" };   
        }        

        public override AnimNodeData Clone()
        {
            AnimationTreeRootData node = new AnimationTreeRootData();
            node.CopyFrom(this);
            return node;
        }

        public override void CopyFrom(AnimNodeData other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
        }        
    }
}
