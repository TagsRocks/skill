using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;


namespace Skill.Editor.AI
{
    #region Condition
    /// <summary>
    /// Defines condition in behavior tree
    /// </summary>
    public class ConditionData : BehaviorData, IParameterData
    {
        public override Skill.Framework.AI.BehaviorType BehaviorType { get { return Skill.Framework.AI.BehaviorType.Condition; } }

        /// <summary> use for simulation AnimationTree</summary>
        public bool IsValid { get; set; }

        public ParameterDataCollection ParameterDifinition { get; private set; }        

        public ConditionData()
            : base("NewCondition")
        {
            ParameterDifinition = new ParameterDataCollection();            
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement pdElement = e[ParameterDataCollection.ElementName];
            if (pdElement != null) ParameterDifinition.Load(pdElement);

            
            XmlElement debug = e["Debug"];
            if (debug != null)
            {
                IsValid = debug.GetAttributeValueAsBoolean("IsValid", false);
            }
            base.ReadAttributes(e);
        }

        protected override void WriteAttributes(XmlElement e)
        {
            XmlElement pdElement = ParameterDifinition.ToXmlElement();
            e.AppendChild(pdElement);
            
            XmlElement debug = new XmlElement("Debug");
            debug.SetAttributeValue("IsValid", IsValid);
            e.AppendChild(debug);
            base.WriteAttributes(e);
        }
    }
    #endregion
}
