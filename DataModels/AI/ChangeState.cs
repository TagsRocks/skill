using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.AI
{
    #region ChangeState
    /// <summary>
    /// Defines an action node in behavior tree
    /// </summary>
    public class ChangeState : Behavior
    {        
        /// <summary> Type of node </summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.ChangeState; } }

        public string DestinationState { get; set; }

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public ChangeState()
            : base("NewChangeState")
        {
            this.DestinationState = BehaviorTree.DefaultDestinationState;
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("DestinationState", DestinationState);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            this.DestinationState = e.GetAttributeValueAsString("DestinationState", BehaviorTree.DefaultDestinationState);
            base.ReadAttributes(e);
        }
    }
    #endregion
}
