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


        public ActionViewModel(BehaviorViewModel parent, Skill.DataModels.AI.Action action)
            : base(parent, action)
        {

        }
    }
    #endregion
}
