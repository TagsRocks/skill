using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace Skill.Editor.AI
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
        public int LoadedChildId { get; private set; }

        /// <summary> if true : when handler function fail return success </summary>
        public bool SuccessOnFailHandler { get; set; }

        protected Decorator(string name, DecoratorType type)
            : base(name)
        {
            this.LoadedChildId = -1;
            this.Type = type;
        }

        public Decorator()
            : this("NewDecorator", DecoratorType.Default)
        {
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            string child = e.Attribute("Child").Value;
            var childArray = Behavior.ConvertToIndices(child);
            if (childArray != null && childArray.Length > 0)
                this.LoadedChildId = childArray[0];
            else
                this.LoadedChildId = -1;
            base.ReadAttributes(e);
        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("DecoratorType", this.Type.ToString());
            e.SetAttributeValue("Child", GetChildrenString());
            base.WriteAttributes(e);
        }
    }
    #endregion

    #region DecoratorViewModel
    public class DecoratorViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Decorator; } }

        [DisplayName("SuccessOnFailHandler")]
        [Description("if true : when handler function fail return success")]
        public bool SuccessOnFailHandler
        {
            get { return ((Decorator)Model).SuccessOnFailHandler; }
            set
            {
                if (value != ((Decorator)Model).SuccessOnFailHandler)
                {
                    ((Decorator)Model).SuccessOnFailHandler = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SuccessOnFailHandler"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "SuccessOnFailHandler", value, !value));
                }
            }
        }

        public DecoratorViewModel(BehaviorViewModel parent, Skill.Editor.AI.Decorator decorator)
            : base(parent, decorator)
        {
        }
    }
    #endregion

    #region AccessKey

    public enum AccessKeyType
    {
        CountLimit,
        TimeLimit
    }

    public class AccessKey : IXElement
    {
        [Browsable(false)]
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

    }

    public class CountLimitAccessKey : AccessKey
    {
        public int MaxAccessCount { get; set; }

        public CountLimitAccessKey()
            : base(AccessKeyType.CountLimit)
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
    #endregion

    #region AccessLimitDecorator
    public class AccessLimitDecorator : Decorator
    {
        public string AccessKey { get; set; }

        public AccessLimitDecorator()
            : base("NewAccessLimitDecorator", DecoratorType.AccessLimit)
        {

        }

        protected override void WriteAttributes(System.Xml.Linq.XElement e)
        {
            e.SetAttributeValue("AccessKey", AccessKey);
            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(System.Xml.Linq.XElement e)
        {
            AccessKey = e.GetAttributeValueAsString("AccessKey", "");
            base.ReadAttributes(e);
        }

    }
    #endregion

    #region AccessLimitDecoratorViewModel
    public class AccessLimitDecoratorViewModel : DecoratorViewModel
    {
        public override string ImageName { get { return Images.Decorator; } }

        [DisplayName("AccessKey")]
        [Description("The key for decorator.")]
        public string AccessKey
        {
            get { return ((AccessLimitDecorator)Model).AccessKey; }
            set
            {
                if (value != ((AccessLimitDecorator)Model).AccessKey)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "AccessKey", value, ((AccessLimitDecorator)Model).AccessKey));
                    ((AccessLimitDecorator)Model).AccessKey = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("AccessKey"));
                }
            }
        }

        public AccessLimitDecoratorViewModel(BehaviorViewModel parent, Skill.Editor.AI.AccessLimitDecorator decorator)
            : base(parent, decorator)
        {
        }

    }
    #endregion
}
