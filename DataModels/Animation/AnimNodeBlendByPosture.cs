using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.Animation
{
    public class AnimNodeBlendByPosture : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.BlendByPosture; } }
        public AnimNodeBlendByPosture()
            : base("AnimNodeBlendByPosture")
        {
            base.Inputs = new Connector[3];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Standing" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Crouched" };
            base.Inputs[2] = new Connector() { Index = 2, Name = "Prone" };
        }

        public override AnimNode Clone()
        {
            AnimNodeBlendByPosture node = new AnimNodeBlendByPosture();
            node.CopyFrom(this);
            return node;
        }
    }
}
