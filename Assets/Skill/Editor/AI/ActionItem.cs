using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Editor.AI
{
    public class ActionItem : TreeViewItem
    {
        public ActionItem(ActionData data)
            : base(data)
        {
        }       


        #region Expose Properties
        [Skill.Framework.ExposeProperty(41, "Posture", "whether this action change posture of actor?")]
        public Posture ChangePosture
        {
            get { return ((ActionData)Data).ChangePosture; }
            set
            {
                if (((ActionData)Data).ChangePosture != value)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "ChangePosture", value, ((Skill.DataModels.AI.Action)Model).ChangePosture));
                    ((ActionData)Data).ChangePosture = value;
                    //OnPropertyChanged(new PropertyChangedEventArgs("ChangePosture"));                
                }
            }
        }

        [Skill.Framework.ExposeProperty(42, "Reset", "If true code generator create a method and hook it to reset event of BehaviorTree")]
        public bool ResetEvent
        {
            get { return ((ActionData)Data).ResetEvent; }
            set
            {
                if (value != ((ActionData)Data).ResetEvent)
                {
                    ((ActionData)Data).ResetEvent = value;
                    //this.OnPropertyChanged(new PropertyChangedEventArgs("ResetEvent"));
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "ResetEvent", value, !value));
                }
            }
        }
        #endregion
    }
}
