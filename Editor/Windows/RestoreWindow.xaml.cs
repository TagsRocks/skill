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
using System.IO;
using System.ComponentModel;

namespace Skill.Editor
{
    /// <summary>
    /// Interaction logic for RestoreWindow.xaml
    /// </summary>
    public partial class RestoreWindow : Window, INotifyPropertyChanged
    {


        private BackupRestore _Backup;
        public ObservableCollection<BackupItem> BackupNames { get; private set; }


        private string _ModifyDate;
        public string ModifyDate
        {
            get { return _ModifyDate; }
            set
            {
                if (_ModifyDate != value)
                {
                    _ModifyDate = value;
                    OnPropertyChanged("ModifyDate");
                }
            }
        }

        public RestoreWindow()
        {
            InitializeComponent();
            _Backup = new BackupRestore();
            BackupNames = new ObservableCollection<BackupItem>();
            LoadBackupNames();
        }

        private void LoadBackupNames()
        {
            BackupNames.Clear();
            string backupRootDir = _Backup.BackupDirectory;
            if (Directory.Exists(backupRootDir))
            {
                string[] names = Directory.GetDirectories(backupRootDir);
                if (names != null)
                    foreach (var path in names)
                    {
                        string backupName = System.IO.Path.GetFileNameWithoutExtension(path);
                        BackupItem item = _Backup.CreateTree(backupName);
                        if (item != null)
                            BackupNames.Add(item);
                    }
            }
            _LbBackupNames.ItemsSource = BackupNames;
            _LbBackupNames.SelectedIndex = 0;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BackupItem item = _TreeView.SelectedItem as BackupItem;
            if (item != null)
            {
                ModifyDate = item.LastModifyTime.ToShortDateString() + "_" + item.LastModifyTime.ToShortTimeString();
            }
            else
                ModifyDate = "";
        }

        private void Mnu_Restore_Click(object sender, RoutedEventArgs e)
        {
            BackupItem item = _TreeView.SelectedItem as BackupItem;
            if (item != null)
            {
                _Backup.Restore(item);
            }
        }

        private void LbMnu_Restore_Click(object sender, RoutedEventArgs e)
        {
            BackupItem item = _LbBackupNames.SelectedItem as BackupItem;
            if (item != null)
            {
                _Backup.Restore(item);
            }
        }

        private void LbMnu_Delete_Click(object sender, RoutedEventArgs e)
        {
            BackupItem item = _LbBackupNames.SelectedItem as BackupItem;
            if (item != null)
            {
                _Backup.Delete(item);
                BackupNames.Remove(item);
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
