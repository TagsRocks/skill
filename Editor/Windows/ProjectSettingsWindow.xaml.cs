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
using Microsoft.Win32;

namespace Skill.Editor
{
    /// <summary>
    /// Interaction logic for ProjectSettingsWindow.xaml
    /// </summary>
    public partial class ProjectSettingsWindow : Window
    {
        ProjectSettings _Model;
        ProjectSettingsViewModel _PreviewSettings;
        internal ProjectSettingsViewModel Settings { get; set; }

        public ProjectSettingsWindow()
        {
            this._Model = new ProjectSettings();
            this._PreviewSettings = new ProjectSettingsViewModel(_Model);
            this.DataContext = _PreviewSettings;
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            if (Settings != null)
                this._PreviewSettings.CopyFrom(Settings);
            base.OnContentRendered(e);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Settings != null)
                this.Settings.CopyFrom(_PreviewSettings);
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnBrowseUnityDir_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = _PreviewSettings.UnityProjectLocaltion;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _PreviewSettings.UnityProjectLocaltion = dialog.SelectedPath;
                }                
            }
        }

    }
}
