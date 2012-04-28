using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Skill.Editor.AI
{
    #region Decorator
    /// <summary>
    /// Defines a decorator in behavior tree
    /// </summary>
    public class Decorator : Behavior
    {
        /// <summary> Type of behavior node</summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Decorator; } }

        /// <summary> Retrieves child node </summary>
        public Behavior Child { get { if (Count > 0) return this[0]; return null; } }

        /// <summary> when decorator loaded from file read this value from file. this value is not valid until decorator loaded from file </summary>
        public int LoadedChildId { get; private set; }

        /// <summary> if true : when handler function fail return success </summary>
        public bool SuccessOnFailHandler { get; set; }

        public Decorator()
            : base("NewDecorator")
        {
            LoadedChildId = -1;
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
}
