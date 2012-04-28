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

namespace Skill.Editor
{
    /// <summary>
    /// Interaction logic for NewBackupWindow.xaml
    /// </summary>
    public partial class NewBackupWindow : Window
    {

        public NewBackupWindow()
        {
            InitializeComponent();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            base.OnKeyUp(e);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_TxtBackupName.Text))
            {
                _TxtError.Text = "Please enter valid name for backup";
                return;
            }
            BackupRestore backup = new BackupRestore();
            if (!backup.IsBackupNameValid(_TxtBackupName.Text))
            {
                _TxtError.Text = "Please enter valid name for backup";
                return;
            }
            if (backup.IsBackupExists(_TxtBackupName.Text))
            {
                _TxtError.Text = "Backup with same name already exists";
                return;
            }

            backup.Create(_TxtBackupName.Text);
            Close();
        }
    }
}
