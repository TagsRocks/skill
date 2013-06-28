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
using System.ComponentModel;

namespace Skill.Studio
{

    /// <summary>
    /// Interaction logic for ProjectWizard.xaml
    /// </summary>
    public partial class ProjectWizard : Window
    {
        public class ProjectWizardViewModel : INotifyPropertyChanged
        {
            public NewProjectInfo Info { get; private set; }

            public ProjectWizardViewModel()
            {
                Info = new NewProjectInfo();
            }

            public string Name { get; private set; }

            public string UnityProjectDirectory
            {
                get { return Info.UnityProjectDirectory; }
                set
                {
                    if (Info.UnityProjectDirectory != value)
                    {
                        if (Validation.LocationValidator.IsValid(value))
                            Info.UnityProjectDirectory = value;
                        OnPropertyChanged(new PropertyChangedEventArgs("UnityProjectDirectory"));
                        Name = Info.Name;
                        OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            }
        }

        ProjectWizardViewModel _ViewModel;
        public NewProjectInfo ProjectInfo { get { return _ViewModel.Info; } }

        //string GetDefaultDir()
        //{
        //      System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skill\\Projects\\");
        //}

        public ProjectWizard()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _ViewModel = new ProjectWizardViewModel();
            this.DataContext = _ViewModel;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.LocationValidator.IsValid(_ViewModel.UnityProjectDirectory))
            {
                System.Windows.MessageBox.Show("Invalid directory");
                return;
            }
            if (!Validation.LocationValidator.IsValid(System.IO.Path.Combine(_ViewModel.UnityProjectDirectory, "Assets")))
            {
                var result = System.Windows.MessageBox.Show(this, "Seems not to be a unity project directory - Continue?\n'Assets' directory does not exist.", "Invalid project directory", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                    return;
            }
            if (System.IO.File.Exists(Project.GetFilename(_ViewModel.UnityProjectDirectory, _ViewModel.Name)))
            {
                var result = System.Windows.MessageBox.Show(this, "The project already exists - Overwrite?", "project already exists", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                    return;
                else
                    System.IO.File.Delete(Project.GetFilename(_ViewModel.UnityProjectDirectory, _ViewModel.Name));
            }

            this.DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void Btn_BrowseLocation_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowser.WPFFolderBrowserDialog dialog = new WPFFolderBrowser.WPFFolderBrowserDialog();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastProjectDir) && System.IO.Directory.Exists(Properties.Settings.Default.LastProjectDir))
                dialog.FileName = Properties.Settings.Default.LastProjectDir;
            if (dialog.ShowDialog() == true)
            {
                _ViewModel.UnityProjectDirectory = dialog.FileName;
                Properties.Settings.Default.LastProjectDir = dialog.FileName;
                Properties.Settings.Default.Save();
            }

        }
    }
}
