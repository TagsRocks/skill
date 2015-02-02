using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class PrioritySelectorItem : CompositeItem
    {

        public PrioritySelectorItem(PrioritySelectorData data)
            : base(data)
        {

        }        


        [Skill.Framework.ExposeProperty(41, "Priority Type", "How to evaluate children")]
        public Skill.Framework.AI.PriorityType Priority
        {
            get { return ((PrioritySelectorData)Data).Priority; }
            set
            {
                if (value != ((PrioritySelectorData)Data).Priority)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Priority", value, ((PrioritySelector)Model).Priority));
                    ((PrioritySelectorData)Data).Priority = value;
                    //((Skill.Framework.AI.PrioritySelector)Debug.Behavior).Priority = (Skill.Framework.AI.PriorityType)value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("Priority"));                
                }
            }
        }
    }
}