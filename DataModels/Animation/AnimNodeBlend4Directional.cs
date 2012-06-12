using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.Animation
{
    public class AnimNodeBlend4Directional : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.Blend4Directional; } }

        public AnimNodeBlend4Directional()
            : base("AnimNodeBlend4Directional")
        {
            base.Inputs = new Connector[4];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Foreward" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Backward" };
            base.Inputs[2] = new Connector() { Index = 2, Name = "Left" };
            base.Inputs[3] = new Connector() { Index = 3, Name = "Right" };
        }

        public override AnimNode Clone()
        {
            AnimNodeBlend4Directional node = new AnimNodeBlend4Directional();
            node.CopyFrom(this);
            return node;
        }
    }
}
