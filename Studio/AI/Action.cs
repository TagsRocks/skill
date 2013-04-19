using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using System.ComponentModel;
using Skill.Studio.Controls;

namespace Skill.Studio.AI
{
    #region ActionViewModel
    /// <summary>
    /// Action View Model
    /// </summary>
    [DisplayName("Action")]
    public class ActionViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Action; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        [Description("Parameters of Action")]
        [DisplayName("Parameters")]
        public ParameterCollectionViewModel Parameters
        {
            get
            {
                return new ParameterCollectionViewModel(((BehaviorViewModel)Parent).GetParameters(this));
            }
        }

        [DefaultValue(false)]
        [Category("Events")]
        [DisplayName("Reset")]
        [Description("If true code generator create an method and hook it to reset event")]
        public bool ResetEvent
        {
            get { return ((Skill.DataModels.AI.Action)Model).ResetEvent; }
            set
            {
                if (value != ((Skill.DataModels.AI.Action)Model).ResetEvent)
                {
                    ((Skill.DataModels.AI.Action)Model).ResetEvent = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ResetEvent"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "ResetEvent", value, !value));
                }
            }
        }

        [Category("Debug")]
        [Description("Is action valid in simulation?")]
        public bool IsValid
        {
            get { return ((Skill.DataModels.AI.Action)Model).IsValid; }
            set
            {
                if (((Skill.DataModels.AI.Action)Model).IsValid != value)
                {
                    SetValid(value);
                    foreach (ActionViewModel avm in Tree.GetSharedModel(Model))
                        if (avm != this) avm.SetValid(value);
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        public override bool IsValidable { get { return true; } }

        private void SetValid(bool value)
        {
            ((Skill.DataModels.AI.Action)Model).IsValid = value;
            Debug.IsValid = value;
            if (IsDebuging) BackBrush = value ? Editor.BehaviorBrushes.EnableBrush : Editor.BehaviorBrushes.DisableBrush;
            OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }

        [Category("Debug")]
        [Description("ExecutionTime of action in simulation")]
        [DefaultValue(1.0)]
        public float ExecutionTime
        {
            get { return ((Skill.DataModels.AI.Action)Model).ExecutionTime; }
            set
            {
                if (((Skill.DataModels.AI.Action)Model).ExecutionTime != value)
                {
                    ((Skill.DataModels.AI.Action)Model).ExecutionTime = value;
                    Debug.ExecutionTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ExecutionTime"));
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        [Category("Debug")]
        [Description("FrameRate of animation")]
        public int FrameRate
        {
            get { return ((Skill.DataModels.AI.Action)Model).AnimationFrameRate; }
            set
            {
                if (((Skill.DataModels.AI.Action)Model).AnimationFrameRate != value)
                {
                    ((Skill.DataModels.AI.Action)Model).AnimationFrameRate = value;
                    _GifAnimation.FrameRate = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FrameRate"));
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        [Category("Debug")]
        [Description("WrapMode of animation")]
        public GifWrapMode WrapMode
        {
            get { return (GifWrapMode)((Skill.DataModels.AI.Action)Model).AnimationWrapMode; }
            set
            {
                if (((Skill.DataModels.AI.Action)Model).AnimationWrapMode != (int)value)
                {
                    ((Skill.DataModels.AI.Action)Model).AnimationWrapMode = (int)value;
                    _GifAnimation.WrapMode = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("WrapMode"));
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        [Category("Debug")]
        [Description("gif Animation")]
        [System.ComponentModel.Editor(typeof(Editor.AnimationPropertyEditor), typeof(Editor.AnimationPropertyEditor))]
        public string Animation
        {
            get
            {
                string result = ((Skill.DataModels.AI.Action)Model).AnimationSource;
                if (!string.IsNullOrEmpty(result))
                    _GifAnimation.GifSource = MainWindow.Instance.Project.GetAnimationPath(result);
                return result;
            }
            set
            {
                if (((Skill.DataModels.AI.Action)Model).AnimationSource != value)
                {
                    ((Skill.DataModels.AI.Action)Model).AnimationSource = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Animation"));
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        private GifImage _GifAnimation;
        [Browsable(false)]
        public GifImage GifAnimation
        {
            get { return _GifAnimation; }
            set
            {
                if (_GifAnimation != value)
                {
                    _GifAnimation = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("GifAnimation"));
                }
            }
        }

        public ActionViewModel(BehaviorViewModel parent, Skill.DataModels.AI.Action action)
            : base(parent, action)
        {
            GifAnimation = new GifImage();
            GifAnimation.FrameRate = FrameRate;
            GifAnimation.WrapMode = WrapMode;

            string anim = action.AnimationSource;
            if (!string.IsNullOrEmpty(anim))
                _GifAnimation.GifSource = MainWindow.Instance.Project.GetAnimationPath(anim);
        }
    }
    #endregion
}
