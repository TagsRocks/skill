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

        public AnimNodeAimOffsetProfileViewModel(AnimNodeAimOffsetProfile model)
        {
            Model = model;
        }

        public static string ElementName = "AimOffsetProfile";

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        string _CenterCenter;
        public string CenterCenter
        {
            get { return _CenterCenter; }
            set
            {
                if (_CenterCenter != value)
                {
                    _CenterCenter = value;
                    OnPropertyChanged("CenterCenter");
                }
            }
        }

        string _CenterUp;
        public string CenterUp
        {
            get { return _CenterUp; }
            set
            {
                if (_CenterUp != value)
                {
                    _CenterUp = value;
                    OnPropertyChanged("CenterUp");
                }
            }
        }

        string _CenterDown;
        public string CenterDown
        {
            get { return _CenterDown; }
            set
            {
                if (_CenterDown != value)
                {
                    _CenterDown = value;
                    OnPropertyChanged("CenterDown");
                }
            }
        }

        string _LeftCenter;
        public string LeftCenter
        {
            get { return _LeftCenter; }
            set
            {
                if (_LeftCenter != value)
                {
                    _LeftCenter = value;
                    OnPropertyChanged("LeftCenter");
                }
            }
        }

        string _LeftUp;
        public string LeftUp
        {
            get { return _LeftUp; }
            set
            {
                if (_LeftUp != value)
                {
                    _LeftUp = value;
                    OnPropertyChanged("LeftUp");
                }
            }
        }

        string _LeftDown;
        public string LeftDown
        {
            get { return _LeftDown; }
            set
            {
                if (_LeftDown != value)
                {
                    _LeftDown = value;
                    OnPropertyChanged("LeftDown");
                }
            }
        }

        string _RightCenter;
        public string RightCenter
        {
            get { return _RightCenter; }
            set
            {
                if (_RightCenter != value)
                {
                    _RightCenter = value;
                    OnPropertyChanged("RightCenter");
                }
            }
        }

        string _RightUp;
        public string RightUp
        {
            get { return _RightUp; }
            set
            {
                if (_RightUp != value)
                {
                    _RightUp = value;
                    OnPropertyChanged("RightUp");
                }
            }
        }

        string _RightDown;
        public string RightDown
        {
            get { return _RightDown; }
            set
            {
                if (_RightDown != value)
                {
                    _RightDown = value;
                    OnPropertyChanged("RightDown");
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
