using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using System.ComponentModel;

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

        public ActionViewModel(BehaviorViewModel parent, Skill.DataModels.AI.Action action)
            : base(parent, action)
        {

        }
    }
    #endregion
}
