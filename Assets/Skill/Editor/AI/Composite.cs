using System;
using System.Collections.Generic;
using Skill.Framework.IO;
using System.Text;

namespace Skill.Editor.AI
{    
    #region Composite
    /// <summary>
    /// Defines base class for composit nodes in behavior tree
    /// </summary>
    public abstract class CompositeData : BehaviorData
    {
        #region Properties
        public override Skill.Framework.AI.BehaviorType BehaviorType { get { return Skill.Framework.AI.BehaviorType.Composite; } }

        public abstract Skill.Framework.AI.CompositeType CompositeType { get; }

        /// <summary> when selector loaded from file read this value from file. this value is not valid until selector loaded from file </summary>
        //public int[] LoadedChildrenIds { get; private set; }
        #endregion

        #region Constructor
        public CompositeData(string name)
            : base(name)
        {
        }
        #endregion

        #region Load & Save methods
        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("CompositeType", this.CompositeType.ToString());
            e.SetAttributeValue("Children", GetChildrenString());
            base.WriteAttributes(e);
        }

        //protected override void ReadAttributes(XmlElement e)
        //{
        //    //string children = e.GetAttribute("Children");
        //    base.ReadAttributes(e);
        //}
        #endregion
    }
    #endregion

    #region SequenceSelector
    public class SequenceSelectorData : CompositeData
    {
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.Sequence; } }

        public SequenceSelectorData()
            : base("NewSequenceSelector")
        {

        }
    }
    #endregion

    #region ConcurrentSelector    

    public class ConcurrentSelectorData : CompositeData
    {
        /// <summary> first check conditions then rest of childs (default true)</summary>
        //public bool FirstConditions { get; set; }

        /// <summary> if true : when a condition child failes return failure </summary>
        public bool BreakOnConditionFailure { get; set; }

        /// <summary> Type of selector </summary>
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.Concurrent; } }

        /// <summary> Policy of Failure</summary>
        public Skill.Framework.AI.FailurePolicy FailurePolicy { get; set; }

        /// <summary> Policy of Success</summary>
        public Skill.Framework.AI.SuccessPolicy SuccessPolicy { get; set; }

        /// <summary>
        /// Create an instance of ConcurrentSelector
        /// </summary>
        public ConcurrentSelectorData()
            : base("NewConcurrentSelector")
        {
            FailurePolicy = Skill.Framework.AI.FailurePolicy.FailOnAll;
            SuccessPolicy = Skill.Framework.AI.SuccessPolicy.SucceedOnAll;
            //FirstConditions = true;
            BreakOnConditionFailure = false;
        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement concurrentData = new XmlElement("ConcurrentProperties");
            //concurrentData.SetAttributeValue("FirstConditions", FirstConditions);
            concurrentData.SetAttributeValue("BreakOnConditionFailure", BreakOnConditionFailure);
            concurrentData.SetAttributeValue("FailurePolicy", FailurePolicy.ToString());
            concurrentData.SetAttributeValue("SuccessPolicy", SuccessPolicy.ToString());
            e.AppendChild(concurrentData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement concurrentData = e["ConcurrentProperties"];
            if (concurrentData != null)
            {
                BreakOnConditionFailure = concurrentData.GetAttributeValueAsBoolean("BreakOnConditionFailure", false);
                FailurePolicy = concurrentData.GetAttributeValueAsEnum("FailurePolicy", Skill.Framework.AI.FailurePolicy.FailOnAll);
                SuccessPolicy = concurrentData.GetAttributeValueAsEnum("SuccessPolicy", Skill.Framework.AI.SuccessPolicy.SucceedOnAll);
            }
            base.ReadAttributes(e);
        }


    }
    #endregion

    #region RandomSelector
    public class RandomSelectorData : CompositeData
    {
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.Random; } }
        public RandomSelectorData()
            : base("NewRandomSelector")
        {

        }
    }
    #endregion

    #region PrioritySelector
    

    public class PrioritySelectorData : CompositeData
    {
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.Priority; } }

        /// <summary> How to evaluate children </summary>
        public Skill.Framework.AI.PriorityType Priority { get; set; }

        public PrioritySelectorData()
            : base("NewPrioritySelector")
        {
            Priority = Skill.Framework.AI.PriorityType.HighestPriority;
        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement priorityData = new XmlElement("PriorityProperties");
            priorityData.SetAttributeValue("Priority", Priority.ToString());
            e.AppendChild(priorityData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement priorityData = e["PriorityProperties"];
            if (priorityData != null)
            {
                Priority = priorityData.GetAttributeValueAsEnum("Priority", Skill.Framework.AI.PriorityType.HighestPriority);
            }
            base.ReadAttributes(e);
        }
    }
    #endregion

    #region LoopSelector
    public class LoopSelectorData : CompositeData
    {
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.Loop; } }

        /// <summary> Number of loop (-1 for infinit)</summary>
        public int LoopCount { get; set; }

        public LoopSelectorData()
            : base("NewLoopSelector")
        {

        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement loopData = new XmlElement("LoopProperties");
            loopData.SetAttributeValue("LoopCount", LoopCount);
            e.AppendChild(loopData);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement loopData = e["LoopProperties"];
            if (loopData != null)
            {
                LoopCount = loopData.GetAttributeValueAsInt("LoopCount", 0);
            }
            base.ReadAttributes(e);
        }
    }
    #endregion

    #region BehaviorTreeState
    public class BehaviorTreeStateData : PrioritySelectorData
    {
        public override Skill.Framework.AI.CompositeType CompositeType { get { return Skill.Framework.AI.CompositeType.State; } }
    }
    #endregion
}
