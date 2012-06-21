using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region Decorator

    /// <summary>
    /// Defines types of Decorator
    /// </summary>
    public enum DecoratorType
    {
        Default,
        AccessLimit
    }

    /// <summary>
    /// Defines a decorator in behavior tree
    /// </summary>
    public class Decorator : Behavior
    {
        /// <summary>
        /// Type of decorator
        /// </summary>
        public DecoratorType Type { get; private set; }

        /// <summary> Type of behavior node</summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Decorator; } }

        /// <summary> Retrieves child node </summary>
        public Behavior Child { get { if (Count > 0) return this[0]; return null; } }

        /// <summary> if true : when handler function fail return success </summary>
        public bool NeverFail { get; set; }

        protected Decorator(string name, DecoratorType type)
            : base(name)
        {
            this.NeverFail = true;
            this.Type = type;
        }

        public Decorator()
            : this("NewDecorator", DecoratorType.Default)
        {
            NeverFail = true;
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            NeverFail = e.GetAttributeValueAsBoolean("NeverFail", true);
            string child = e.Attribute("Child").Value;
            var childArray = Behavior.ConvertToIndices(child);
            base.ReadAttributes(e);
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("DecoratorType", this.Type.ToString());
            e.SetAttributeValue("Child", GetChildrenString());
            e.SetAttributeValue("NeverFail", NeverFail);
            base.WriteAttributes(e);
        }
    }
    #endregion


    #region AccessKey

    /// <summary>
    /// Defines types of AccessKey
    /// </summary>
    public enum AccessKeyType
    {
        CounterLimit,// limit count of behavior can access the key
        TimeLimit // the key will lock untile TimeInterval after each access
    }

    /// <summary>
    /// Defines base class for AccessKeys
    /// </summary>
    public abstract class AccessKey : IXElement
    {
        /// <summary> Type of AccessKey </summary>
        public AccessKeyType Type { get; private set; }

        /// <summary> Unique name for key </summary>
        public string Key { get; set; }

        public AccessKey(AccessKeyType type)
        {
            this.Type = type;
            this.Key = "";
        }

        public XElement ToXElement()
        {
            XElement accessKey = new XElement(Type.ToString());
            accessKey.SetAttributeValue("Key", Key);
            WriteAttributes(accessKey);
            return accessKey;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        public void Load(System.Xml.Linq.XElement e)
        {
            this.Key = e.GetAttributeValueAsString("Key", "");
            ReadAttributes(e);
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XElement e) { }

        public override string ToString()
        {
            return string.Format("{0} ( {1} )", Type, Key);
        }
    }

    /// <summary>
    // limit count of behavior can access the key
    /// </summary>
    public class CounterLimitAccessKey : AccessKey
    {
        /// <summary>
        /// Maximum number of behavior that can access this key
        /// </summary>
        public int MaxAccessCount { get; set; }

        public CounterLimitAccessKey()
            : base(AccessKeyType.CounterLimit)
        {
            MaxAccessCount = 1;
        }

        protected override void WriteAttributes(XElement e)
        {
            base.WriteAttributes(e);
            e.SetAttributeValue("MaxAccessCount", MaxAccessCount);
        }

        protected override void ReadAttributes(XElement e)
        {
            base.ReadAttributes(e);
            MaxAccessCount = e.GetAttributeValueAsInt("MaxAccessCount", 1);
        }
    }

    /// <summary>
    /// The key will lock untile TimeInterval after each access
    /// </summary>
    public class TimeLimitAccessKey : AccessKey
    {
        /// <summary>
        /// The Time Interval between each access
        /// </summary>
        public float TimeInterval { get; set; }

        public TimeLimitAccessKey()
            : base(AccessKeyType.TimeLimit)
        {
            TimeInterval = 1;
        }

        protected override void WriteAttributes(XElement e)
        {
            base.WriteAttributes(e);
            e.SetAttributeValue("TimeInterval", TimeInterval);
        }

        protected override void ReadAttributes(XElement e)
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
    public class AccessLimitDecorator : Decorator
    {
        /// <summary>
        /// AccessKey
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Address of SharedAccesskeys class
        /// </summary>
        public string Address { get; set; }

        public AccessLimitDecorator()
            : base("NewAccessLimitDecorator", DecoratorType.AccessLimit)
        {

        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("AccessKey", AccessKey);
            e.SetAttributeValue("Address", (Address != null) ? Address : string.Empty);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            AccessKey = e.GetAttributeValueAsString("AccessKey", "");
            Address = e.GetAttributeValueAsString("Address", "");
            base.ReadAttributes(e);
        }

    }
    #endregion
}
