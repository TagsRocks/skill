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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio.Animation
{



    /// <summary>
    /// Interaction logic for AnimationTreeProfileEditor.xaml
    /// </summary>
    public partial class AnimationTreeProfileEditor : Window, INotifyPropertyChanged
    {
        private ObservableCollection<AnimationTreeProfileViewModel> _Profiles;

        private bool _RemoveEnable;
        public bool RemoveEnable
        {
            get { return _RemoveEnable; }
            set
            {
                if (_RemoveEnable != value)
                {
                    _RemoveEnable = value;
                    OnPropertyChanged("RemoveEnable");
                }
            }
        }

        public AnimationTreeProfileEditor()
            : this(new ObservableCollection<AnimationTreeProfileViewModel>())
        {
        }
        public AnimationTreeProfileEditor(ObservableCollection<AnimationTreeProfileViewModel> profiles)
        {
            InitializeComponent();
            this._Profiles = profiles;
            _LbProfiles.ItemsSource = this._Profiles;
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

        private void _LbProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveEnable = _LbProfiles.SelectedIndex >= 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _Profiles.Add(new AnimationTreeProfileViewModel(_Profiles));
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveEnable)
            {
                _Profiles.RemoveAt(_LbProfiles.SelectedIndex);
            }
        }
    }


    
}
