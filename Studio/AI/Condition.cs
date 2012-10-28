using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using System.ComponentModel;

namespace Skill.Studio.AI
{
    #region ConditionViewModel
    [DisplayName("Condition")]
    public class ConditionViewModel : BehaviorViewModel
    {
        [Browsable(false)]
        public override string ImageName { get { return Images.Condition; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        [DisplayName("Parameters")]
        [Description("Parameters of Condition")]
        public ParameterCollectionViewModel Parameters
        {
            get
            {
                return new ParameterCollectionViewModel(((BehaviorViewModel)Parent).GetParameters(this));
            }
        }

        public ConditionViewModel(BehaviorViewModel parent, Condition condition)
            : base(parent, condition)
        {

        }

        [Category("Debug")]
        [Description("Is condition valid in simulation runtime")]
        public bool IsValid
        {
            get { return ((Skill.DataModels.AI.Condition)Model).IsValid; }
            set
            {
                if (((Skill.DataModels.AI.Condition)Model).IsValid != value)
                {
                    SetValid(value);
                    foreach (ConditionViewModel cvm in Tree.GetSharedModel(Model))
                        if (cvm != this) cvm.SetValid(value);
                    Tree.Editor.SetChanged(true);
                }
            }
        }

        private void SetValid(bool value)
        {
            ((Skill.DataModels.AI.Condition)Model).IsValid = value;
            Debug.IsValid = value;
            if (IsDebuging) BackBrush = value ? Editor.BehaviorBrushes.EnableBrush : Editor.BehaviorBrushes.DisableBrush;
            OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }
    }
    #endregion
}
