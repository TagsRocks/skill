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

        [DefaultValue(BehaviorTree.DefaultStateName)]        
        [Description("Destination state")]
        public string StateName
        {
            get { return ((ChangeState)Model).StateName; }
            set
            {
                if (value != ((ChangeState)Model).StateName)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "StateName", value, ((ChangeState)Model).StateName));
                    ((ChangeState)Model).StateName = value;
                    //((Skill.Framework.AI.PrioritySelector)Debug.Behavior).Priority = (Skill.Framework.AI.PriorityType)value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StateName"));
                }
            }
        }

        public ChangeStateViewModel(BehaviorViewModel parent, Skill.DataModels.AI.ChangeState changeState)
            : base(parent, changeState)
        {            
        }
    }
    #endregion
}
