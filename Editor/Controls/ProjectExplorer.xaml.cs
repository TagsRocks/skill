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

namespace Skill.Editor.Controls
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
        ProjectExplorerViewModel _ViewModel;// View model for Project Explorer
        TreeViewItem _SelectedItem; // keep reference to selected item whenever selected item changed
        #endregion

        #region Properties
        public ProjectViewModel ProjectVM
        {
            get { return _ViewModel.Project; }
            set
            {
                if (value != _ViewModel.Project)
                {
                    _ViewModel.Project = value;
                }
            }
        }
        #endregion

        #region Constructor
        public ProjectExplorer()
        {
            _ViewModel = new ProjectExplorerViewModel();
            InitializeComponent();
            this.DataContext = _ViewModel;
            CheckContextMenuVisibility();
        }
        #endregion

        #region Open Project
        public void Open(string fileName)
        {
            Project p = Project.Load(fileName);
            if (p != null)
            {
                ProjectVM = new ProjectViewModel(p);
            }
        }
        #endregion

        #region New Projct
        public void New(NewProjectInfo info)
        {
            Project p = new Project();
            ProjectVM = new ProjectViewModel(p);
            p.Settings.UnityProjectLocaltion = info.UnityProjectLocaltion;
            string dir = System.IO.Path.Combine(info.Location, info.Name);
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            ProjectVM.FileName = System.IO.Path.Combine(dir, info.Name + Project.Extension);
            ProjectVM.Save();
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
            }
        }
        private void TreeViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // if already renamming node ignore double click
            if (_EditingNode != null)
            {
                return;
            }
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                MainWindow.Instance.OpenContent(selected.LocalFileName);
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
                _ViewModel.CanBuild = true;
                _ViewModel.CanAdd = (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root);
                _ViewModel.CanCopy = (selected.EntityType != EntityType.Root);
                _ViewModel.CanCut = _ViewModel.CanCopy;
                _ViewModel.CanDelete = _ViewModel.CanCopy;
                _ViewModel.CanPaste = (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root) &&
                    (_CuttingNode != null || _CopyingNode != null);
                _ViewModel.CanProperties = true;
                _ViewModel.CanRename = (selected.EntityType != EntityType.Root);

            }
            else
            {
                _ViewModel.CanBuild = false;
                _ViewModel.CanAdd = false;
                _ViewModel.CanCopy = false;
                _ViewModel.CanCut = false;
                _ViewModel.CanDelete = false;
                _ViewModel.CanPaste = false;
                _ViewModel.CanProperties = false;
                _ViewModel.CanRename = false;
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
                else if (e.Key == Key.F2 && _EditingNode == null)
                {
                    if (selected.EntityType != EntityType.Root)
                    {
                        _EditingNode = selected;
                        selected.Editing = true;
                        SelectText();
                        ResetCut();
                        ResetCopy();
                    }
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
            if (_ViewModel.CanAdd)
            {
                if (sender is MenuItem)
                {
                    var selected = GetSelectedEntity();
                    if (selected != null)
                    {
                        if (selected.EntityType == EntityType.Folder || selected.EntityType == EntityType.Root)
                        {
                            EntityType newType = (EntityType)((MenuItem)sender).Tag;
                            var newNode = selected.AddNode(newType);
                            if (newNode != null)
                            {
                                newNode.IsSelected = true;
                                ResetEdit();
                                _EditingNode = newNode;
                                _EditingNode.Editing = true;
                                _ForceSelectText = true;
                                ResetCut();
                                ResetCopy();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Rename
        private void Mnu_Rename_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel.CanRename)
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
            if (_ViewModel.CanCut)
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
            if (_ViewModel.CanCopy)
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
            if (_ViewModel.CanDelete)
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
            if (_ViewModel.CanProperties)
            {
                var selected = GetSelectedEntity();
                if (selected != null)
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
    }
}
