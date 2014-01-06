﻿using System;
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
        public override double MinHeight { get { return 42; } }
    }
    #endregion

    #region SequenceSelectorViewModel
    [DisplayName("SequenceSelector")]
    public class SequenceSelectorViewModel : CompositeViewModel
    {
        [Browsable(false)]
        public override string ImageName { get { return Images.Sequence; } }

        public SequenceSelectorViewModel(BehaviorViewModel parent, SequenceSelector selector)
            : base(parent, selector)
        {
        }
        public SequenceSelectorViewModel(BehaviorTreeViewModel tree, SequenceSelector selector)
            : base(tree, selector)
        {
        }
    }
    #endregion

    #region RandomSelectorViewModel
    [DisplayName("RandomSelector")]
    public class RandomSelectorViewModel : CompositeViewModel
    {
        [Browsable(false)]
        public override string ImageName { get { return Images.Random; } }

        public RandomSelectorViewModel(BehaviorViewModel parent, RandomSelector selector)
            : base(parent, selector)
        {
        }
        public RandomSelectorViewModel(BehaviorTreeViewModel tree, RandomSelector selector)
            : base(tree, selector)
        {
        }
    }
    #endregion

    #region ConcurrentSelectorViewModel
    [DisplayName("ConcurrentSelector")]
    public class ConcurrentSelectorViewModel : CompositeViewModel
    {
        [DefaultValue(false)]
        [Category("Concurrency")]
        [DisplayName("BreakOnConditionFailure")]
        [Description("If true : when one condition child failed, return failure")]
        public bool BreakOnConditionFailure
        {
            get { return ((ConcurrentSelector)Model).BreakOnConditionFailure; }
            set
            {
                if (value != ((ConcurrentSelector)Model).BreakOnConditionFailure)
                {
                    ((ConcurrentSelector)Model).BreakOnConditionFailure = value;
                    ((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).BreakOnConditionFailure = value;
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
                    ((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).FailurePolicy = (Skill.Framework.AI.FailurePolicy)value;
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
                    ((Skill.Framework.AI.ConcurrentSelector)Debug.Behavior).SuccessPolicy = (Skill.Framework.AI.SuccessPolicy)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SuccessPolicy"));
                }
            }
        }

        [Browsable(false)]
        public override string ImageName { get { return Images.Concurrent; } }

        public ConcurrentSelectorViewModel(BehaviorViewModel parent, ConcurrentSelector selector)
            : base(parent, selector)
        {
        }
        public ConcurrentSelectorViewModel(BehaviorTreeViewModel tree, ConcurrentSelector selector)
            : base(tree, selector)
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
                    ((Skill.Framework.AI.PrioritySelector)Debug.Behavior).Priority = (Skill.Framework.AI.PriorityType)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Priority"));
                }
            }
        }

        [Browsable(false)]
        public override string ImageName { get { return Images.Priority; } }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (base.Name != value)
                {
                    string preName = base.Name;
                    base.Name = value;
                    Tree.NotifyChangeDestinationState(preName, value);
                }
            }
        }

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
                    ((Skill.Framework.AI.LoopSelector)Debug.Behavior).LoopCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LoopCount"));
                }
            }
        }

        [Browsable(false)]
        public override string ImageName { get { return Images.Loop; } }

        public LoopSelectorViewModel(BehaviorViewModel parent, LoopSelector selector)
            : base(parent, selector)
        {
        }
        public LoopSelectorViewModel(BehaviorTreeViewModel tree, LoopSelector selector)
            : base(tree, selector)
        {
        }
    }
    #endregion

    #region BehaviorTreeStateViewModel
    [DisplayName("BehaviorTreeState")]
    public class BehaviorTreeStateViewModel : PrioritySelectorViewModel
    {

        [DefaultValue(false)]
        [DisplayName("Expand Methods")]
        [Description("Used by code generation to decide how implement methods")]
        public bool ExpandMethods
        {
            get { return ((BehaviorTree)Tree.Model).ExpandMethods; }
            set
            {
                if (value != ((BehaviorTree)Tree.Model).ExpandMethods)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "ExpandMethods", value, ((BehaviorTree)Tree.Model).ExpandMethods));
                    ((BehaviorTree)Tree.Model).ExpandMethods = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ExpandMethods"));
                }
            }
        }

        public BehaviorTreeStateViewModel(BehaviorViewModel parent, BehaviorTreeState state)
            : base(parent, state)
        {
        }
        public BehaviorTreeStateViewModel(BehaviorTreeViewModel tree, BehaviorTreeState state)
            : base(tree, state)
        {
        }
    }
    #endregion
}
