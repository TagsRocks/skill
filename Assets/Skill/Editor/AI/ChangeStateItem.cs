using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class ChangeStateItem : TreeViewItem
    {
        public ChangeStateItem(ChangeStateData data)
            : base(data)
        {

        }        

        public override string Name2
        {
            get
            {
                return base.Name2;
            }
            set
            {
                base.Name2 = value;
                Editor.RefreshStateNames();
            }
        }
    }
}
