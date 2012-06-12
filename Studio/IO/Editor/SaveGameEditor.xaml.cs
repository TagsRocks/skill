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
        }

        #region SaveGame members
        private void BtnAddSaveGameMember_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbSaveGameMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LbSaveGameMembers_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnDelSaveGameMember_Click(object sender, RoutedEventArgs e)
        {

        } 
        #endregion

        #region Class
        private void BtnAddClass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbClasseses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LbClasses_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnDelClass_Click(object sender, RoutedEventArgs e)
        {

        } 
        #endregion

        #region ClassPoperties
        private void BtnAddClassProperty_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbClassPoperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _LbClassPoperties_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnDelClassProperty_Click(object sender, RoutedEventArgs e)
        {

        } 
        #endregion
    }
}
