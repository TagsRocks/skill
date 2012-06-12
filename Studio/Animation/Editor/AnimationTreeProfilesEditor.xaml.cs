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

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimationTreeProfilesEditor.xaml
    /// </summary>
    public partial class AnimationTreeProfilesEditor : Window, INotifyPropertyChanged
    {
        private AnimationTreeRootViewModel _Root;
        private ObservableCollection<AnimationTreeProfileViewModel> _Profiles;

        private bool _HasSelected;
        public bool HasSelected
        {
            get { return _HasSelected; }
            set
            {
                if (_HasSelected != value)
                {
                    _HasSelected = value;
                    OnPropertyChanged("HasSelected");
                }
            }
        }

        public AnimationTreeProfilesEditor()
        {
            InitializeComponent();
        }

        public AnimationTreeProfilesEditor(AnimationTreeRootViewModel root)
        {
            InitializeComponent();
            this._Root = root;
            this._Profiles = new ObservableCollection<AnimationTreeProfileViewModel>();            

            if (this._Root.Profiles != null) // clone profiles
            {
                foreach (var item in root.Profiles)
                {
                    AnimationTreeProfileViewModel profile = new AnimationTreeProfileViewModel();
                    profile.Model.Name = item.Name;
                    profile.Model.Format = item.Format;
                    this._Profiles.Add(profile);
                }
            }

            this._LbProfiles.ItemsSource = this._Profiles;
        }

        private void LbProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelected = _LbProfiles.SelectedItem != null;
        }

        private void LbProfiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSelected();
        }

        private string GetValidName(string initialName)
        {
            string name = initialName;
            int i = 1;

            while (this._Profiles.Count(p => p.Name == name) > 0)
            {
                name = initialName + i;
                i++;
            }

            return name;
        }

        private void BtnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            AnimationTreeProfileViewModel profile = new AnimationTreeProfileViewModel();
            profile.Model.Name = GetValidName(profile.Name);
            profile.Model.Format = "{0}";
            this._Profiles.Add(profile);
        }

        private void BtnDelProfile_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void DeleteSelected()
        {
            if (_LbProfiles.SelectedIndex >= 0)
                _Profiles.RemoveAt(_LbProfiles.SelectedIndex);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // check for duplicate names
            foreach (var profile in this._Profiles)
            {
                if (_Profiles.Count(p => p.Name == profile.Name) > 1)
                {
                    System.Windows.MessageBox.Show("The profile with name : " + profile.Name + " is duplicated", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (_Profiles.Count(p => p.Format == profile.Format) > 1)
                {
                    System.Windows.MessageBox.Show("The profile with format : " + profile.Format + " is duplicated", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }


            Skill.DataModels.Animation.AnimationTreeProfile[] result = new DataModels.Animation.AnimationTreeProfile[this._Profiles.Count];
            for (int i = 0; i < this._Profiles.Count; i++)
            {
                result[i] = this._Profiles[i].Model;
            }
            _Root.Profiles = result;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
