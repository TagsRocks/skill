using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region Decorator

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
        public DecoratorType Type { get; private set; }

        /// <summary> Type of behavior node</summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Decorator; } }

        /// <summary> Retrieves child node </summary>
        public Behavior Child { get { if (Count > 0) return this[0]; return null; } }

        /// <summary> when decorator loaded from file read this value from file. this value is not valid until decorator loaded from file </summary>
        //public int LoadedChildId { get; private set; }

        /// <summary> if true : when handler function fail return success </summary>
        public bool NeverFail { get; set; }

        protected Decorator(string name, DecoratorType type)
            : base(name)
        {
            //this.LoadedChildId = -1;
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
            //if (childArray != null && childArray.Length > 0)
            //    this.LoadedChildId = childArray[0];
            //else
            //    this.LoadedChildId = -1;
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

    public enum AccessKeyType
    {
        CounterLimit,
        TimeLimit
    }

    public abstract class AccessKey : IXElement
    {
        public AccessKeyType Type { get; private set; }

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

    public class CounterLimitAccessKey : AccessKey
    {
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


    public class TimeLimitAccessKey : AccessKey
    {
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
    public class AccessLimitDecorator : Decorator
    {
        public string AccessKey { get; set; }
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
