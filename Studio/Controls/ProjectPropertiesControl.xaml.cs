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
            this.DataContext = project;
            InitializeComponent();
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
    }
}
