using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region Action
    /// <summary>
    /// Defines an action node in behavior tree
    /// </summary>
    public class Action : Behavior
    {
        /// <summary> Type of node </summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Action; } }

        /// <summary> If true code generator create an method and hook it to reset event </summary>
        public bool ResetEvent { get; set; }

        /// <summary> used for debuging </summary>
        public float ExecutionTime { get; set; }

        /// <summary> use for simulation AnimationTree</summary>
        public int AnimationWrapMode { get; set; }
        /// <summary> use for simulation AnimationTree</summary>
        public string AnimationSource { get; set; }
        /// <summary> use for simulation AnimationTree</summary>
        public int AnimationFrameRate { get; set; }

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public Action()
            : base("NewAction")
        {
            ExecutionTime = 1;
            AnimationSource = "";
            AnimationWrapMode = 0;
            AnimationFrameRate = 30;
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("ResetEvent", ResetEvent);
            e.SetAttributeValue("ExecutionTime", ExecutionTime);

            XElement debug = new XElement("Debug");

            debug.SetAttributeValue("WrapMode", AnimationWrapMode);
            debug.SetAttributeValue("Source", AnimationSource);
            debug.SetAttributeValue("FrameRate", AnimationFrameRate);

            e.Add(debug);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            XElement debug = e.FindChildByName("Debug");
            if (debug != null)
            {
                AnimationWrapMode = debug.GetAttributeValueAsInt("WrapMode", 0);
                AnimationSource = debug.GetAttributeValueAsString("Source", "");
                AnimationFrameRate = debug.GetAttributeValueAsInt("FrameRate", 0);
            }

            this.ResetEvent = e.GetAttributeValueAsBoolean("ResetEvent", false);
            this.ExecutionTime = e.GetAttributeValueAsFloat("ExecutionTime", 1);
            base.ReadAttributes(e);
        }
    }
    #endregion
}
