using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.AI;

namespace Skill.Studio.AI
{

    #region CompositeViewModel
    public abstract class CompositeViewModel : BehaviorViewModel
    {
        public CompositeViewModel(BehaviorViewModel parent, Composite selector)
            : base(parent, selector)
        {
        }
        protected CompositeViewModel(BehaviorTreeViewModel tree, Composite selector)
            : base(tree, selector)
        {
        }

        [Browsable(false)]
        public CompositeType CompositeType { get { return ((Composite)Model).CompositeType; } }
    }
    #endregion

    #region SequenceSelectorViewModel
    [DisplayName("SequenceSelector")]
    public class SequenceSelectorViewModel : CompositeViewModel
    {
        public override string ImageName { get { return Images.Sequence; } }

        public SequenceSelectorViewModel(BehaviorViewModel parent, SequenceSelector selector)
            : base(parent, selector)
        {
        }
    }
    #endregion

    #region RandomSelectorViewModel
    [DisplayName("RandomSelector")]
    public class RandomSelectorViewModel : CompositeViewModel
    {
        public override string ImageName { get { return Images.Random; } }
        public RandomSelectorViewModel(BehaviorViewModel parent, RandomSelector selector)
            : base(parent, selector)
        {
        }
    }
    #endregion

    #region ConcurrentSelectorViewModel
    [DisplayName("ConcurrentSelector")]
    public class ConcurrentSelectorViewModel : CompositeViewModel
    {
        [DefaultValue(true)]
        [Category("Concurrency")]
        [DisplayName("FirstConditions")]
        [Description("First check conditions then rest of children")]
        public bool FirstConditions
        {
            get { return ((ConcurrentSelector)Model).FirstConditions; }
            set
            {
                if (value != ((ConcurrentSelector)Model).FirstConditions)
                {
                    ((ConcurrentSelector)Model).FirstConditions = value;
                    ((Skill.AI.ConcurrentSelector)Debug.Behavior).FirstConditions = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FirstConditions"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "FirstConditions", value, !value));
                }
            }
        }

        [DefaultValue(false)]
        [Category("Concurrency")]
        [DisplayName("BreakOnConditionFailure")]
        [Description("If true : when a condition child fails, return failure")]
        public bool BreakOnConditionFailure
        {
            get { return ((ConcurrentSelector)Model).BreakOnConditionFailure; }
            set
            {
                if (value != ((ConcurrentSelector)Model).BreakOnConditionFailure)
                {
                    ((ConcurrentSelector)Model).BreakOnConditionFailure = value;
                    ((Skill.AI.ConcurrentSelector)Debug.Behavior).BreakOnConditionFailure = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("BreakOnConditionFailure"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "BreakOnConditionFailure", value, !value));
                }
            }
        }

        [DefaultValue(FailurePolicy.FailOnOne)]
        [Category("Concurrency")]
        [DisplayName("FailurePolicy")]
        [Description("Policy of Failure")]
        public FailurePolicy FailurePolicy
        {
            get { return ((ConcurrentSelector)Model).FailurePolicy; }
            set
            {
                if (value != ((ConcurrentSelector)Model).FailurePolicy)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "FailurePolicy", value, ((ConcurrentSelector)Model).FailurePolicy));
                    ((ConcurrentSelector)Model).FailurePolicy = value;
                    ((Skill.AI.ConcurrentSelector)Debug.Behavior).FailurePolicy = (Skill.AI.FailurePolicy)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FailurePolicy"));
                }
            }
        }


        [DefaultValue(SuccessPolicy.SucceedOnAll)]
        [Category("Concurrency")]
        [DisplayName("SuccessPolicy")]
        [Description("Policy of Success")]
        public SuccessPolicy SuccessPolicy
        {
            get { return ((ConcurrentSelector)Model).SuccessPolicy; }
            set
            {
                if (value != ((ConcurrentSelector)Model).SuccessPolicy)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "SuccessPolicy", value, ((ConcurrentSelector)Model).SuccessPolicy));
                    ((ConcurrentSelector)Model).SuccessPolicy = value;
                    ((Skill.AI.ConcurrentSelector)Debug.Behavior).SuccessPolicy = (Skill.AI.SuccessPolicy)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SuccessPolicy"));
                }
            }
        }


        public override string ImageName { get { return Images.Concurrent; } }
        public ConcurrentSelectorViewModel(BehaviorViewModel parent, ConcurrentSelector selector)
            : base(parent, selector)
        {
        }
    }
    #endregion

    #region PrioritySelectorViewModel
    [DisplayName("PrioritySelector")]
    public class PrioritySelectorViewModel : CompositeViewModel
    {
        [DefaultValue(PriorityType.HighestPriority)]
        [Category("Priority")]
        [DisplayName("Priority Type")]
        [Description("How to evaluate children")]
        public PriorityType Priority
        {
            get { return ((PrioritySelector)Model).Priority; }
            set
            {
                if (value != ((PrioritySelector)Model).Priority)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Priority", value, ((PrioritySelector)Model).Priority));
                    ((PrioritySelector)Model).Priority = value;
                    ((Skill.AI.PrioritySelector)Debug.Behavior).Priority = (Skill.AI.PriorityType)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Priority"));
                }
            }
        }

        public override string ImageName { get { return Images.Priority; } }
        public PrioritySelectorViewModel(BehaviorViewModel parent, PrioritySelector selector)
            : base(parent, selector)
        {
        }
        public PrioritySelectorViewModel(BehaviorTreeViewModel tree, PrioritySelector selector)
            : base(tree, selector)
        {
        }
    }
    #endregion

    #region LoopSelectorViewModel
    [DisplayName("LoopSelector")]
    public class LoopSelectorViewModel : CompositeViewModel
    {
        [DefaultValue(1)]
        [Category("Loop")]
        [DisplayName("LoopCount")]
        [Description("Number of loops (-1 for infinit)")]
        public int LoopCount
        {
            get { return ((LoopSelector)Model).LoopCount; }
            set
            {
                if (value != ((LoopSelector)Model).LoopCount)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "LoopCount", value, ((LoopSelector)Model).LoopCount));
                    ((LoopSelector)Model).LoopCount = value;
                    ((Skill.AI.LoopSelector)Debug.Behavior).LoopCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LoopCount"));
                }
            }
        }

        public override string ImageName { get { return Images.Loop; } }
        public LoopSelectorViewModel(BehaviorViewModel parent, LoopSelector selector)
            : base(parent, selector)
        {
        }
    }
    #endregion
}
