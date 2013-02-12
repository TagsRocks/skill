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
    /// Interaction logic for SaveDataEditor.xaml
    /// </summary>
    public partial class SaveDataEditor : TabDocument
    {

        #region Propeties

        private bool _HasSaveDataMemberSelected;
        public bool HasSaveDataMemberSelected
        {
            get { return _HasSaveDataMemberSelected; }
            set
            {
                if (_HasSaveDataMemberSelected != value)
                {
                    _HasSaveDataMemberSelected = value;
                    RaisePropertyChanged("HasSaveDataMemberSelected");
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

        public SaveDataViewModel SaveData { get; private set; }
        #endregion


        public SaveDataEditor()
            : this(null)
        {
        }

        /// <summary>
        /// Create an AnimationTreeEditor
        /// </summary>
        /// <param name="viewModel"></param>
        public SaveDataEditor(SaveDataNodeViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            if (viewModel != null)
            {
                Skill.DataModels.IO.SaveData sg = viewModel.LoadData() as Skill.DataModels.IO.SaveData;
                if (sg != null)
                {
                    Data = SaveData = new SaveDataViewModel(sg) { Editor = this };
                    _LbClasses.ItemsSource = SaveData.Classes;
                    _LbSaveDataMembers.ItemsSource = SaveData.Properties;
                }
            }

            _CmbClassProperties.ItemsSource = _CmbSaveDataMembers.ItemsSource = new Skill.DataModels.IO.PropertyType[] { Skill.DataModels.IO.PropertyType.Primitive, Skill.DataModels.IO.PropertyType.Class };
            _CmbClassProperties.SelectedIndex = _CmbSaveDataMembers.SelectedIndex = 0;
        }

        private void Lb_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ApplicationCommands.Properties.Execute(sender, null);
        }

        #region SaveData members

        private void BtnAddSaveDataMember_Click(object sender, RoutedEventArgs e)
        {
            Skill.DataModels.IO.PropertyType type = (Skill.DataModels.IO.PropertyType)_CmbSaveDataMembers.SelectedItem;
            SaveData.AddProperty(type);
            _LbSaveDataMembers.SelectedIndex = _LbSaveDataMembers.Items.Count - 1;
        }

        private void LbSaveDataMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSaveDataMemberSelected = _LbSaveDataMembers.SelectedItem != null;
            ApplicationCommands.Properties.Execute(_LbSaveDataMembers.SelectedItem, null);
        }

        private void LbSaveDataMembers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSaveDataMemeber();
        }

        private void BtnDelSaveDataMember_Click(object sender, RoutedEventArgs e)
        {
            DeleteSaveDataMemeber();
        }

        private void DeleteSaveDataMemeber()
        {
            if (HasSaveDataMemberSelected)
            {
                if (_LbSaveDataMembers.SelectedIndex >= 0)
                    SaveData.RemovePropertyAt(_LbSaveDataMembers.SelectedIndex);
            }
        }
        #endregion

        #region Class
        private void BtnAddClass_Click(object sender, RoutedEventArgs e)
        {
            SaveData.AddClass();
            _LbClasses.SelectedIndex = _LbClasses.Items.Count - 1;
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
                    SaveData.RemoveClassAt(_LbClasses.SelectedIndex);
            }
        }
        #endregion

        #region ClassPoperties
        private void BtnAddClassProperty_Click(object sender, RoutedEventArgs e)
        {
            Skill.DataModels.IO.PropertyType type = (Skill.DataModels.IO.PropertyType)_CmbClassProperties.SelectedItem;
            SaveClassViewModel cl = _LbClasses.SelectedItem as SaveClassViewModel;
            if (cl != null)
            {
                cl.AddProperty(type);
                _LbClassPoperties.SelectedIndex = _LbClassPoperties.Items.Count - 1;
            }
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
