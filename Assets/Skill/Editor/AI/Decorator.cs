using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.AI
{
    #region Decorator

    /// <summary>
    /// Defines a decorator in behavior tree
    /// </summary>
    public class DecoratorData : BehaviorData, IParameterData
    {
        /// <summary>
        /// Type of decorator
        /// </summary>
        public Skill.Framework.AI.DecoratorType Type { get; private set; }

        /// <summary> Type of behavior node</summary>
        public override Skill.Framework.AI.BehaviorType BehaviorType { get { return Skill.Framework.AI.BehaviorType.Decorator; } }

        /// <summary> Retrieves child node </summary>
        public BehaviorData Child { get { if (Count > 0) return this[0]; return null; } }

        /// <summary> if true : when handler function fail return success </summary>
        public bool NeverFail { get; set; }

        /// <summary> use for simulation AnimationTree</summary>
        public bool IsValid { get; set; }

        public ParameterDataCollection ParameterDifinition { get; private set; }

        protected DecoratorData(string name, Skill.Framework.AI.DecoratorType type)
            : base(name)
        {
            this.NeverFail = false;
            this.Type = type;
            ParameterDifinition = new ParameterDataCollection();
        }

        public DecoratorData()
            : this("NewDecorator", Skill.Framework.AI.DecoratorType.Default)
        {
        }

        protected override void ReadAttributes(XmlElement e)
        {

            NeverFail = e.GetAttributeValueAsBoolean("NeverFail", false);

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
            e.SetAttributeValue("DecoratorType", this.Type.ToString());
            e.SetAttributeValue("Child", GetChildrenString());
            e.SetAttributeValue("NeverFail", NeverFail);

            XmlElement pdElement = ParameterDifinition.ToXmlElement();
            e.AppendChild(pdElement);

            XmlElement debug = new XmlElement("Debug");
            debug.SetAttributeValue("IsValid", IsValid);
            e.AppendChild(debug);

            base.WriteAttributes(e);
        }
    }
    #endregion


    #region AccessKey

    /// <summary>
    /// Defines base class for AccessKeys
    /// </summary>
    public abstract class AccessKeyData : IXmlElementSerializable
    {
        /// <summary> Type of AccessKey </summary>
        public Skill.Framework.AI.AccessKeyType Type { get; private set; }

        /// <summary> Unique name for key </summary>
        public string Key { get; set; }

        public AccessKeyData(Skill.Framework.AI.AccessKeyType type)
        {
            this.Type = type;
            this.Key = "";
        }

        public XmlElement ToXmlElement()
        {
            XmlElement accessKey = new XmlElement(Type.ToString());
            accessKey.SetAttributeValue("Key", Key);
            WriteAttributes(accessKey);
            return accessKey;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XmlElement e) { }

        public void Load(XmlElement e)
        {
            this.Key = e.GetAttributeValueAsString("Key", "");
            ReadAttributes(e);
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XmlElement e) { }

        public override string ToString()
        {
            return string.Format("{0} ( {1} )", Type, Key);
        }
    }

    /// <summary>
    // limit count of behavior can access the key
    /// </summary>
    public class CounterLimitAccessKeyData : AccessKeyData
    {
        /// <summary>
        /// Maximum number of behavior that can access this key
        /// </summary>
        public int MaxAccessCount { get; set; }

        public CounterLimitAccessKeyData()
            : base(Skill.Framework.AI.AccessKeyType.CounterLimit)
        {
            MaxAccessCount = 1;
        }

        protected override void WriteAttributes(XmlElement e)
        {
            base.WriteAttributes(e);
            e.SetAttributeValue("MaxAccessCount", MaxAccessCount);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            base.ReadAttributes(e);
            MaxAccessCount = e.GetAttributeValueAsInt("MaxAccessCount", 1);
        }
    }

    /// <summary>
    /// The key will lock untile TimeInterval after each access
    /// </summary>
    public class TimeLimitAccessKeyData : AccessKeyData
    {
        /// <summary>
        /// The Time Interval between each access
        /// </summary>
        public float TimeInterval { get; set; }

        public TimeLimitAccessKeyData()
            : base(Skill.Framework.AI.AccessKeyType.TimeLimit)
        {
            TimeInterval = 1;
        }

        protected override void WriteAttributes(XmlElement e)
        {
            base.WriteAttributes(e);
            e.SetAttributeValue("TimeInterval", TimeInterval);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            base.ReadAttributes(e);
            TimeInterval = e.GetAttributeValueAsFloat("TimeInterval", 1);
        }
    }

    #endregion

    #region AccessLimitDecorator
    /// <summary>
    /// This decorator needs access to an accesskey to execute
    /// </summary>
    public class AccessLimitDecoratorData : DecoratorData
    {
        /// <summary>
        /// AccessKey
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Name of SharedAccesskeys class
        /// </summary>
        public string ClassName { get; set; }


        public Skill.Framework.AI.AccessKeyType KeyType { get; set; }

        public AccessLimitDecoratorData()
            : base("NewAccessLimitDecorator", Skill.Framework.AI.DecoratorType.AccessLimit)
        {

        }

        protected override void WriteAttributes(XmlElement e)
        {
            e.SetAttributeValue("KeyType", KeyType.ToString());
            e.SetAttributeValue("AccessKey", AccessKey);
            e.SetAttributeValue("ClassName", (ClassName != null) ? ClassName : string.Empty);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XmlElement e)
        {
            KeyType = e.GetAttributeValueAsEnum("KeyType", Skill.Framework.AI.AccessKeyType.CounterLimit);
            AccessKey = e.GetAttributeValueAsString("AccessKey", "");
            ClassName = e.GetAttributeValueAsString("ClassName", "");
            base.ReadAttributes(e);
        }

    }
    #endregion
}
