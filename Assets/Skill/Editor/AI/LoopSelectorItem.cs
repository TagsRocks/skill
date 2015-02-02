using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class LoopSelectorItem : CompositeItem
    {

        public LoopSelectorItem(LoopSelectorData data)
            : base(data)
        {

        }
        

        [Skill.Framework.ExposeProperty(41, "LoopCount", "Number of loops (-1 for infinit)")]
        public int LoopCount
        {
            get { return ((LoopSelectorData)Data).LoopCount; }
            set
            {
                if (value != ((LoopSelectorData)Data).LoopCount)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "LoopCount", value, ((LoopSelector)Model).LoopCount));
                    ((LoopSelectorData)Data).LoopCount = value;
                    //((Skill.Framework.AI.LoopSelector)Debug.Behavior).LoopCount = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("LoopCount"));                
                }
            }
        }
    }
}