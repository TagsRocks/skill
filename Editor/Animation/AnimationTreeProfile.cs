using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Editor.Animation
{
    public class AnimationTreeProfile : IXElement
    {
        public string Name { get; set; }
        public string Format { get; set; }

        public AnimationTreeProfile()
        {
            this.Name = "Profile";
            this.Format = "{0}";
        }

        public XElement ToXElement()
        {
            XElement e = new XElement("Profile");
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Format", Format);
            return e;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "Profile");
            this.Format = e.GetAttributeValueAsString("Format", "{0}");
        }
    }

    public class AnimationTreeProfileViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<AnimationTreeProfileViewModel> _Container;

        [Browsable(false)]
        public AnimationTreeProfile Model { get; private set; }

        public AnimationTreeProfileViewModel(ObservableCollection<AnimationTreeProfileViewModel> container)
            : this(container, new AnimationTreeProfile())
        {
        }

        public AnimationTreeProfileViewModel(ObservableCollection<AnimationTreeProfileViewModel> container, AnimationTreeProfile model)
        {
            this._Container = container;
            this.Model = model;
            this.Model.Name = GetValidName(this.Model.Name);
        }

        private string GetValidName(string initialName)
        {
            string name = initialName;
            int i = 1;

            while (this._Container.Count(p => p.Name == name) > 0)
            {
                name = initialName + i;
                i++;
            }

            return name;
        }

        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name)
                {
                    if (string.IsNullOrEmpty(value)) return;
                    if (this._Container.Count(p => p.Name == value) > 0) return;
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
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
