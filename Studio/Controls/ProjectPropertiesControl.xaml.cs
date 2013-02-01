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

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for ProjectPropertiesControl.xaml
    /// </summary>
    public partial class ProjectPropertiesControl : TabDocument
    {
        public ProjectViewModel Project { get; private set; }

        public ProjectPropertiesControl()
        {
            InitializeComponent();
        }


        public ProjectPropertiesControl(ProjectViewModel project)
        {
            this.Project = project;
            this.Project.Settings.History = History;
            this.DataContext = project;
            InitializeComponent();

            SetChanged(false);
            History.Change += new EventHandler(History_Change);
        }

        void History_Change(object sender, EventArgs e)
        {
            SetChanged(History.ChangeCount != 0);
        }

        protected override void ChangeTitle()
        {
            string newTitle = Project.Name + (IsChanged ? "*" : "");
            if (this.Title != newTitle) this.Title = newTitle;
        }

        private void BtnBrowseUnityDir_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Project.Settings.OutputLocaltion;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (System.IO.Directory.Exists(dialog.SelectedPath))
                    {
                        Project.Settings.OutputLocaltion = dialog.SelectedPath;
                        Project.Save();
                    }
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (History.ChangeCount != 0)
                this.Project.Save();
            this.Project.Settings.History = null;
            base.OnClosing(e);
        }

        private void OutputText_Changed(object sender, TextChangedEventArgs e)
        {
            Project.Settings.OutputLocaltion = ((TextBox)sender).Text;
        }

        public override void Save()
        {
            Project.Save();
            base.Save();
        }
    }
}
