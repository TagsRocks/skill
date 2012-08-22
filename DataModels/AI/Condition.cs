using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region Condition
    /// <summary>
    /// Defines condition in behavior tree
    /// </summary>
    public class Condition : Behavior
    {
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Condition; } }

        /// <summary> use for simulation AnimationTree</summary>
        public bool IsValid { get; set; }

        public Condition()
            : base("NewCondition")
        {

        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            XElement debug = e.FindChildByName("Debug");
            if (debug != null)
            {
                IsValid = debug.GetAttributeValueAsBoolean("IsValid", false);
            }            
            base.ReadAttributes(e);
        }

        protected override void WriteAttributes(XElement e)
        {
            XElement debug = new XElement("Debug");
            debug.SetAttributeValue("IsValid", IsValid);
            e.Add(debug);            
            base.WriteAttributes(e);
        }
    }
    #endregion
}
