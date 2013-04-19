using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using System.ComponentModel;
using Skill.Studio.Controls;

namespace Skill.Studio.AI
{
    #region ChangeStateViewModel
    /// <summary>
    /// Action View Model
    /// </summary>
    [DisplayName("ChangeState")]
    public class ChangeStateViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.ChangeState; } }        

        [DefaultValue(BehaviorTree.DefaultDestinationState)]
        [Description("Destination state")]
        [System.ComponentModel.Editor(typeof(Editor.DestinationStatePropertyEditor), typeof(Editor.DestinationStatePropertyEditor))]
        public string DestinationState
        {
            get { return ((ChangeState)Model).DestinationState; }
            set
            {
                if (value != ((ChangeState)Model).DestinationState)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "DestinationState", value, ((ChangeState)Model).DestinationState));
                    ((ChangeState)Model).DestinationState = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DestinationState"));                    
                }
            }
        }

        public static string CreateName(string destinationState)
        {
            return string.Format("GoTo{0}", destinationState);
        }

        public ChangeStateViewModel(BehaviorViewModel parent, Skill.DataModels.AI.ChangeState changeState)
            : base(parent, changeState)
        {
        }
    }
    #endregion
}
