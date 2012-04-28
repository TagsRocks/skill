using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Skill.Editor.AI
{
    #region CompositeType
    /// <summary>
    /// Defines type of selectors
    /// </summary>
    public enum CompositeType
    {
        Sequence,
        Concurrent,
        Random,
        Priority,
        Loop,
    }
    #endregion

    #region Composite
    /// <summary>
    /// Defines base class for composit nodes in behavior tree
    /// </summary>
    public abstract class Composite : Behavior
    {
        #region Properties
        public override BehaviorType BehaviorType { get { return BehaviorType.Composite; } }

        public abstract CompositeType CompositeType { get; }

        /// <summary> when selector loaded from file read this value from file. this value is not valid until selector loaded from file </summary>
        public int[] LoadedChildrenIds { get; private set; }
        #endregion

        #region Constructor
        public Composite(string name)
            : base(name)
        {
        }
        #endregion

        #region Load & Save methods
        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("CompositeType", this.CompositeType.ToString());
            e.SetAttributeValue("Children", GetChildrenString());
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            string children = e.Attribute("Children").Value;
            this.LoadedChildrenIds = ConvertToIndices(children);
            base.ReadAttributes(e);
        }
        #endregion
    }
    #endregion

    #region SequenceSelector
    public class SequenceSelector : Composite
    {
        public override CompositeType CompositeType { get { return CompositeType.Sequence; } }

        public SequenceSelector()
            : base("NewSequenceSelector")
        {

        }
    }
    #endregion

    #region ConcurrentSelector

    /// <summary>
    /// Enumerates the options for when a ConcurrentSelector is considered to have failed.
    /// </summary>
    /// <remarks> If FailOnOne and SuceedOnOne are both active and are both trigerred in the same time step, failure will take precedence. </remarks>
    public enum FailurePolicy
    {
        /// <summary>  indicates that the node will return failure as soon as one of its children fails.</summary>
        FailOnOne,
        /// <summary>  indicates that all of the node's children must fail before it returns failure.</summary>
        FailOnAll
    }

    /// <summary>
    /// Enumerates the options for when a ConcurrentSelector is considered to have succeeded.
    /// </summary>    
    public enum SuccessPolicy
    {
        /// <summary>
        /// indicates that the node will return success as soon as one of its children succeeds.
        /// </summary>
        SucceedOnOne,
        /// <summary>
        /// indicates that all of the node's children must succeed before it returns success.
        /// </summary>
        SucceedOnAll
    }

    public class ConcurrentSelector : Composite
    {
        /// <summary> first check conditions then rest of childs (default true)</summary>
        public bool FirstConditions { get; set; }

        /// <summary> if true : when a condition child failes return failure </summary>
        public bool BreakOnConditionFailure { get; set; }

        /// <summary> Type of selector </summary>
        public override CompositeType CompositeType { get { return CompositeType.Concurrent; } }

        /// <summary> Policy of Failure</summary>
        public FailurePolicy FailurePolicy { get; set; }

        /// <summary> Policy of Success</summary>
        public SuccessPolicy SuccessPolicy { get; set; }

        /// <summary>
        /// Create an instance of ConcurrentSelector
        /// </summary>
        public ConcurrentSelector()
            : base("NewConcurrentSelector")
        {
            FailurePolicy = AI.FailurePolicy.FailOnAll;
            SuccessPolicy = AI.SuccessPolicy.SucceedOnAll;
            FirstConditions = true;
            BreakOnConditionFailure = false;
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement concurrentData = new System.Xml.Linq.XElement("ConcurrentProperties");
            concurrentData.SetAttributeValue("FirstConditions", FirstConditions);
            concurrentData.SetAttributeValue("BreakOnConditionFailure", BreakOnConditionFailure);
            concurrentData.SetAttributeValue("FailurePolicy", FailurePolicy);
            concurrentData.SetAttributeValue("SuccessPolicy", SuccessPolicy);
            e.Add(concurrentData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement concurrentData = FindChild(e, "ConcurrentProperties");
            if (concurrentData != null)
            {
                var firstConditions = concurrentData.Attribute("FirstConditions");
                var breakOnConditionFailure = concurrentData.Attribute("BreakOnConditionFailure");
                var failurePolicy = concurrentData.Attribute("FailurePolicy");
                var successPolicy = concurrentData.Attribute("SuccessPolicy");

                if (firstConditions != null) FirstConditions = bool.Parse(firstConditions.Value);
                if (breakOnConditionFailure != null) BreakOnConditionFailure = bool.Parse(breakOnConditionFailure.Value);
                if (failurePolicy != null) FailurePolicy = (FailurePolicy)Enum.Parse(typeof(FailurePolicy), failurePolicy.Value);
                if (successPolicy != null) SuccessPolicy = (SuccessPolicy)Enum.Parse(typeof(SuccessPolicy), successPolicy.Value);
            }
            base.ReadAttributes(e);
        }


    }
    #endregion

    #region RandomSelector
    public class RandomSelector : Composite
    {
        public override CompositeType CompositeType { get { return CompositeType.Random; } }
        public RandomSelector()
            : base("NewRandomSelector")
        {

        }
    }
    #endregion

    #region PrioritySelector
    /// <summary>
    /// Defines type of priority
    /// </summary>
    public enum PriorityType
    {
        /// <summary>
        /// Continue running node
        /// </summary>
        RunningNode,
        /// <summary>
        /// always start from first node
        /// </summary>
        HighestPriority,
    }
    public class PrioritySelector : Composite
    {
        public override CompositeType CompositeType { get { return CompositeType.Priority; } }

        /// <summary> How to evaluate children </summary>
        public PriorityType Priority { get; set; }

        public PrioritySelector()
            : base("NewPrioritySelector")
        {

        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement priorityData = new System.Xml.Linq.XElement("PriorityProperties");
            priorityData.SetAttributeValue("Priority", Priority);
            e.Add(priorityData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement priorityData = FindChild(e, "PriorityProperties");
            if (priorityData != null)
            {
                var priority = priorityData.Attribute("Priority");
                if (priority != null) Priority = (PriorityType)Enum.Parse(typeof(PriorityType), priority.Value);
            }
            base.ReadAttributes(e);
        }
    }
    #endregion

    #region LoopSelector
    public class LoopSelector : Composite
    {
        public override CompositeType CompositeType { get { return CompositeType.Loop; } }

        /// <summary> Number of loop (-1 for infinit)</summary>
        public int LoopCount { get; set; }

        public LoopSelector()
            : base("NewLoopSelector")
        {

        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement loopData = new System.Xml.Linq.XElement("LoopProperties");
            loopData.SetAttributeValue("LoopCount", LoopCount);
            e.Add(loopData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            System.Xml.Linq.XElement loopData = FindChild(e, "LoopProperties");
            if (loopData != null)
            {
                var loopCount = loopData.Attribute("LoopCount");
                if (loopCount != null) LoopCount = int.Parse(loopCount.Value);
            }
            base.ReadAttributes(e);
        }
    }
    #endregion



    #region SelectorViewModel
    public abstract class CompositeViewModel : BehaviorViewModel
    {
        public CompositeViewModel(BehaviorViewModel parent, Skill.Editor.AI.Composite selector)
            : base(parent, selector)
        {
        }
        protected CompositeViewModel(BehaviorTreeViewModel tree, Skill.Editor.AI.Composite selector)
            : base(tree, selector)
        {
        }
        public Skill.Editor.AI.CompositeType CompositeType { get { return ((Skill.Editor.AI.Composite)Model).CompositeType; } }
    }
    #endregion

    #region SequenceSelectorViewModel
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
    public class ConcurrentSelectorViewModel : CompositeViewModel
    {
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
                    OnPropertyChanged(new PropertyChangedEventArgs("FirstConditions"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "FirstConditions", value, !value));
                }
            }
        }

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
                    OnPropertyChanged(new PropertyChangedEventArgs("BreakOnConditionFailure"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "BreakOnConditionFailure", value, !value));
                }
            }
        }

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
                    OnPropertyChanged(new PropertyChangedEventArgs("FailurePolicy"));
                }
            }
        }


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
    public class PrioritySelectorViewModel : CompositeViewModel
    {

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
    public class LoopSelectorViewModel : CompositeViewModel
    {

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
