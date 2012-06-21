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
    } 
    #endregion
}
