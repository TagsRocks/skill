using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.AI
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
        //public int[] LoadedChildrenIds { get; private set; }
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
            //this.LoadedChildrenIds = ConvertToIndices(children);
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
        /// always start from first node
        /// </summary>
        HighestPriority,
        /// <summary>
        /// Continue running node
        /// </summary>
        RunningNode,

    }
    public class PrioritySelector : Composite
    {
        public override CompositeType CompositeType { get { return CompositeType.Priority; } }

        /// <summary> How to evaluate children </summary>
        public PriorityType Priority { get; set; }

        public PrioritySelector()
            : base("NewPrioritySelector")
        {
            Priority = PriorityType.HighestPriority;
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
}
