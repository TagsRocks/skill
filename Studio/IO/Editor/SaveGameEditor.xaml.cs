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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skill.Studio.IO.Editor
{
    /// <summary>
    /// Interaction logic for SaveGameEditor.xaml
    /// </summary>
    public partial class SaveGameEditor : TabDocument
    {

        #region Propeties

        private bool _HasSaveGameMemberSelected;
        public bool HasSaveGameMemberSelected
        {
            get { return _HasSaveGameMemberSelected; }
            set
            {
                if (_HasSaveGameMemberSelected != value)
                {
                    _HasSaveGameMemberSelected = value;
                    RaisePropertyChanged("HasSaveGameMemberSelected");
                }
            }
        }

        private bool _HasClassSelected;
        public bool HasClassSelected
        {
            get { return _HasClassSelected; }
            set
            {
                if (_HasClassSelected != value)
                {
                    _HasClassSelected = value;
                    RaisePropertyChanged("HasClassSelected");
                }
            }
        }

        private bool _HasClassPropertySelected;
        public bool HasClassPropertySelected
        {
            get { return _HasClassPropertySelected; }
            set
            {
                if (_HasClassPropertySelected != value)
                {
                    _HasClassPropertySelected = value;
                    RaisePropertyChanged("HasClassPropertySelected");
                }
            }
        }

        public SaveGameViewModel SaveGame { get; private set; }
        #endregion


        public SaveGameEditor()
            : this(null)
        {
        }

        /// <summary>
        /// Create an AnimationTreeEditor
        /// </summary>
        /// <param name="viewModel"></param>
        public SaveGameEditor(SaveGameNodeViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            if (viewModel != null)
            {
                Skill.DataModels.IO.SaveGame sg = viewModel.LoadData() as Skill.DataModels.IO.SaveGame;
                if (sg != null)
                {
                    Data = SaveGame = new SaveGameViewModel(sg) { Editor = this };
                    _LbClasses.ItemsSource = SaveGame.Classes;
                    _LbSaveGameMembers.ItemsSource = SaveGame.Properties;
                }
            }

            _CmbClassProperties.ItemsSource = _CmbSaveGameMembers.ItemsSource = new Skill.DataModels.IO.PropertyType[] { Skill.DataModels.IO.PropertyType.Primitive, Skill.DataModels.IO.PropertyType.Class };
            _CmbClassProperties.SelectedIndex = _CmbSaveGameMembers.SelectedIndex = 0;
        }

        private void Lb_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ApplicationCommands.Properties.Execute(sender, null);
        }

        #region SaveGame members

        private void BtnAddSaveGameMember_Click(object sender, RoutedEventArgs e)
        {
            Skill.DataModels.IO.PropertyType type = (Skill.DataModels.IO.PropertyType)_CmbSaveGameMembers.SelectedItem;
            SaveGame.AddProperty(type);
        }

        private void LbSaveGameMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSaveGameMemberSelected = _LbSaveGameMembers.SelectedItem != null;
            ApplicationCommands.Properties.Execute(_LbSaveGameMembers.SelectedItem, null);
        }

        private void LbSaveGameMembers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSaveGameMemeber();
        }

        private void BtnDelSaveGameMember_Click(object sender, RoutedEventArgs e)
        {
            DeleteSaveGameMemeber();
        }

        private void DeleteSaveGameMemeber()
        {
            if (HasSaveGameMemberSelected)
            {
                if (_LbSaveGameMembers.SelectedIndex >= 0)
                    SaveGame.RemovePropertyAt(_LbSaveGameMembers.SelectedIndex);
            }
        }
        #endregion

        #region Class
        private void BtnAddClass_Click(object sender, RoutedEventArgs e)
        {
            SaveGame.AddClass();
        }

        private void LbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasClassSelected = _LbClasses.SelectedItem != null;

            SaveClassViewModel cl = _LbClasses.SelectedItem as SaveClassViewModel;
            if (cl != null)
                _LbClassPoperties.ItemsSource = cl.Properties;
            else
                _LbClassPoperties.ItemsSource = null;

            ApplicationCommands.Properties.Execute(_LbClasses.SelectedItem, null);
        }

        private void LbClasses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSelectedClass();
        }

        private void BtnDelClass_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedClass();
        }

        private void DeleteSelectedClass()
        {
            if (HasClassSelected)
            {
                if (_LbClasses.SelectedIndex >= 0)
                    SaveGame.RemoveClassAt(_LbClasses.SelectedIndex);
            }
        }
        #endregion

        #region ClassPoperties
        private void BtnAddClassProperty_Click(object sender, RoutedEventArgs e)
        {
            Skill.DataModels.IO.PropertyType type = (Skill.DataModels.IO.PropertyType)_CmbClassProperties.SelectedItem;
            SaveClassViewModel cl = _LbClasses.SelectedItem as SaveClassViewModel;
            if (cl != null)
                cl.AddProperty(type);
        }

        private void LbClassPoperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasClassPropertySelected = _LbClassPoperties.SelectedItem != null;
            ApplicationCommands.Properties.Execute(_LbClassPoperties.SelectedItem, null);
        }

        private void _LbClassPoperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSelectedClassProperty();
        }

        private void BtnDelClassProperty_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedClassProperty();
        }

        private void DeleteSelectedClassProperty()
        {
            if (HasClassPropertySelected)
            {
                if (_LbClassPoperties.SelectedIndex >= 0)
                {
                    SaveClassViewModel cl = _LbClasses.SelectedItem as SaveClassViewModel;
                    if (cl != null)
                        cl.RemovePropertyAt(_LbClassPoperties.SelectedIndex);
                }
            }
        }
        #endregion
    }
}
