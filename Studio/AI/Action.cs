using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.Studio.AI
{    
    #region ActionViewModel
    /// <summary>
    /// Action View Model
    /// </summary>
    public class ActionViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Action; } }

        [System.ComponentModel.Editor(typeof(Editor.ParametersPropertyEditor), typeof(Editor.ParametersPropertyEditor))]
        public ParameterCollectionViewModel Parameters
        {
            get 
            {
                return ((BehaviorViewModel)Parent).GetParameters(this);
            }
        }

        public ActionViewModel(BehaviorViewModel parent, Skill.DataModels.AI.Action action)
            : base(parent, action)
        {

        }
    } 
    #endregion
}
