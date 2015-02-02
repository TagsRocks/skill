using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class BehaviorTreeStateItem : PrioritySelectorItem
    {
        public BehaviorTreeStateItem(BehaviorTreeStateData data)
            : base(data)
        {

        }        

        private bool _IsDefaultState;
        public bool IsDefaultState
        {
            get { return _IsDefaultState; }
            set
            {
                if (_IsDefaultState != value)
                {
                    _IsDefaultState = value;
                }
            }
        }
    }
}