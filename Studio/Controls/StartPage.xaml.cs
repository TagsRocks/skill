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



        public RecentProjectInfoCollection RecentProjects { get; private set; }

        public StartPage()
        {
            RecentProjects = new RecentProjectInfoCollection();
            RecentProjects.Load();
            InitializeComponent();

            _CbCloseAfterLoad.IsChecked = Properties.Settings.Default.CloseSPAfterProjectLoad;
            _CbShowOnStartup.IsChecked = Properties.Settings.Default.ShowSPOnStartup;

            _CbCloseAfterLoad.Click += new RoutedEventHandler(_CbCloseAfterLoad_Click);
            _CbShowOnStartup.Click += new RoutedEventHandler(_CbShowOnStartup_Click);
        }

        public void LoadRecents()
        {
            RecentProjects.Load();
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
                    if (RecentProjects.Contains(path))
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
                                RecentProjects.Remove(path);
                                RecentProjects.Save();
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

    public class RecentProjectInfoCollection : ObservableCollection<RecentProjectInfo>
    {
        private static string[] Splitter = new string[] { "||" };


        public void Load()
        {
            this.Clear();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RecentProjects))
            {
                string[] projects = Properties.Settings.Default.RecentProjects.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in projects)
                {
                    //if (System.IO.File.Exists(p))
                    this.Add(p);
                }
            }
        }
        public void Save()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var p in this)
            {
                builder.Append(p.Path);
                builder.Append(Splitter[0]);
            }
            Properties.Settings.Default.RecentProjects = builder.ToString();
            Properties.Settings.Default.Save();
        }
        public void Add(string project)
        {
            if (!Contains(project))
                this.Add(new RecentProjectInfo(System.IO.Path.GetFileNameWithoutExtension(project), project));
        }
        public void AddFirst(string project)
        {
            Remove(project);
            this.Insert(0, new RecentProjectInfo(System.IO.Path.GetFileNameWithoutExtension(project), project));
        }
        public bool Remove(string project)
        {
            RecentProjectInfo pr = null;
            foreach (var p in this)
            {
                if (p.Path.Equals(project, StringComparison.OrdinalIgnoreCase))
                {
                    pr = p;
                    break;
                }
            }
            if (pr != null)
                return this.Remove(pr);
            return false;
        }

        public bool Contains(string project)
        {
            bool exists = false;
            foreach (var p in this)
            {
                if (p.Path.Equals(project, StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }
    }
}
