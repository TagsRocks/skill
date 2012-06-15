using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Skill.DataModels.Animation;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Skill.Studio.Animation
{
    public class AnimationTreeViewModel : INotifyPropertyChanged, IDataViewModel, ICollection<AnimNodeViewModel>
    {
        #region Properties

        public AnimationTree Model { get; private set; }

        /// <summary> Name of tree.</summary>
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public double Scale
        {
            get { return Model.Scale; }
            set
            {
                if (Model.Scale != value)
                {
                    Model.Scale = value;
                    OnPropertyChanged("Scale");
                    Editor.SetChanged(true);
                }
            }
        }

        public double HorizontalOffset
        {
            get { return Model.HorizontalOffset; }
            set
            {
                if (Model.HorizontalOffset != value)
                {
                    Model.HorizontalOffset = value;
                    OnPropertyChanged("HorizontalOffset");
                    Editor.SetChanged(true);
                }
            }
        }

        public double VerticalOffset
        {
            get { return Model.VerticalOffset; }
            set
            {
                if (Model.VerticalOffset != value)
                {
                    Model.VerticalOffset = value;
                    OnPropertyChanged("VerticalOffset");
                    Editor.SetChanged(true);
                }
            }
        }

        public Editor.AnimationTreeEditor Editor { get; set; }

        public AnimationTreeRootViewModel Root { get { return (AnimationTreeRootViewModel)_Nodes.First(n => n.NodeType == AnimNodeType.Root); } }

        public ObservableCollection<AnimNodeViewModel> Nodes { get { return _Nodes; } }

        public ObservableCollection<AnimationConnectionViewModel> Connections { get { return _Connections; } }

        private SkinMeshViewModel _SkinMesh;
        public SkinMeshViewModel SkinMesh
        {
            get
            {
                if (!string.IsNullOrEmpty(Model.SkinMesh))
                {
                    if (MainWindow.Instance.IsProjectLoaded)
                    {
                        SkinMeshNodeViewModel skinMeshNode = MainWindow.Instance.Project.GetNode(Model.SkinMesh) as SkinMeshNodeViewModel;
                        if (skinMeshNode != null)
                        {
                            if (_SkinMesh == null || _SkinMesh.Model != skinMeshNode.SavedData)
                            {
                                Skill.DataModels.Animation.SkinMesh skinMesh = skinMeshNode.SavedData as Skill.DataModels.Animation.SkinMesh;
                                if (skinMesh != null)
                                {
                                    _SkinMesh = new SkinMeshViewModel(skinMesh);
                                }
                            }
                        }
                    }
                }
                else
                    _SkinMesh = null;

                return _SkinMesh;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public AnimationTreeViewModel(AnimationTree model)
        {
            this.Model = model;
            this._Nodes = new ObservableCollection<AnimNodeViewModel>();
            this._Connections = new ObservableCollection<AnimationConnectionViewModel>();

            foreach (var item in model)
            {
                this._Nodes.Add(CreateViewModel(item));
            }
            if (Model.Connections != null)
            {
                foreach (var item in model.Connections)
                {
                    this._Connections.Add(new AnimationConnectionViewModel(this, item));
                }
            }
        }
        #endregion

        #region Connections
        /// <summary> array of connections. create internally when loading file and set before save  </summary>
        private ObservableCollection<AnimationConnectionViewModel> _Connections;

        public void Add(AnimationConnectionViewModel newConnection)
        {
            if (!_Connections.Contains(newConnection))
            {
                // by now it is possible to find one connection to remove because only one connection can connect to each input
                AnimationConnectionViewModel connectionToRemove = null;
                foreach (var connection in this._Connections)
                {
                    if (connection.SinkConnectorIndex == newConnection.SinkConnectorIndex && connection.Sink == newConnection.Sink)
                    {
                        connectionToRemove = connection;
                        break;
                    }
                }

                if (connectionToRemove != null)
                {
                    this.Remove(connectionToRemove);
                }

                _Connections.Add(newConnection);
                if (Editor.History.IsEnable)
                    Editor.History.Insert(new AddAnimNodeUnDoRedo(new AnimNodeViewModel[0], new AnimationConnectionViewModel[] { newConnection }, this));
            }
        }
        public bool Remove(AnimationConnectionViewModel connection)
        {
            bool b = _Connections.Remove(connection);
            if (b)
            {
                Editor.Elements.Remove(connection);
                if (Editor.History.IsEnable)
                    Editor.History.Insert(new AddAnimNodeUnDoRedo(new AnimNodeViewModel[0], new AnimationConnectionViewModel[] { connection }, this, true));
            }
            return b;
        }

        public void RemoveConnectionsFrom(AnimNodeViewModel source)
        {
            foreach (var c in _Connections)
            {
                if (c.Source == source)
                {
                    Remove(c);
                    break;
                }
            }
        }

        public void RemoveConnectionsTo(AnimNodeViewModel sink, int sinkConnectorIndex)
        {
            foreach (var c in _Connections)
            {
                if (c.Sink == sink && c.SinkConnectorIndex == sinkConnectorIndex)
                {
                    Remove(c);
                    break;
                }
            }
        }
        #endregion

        #region ICollection<AnimNodeViewModel>

        ObservableCollection<AnimNodeViewModel> _Nodes;

        public void Add(AnimNodeViewModel item)
        {
            if (!Contains(item))
            {
                _Nodes.Add(item);
                Model.Add(item.Model);

                if (Editor.History.IsEnable)
                    Editor.History.Insert(new AddAnimNodeUnDoRedo(new AnimNodeViewModel[] { item }, new AnimationConnectionViewModel[0], this));
            }
        }

        public void Add(AnimNodeViewModel[] nodes, AnimationConnectionViewModel[] connections)
        {
            foreach (var item in nodes)
            {
                _Nodes.Add(item);
                Model.Add(item.Model);
            }
            foreach (var item in connections)
            {
                _Connections.Add(item);
            }

            if (Editor.History.IsEnable)
                Editor.History.Insert(new AddAnimNodeUnDoRedo(nodes, connections, this));
        }

        public void Clear()
        {
            _Nodes.Clear();
            Model.Clear();
        }

        public bool Contains(AnimNodeViewModel item)
        {
            return _Nodes.Contains(item);
        }

        public void CopyTo(AnimNodeViewModel[] array, int arrayIndex)
        {
            _Nodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(AnimNodeViewModel item)
        {
            bool b = _Nodes.Remove(item);
            if (b)
            {
                Model.Remove(item.Model);
                if (Editor.History.IsEnable)
                    Editor.History.Insert(new AddAnimNodeUnDoRedo(new AnimNodeViewModel[] { item }, new AnimationConnectionViewModel[0], this, true));
            }
            return b;
        }

        public void Remove(AnimNodeViewModel[] nodes, AnimationConnectionViewModel[] connections)
        {
            foreach (var item in nodes)
            {
                _Nodes.Remove(item);
                Model.Remove(item.Model);
                Editor.Elements.Remove(item);
            }
            foreach (var item in connections)
            {
                _Connections.Remove(item);
                Editor.Elements.Remove(item);
            }

            if (Editor.History.IsEnable)
                Editor.History.Insert(new AddAnimNodeUnDoRedo(nodes, connections, this, true));
        }

        public IEnumerator<AnimNodeViewModel> GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Nodes as System.Collections.IEnumerable).GetEnumerator();
        }

        public AnimNodeViewModel Find(int id)
        {
            foreach (var item in this)
            {
                if (item.Id == id) return item;
            }
            return null;
        }
        #endregion

        #region Load

        public AnimNodeViewModel CreateViewModel(AnimNode model)
        {
            AnimNodeViewModel result = null;
            switch (model.NodeType)
            {
                case AnimNodeType.Sequence:
                    result = new AnimNodeSequenceViewModel(this, (AnimNodeSequence)model);
                    break;
                case AnimNodeType.Override:
                    result = new AnimNodeOverrideViewModel(this, (AnimNodeOverride)model);
                    break;
                case AnimNodeType.BlendBySpeed:
                    result = new AnimNodeBlendBySpeedViewModel(this, (AnimNodeBlendBySpeed)model);
                    break;
                case AnimNodeType.BlendByPosture:
                    result = new AnimNodeBlendByPostureViewModel(this, (AnimNodeBlendByPosture)model);
                    break;
                case AnimNodeType.BlendByIdle:
                    result = new AnimNodeBlendByIdleViewModel(this, (AnimNodeBlendByIdle)model);
                    break;
                case AnimNodeType.Blend4Directional:
                    result = new AnimNodeBlend4DirectionalViewModel(this, (AnimNodeBlend4Directional)model);
                    break;
                case AnimNodeType.AimOffset:
                    result = new AnimNodeAimOffsetViewModel(this, (AnimNodeAimOffset)model);
                    break;
                case AnimNodeType.AdditiveBlending:
                    result = new AnimNodeAdditiveBlendingViewModel(this, (AnimNodeAdditiveBlending)model);
                    break;
                case AnimNodeType.Random:
                    result = new AnimNodeRandomViewModel(this, (AnimNodeRandom)model);
                    break;
                case AnimNodeType.BlendByIndex:
                    result = new AnimNodeBlendByIndexViewModel(this, (AnimNodeBlendByIndex)model);
                    break;
                case AnimNodeType.Root:
                    result = new AnimationTreeRootViewModel(this, (AnimationTreeRoot)model);
                    break;
            }

            return result;
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

        #region IDataViewModel members
        public void NotifyEntityChange(EntityType type, string previousPath, string newPath)
        {
        }

        public UnDoRedo History { get; set; }

        public object GetDataModel() { return Model; }

        public void CommiteChanges()
        {
            foreach (var vm in this)
            {
                vm.CommiteChangesToModel();
            }
            Model.Connections = new AnimationConnection[this._Connections.Count];
            for (int i = 0; i < this._Connections.Count; i++)
            {
                Model.Connections[i] = this._Connections[i].Model;
            }
        }
        #endregion

        #region Find
        public AnimNodeViewModel FindByModel(AnimNode model)
        {
            foreach (var node in this)
            {
                if (node.Model == model)
                    return node;
            }
            return null;
        }
        #endregion

        #region GetValidName
        public string GetValidName(string currentName)
        {
            int i = 1;
            string name = currentName;
            while (this.Count(n => n.Name == name) > 0)
            {
                name = currentName + i++;
            }
            return name;
        }
        #endregion


        #region UnDoRedo helper classes
        class AddAnimNodeUnDoRedo : IUnDoRedoCommand
        {
            AnimNodeViewModel[] _NewNodes;
            AnimationConnectionViewModel[] _NewConnections;
            AnimationTreeViewModel _Tree;
            bool _Reverse;

            public AddAnimNodeUnDoRedo(AnimNodeViewModel[] newNodes, AnimationConnectionViewModel[] newConnections, AnimationTreeViewModel tree, bool reverse = false)
            {
                this._NewNodes = newNodes;
                this._NewConnections = newConnections;
                this._Tree = tree;
                this._Reverse = reverse;
            }

            public void Undo()
            {
                if (_Reverse)
                    _Tree.Add(_NewNodes, _NewConnections);
                else
                    _Tree.Remove(_NewNodes, _NewConnections);
            }

            public void Redo()
            {
                if (_Reverse)
                    _Tree.Remove(_NewNodes, _NewConnections);
                else
                    _Tree.Add(_NewNodes, _NewConnections);
            }
        }
        #endregion
    }


}
