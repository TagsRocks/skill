using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Interaction logic for AnimationAimOffsetProfileEditor.xaml
    /// </summary>
    public partial class AnimationAimOffsetProfileEditor : Window, INotifyPropertyChanged
    {
        private ObservableCollection<AnimationAimOffsetProfile> _Profiles;
        public ObservableCollection<AnimationAimOffsetProfile> Profiles
        {
            get
            {
                return _Profiles;
            }
            private set
            {
                if (_Profiles != value)
                {
                    _Profiles = value;
                    OnPropertyChanged("Profiles");
                }
            }
        }

        public AnimationAimOffsetProfileEditor()
            : this(null)
        {
        }
        public AnimationAimOffsetProfileEditor(ObservableCollection<AnimationAimOffsetProfile> profiles)
        {
            InitializeComponent();
            this.Profiles = profiles;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AnimationAimOffsetProfile p = new AnimationAimOffsetProfile() { Name = "New Profile" };
            Profiles.Add(p);
            _CbProfiles.SelectedItem = p;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_CbProfiles.SelectedItem != null)
            {
                _CbProfiles.Items.Remove(_CbProfiles.SelectedItem);
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
