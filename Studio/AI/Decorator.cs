using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using Skill.DataModels.AI;

namespace Skill.Studio.AI
{

    #region DecoratorViewModel
    public class DecoratorViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Decorator; } }

        [Browsable(false)]
        public DecoratorType DecoratorType { get { return ((Decorator)Model).Type; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        public ParameterCollectionViewModel Parameters
        {
            get
            {
                return ((BehaviorViewModel)Parent).GetParameters(this);
            }
        }

        [DisplayName("NeverFail")]
        [Description("if true : when handler function fail return success")]
        public virtual bool NeverFail
        {
            get { return ((Decorator)Model).NeverFail; }
            set
            {
                if (value != ((Decorator)Model).NeverFail)
                {
                    ((Decorator)Model).NeverFail = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NeverFail"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "NeverFail", value, !value));
                }
            }
        }

        public DecoratorViewModel(BehaviorViewModel parent, Decorator decorator)
            : base(parent, decorator)
        {
        }
    }
    #endregion



    #region AccessLimitDecoratorViewModel
    public class AccessLimitDecoratorViewModel : DecoratorViewModel
    {
        public override string ImageName { get { return Images.Decorator; } }
        
        

        [DisplayName("AccessKey")]
        [Description("The key for decorator.")]
        [Editor(typeof(Editor.AccessKeyPropertyEditor), typeof(Editor.AccessKeyPropertyEditor))]
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

        [Browsable(false)]
        public string Address
        {
            get { return ((AccessLimitDecorator)Model).Address; }
            set
            {
                if (value != ((AccessLimitDecorator)Model).Address)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Address", value, ((AccessLimitDecorator)Model).Address));
                    ((AccessLimitDecorator)Model).Address = value;
                    //OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Address"));
                }
            }
        }

        public AccessLimitDecoratorViewModel(BehaviorViewModel parent, AccessLimitDecorator decorator)
            : base(parent, decorator)
        {
        }

    }
    #endregion
}
