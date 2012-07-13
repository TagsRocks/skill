using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public Action()
            : base("NewAction")
        {
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("ResetEvent", ResetEvent);
            base.WriteAttributes(e);            
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            this.ResetEvent = e.GetAttributeValueAsBoolean("ResetEvent", false);
            base.ReadAttributes(e);
        }
    }
    #endregion
}
