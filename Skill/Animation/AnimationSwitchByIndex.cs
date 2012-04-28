using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationSwitchByIndex : AnimationSwitchBase
    {

        private int _SelectedChildIndex;

        public int SelectedChildIndex
        {
            get { return _SelectedChildIndex; }
            set
            {
                if (value < 0) value = 0;
                else if (value >= ChildCount) value = ChildCount - 1;
                _SelectedChildIndex = value;
            }
        }

        public string SelectedChild
        {
            get
            {
                if (_SelectedChildIndex < ChildCount)
                {
                    if (this[_SelectedChildIndex] != null)
                        return this[_SelectedChildIndex].Name;
                }
                return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        AnimationNode childNode = this[i];
                        if (childNode != null)
                        {
                            if (childNode.Name == value)
                            {
                                _SelectedChildIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }


        public AnimationSwitchByIndex(int childCount)
            : base(childCount)
        {

        }


        protected override int SelectActiveChildIndex()
        {
            return _SelectedChildIndex;
        }
    }
}
