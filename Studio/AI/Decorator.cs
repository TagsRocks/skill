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
    [DisplayName("Decorator")]
    public class DecoratorViewModel : BehaviorViewModel
    {
        [Browsable(false)]
        public override string ImageName { get { return Images.Decorator; } }

        [Browsable(false)]
        public DecoratorType DecoratorType { get { return ((Decorator)Model).Type; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        [DisplayName("Parameters")]
        [Description("Parameters to pass to handler method")]
        public virtual ParameterCollectionViewModel Parameters
        {
            get
            {
                return new ParameterCollectionViewModel(((BehaviorViewModel)Parent).GetParameters(this));
            }
        }

        [DefaultValue(true)]
        [DisplayName("NeverFail")]
        [Description("if true : when handler function fail return success. the result will be Running or Success")]
        public virtual bool NeverFail
        {
            get { return ((Decorator)Model).NeverFail; }
            set
            {
                if (value != ((Decorator)Model).NeverFail)
                {
                    ((Decorator)Model).NeverFail = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NeverFail"));
                    ((Skill.AI.Decorator)Debug.Behavior).NeverFail = value;
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "NeverFail", value, !value));
                }
            }
        }

        public DecoratorViewModel(BehaviorViewModel parent, Decorator decorator)
            : base(parent, decorator)
        {
        }

        [Category("Debug")]
        [Description("Is decorator valid in simulation runtime")]
        public bool IsValid
        {
            get { return ((Skill.DataModels.AI.Decorator)Model).IsValid; }
            set
            {
                if (((Skill.DataModels.AI.Decorator)Model).IsValid != value)
                {
                    SetValid(value);

                    foreach (DecoratorViewModel dvm in Tree.GetSharedModel(Model))
                        if (dvm != this) dvm.SetValid(value);
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        private void SetValid(bool value)
        {
            ((Skill.DataModels.AI.Decorator)Model).IsValid = value;
            Debug.IsValid = value;
            if (IsDebuging) BackBrush = value ? Editor.BehaviorBrushes.EnableBrush : Editor.BehaviorBrushes.DisableBrush;
            OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }
    }
    #endregion



    #region AccessLimitDecoratorViewModel
    [DisplayName("AccessLimitDecorator")]
    public class AccessLimitDecoratorViewModel : DecoratorViewModel
    {
        [Browsable(false)]
        public override string ImageName { get { return Images.AccessLimitDecorator; } }
        
        [DisplayName("AccessKey")]
        [Description("The Accesskey for decorator.")]
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
