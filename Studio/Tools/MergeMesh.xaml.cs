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
using System.Collections.ObjectModel;
using Skill.Fbx;

namespace Skill.Studio.Tools
{
    /// <summary>
    /// Interaction logic for MeshOptimizer.xaml
    /// </summary>
    public partial class MergeMeshTool : Window, INotifyPropertyChanged
    {
        private string _OpenedFilename;

        private bool _IsFileOpened;
        public bool IsFileOpened
        {
            get { return _IsFileOpened; }
            private set
            {
                if (_IsFileOpened != value)
                {
                    _IsFileOpened = value;
                    OnPropertyChanged("IsFileOpened");
                }
            }
        }

        private ObservableCollection<SceneNodeViewModel> _Nodes;
        public ObservableCollection<SceneNodeViewModel> Nodes
        {
            get { return _Nodes; }
            private set
            {
                if (_Nodes != value)
                {
                    _Nodes = value;
                    OnPropertyChanged("Nodes");
                }
            }
        }

        private double _PositionsTelorance = 0.02;
        public double PositionsTelorance
        {
            get { return _PositionsTelorance; }
            set
            {
                if (_PositionsTelorance != value)
                {
                    _PositionsTelorance = value;
                    OnPropertyChanged("PositionsTelorance");
                }
            }
        }

        private double _UvTelorance = 0.01;
        public double UvTelorance
        {
            get { return _UvTelorance; }
            set
            {
                if (_UvTelorance != value)
                {
                    _UvTelorance = value;
                    OnPropertyChanged("UvTelorance");
                }
            }
        }

        private OptimizeScene _Scene = null;
        private SceneNodeViewModel _Root = null;

        public MergeMeshTool()
        {
            InitializeComponent();
            Nodes = new ObservableCollection<SceneNodeViewModel>();
        }

        private void DestroyScene()
        {
            if (_Scene != null)
            {
                _Nodes.Clear();
                _Scene.Destroy();
                _Root = null;
                _Scene = null;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members


        protected override void OnClosed(EventArgs e)
        {
            DestroyScene();
            base.OnClosed(e);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.Filter = "fbx|*.fbx";
            if (open.ShowDialog() == true)
            {
                try
                {
                    DestroyScene();
                    _Scene = new OptimizeScene(open.FileName);
                    _Root = new SceneNodeViewModel(null, _Scene.Root);
                    _Root.IsExpanded = true;
                    _Nodes.Add(_Root);
                    IsFileOpened = true;
                    _OpenedFilename = System.IO.Path.GetFileName(open.FileName);

                }
                catch (Exception ex)
                {
                    IsFileOpened = false;
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnMerge_Click(object sender, RoutedEventArgs e)
        {
            List<SceneNodeViewModel> checkedList = GetCheckedList();

            if (checkedList.Count > 1)
            {
                SceneNode[] arrayNodes = new SceneNode[checkedList.Count];
                for (int i = 0; i < checkedList.Count; i++)
                {
                    arrayNodes[i] = checkedList[i].Model;
                    if (!arrayNodes[i].IsTriangleMesh)
                    {
                        System.Windows.MessageBox.Show("Only meshes in triangulate mode can be merged");
                        return;
                    }
                }

                string newName = string.Format("{0}_{1}{2}", checkedList[0].Name, checkedList[1].Name, checkedList.Count > 2 ? "..." : "");
                SceneNode result = _Scene.Merge(arrayNodes, newName);

                foreach (SceneNodeViewModel item in checkedList)
                {
                    if (item.Parent != null)
                    {
                        item.Parent.Remove(item);
                        ((SceneNodeViewModel)item.Parent).Model.Remove(item.Model);
                    }
                }

                SceneNodeViewModel parent = _Root;

                parent.Model.Add(result);
                parent.Add(new SceneNodeViewModel(parent, result));
                checkedList.Clear();
            }
        }

        void AddCheckedToList(List<SceneNodeViewModel> checkedList, SceneNodeViewModel node)
        {
            if (node.IsChecked) checkedList.Add(node);
            foreach (SceneNodeViewModel child in node)
            {
                AddCheckedToList(checkedList, child);
            }
        }

        private List<SceneNodeViewModel> GetCheckedList()
        {
            List<SceneNodeViewModel> checkedList = new List<SceneNodeViewModel>();
            AddCheckedToList(checkedList, _Root);
            return checkedList;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
            save.Filter = "fbx|*.fbx";
            save.FileName = _OpenedFilename;
            if (save.ShowDialog() == true)
            {
                if (System.IO.File.Exists(save.FileName))
                    System.IO.File.Delete(save.FileName);
                _Scene.Save(save.FileName, _SaveOptions.SaveOptions);
            }
        }

        private void BtnOptimize_Click(object sender, RoutedEventArgs e)
        {
            List<SceneNodeViewModel> checkedList = GetCheckedList();
            foreach (SceneNodeViewModel item in checkedList)
            {
                _Scene.PositionsTelorance = _PositionsTelorance;
                _Scene.UvTelorance = _UvTelorance;
                _Scene.Optimize(item.Model, _GenerateNormals.IsChecked.Value, _GenerateTangents.IsChecked.Value);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnBatchConvert_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowser.WPFFolderBrowserDialog sourceDialog = new WPFFolderBrowser.WPFFolderBrowserDialog();
            if (sourceDialog.ShowDialog() == true)
            {
                WPFFolderBrowser.WPFFolderBrowserDialog destDialog = new WPFFolderBrowser.WPFFolderBrowserDialog();
                destDialog.FileName = sourceDialog.FileName;
                if (destDialog.ShowDialog() == true)
                {
                    string[] files = System.IO.Directory.GetFiles(sourceDialog.FileName, "*.Fbx");
                    ConvertBatch(files, destDialog.FileName);
                }
            }
        }

        private void ConvertBatch(string[] files, string destDir)
        {
            if (files == null || string.IsNullOrEmpty(destDir) || !System.IO.Directory.Exists(destDir))
            {
                System.Windows.MessageBox.Show("Invalid files", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                foreach (var f in files)
                {
                    OptimizeScene scene = new OptimizeScene(f);

                    string name = System.IO.Path.GetFileName(f);
                    string saveFilename = System.IO.Path.Combine(destDir, name);
                    if (System.IO.File.Exists(saveFilename))
                        System.IO.File.Delete(saveFilename);

                    scene.Save(saveFilename, _SaveOptions.SaveOptions);
                    scene.Destroy();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }

    }
}
