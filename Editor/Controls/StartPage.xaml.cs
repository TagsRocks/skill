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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : TabDocument
    {


        public ObservableCollection<RecentProjectInfo> RecentProjects { get; private set; }

        public StartPage()
        {
            RecentProjects = new ObservableCollection<RecentProjectInfo>();
            InitializeComponent();

            _CbCloseAfterLoad.IsChecked = Properties.Settings.Default.CloseSPAfterProjectLoad;
            _CbShowOnStartup.IsChecked = Properties.Settings.Default.ShowSPOnStartup;

            _CbCloseAfterLoad.Click += new RoutedEventHandler(_CbCloseAfterLoad_Click);
            _CbShowOnStartup.Click += new RoutedEventHandler(_CbShowOnStartup_Click);

            LoadRecents();
        }

        void LoadRecents()
        {
            RecentProjects.Clear();
            if (Properties.Settings.Default.RecentProjects != null)
            {
                foreach (var rp in Properties.Settings.Default.RecentProjects)
                {
                    RecentProjects.Add(new RecentProjectInfo(System.IO.Path.GetFileNameWithoutExtension(rp), rp));
                }
            }
        }

        void _CbShowOnStartup_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowSPOnStartup = _CbShowOnStartup.IsChecked != null && _CbShowOnStartup.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        void _CbCloseAfterLoad_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CloseSPAfterProjectLoad = _CbCloseAfterLoad.IsChecked != null && _CbCloseAfterLoad.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void RecentItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                string path = item.Tag as string;
                if (path != null)
                {
                    RecentProjectInfo rpInfo = null;

                    foreach (var rp in RecentProjects)
                    {
                        if (rp.Path == path)
                        {
                            rpInfo = rp;
                            break;
                        }
                    }

                    if (rpInfo != null)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            MainWindow.Instance.LoadProject(path);
                        }
                        else
                        {
                            var msgResult = System.Windows.MessageBox.Show("File does not exist \n remove it from list?", "Load", MessageBoxButton.YesNo);
                            if (msgResult == MessageBoxResult.Yes)
                            {
                                if (Properties.Settings.Default.RecentProjects == null)
                                    Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();
                                var recentProjects = Properties.Settings.Default.RecentProjects;

                                recentProjects.Remove(path);
                                RecentProjects.Remove(rpInfo);
                                Properties.Settings.Default.Save();
                            }
                        }
                    }
                }
            }
        }


    }

    public class RecentProjectInfo
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public RecentProjectInfo(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
