using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.Animation
{
    public class AnimNodeBlendByIdle : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByIdle; } }

        public AnimNodeBlendByIdle()
            : base("AnimNodeBlendByIdle")
        {
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Moving" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Idle" };
        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByIdle node = new AnimNodeBlendByIdle();
            node.CopyFrom(this);
            return node;
        }
    }
}
