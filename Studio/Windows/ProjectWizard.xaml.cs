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

            public string Name
            {
                get { return Info.Name; }
                set
                {
                    if (Info.Name != value)
                    {
                        if (value != null)
                            value = value.Trim();
                        if (Validation.VariableNameValidator.IsValid(value))
                            Info.Name = value;
                        OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                    }
                }
            }

            public string Location
            {
                get { return Info.Location; }
                set
                {
                    if (Info.Location != value)
                    {
                        if (Validation.LocationValidator.IsValid(value))
                            Info.Location = value;
                        OnPropertyChanged(new PropertyChangedEventArgs("Location"));
                    }
                }
            }

            public string OutputLocaltion
            {
                get { return Info.OutputLocaltion; }
                set
                {
                    if (Info.OutputLocaltion != value)
                    {
                        if (Validation.LocationValidator.IsValid(value))
                            Info.OutputLocaltion = value;
                        OnPropertyChanged(new PropertyChangedEventArgs("OutputLocaltion"));
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

        string GetDefaultDir()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skill\\Projects\\");
        }

        public ProjectWizard()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _ViewModel = new ProjectWizardViewModel();
            string dir = Properties.Settings.Default.LastProjectDir;
            if (!string.IsNullOrEmpty(dir))
            {
                if (System.IO.Directory.Exists(dir))
                    _ViewModel.Location = dir;
                else
                    _ViewModel.Location = GetDefaultDir();
            }
            else
                _ViewModel.Location = GetDefaultDir();
            _ViewModel.Name = "SkillProject1";
            _ViewModel.OutputLocaltion = "C:\\Unity\\SkillProject1\\Assets\\Scripts\\";
            this.DataContext = _ViewModel;            
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.LocationValidator.IsValid(_ViewModel.Location))
            {
                System.Windows.MessageBox.Show("Invalid location");
                return;
            }
            if (!Validation.LocationValidator.IsValid(_ViewModel.OutputLocaltion))
            {
                System.Windows.MessageBox.Show("Invalid Output Directory");
                return;
            }
            if (!Validation.VariableNameValidator.IsValid(_ViewModel.Name))
            {
                System.Windows.MessageBox.Show("Invalid Name");
                return;
            }

            string projectfilename = ProjectInfo.Filename;
            if (System.IO.File.Exists(projectfilename))
            {
                System.Windows.MessageBox.Show("The project already exists.");
                return;
            }


            Properties.Settings.Default.LastProjectDir = _ViewModel.Location;
            Properties.Settings.Default.Save();
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
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = _ViewModel.Location;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _ViewModel.Location = dialog.SelectedPath;
                }
            }
        }

        private void Btn_BrowseUPlocation_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = _ViewModel.OutputLocaltion;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _ViewModel.OutputLocaltion = dialog.SelectedPath;
                }
            }
        }
    }
}
