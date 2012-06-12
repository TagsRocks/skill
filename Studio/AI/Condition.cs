using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.Studio.AI
{    
    #region ConditionViewModel
    public class ConditionViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Condition; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        public ParameterCollectionViewModel Parameters
        {
            get
            {
                return ((BehaviorViewModel)Parent).GetParameters(this);
            }
        }

        public ConditionViewModel(BehaviorViewModel parent, Condition condition)
            : base(parent, condition)
        {

        }
    } 
    #endregion
}
