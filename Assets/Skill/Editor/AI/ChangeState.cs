using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;


namespace Skill.Editor.AI
{
    #region ChangeState
    /// <summary>
    /// Defines an action node in behavior tree
    /// </summary>
    public class ChangeStateData : BehaviorData
    {        
        /// <summary> Type of node </summary>
        public override Skill.Framework.AI.BehaviorType BehaviorType { get { return Skill.Framework.AI.BehaviorType.ChangeState; } }

        public string DestinationState { get; set; }

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public ChangeStateData()
            : base("NewChangeState")
        {
            this.DestinationState = BehaviorTreeData.DefaultDestinationState;
        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("DestinationState", DestinationState);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            this.DestinationState = e.GetAttributeValueAsString("DestinationState", BehaviorTreeData.DefaultDestinationState);
            base.ReadAttributes(e);
        }
    }
    #endregion
}
