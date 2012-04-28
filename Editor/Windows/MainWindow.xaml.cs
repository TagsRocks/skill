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
using System.Collections.ObjectModel;
using AvalonDock;
using System.ComponentModel;
using Microsoft.Win32;

namespace Skill.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Commands
        static RoutedCommand _ShowSettingsCommand = new RoutedCommand();
        public static RoutedCommand ShowSettingsCommand { get { return _ShowSettingsCommand; } }

        static RoutedCommand _SaveAllCommand = new RoutedCommand();
        public static RoutedCommand SaveAllCommand { get { return _SaveAllCommand; } }

        static RoutedCommand _BuildCommand = new RoutedCommand();
        public static RoutedCommand BuildCommand { get { return _BuildCommand; } }

        static RoutedCommand _CheckForErrorsCommand = new RoutedCommand();
        public static RoutedCommand CheckForErrorsCommand { get { return _CheckForErrorsCommand; } }

        #endregion

        public static MainWindow Instance { get; private set; }

        #region Variables
        private Controls.StartPage _StartPage = null;
        #endregion

        #region Properties
        private bool _IsProjectLoaded;
        public bool IsProjectLoaded
        {
            get { return _IsProjectLoaded; }
            set
            {
                if (_IsProjectLoaded != value)
                {
                    _IsProjectLoaded = value;
                    OnPropertyChanged("IsProjectLoaded");
                }
            }
        }

        public ProjectViewModel Project
        {
            get
            {
                if (_ProjectExplorer.ProjectVM != null)
                {
                    return _ProjectExplorer.ProjectVM;
                }
                return null;
            }
        }

        #endregion

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

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            ChangeTitle();
            _DocumentCloseHandler = new EventHandler<CancelEventArgs>(this.TabDocument_Closing);

            if (Properties.Settings.Default.ShowSPOnStartup)
                ShowStartPage();
        }
        #endregion

        #region Start Page
        private void ShowStartPage()
        {
            if (_StartPage != null)
                _StartPage.Show(_DockManager);
            else
            {
                _StartPage = new Controls.StartPage();
                DocumentPane_Center.Items.Add(_StartPage);
                _StartPage.Closing += new EventHandler<CancelEventArgs>(_StartPage_Closing);
            }
        }

        void _StartPage_Closing(object sender, CancelEventArgs e)
        {
            if (sender == _StartPage)
            {
                CloseStartPage();
            }
        }

        void CloseStartPage()
        {
            if (_StartPage != null)
            {
                DocumentPane_Center.Items.Remove(_StartPage);
                _StartPage = null;
            }
        }
        #endregion

        #region Title
        /// <summary>
        /// update title with name of project
        /// </summary>
        private void ChangeTitle()
        {
            if (_ProjectExplorer.ProjectVM != null)
                Title = string.Format("{0} - {1}", _ProjectExplorer.ProjectVM.Model.Name, Properties.Resources.AppTitle);
            else
                Title = string.Format(Properties.Resources.AppTitle);
        }
        #endregion

        #region Exit
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CloseAllDocuments())
                e.Cancel = true;
            base.OnClosing(e);
        }
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool CloseAllDocuments()
        {
            if (Project == null) return true;
            List<string> docNames = new List<string>();
            foreach (var doc in _Documents)
            {
                if (doc.IsChanged)
                {
                    string name = doc.FileName.Replace(Project.Directory, "");
                    docNames.Add(name);
                }
            }
            if (docNames.Count == 0) return true;

            SaveDialog dialog = new SaveDialog();
            dialog.Owner = this;
            foreach (var item in docNames) dialog.AddItem(item);
            dialog.ShowDialog();

            if (dialog.Result == MessageBoxResult.Cancel)
            {
                return false;
            }
            else
            {
                foreach (var doc in _Documents)
                {
                    if (dialog.Result == MessageBoxResult.Yes && doc.IsChanged)
                    {
                        doc.Save();
                    }
                    doc.UnLoad();
                }
                _Documents.Clear();
            }
            return true;
        }
        #endregion


        // Commands        

        #region Recent Projects
        private void AddToRecent(string path)
        {

            if (Properties.Settings.Default.RecentProjects == null)
                Properties.Settings.Default.RecentProjects = new System.Collections.Specialized.StringCollection();
            var recentProjects = Properties.Settings.Default.RecentProjects;

            if (recentProjects.Contains(path))
            {
                recentProjects.Remove(path);
            }
            recentProjects.Insert(0, path);
            Properties.Settings.Default.Save();
        }
        #endregion

        #region New Project
        void NewCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (CloseAllDocuments())
            {
                ProjectWizard wizard = new ProjectWizard();
                wizard.Owner = this;
                wizard.ShowDialog();
                if (wizard.Ok)
                {
                    _ProjectExplorer.New(wizard.ProjectInfo);
                    if (Properties.Settings.Default.CloseSPAfterProjectLoad)
                        CloseStartPage();
                    AddToRecent(wizard.ProjectInfo.Location);
                    IsProjectLoaded = true;
                }
                ChangeTitle();
            }
            e.Handled = true;
        }
        #endregion

        #region Open Project
        void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = Skill.Editor.Project.FilterExtension;
            bool? toOpen = open.ShowDialog(this);

            if (toOpen != null && toOpen.HasValue && toOpen.Value)
            {
                LoadProject(open.FileName);
            }

            e.Handled = true;
        }

        public void LoadProject(string filename)
        {
            if (CloseAllDocuments())
            {
                _ProjectExplorer.Open(filename);
                if (Properties.Settings.Default.CloseSPAfterProjectLoad)
                    CloseStartPage();
                AddToRecent(filename);
                IsProjectLoaded = true;
                ChangeTitle();
            }
        }
        #endregion

        #region Project Settings
        void ShowSettingsCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            e.CanExecute = _ProjectExplorer.ProjectVM != null;
        }
        void ShowSettingsCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (_ProjectExplorer.ProjectVM != null)
            {
                ProjectSettingsWindow projectSettingsWindow = new ProjectSettingsWindow();
                projectSettingsWindow.Owner = this;
                projectSettingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                projectSettingsWindow.Settings = _ProjectExplorer.ProjectVM.ProjectSettingsVM;
                projectSettingsWindow.ShowDialog();
                _ProjectExplorer.ProjectVM.Save();
            }
            e.Handled = true;
        }
        #endregion

        #region Save
        void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            TabDocument doc = GetSelectedDocument();
            if (doc != null)
            {
                e.CanExecute = doc.IsChanged;
            }
            else
                e.CanExecute = false;
        }
        void SaveCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null)
            {
                doc.Save();
            }
            e.Handled = true;
        }
        #endregion

        #region SaveAll
        void SaveAllCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null)
                e.CanExecute = false;
            else
                e.CanExecute = this._ProjectExplorer.ProjectVM != null;
        }
        void SaveAllCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            SaveAllDocuments();
            e.Handled = true;
        }
        void SaveAllDocuments()
        {
            foreach (var item in _Documents)
            {
                item.Save();
            }
        }
        #endregion

        #region Properties
        void PropertiesCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            //e.CanExecute = this._ProjectExplorer.ProjectVM != null;
            e.CanExecute = true;
        }
        void PropertiesCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            _PropertyGrid.SetViewModel(e.Parameter);
            e.Handled = true;
        }
        #endregion

        #region Build
        void BuildCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            e.CanExecute = _ProjectExplorer.ProjectVM != null;
        }
        void BuildCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (_ProjectExplorer.ProjectVM == null) return;
            BuildNode(_ProjectExplorer.ProjectVM.RootVM);
            e.Handled = true;
        }

        public void BuildNode(EntityNodeViewModel vm)
        {
            CheckErrors();
            if (_ErrorList.GetErrorCount(Controls.ErrorType.Error) == 0)
            {
                BackupRestore backup = new BackupRestore();
                backup.CreateAuto();

                try
                {
                    Build(vm);
                    _StatusText.Text = "Build Succeed";
                }
                catch (Exception)
                {
                    _StatusText.Text = "Build Failed";
                }
            }
        }

        private void Build(EntityNodeViewModel vm)
        {
            if (vm.EntityType == EntityType.BehaviorTree)
            {
                string fileName = System.IO.Path.Combine(_ProjectExplorer.ProjectVM.Directory, vm.LocalFileName);
                if (System.IO.File.Exists(fileName))
                {
                    AI.BehaviorTree tree = AI.BehaviorTree.Load(fileName);
                    CG.Builder builder = new CG.Builder(_ProjectExplorer.ProjectVM.Model.Name, _ProjectExplorer.ProjectVM.ProjectSettingsVM.UnityProjectLocaltion);
                    builder.Build(tree, vm.GetLocalDirectory(), vm.Name, _ProjectExplorer.ProjectVM.Model.Settings.UserCodePostfix);
                }
            }
            else if (vm.EntityType == EntityType.AnimationTree)
            {
                string fileName = System.IO.Path.Combine(_ProjectExplorer.ProjectVM.Directory, vm.LocalFileName);
                if (System.IO.File.Exists(fileName))
                {
                    Animation.AnimationTree tree = Animation.AnimationTree.Load(fileName);
                    CG.Builder builder = new CG.Builder(_ProjectExplorer.ProjectVM.Model.Name, _ProjectExplorer.ProjectVM.ProjectSettingsVM.UnityProjectLocaltion);
                    builder.Build(tree, vm.GetLocalDirectory(), vm.Name, _ProjectExplorer.ProjectVM.Model.Settings.UserCodePostfix);
                }
            }
            else
            {
                foreach (var item in vm)
                {
                    Build((EntityNodeViewModel)item);
                }
            }
        }

        void CheckForErrorsCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            e.CanExecute = _ProjectExplorer.ProjectVM != null;
        }
        void CheckForErrorsCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            CheckErrors();
        }

        private void CheckErrors()
        {
            SaveAllDocuments();
            _ErrorList.CheckForErrors();
            _StatusText.Text = "Ready";
        }
        #endregion

        #region Undo
        void UndoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.History.CanUndo;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void UndoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.History.CanUndo)
                {
                    doc.History.Undo();
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Redo
        void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.History.CanRedo;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void RedoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.History.CanRedo)
                {
                    doc.History.Redo();
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Edit
        void CutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.CanCut;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void CutCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.CanCut)
                {
                    doc.Cut();
                    e.Handled = true;
                }
            }
        }

        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.CanCopy;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.CanCopy)
                {
                    doc.Copy();
                    e.Handled = true;
                }
            }
        }

        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.CanPaste;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.CanPaste)
                {
                    doc.Paste();
                    e.Handled = true;
                }
            }
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                e.CanExecute = doc.CanDelete;
                e.Handled = true;
            }
            else
                e.CanExecute = false;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            TabDocument doc = GetSelectedDocument();
            if (doc != null && doc.IsEnabled)
            {
                if (doc.CanDelete)
                {
                    doc.Delete();
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Menu

        private void Mnu_ShowProjectExplorer_Click(object sender, RoutedEventArgs e)
        {
            _ProjectExplorer.Show(_DockManager);
            e.Handled = true;
        }

        private void Mnu_ShowProperties_Click(object sender, RoutedEventArgs e)
        {
            _PropertyGrid.Show(_DockManager);
            e.Handled = true;
        }

        private void Mnu_ShowErrorList_Click(object sender, RoutedEventArgs e)
        {
            _ErrorList.Show(_DockManager);
            e.Handled = true;
        }

        private void Mnu_ShowStartPage_Click(object sender, RoutedEventArgs e)
        {
            ShowStartPage();
            e.Handled = true;
        }

        private void Mnu_About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnuCopySkillDll_Click(object sender, RoutedEventArgs e)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            string destinaion = System.IO.Path.Combine(Project.ProjectSettingsVM.UnityProjectLocaltion, "Skill.dll");

            string filePath1 = System.IO.Path.Combine(appDir, "Skill.dll");

#if DEBUG
            string filePath2 = System.IO.Path.Combine(appDir, "../../../Skill/bin/Debug/Skill.dll");
#else
            string filePath2 = System.IO.Path.Combine(appDir, "../../../Skill/bin/Release/Skill.dll");
#endif

            if (System.IO.File.Exists(filePath1))
            {
                System.IO.File.Copy(filePath1, destinaion, true);
            }
            else if (System.IO.File.Exists(filePath2))
            {
                System.IO.File.Copy(filePath2, destinaion, true);
            }
            else
                System.Windows.Forms.MessageBox.Show("Dll not found");
        }
        #endregion


        #region Documents
        // list of openet documents for edit
        List<TabDocument> _Documents = new List<TabDocument>();
        EventHandler<CancelEventArgs> _DocumentCloseHandler;
        // get selected active document
        TabDocument GetSelectedDocument()
        {
            if (DocumentPane_Center == null) return null;
            if (DocumentPane_Center.SelectedItem != null)
            {
                if (DocumentPane_Center.SelectedItem is TabDocument)
                    return (TabDocument)DocumentPane_Center.SelectedItem;
            }
            return null;
        }
        /// <summary>
        /// Open apropriate document for edit specyfied file
        /// </summary>
        /// <param name="localFilename">local file name in project directory</param>
        public void OpenContent(string localFilename)
        {
            TabDocument docToShow = null;
            foreach (var item in _Documents)
            {
                if (item.FileName.EndsWith(localFilename, StringComparison.OrdinalIgnoreCase))
                {
                    docToShow = item;
                    docToShow.Show(_DockManager);
                    break;
                }
            }

            if (docToShow == null)
            {
                string ext = System.IO.Path.GetExtension(localFilename).ToLower();
                switch (ext)
                {
                    case ".sbt": // extension of BehaviorTree file
                        AI.BehaviorTreeEditor btreeEditor = new AI.BehaviorTreeEditor(System.IO.Path.Combine(_ProjectExplorer.ProjectVM.Directory, localFilename));
                        DocumentPane_Center.Items.Add(btreeEditor);
                        _Documents.Add(btreeEditor);
                        btreeEditor.Closing += _DocumentCloseHandler;
                        break;
                    case ".sat": // extension of AnimationTree file
                        Animation.AnimationTreeEditor aTreeEditor = new Animation.AnimationTreeEditor(System.IO.Path.Combine(_ProjectExplorer.ProjectVM.Directory, localFilename));
                        DocumentPane_Center.Items.Add(aTreeEditor);
                        _Documents.Add(aTreeEditor);
                        aTreeEditor.Closing += _DocumentCloseHandler;
                        break;
                }
            }
            if (docToShow != null)
            {
                DocumentPane_Center.SelectedItem = docToShow;
            }
        }

        /// <summary>
        /// Close apropriate document if already opened
        /// </summary>
        /// <param name="localFilename">local file name in project directory</param>
        public void CloseContent(string localFilename)
        {
            foreach (var item in _Documents)
            {
                if (item.FileName.EndsWith(localFilename, StringComparison.OrdinalIgnoreCase))
                {
                    item.Close();
                    return;
                }
            }
        }

        void TabDocument_Closing(object sender, CancelEventArgs e)
        {
            if (sender is TabDocument)
            {
                TabDocument doc = (TabDocument)sender;
                if (doc.IsChanged)
                {
                    if (!AskForSave(doc))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                doc.Closing -= _DocumentCloseHandler;
                doc.UnLoad();
                _Documents.Remove(doc);
                DocumentPane_Center.Items.Remove(doc);
            }
        }

        /// <summary>
        /// ask user to save document, 
        /// </summary>
        /// <returns>True if user save file, and false if user cancel save</returns>
        private bool AskForSave(TabDocument doc)
        {
            var result = System.Windows.MessageBox.Show(this, "Save changes to " + doc.Title, Properties.Resources.Save,
                   System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.No);
            if (result == System.Windows.MessageBoxResult.Yes)
                doc.Save();
            else if (result == System.Windows.MessageBoxResult.Cancel)
                return false;

            doc.UnLoad();
            return true;
        }

        /// <summary>
        /// Allow openend documents now about change their content filenames
        /// </summary>
        /// <param name="localFileName">local file name</param>
        /// <param name="newName">new name of file</param>
        public void ChangeDocumentName(string localFileName, string newName)
        {
            foreach (var item in _Documents)
            {
                if (item.FileName.EndsWith(localFileName, StringComparison.OrdinalIgnoreCase))
                {
                    item.OnChangeName(newName);
                    return;
                }
            }
        }
        #endregion

        #region ShowError
        /// <summary>
        /// Show an error dialog to user
        /// </summary>
        /// <param name="text">Text to show in dialog</param>
        public void ShowError(string text)
        {
            System.Windows.MessageBox.Show(this, text);
        }
        #endregion

        #region Backup

        private void MnuNewBackup_Click(object sender, RoutedEventArgs e)
        {
            NewBackupWindow backupWindow = new NewBackupWindow();
            backupWindow.Owner = this;
            backupWindow.ShowDialog();
        }


        private void MnuRestore_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindow restoreWindow = new RestoreWindow();
            restoreWindow.Owner = this;
            restoreWindow.ShowDialog();
        }

        #endregion




    }
}
