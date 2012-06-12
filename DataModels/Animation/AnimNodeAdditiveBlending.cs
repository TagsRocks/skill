using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.Animation
{
    public class AnimNodeAdditiveBlending : AnimNode
    {
        public override AnimNodeType NodeType { get { return AnimNodeType.AdditiveBlending; } }

        public AnimNodeAdditiveBlending()
            : base("AnimNodeAdditiveBlending")
        {
            base.Inputs = new Connector[2];
            base.Inputs[0] = new Connector() { Index = 0, Name = "Normal" };
            base.Inputs[1] = new Connector() { Index = 1, Name = "Additive" };            
        }

        public override AnimNode Clone()
        {
            AnimNodeAdditiveBlending node = new AnimNodeAdditiveBlending();
            node.CopyFrom(this);
            return node;
        }
    }
}
