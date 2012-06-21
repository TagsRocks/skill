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
using System.Windows.Threading;

namespace Skill.Studio.Controls
{
    /// <summary>
    /// Interaction logic for ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : AvalonDock.DockableContent
    {

        #region Variables
        EntityNodeViewModel _CuttingNode; // node to cut
        EntityNodeViewModel _CopyingNode; // node to copy
        EntityNodeViewModel _EditingNode; // renamming node        
        TreeViewItem _SelectedItem; // keep reference to selected item whenever selected item changed        
        #endregion


        #region Properties
        private ProjectViewModel _CurrentProject;
        public ProjectViewModel CurrentProject
        {
            get { return _CurrentProject; }
            set
            {
                if (_CurrentProject != value)
                {
                    _CurrentProject = value;
                    RaisePropertyChanged("CurrentProject");
                }
            }
        }
        #endregion

        #region ContextMenu options

        bool _CanOpen;
        public bool CanOpen
        {
            get { return _CanOpen; }
            set { if (value != _CanOpen) { _CanOpen = value; RaisePropertyChanged("CanOpen"); } }
        }
        bool _CanBuild;
        public bool CanBuild
        {
            get { return _CanBuild; }
            set { if (value != _CanBuild) { _CanBuild = value; RaisePropertyChanged("CanBuild"); } }
        }
        bool _CanAdd;
        public bool CanAdd
        {
            get { return _CanAdd; }
            set { if (value != _CanAdd) { _CanAdd = value; RaisePropertyChanged("CanAdd"); } }
        }
        bool _CanCut;
        public bool CanCut
        {
            get { return _CanCut; }
            set { if (value != _CanCut) { _CanCut = value; RaisePropertyChanged("CanCut"); } }
        }
        bool _CanCopy;
        public bool CanCopy
        {
            get { return _CanCopy; }
            set { if (value != _CanCopy) { _CanCopy = value; RaisePropertyChanged("CanCopy"); } }
        }
        bool _CanPaste;
        public bool CanPaste
        {
            get { return _CanPaste; }
            set { if (value != _CanPaste) { _CanPaste = value; RaisePropertyChanged("CanPaste"); } }
        }
        bool _CanDelete;
        public bool CanDelete
        {
            get { return _CanDelete; }
            set { if (value != _CanDelete) { _CanDelete = value; RaisePropertyChanged("CanDelete"); } }
        }
        bool _CanRename;
        public bool CanRename
        {
            get { return _CanRename; }
            set { if (value != _CanRename) { _CanRename = value; RaisePropertyChanged("CanRename"); } }
        }
        bool _CanProperties;
        public bool CanProperties
        {
            get { return _CanProperties; }
            set { if (value != _CanProperties) { _CanProperties = value; RaisePropertyChanged("CanProperties"); } }
        }

        #endregion

        #region Constructor
        public ProjectExplorer()
        {
            InitializeComponent();
            this.DataContext = this;
            CheckContextMenuVisibility();

            _ClickWaitTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Background, MouseWaitTimer_Tick, Dispatcher.CurrentDispatcher);
        }
        #endregion

        #region Open Project
        public void Open(string fileName)
        {
            Project p = Project.Load(fileName);
            if (p != null)
            {
                CurrentProject = new ProjectViewModel(p);
            }
        }
        #endregion

        #region New Projct
        public void New(NewProjectInfo info)
        {
            Project p = new Project();
            CurrentProject = new ProjectViewModel(p);
            p.Settings.OutputLocaltion = info.OutputLocaltion;
            string dir = System.IO.Path.Combine(info.Location, info.Name);
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            CurrentProject.Path = System.IO.Path.Combine(dir, info.Name + Project.Extension);
            CurrentProject.Save();
        }
        #endregion

        #region Reset
        private void ResetEdit()
        {
            if (_EditingNode != null)
                _EditingNode.Editing = false;
            _EditingNode = null;
        }
        private void ResetCut()
        {
            if (_CuttingNode != null)
                _CuttingNode.Cutting = false;
            _CuttingNode = null;
        }
        private void ResetCopy()
        {
            _CopyingNode = null;
        }
        #endregion

        #region SelectedItemChanged



        private void NodesTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetEdit();
            CheckContextMenuVisibility();
        }
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            // set the last tree view item selected variable which may be used elsewhere as there is no other way I have found to obtain the TreeViewItem container (may be null)
            this._SelectedItem = tvi;
        }
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            // focuse item on right click
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                this._SelectedItem = item;
            }
        }

        private DispatcherTimer _ClickWaitTimer;

        private void MouseWaitTimer_Tick(object sender, EventArgs e)
        {
            _ClickWaitTimer.Stop();
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                EnterRename(selected);
                _ForceSelectText = true;
            }
        }

        private int _LastMouseLeftButtonUp;
        private EntityNodeViewModel _LastSelected;
        private void TreeViewItem_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                if (_LastSelected == selected && selected.EntityType != EntityType.Root && e.Timestamp > _LastMouseLeftButtonUp + 4000)
                {
                    _ClickWaitTimer.Start();
                    _LastMouseLeftButtonUp = e.Timestamp;
                }
                _LastSelected = selected;
            }

        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _ClickWaitTimer.Stop();
            _LastSelected = null;
            // if already renamming node ignore double click
            if (_EditingNode != null)
            {
                return;
            }
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                MainWindow.Instance.OpenContent(selected);
                e.Handled = true;
            }

        }

        /// <summary>
        /// Return selected entity
        /// </summary>
        /// <returns>selected entity</returns>
        private EntityNodeViewModel GetSelectedEntity()
        {
            if (_NodesTree.SelectedItem != null)
            {
                if (_NodesTree.SelectedItem is EntityNodeViewModel)
                {
                    return (EntityNodeViewModel)_NodesTree.SelectedItem;
                }
            }
            return null;
        }
        #endregion

        #region CheckContextMenuVisibility
        private void CheckContextMenuVisibility()
        {
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                CanBuild = (selected.EntityType != EntityType.SkinMesh);
                CanAdd = (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root);
                CanOpen = !CanAdd;
                CanCopy = (selected.EntityType != EntityType.Root);
                CanCut = CanCopy;
                CanDelete = CanCopy;
                CanPaste = (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root) &&
                    (_CuttingNode != null || _CopyingNode != null);
                CanProperties = selected.EntityType == EntityType.Root;
                CanRename = (selected.EntityType != EntityType.Root);

            }
            else
            {
                CanOpen = false;
                CanBuild = false;
                CanAdd = false;
                CanCopy = false;
                CanCut = false;
                CanDelete = false;
                CanPaste = false;
                CanProperties = false;
                CanRename = false;
            }
        }
        #endregion


        #region MouseMove
        private bool _ForceSelectText;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_ForceSelectText)
            {
                SelectText();
                _ForceSelectText = false;
            }
        }
        #endregion

        #region KeyDown
        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                // ignore rename
                if (e.Key == Key.Enter || e.Key == Key.Escape)
                {
                    ResetEdit();
                }
                // enable rename
                else if (e.Key == Key.F2)
                {
                    EnterRename(selected);
                }
            }
        }
        #endregion

        #region Build
        private void Mnu_Build_Click(object sender, RoutedEventArgs e)
        {
            EntityNodeViewModel node = GetSelectedEntity();
            if (node != null)
                MainWindow.Instance.BuildNode(node);
        }
        #endregion

        #region Add
        private void Mnu_Add_Click(object sender, RoutedEventArgs e)
        {
            if (CanAdd)
            {
                MenuItem item = sender as MenuItem;
                if (item != null)
                {
                    EntityType newType = (EntityType)item.Tag;
                    var selected = GetSelectedEntity();
                    if (selected != null)
                    {
                        var newNode = selected.AddNode(newType);
                        if (newNode != null)
                        {
                            newNode.IsSelected = true;
                            EnterRename(newNode);
                            _ForceSelectText = true;
                        }
                    }

                }
            }
        }

        private void Mnu_AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (CanAdd)
            {
                if (sender is MenuItem)
                {
                    var selected = GetSelectedEntity();
                    AddNewItem(selected);
                }
            }
        }

        private void AddNewItem(EntityNodeViewModel selected, EntityType? defaultType = null)
        {
            if (selected != null)
            {
                if (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root)
                {
                    string newName = null;
                    EntityType? newType = defaultType;

                    bool? result = false;
                    bool repeat = false;
                    do
                    {
                        repeat = false;
                        AddItemWindow window = new AddItemWindow(newType, newName);
                        result = window.ShowDialog();
                        if (result == true)
                        {
                            newName = window.NewItemName;
                            newType = window.NewItemType.Value;

                            string filename = CurrentProject.GetProjectPath(System.IO.Path.Combine(selected.LocalPath, window.NewItemName + FileExtensions.GetExtension(window.NewItemType.Value)));
                            if (System.IO.File.Exists(filename))
                            {
                                System.Windows.MessageBox.Show("There is an item with same name");
                                repeat = true;
                                result = false;
                            }
                            else
                            {
                                newName = window.NewItemName;
                                newType = window.NewItemType.Value;
                            }
                        }
                    }
                    while (repeat);

                    if (result == true)
                    {
                        var newNode = selected.AddNode(newType.Value, newName);
                        if (newNode != null)
                        {
                            newNode.IsSelected = true;
                            ResetEdit();
                            ResetCut();
                            ResetCopy();
                        }
                    }
                }
            }
        }     
        #endregion

        #region Rename

        private void EnterRename(EntityNodeViewModel selected)
        {
            if (_EditingNode == null && selected.EntityType != EntityType.Root)
            {
                if (_EditingNode != null) _EditingNode.Editing = false;
                _EditingNode = selected;
                selected.Editing = true;
                SelectText();
                ResetCut();
                ResetCopy();
            }
        }

        private void Mnu_Rename_Click(object sender, RoutedEventArgs e)
        {
            if (CanRename)
            {
                var slected = GetSelectedEntity();
                if (slected != null)
                {
                    slected.Editing = true;
                    _EditingNode = slected;
                    SelectText();
                }
            }
        }
        /// <summary>
        /// selected all text of entity name at editing mode
        /// </summary>
        private void SelectText()
        {
            if (this._SelectedItem != null)
            {
                TextBox textBox = FindChild<TextBox>(this._SelectedItem, "_TxtName");
                if (textBox != null)
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        #endregion

        #region Cut
        private void Mnu_Cut_Click(object sender, RoutedEventArgs e)
        {
            if (CanCut)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
                {
                    ResetCut();
                    ResetCopy();
                    _CuttingNode = selected;
                    _CuttingNode.Cutting = true;
                }
            }
        }
        #endregion

        #region Copy
        private void Mnu_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (CanCopy)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
                {
                    ResetCut();
                    ResetCopy();
                    _CopyingNode = selected;
                }
            }
        }
        #endregion

        #region Delete
        private void Mnu_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CanDelete)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
                {
                    selected.Delete();
                    ResetCut();
                    ResetCopy();
                    ResetEdit();
                }
            }
        }
        #endregion

        #region Properties
        private void Mnu_Properties_Click(object sender, RoutedEventArgs e)
        {
            if (CanProperties)
            {
                var selected = GetSelectedEntity();
                if (selected != null && selected.EntityType == EntityType.Root)
                {
                    ApplicationCommands.Properties.Execute(selected, null);
                }
            }
        }
        #endregion

        #region Paste
        private void Mnu_Paste_Click(object sender, RoutedEventArgs e)
        {
            if (_CuttingNode != null)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
                {
                    if (selected.EntityType == EntityType.Root || selected.EntityType == EntityType.Folder)
                    {
                        _CuttingNode.MoveTo(selected);
                    }
                }
            }
            else if (_CopyingNode != null)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
                {
                    if (selected.EntityType == EntityType.Root || selected.EntityType == EntityType.Folder)
                    {
                        _CopyingNode.CopyTo(selected);
                    }
                }
            }

            ResetCut();
            ResetCopy();
            ResetEdit();
        }
        #endregion


        #region Open file

        private void Mnu_Open_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEntity();
            if (selected != null && selected.EntityType != EntityType.Folder && selected.EntityType != EntityType.Root)
            {
                ResetEdit();
                MainWindow.Instance.OpenContent(selected);
            }
        }
        #endregion
    }
}
