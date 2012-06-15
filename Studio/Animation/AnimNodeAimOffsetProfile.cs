using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{
    public class AnimNodeAimOffsetProfileViewModel : INotifyPropertyChanged
    {
        public AnimNodeAimOffsetProfile Model { get; private set; }
        public AnimNodeAimOffsetViewModel AimOffset { get; private set; }

        public AnimNodeAimOffsetProfileViewModel(AnimNodeAimOffsetViewModel aimOffset, AnimNodeAimOffsetProfile model)
        {
            this.AimOffset = aimOffset;
            this.Model = model;
            this.CenterCenter = new AnimNodeSequenceViewModel(aimOffset.Tree, model.CenterCenter);
            this.CenterUp = new AnimNodeSequenceViewModel(aimOffset.Tree, model.CenterUp);
            this.CenterDown = new AnimNodeSequenceViewModel(aimOffset.Tree, model.CenterDown);
            this.LeftCenter = new AnimNodeSequenceViewModel(aimOffset.Tree, model.LeftCenter);
            this.LeftUp = new AnimNodeSequenceViewModel(aimOffset.Tree, model.LeftUp);
            this.LeftDown = new AnimNodeSequenceViewModel(aimOffset.Tree, model.LeftDown);
            this.RightCenter = new AnimNodeSequenceViewModel(aimOffset.Tree, model.RightCenter);
            this.RightUp = new AnimNodeSequenceViewModel(aimOffset.Tree, model.RightUp);
            this.RightDown = new AnimNodeSequenceViewModel(aimOffset.Tree, model.RightDown);
        }

        public void CommiteChangesToModel()
        {
            this.CenterCenter.CommiteChangesToModel();
            this.CenterUp.CommiteChangesToModel();
            this.CenterDown.CommiteChangesToModel();
            this.LeftCenter.CommiteChangesToModel();
            this.LeftUp.CommiteChangesToModel();
            this.LeftDown.CommiteChangesToModel();
            this.RightCenter.CommiteChangesToModel();
            this.RightUp.CommiteChangesToModel();
            this.RightDown.CommiteChangesToModel();
        }

        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public AnimNodeSequenceViewModel CenterCenter { get; private set; }
        public AnimNodeSequenceViewModel CenterUp { get; private set; }
        public AnimNodeSequenceViewModel CenterDown { get; private set; }
        public AnimNodeSequenceViewModel LeftCenter { get; private set; }
        public AnimNodeSequenceViewModel LeftUp { get; private set; }
        public AnimNodeSequenceViewModel LeftDown { get; private set; }
        public AnimNodeSequenceViewModel RightCenter { get; private set; }
        public AnimNodeSequenceViewModel RightUp { get; private set; }
        public AnimNodeSequenceViewModel RightDown { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
