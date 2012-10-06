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

namespace Skill.Studio
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


        static RoutedCommand _AlignTopCommand = new RoutedCommand();
        public static RoutedCommand AlignTopCommand { get { return _AlignTopCommand; } }

        static RoutedCommand _AlignBottomCommand = new RoutedCommand();
        public static RoutedCommand AlignBottomCommand { get { return _AlignBottomCommand; } }

        static RoutedCommand _AlignLeftCommand = new RoutedCommand();
        public static RoutedCommand AlignLeftCommand { get { return _AlignLeftCommand; } }

        static RoutedCommand _AlignRightCommand = new RoutedCommand();
        public static RoutedCommand AlignRightCommand { get { return _AlignRightCommand; } }

        static RoutedCommand _AlignCenteredHorizontalCommand = new RoutedCommand();
        public static RoutedCommand AlignCenteredHorizontalCommand { get { return _AlignCenteredHorizontalCommand; } }

        static RoutedCommand _AlignCenteredVerticalCommand = new RoutedCommand();
        public static RoutedCommand AlignCenteredVerticalCommand { get { return _AlignCenteredVerticalCommand; } }

        #endregion


        #region Variables

        public static string ProjectAddressToOpen { get; set; }
        public static MainWindow Instance { get; private set; }


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
                ChangeTitle();
            }
        }

        public ProjectViewModel Project
        {
            get
            {
                if (_ProjectExplorer.CurrentProject != null)
                {
                    return _ProjectExplorer.CurrentProject;
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
#if !DEBUG
            this.WindowState = System.Windows.WindowState.Maximized;
#endif
            Instance = this;
            _DocumentCloseHandler = new EventHandler<CancelEventArgs>(this.TabDocument_Closing);

            if (ProjectAddressToOpen != null)
            {
                _ProjectExplorer.Open(ProjectAddressToOpen);
                ProjectAddressToOpen = null;
            }
            else if (Properties.Settings.Default.ShowSPOnStartup)
            {
                ShowStartPage();
            }
            _DockManager.DeserializationCallback = CustomDeserializationCallbackHandler;
            _DockManager.Loaded += new RoutedEventHandler(_DockManager_Loaded);
            _DockManager.DocumentClosed += new EventHandler(_DockManager_DocumentClosed);
            DocumentPane_DocsCenter.SelectionChanged += new SelectionChangedEventHandler(DocumentPane_DocsCenter_SelectionChanged); 
        }

        void DocumentPane_DocsCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_PropertyGrid.SetViewModel(null);
        }

        void _DockManager_DocumentClosed(object sender, EventArgs e)
        {
            _PropertyGrid.SetViewModel(null);
        }

        void _DockManager_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreLayout();
        }

        #endregion

        #region Layout

        private void CustomDeserializationCallbackHandler(object sender, DeserializationCallbackEventArgs e)
        {

            //if (IsProjectLoaded)
            //{
            //    string localFileName = Project.FindFirstLocalFileName(e.Name);
            //    if (localFileName != null)
            //    {
            //        TabDocument doc = OpenContent(localFileName);
            //        e.Content = doc;
            //    }
            //}

            switch (e.Name)
            {
                case "StartPage":
                    CreateStartPage();
                    e.Content = _StartPage;
                    break;
                default:
                    break;
            }
        }

        public string LayoutFile { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skill\\Layout.Xml"); } }

        private void SaveLayout()
        {
            string layoutFilename = LayoutFile;
            string dir = System.IO.Path.GetDirectoryName(layoutFilename);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            _DockManager.SaveLayout(LayoutFile);
        }
        private void RestoreLayout()
        {
            if (IsProjectLoaded)
            {
                if (System.IO.File.Exists(LayoutFile))
                {
                    try
                    {
                        _DockManager.RestoreLayout(LayoutFile);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                        System.IO.File.Delete(LayoutFile);
                    }
                }
            }
        }

        #endregion

        #region Start Page

        private void CreateStartPage()
        {
            if (_StartPage == null)
            {
                _StartPage = new Controls.StartPage() { Name = "StartPage" };
                _StartPage.Closing += new EventHandler<CancelEventArgs>(_StartPage_Closing);
            }
        }

        private void ShowStartPage()
        {
            CreateStartPage();
            if (_StartPage != null)
            {
                _StartPage.LoadRecents();
                DocumentPane_DocsCenter.Items.Add(_StartPage);
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
                DocumentPane_DocsCenter.Items.Remove(_StartPage);
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
            if (IsProjectLoaded)
                Title = string.Format("{0} - {1}", _ProjectExplorer.CurrentProject.Model.Name, Properties.Resources.AppTitle);
            else
                Title = string.Format(Properties.Resources.AppTitle);
        }
        #endregion

        #region Exit
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CloseAllDocuments())
                e.Cancel = true;
            else
                SaveLayout();
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
                    string name = doc.ViewModel.LocalPath;
                    docNames.Add(name);
                }
            }
            bool saveAll = false;
            if (docNames.Count > 0)
            {
                SaveDialog dialog = new SaveDialog();
                dialog.Owner = this;
                foreach (var item in docNames) dialog.AddItem(item);
                dialog.ShowDialog();

                if (dialog.Result == MessageBoxResult.Cancel)
                {
                    return false;
                }
                saveAll = dialog.Result == MessageBoxResult.Yes;
            }

            foreach (var doc in _Documents)
            {
                if (saveAll && doc.IsChanged)
                {
                    doc.Save();
                }
                doc.UnLoad();
            }
            _Documents.Clear();
            ApplicationCommands.Properties.Execute(null, null);
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
            if (_StartPage != null)
                _StartPage.LoadRecents();
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
                if (wizard.DialogResult == true)
                {
                    _ProjectExplorer.New(wizard.ProjectInfo);
                    if (Project != null)
                    {
                        if (Properties.Settings.Default.CloseSPAfterProjectLoad)
                            CloseStartPage();
                        AddToRecent(wizard.ProjectInfo.Filename);
                        IsProjectLoaded = true;
                        CopyRequiredFiles(true);
                    }
                    else
                        IsProjectLoaded = false;
                }
                ApplicationCommands.Properties.Execute(null, null);
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
            open.Filter = Skill.Studio.Project.FilterExtension;
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
                if (Project != null)
                {
                    if (Properties.Settings.Default.CloseSPAfterProjectLoad)
                        CloseStartPage();
                    AddToRecent(filename);
                    if (_StartPage != null)
                        _StartPage.LoadRecents();
                    IsProjectLoaded = true;
                }
                else
                    IsProjectLoaded = false;
                ApplicationCommands.Properties.Execute(null, null);
            }
        }
        #endregion

        #region Project Settings

        private Controls.ProjectPropertiesControl _ProjectProperties;
        void ShowSettingsCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectLoaded;
        }
        void ShowSettingsCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (IsProjectLoaded)
            {
                if (_ProjectProperties == null)
                {
                    _ProjectProperties = new Controls.ProjectPropertiesControl(Project);
                    _ProjectProperties.Closing += new EventHandler<CancelEventArgs>(ProjectProperties_Closing);
                }
            }
            if (!DocumentPane_DocsCenter.Items.Contains(_ProjectProperties))
                DocumentPane_DocsCenter.Items.Add(_ProjectProperties);
            DocumentPane_DocsCenter.SelectedItem = _ProjectProperties;
            e.Handled = true;
        }

        void ProjectProperties_Closing(object sender, CancelEventArgs e)
        {
            if (sender == _ProjectProperties)
            {
                DocumentPane_DocsCenter.Items.Remove(_ProjectProperties);
                _ProjectProperties = null;
            }
        }
        #endregion

        #region Save
        void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsProjectLoaded)
            {
                TabDocument doc = GetSelectedDocument();
                if (doc != null)
                {
                    e.CanExecute = doc.IsChanged;
                }
                else
                    e.CanExecute = false;
            }
        }
        void SaveCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (IsProjectLoaded)
            {
                TabDocument doc = GetSelectedDocument();
                if (doc != null)
                {
                    doc.Save();
                }
                e.Handled = true;
            }
        }
        #endregion

        #region SaveAll
        void SaveAllCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectLoaded;
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
            e.CanExecute = IsProjectLoaded;
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
            e.CanExecute = IsProjectLoaded;
        }
        void BuildCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (!IsProjectLoaded) return;
            BuildNode(Project.Root);
            e.Handled = true;
        }

        public void BuildNode(EntityNodeViewModel vm)
        {
            if (PluginManager.Plugin == null)
            {
                ShowError("Invalid plugin. Please select valid plugin.");
                return;
            }
            CheckErrors();
            if (_ErrorList.GetErrorCount(Skill.Studio.Compiler.ErrorType.Error) == 0)
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
                Builder.Build((Skill.DataModels.AI.BehaviorTree)vm.SavedData, vm.GetLocalDirectory(), vm.Name);
            }
            else if (vm.EntityType == EntityType.SharedAccessKeys)
            {
                Builder.Build((Skill.DataModels.AI.SharedAccessKeys)vm.SavedData, vm.GetLocalDirectory(), vm.Name);
            }
            else if (vm.EntityType == EntityType.AnimationTree)
            {
                Builder.Build((Skill.DataModels.Animation.AnimationTree)vm.SavedData, vm.GetLocalDirectory(), vm.Name);
            }
            else if (vm.EntityType == EntityType.SkinMesh)
            {
                Builder.Build((Skill.DataModels.Animation.SkinMesh)vm.SavedData, vm.GetLocalDirectory(), vm.Name);
            }
            else if (vm.EntityType == EntityType.SaveData)
            {
                Builder.Build((Skill.DataModels.IO.SaveData)vm.SavedData, vm.GetLocalDirectory(), vm.Name);
            }
            else
            {
                foreach (EntityNodeViewModel item in vm)
                {
                    Build(item);
                }
            }
        }

        void CheckForErrorsCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_ProjectExplorer == null) return;
            e.CanExecute = _ProjectExplorer.CurrentProject != null;
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
            e.CanExecute = false;
        }
        void UndoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }
        #endregion

        #region Redo
        void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        void RedoCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }
        #endregion

        #region Edit
        void CutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = false;
        }
        void CutCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }

        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }

        void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        void PasteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
        }
        #endregion

        #region Copy Required Files

        private void CopyRequiredFiles(bool overWrite = false)
        {

#if DEBUG
            CopyEditorFile("Editor", "Skill.Editor.dll", overWrite);
            CopyEditorFile("DataModels", "Skill.DataModels.dll", overWrite);
            CopyFile("Skill", "Skill.dll", overWrite);
            CopyFile("Skill", "Skill.xml", overWrite);
#else
            Skill.CodeGeneration.RequiredFile[] files = PluginManager.Plugin.RequiredFiles;
            if (files != null && files.Length > 0)
            {
                foreach (var rf in files)
                {
                    CopyFile(rf, overWrite);
                }
            }
#endif


        }

        private void CopyFile(Skill.CodeGeneration.RequiredFile rf, bool overWrite)
        {
            string destinaion = Project.GetRequiredFilePath(rf);

            if (System.IO.File.Exists(destinaion) && !overWrite)
                return;

            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(appDir, rf.SourceFile);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    Project.CreateDirectory(destinaion);
                    System.IO.File.Copy(filePath, destinaion, true);
                }
                else if (System.IO.Directory.Exists(filePath))
                {
                    Project.CreateDirectory(destinaion);
                    DirectoryCopy(filePath, destinaion, true);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirName);
            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                return;
                //throw new System.IO.DirectoryNotFoundException(
                //    "Source directory does not exist or could not be found: "
                //    + sourceDirName);
            }

            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
            }

            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        private void CopyFile(string projectName, string filename, bool overWrite)
        {
            string destinaion = Project.GetDesignerOutputPath(filename);

            if (System.IO.File.Exists(destinaion) && !overWrite)
                return;

            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            string name = System.IO.Path.GetFileNameWithoutExtension(filename);

            string filePath = System.IO.Path.Combine(appDir, string.Format("../../../{0}/bin/Debug", projectName), filename);

            Project.CreateDirectory(destinaion);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Copy(filePath, destinaion, true);
            }
        }

        private void CopyEditorFile(string projectName, string filename, bool overWrite)
        {
            string destinaion = Project.GetEditorOutputPath(System.IO.Path.Combine("Skill", filename));

            if (System.IO.File.Exists(destinaion) && !overWrite)
                return;

            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            string name = System.IO.Path.GetFileNameWithoutExtension(filename);

            string filePath = System.IO.Path.Combine(appDir, string.Format("../../../{0}/bin/Debug", projectName), filename);

            Project.CreateDirectory(destinaion);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Copy(filePath, destinaion, true);
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
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void MnuCopyRequiredFiles_Click(object sender, RoutedEventArgs e)
        {
            CopyRequiredFiles(true);
        }

        private void MnuSelectPlugin_Click(object sender, RoutedEventArgs e)
        {
            PluginWindow window = new PluginWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void MnuShowGifAnimations_Click(object sender, RoutedEventArgs e)
        {
            if (IsProjectLoaded)
            {
                GifBrowser browser = new GifBrowser();
                browser.Owner = this;
                browser.ShowDialog();
            }
        }

        private void MnuHToM_Click(object sender, RoutedEventArgs e)
        {
            Skill.Studio.Tools.HeightmapToMesh hToM = new Tools.HeightmapToMesh();
            hToM.Owner = this;
            hToM.ShowDialog();
        }

        private void MnuMeshOpt_Click(object sender, RoutedEventArgs e)
        {
            Skill.Studio.Tools.MergeMeshTool meshOpt = new Tools.MergeMeshTool();
            meshOpt.Owner = this;
            meshOpt.ShowDialog();
        }
        #endregion

        #region Documents
        // list of openet documents for edit
        List<TabDocument> _Documents = new List<TabDocument>();
        EventHandler<CancelEventArgs> _DocumentCloseHandler;
        // get selected active document
        TabDocument GetSelectedDocument()
        {
            if (DocumentPane_DocsCenter == null) return null;
            if (DocumentPane_DocsCenter.SelectedItem != null)
            {
                if (DocumentPane_DocsCenter.SelectedItem is TabDocument)
                    return (TabDocument)DocumentPane_DocsCenter.SelectedItem;
            }
            return null;
        }
        /// <summary>
        /// Open apropriate document for edit specyfied file
        /// </summary>
        /// <param name="localFilename">local file name in project directory</param>
        public TabDocument OpenContent(EntityNodeViewModel vm)
        {
            TabDocument docToShow = null;
            foreach (var item in _Documents)
            {
                if (item.ViewModel == vm)
                {
                    docToShow = item;
                    break;
                }
            }

            if (docToShow == null)
            {
                switch (vm.EntityType)
                {
                    case EntityType.Root:
                        break;
                    case EntityType.Folder:
                        break;
                    case EntityType.BehaviorTree:
                        docToShow = new AI.Editor.BehaviorTreeEditor((BehaviorTreeNodeViewModel)vm);
                        break;
                    case EntityType.SharedAccessKeys:
                        docToShow = new AI.Editor.SharedAccessKeysEditor((SharedAccessKeysNodeViewModel)vm);
                        break;
                    case EntityType.AnimationTree:
                        docToShow = new Animation.Editor.AnimationTreeEditor((AnimationTreeNodeViewModel)vm);
                        break;
                    case EntityType.SkinMesh:
                        docToShow = new Animation.Editor.SkinMeshEditor((SkinMeshNodeViewModel)vm);
                        break;
                    case EntityType.SaveData:
                        docToShow = new IO.Editor.SaveDataEditor((SaveDataNodeViewModel)vm);
                        break;
                    default:
                        break;
                }
                if (docToShow != null)
                {
                    DocumentPane_DocsCenter.Items.Add(docToShow);
                    _Documents.Add(docToShow);
                    docToShow.Closing += _DocumentCloseHandler;
                }
            }
            if (docToShow != null)
            {
                DocumentPane_DocsCenter.SelectedItem = docToShow;
                docToShow.Show(_DockManager);
            }
            return docToShow;
        }

        /// <summary>
        /// Close apropriate document if already opened
        /// </summary>
        /// <param name="localFilename">local file name in project directory</param>
        public void CloseDocument(EntityNodeViewModel vm)
        {
            foreach (TabDocument item in _Documents)
            {
                if (item.ViewModel == vm)
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
                DocumentPane_DocsCenter.Items.Remove(doc);
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
