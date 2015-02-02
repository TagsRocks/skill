using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class ConcurrentSelectorItem : CompositeItem
    {

        public ConcurrentSelectorItem(ConcurrentSelectorData data)
            : base(data)
        {

        }        

        [Skill.Framework.ExposeProperty(41, "BreakOnConditionFailure", "if true : when one condition child failed, return failure")]
        public bool BreakOnConditionFailure
        {
            get { return ((ConcurrentSelectorData)Data).BreakOnConditionFailure; }
            set
            {
                if (value != ((ConcurrentSelectorData)Data).BreakOnConditionFailure)
                {
                    ((ConcurrentSelectorData)Data).BreakOnConditionFailure = value;
                    //((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).BreakOnConditionFailure = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("BreakOnConditionFailure"));
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "BreakOnConditionFailure", value, !value));                
                }
            }
        }

        [Skill.Framework.ExposeProperty(42, "FailurePolicy", "policy of failure")]
        public Skill.Framework.AI.FailurePolicy FailurePolicy
        {
            get { return ((ConcurrentSelectorData)Data).FailurePolicy; }
            set
            {
                if (value != ((ConcurrentSelectorData)Data).FailurePolicy)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "FailurePolicy", value, ((ConcurrentSelector)Model).FailurePolicy));
                    ((ConcurrentSelectorData)Data).FailurePolicy = value;
                    //((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).FailurePolicy = (Skill.Framework.AI.FailurePolicy)value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("FailurePolicy"));                
                }
            }
        }

        [Skill.Framework.ExposeProperty(43, "SuccessPolicy", "policy of success")]
        public Skill.Framework.AI.SuccessPolicy SuccessPolicy
        {
            get { return ((ConcurrentSelectorData)Data).SuccessPolicy; }
            set
            {
                if (value != ((ConcurrentSelectorData)Data).SuccessPolicy)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "SuccessPolicy", value, ((ConcurrentSelector)Model).SuccessPolicy));
                    ((ConcurrentSelectorData)Data).SuccessPolicy = value;
                    //((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).SuccessPolicy = (Skill.Framework.AI.SuccessPolicy)value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("SuccessPolicy"));                
                }
            }
        }
    }
}
