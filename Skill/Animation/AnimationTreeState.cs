using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Contains data to send throw AnimNodes at update
    /// </summary>
    public class AnimationTreeState
    {
        /// <summary>
        /// AnimationTree
        /// </summary>
        public AnimationTree Tree { get; private set; }

        /// <summary>
        /// The updating controller
        /// </summary>
        public Controllers.Controller Controller { get; internal set; }

        /// <summary>
        /// Create an instance of AnimationTreeState
        /// </summary>
        /// <param name="tree">AnimationTree</param>
        public AnimationTreeState(AnimationTree tree)
        {
            this.Tree = tree;
        }
    }
}
