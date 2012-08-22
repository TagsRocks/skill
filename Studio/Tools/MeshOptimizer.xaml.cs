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
    public partial class MeshOptimizer : Window, INotifyPropertyChanged
    {

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

        public double Threshold
        {
            get { return _Scene.PositionsTelorance; }
            set
            {                
                if ( _Scene.PositionsTelorance != value)
                {
                    _Scene.PositionsTelorance = value;
                    _Scene.UvTelorance = value / 2;
                    OnPropertyChanged("Threshold");
                }
            }
        }

        private OptimizeScene _Scene = null;
        private SceneNodeViewModel _Root = null;

        public MeshOptimizer()
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
                _Scene.Optimize(item.Model, _GenerateNormals.IsChecked.Value, _GenerateTangents.IsChecked.Value);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
