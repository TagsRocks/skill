using UnityEngine;
using System.Collections;


namespace Skill.Editor.Animation
{
    public class AnimationTreeRootItem : AnimNodeItem
    {

        protected override bool HasOutput { get { return false; } }
        public AnimationTreeRootItem(AnimationTreeRootData data)
            : base(data)
        {

        }        
    }
}
