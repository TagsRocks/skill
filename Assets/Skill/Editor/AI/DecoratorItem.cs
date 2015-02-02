using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class DecoratorItem : TreeViewFolder
    {

        public DecoratorItem(DecoratorData data)
            : base(data)
        {

        }        

        [Skill.Framework.ExposeProperty(41, "NeverFail", "if true : when handler function failed return success. the result will be Running or Success")]
        public virtual bool NeverFail
        {
            get { return ((DecoratorData)Data).NeverFail; }
            set
            {
                if (value != ((DecoratorData)Data).NeverFail)
                {
                    ((DecoratorData)Data).NeverFail = value;
                    //OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NeverFail"));
                    //((Skill.Framework.AI.Decorator)Debug.Behavior).NeverFail = value;
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "NeverFail", value, !value));
                }
            }
        }
    }
}