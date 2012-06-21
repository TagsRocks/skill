using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.DataModels.Animation;

namespace Skill.Studio.Animation
{

    [DisplayName("AnimationTreeProfile.")]
    public class AnimationTreeProfileViewModel : INotifyPropertyChanged
    {        
        [Browsable(false)]
        public AnimationTreeProfile Model { get; private set; }

        public AnimationTreeProfileViewModel()
            : this(new AnimationTreeProfile())
        {
        }

        public AnimationTreeProfileViewModel(AnimationTreeProfile model)
        {            
            this.Model = model;            
        }        

        [Description("Unique name of profile.")]
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name)
                {
                    if (string.IsNullOrEmpty(value)) return;
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        [Description("Format of profile. contains at least one {0}.")]
        public string Format
        {
            get { return Model.Format; }
            set
            {
                if (value != Model.Format)
                {
                    Model.Format = value;
                    OnPropertyChanged("Format");
                }
            }
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
