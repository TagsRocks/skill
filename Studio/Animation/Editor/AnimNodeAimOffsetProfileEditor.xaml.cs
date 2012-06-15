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
using System.Windows.Controls.Primitives;

namespace Skill.Studio.Animation.Editor
{
    /// <summary>
    /// Interaction logic for AnimNodeAimOffsetProfileEditor.xaml
    /// </summary>
    public partial class AnimNodeAimOffsetProfileEditor : Window, INotifyPropertyChanged
    {
        private AnimNodeAimOffsetViewModel _Node;

        private AnimNodeAimOffsetProfileViewModel _SelectedProfile;
        public AnimNodeAimOffsetProfileViewModel SelectedProfile
        {
            get { return _SelectedProfile; }
            set
            {
                if (_SelectedProfile != value)
                {
                    _SelectedProfile = value;
                    OnPropertyChanged("SelectedProfile");
                }
            }
        }

        private bool _HasSelectedProfile;
        public bool HasSelectedProfile
        {
            get { return _HasSelectedProfile; }
            set
            {
                if (_HasSelectedProfile != value)
                {
                    _HasSelectedProfile = value;
                    OnPropertyChanged("HasSelectedProfile");
                }
            }
        }

        public AnimNodeAimOffsetProfileEditor()
        {
            InitializeComponent();
        }

        public AnimNodeAimOffsetProfileEditor(AnimNodeAimOffsetViewModel node)
        {
            InitializeComponent();
            this._Node = node;
            this._CmbProfiles.ItemsSource = this._Node.Profiles;
        }

        private void SelectProfile(AnimNodeAimOffsetProfileViewModel selectedProfile)
        {
            this.SelectedProfile = selectedProfile;

            if (SelectedProfile != null)
            {
                if (_BtnLeftUp.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.LeftUp;
                else if (_BtnLeftCenter.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.LeftCenter;
                else if (_BtnLeftDown.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.LeftDown;
                else if (_BtnCenterUp.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.CenterUp;
                else if (_BtnCenterCenter.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.CenterCenter;
                else if (_BtnCenterDown.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.CenterDown;
                else if (_BtnRightUp.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.RightUp;
                else if (_BtnRightCenter.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.RightCenter;
                else if (_BtnRightDown.IsChecked == true)
                    _ClipSelector.AnimNodeSequence = SelectedProfile.RightDown;
            }
            else
                _ClipSelector.AnimNodeSequence = null;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CmbProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelectedProfile = _CmbProfiles.SelectedItem != null;
            AnimNodeAimOffsetProfileViewModel profile = _CmbProfiles.SelectedItem as AnimNodeAimOffsetProfileViewModel;
            if (profile != null)
                SelectProfile(profile);
            else
                SelectProfile(null);
        }

        private string GetValidName(string initialName)
        {
            string name = initialName;
            int i = 1;

            while (this._Node.Profiles.Count(p => p.Name == name) > 0)
            {
                name = initialName + i;
                i++;
            }

            return name;
        }

        private void BtnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            Skill.DataModels.Animation.AnimNodeAimOffsetProfile newProfile = new DataModels.Animation.AnimNodeAimOffsetProfile();
            newProfile.Name = GetValidName(newProfile.Name);
            AnimNodeAimOffsetProfileViewModel newProfileVM = new AnimNodeAimOffsetProfileViewModel(_Node, newProfile);
            this._Node.Profiles.Add(newProfileVM);
            this._CmbProfiles.SelectedIndex = this._Node.Profiles.Count - 1;
        }

        private void BtnDelProfile_Click(object sender, RoutedEventArgs e)
        {
            if (HasSelectedProfile)
            {
                if (_CmbProfiles.SelectedIndex >= 0)
                {
                    _Node.Profiles.RemoveAt(_CmbProfiles.SelectedIndex);
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

        private void BtnAimDirection_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (button != null)
            {
                string tag = button.Tag as string;
                if (tag != null)
                {
                    int id;
                    if (int.TryParse(tag, out id))
                    {
                        ValidateButtons(id);
                        SelectProfile(SelectedProfile);
                    }
                }
            }
        }

        private void ValidateButtons(int id)
        {
            _BtnLeftUp.IsChecked = id == 1;
            _BtnLeftCenter.IsChecked = id == 2;
            _BtnLeftDown.IsChecked = id == 3;

            _BtnCenterUp.IsChecked = id == 4;
            _BtnCenterCenter.IsChecked = id == 5;
            _BtnCenterDown.IsChecked = id == 6;

            _BtnRightUp.IsChecked = id == 7;
            _BtnRightCenter.IsChecked = id == 8;
            _BtnRightDown.IsChecked = id == 9;
        }
    }
}
