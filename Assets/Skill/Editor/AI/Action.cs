using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;
using Skill.Framework;


namespace Skill.Editor.AI
{
    #region Action
    /// <summary>
    /// Defines an action node in behavior tree
    /// </summary>
    public class ActionData : BehaviorData, IParameterData
    {
        /// <summary> Type of node </summary>
        public override Skill.Framework.AI.BehaviorType BehaviorType { get { return Skill.Framework.AI.BehaviorType.Action; } }

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

        /// <summary> use for simulation AnimationTree</summary>
        public bool IsValid { get; set; }

        /// <summary>Whether this action change posture of actor </summary>
        public Posture ChangePosture { get; set; }


        public ParameterDataCollection ParameterDifinition { get; private set; }        

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public ActionData()
            : base("NewAction")
        {
            ExecutionTime = 1;
            AnimationSource = "";
            AnimationWrapMode = 0;
            AnimationFrameRate = 30;
            ParameterDifinition = new ParameterDataCollection();            
        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("ResetEvent", ResetEvent);
            e.SetAttributeValue("ExecutionTime", ExecutionTime);
            e.SetAttributeValue("ChangePosture", ChangePosture.ToString());

            XmlElement pdElement = ParameterDifinition.ToXmlElement();
            e.AppendChild(pdElement);
            
            XmlElement debug = new XmlElement("Debug");
            debug.SetAttributeValue("IsValid", IsValid);
            debug.SetAttributeValue("WrapMode", AnimationWrapMode);
            debug.SetAttributeValue("Source", AnimationSource);
            debug.SetAttributeValue("FrameRate", AnimationFrameRate);
            e.AppendChild(debug);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            XmlElement debug = e["Debug"];
            if (debug != null)
            {
                IsValid = debug.GetAttributeValueAsBoolean("IsValid", true);
                AnimationWrapMode = debug.GetAttributeValueAsInt("WrapMode", 0);
                AnimationSource = debug.GetAttributeValueAsString("Source", "");
                AnimationFrameRate = debug.GetAttributeValueAsInt("FrameRate", 0);
            }

            XmlElement pdElement = e[ParameterDataCollection.ElementName];
            if (pdElement != null) ParameterDifinition.Load(pdElement);            


            this.ResetEvent = e.GetAttributeValueAsBoolean("ResetEvent", false);
            this.ExecutionTime = e.GetAttributeValueAsFloat("ExecutionTime", 1);
            this.ChangePosture = e.GetAttributeValueAsEnum<Posture>("ChangePosture", Posture.Unknown);
            base.ReadAttributes(e);
        }
    }
    #endregion
}
