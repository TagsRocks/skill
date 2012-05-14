using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Skill.Studio.AI
{
    #region BehaviorTree
    /// <summary>
    /// Defines a Behavior Tree
    /// </summary>
    public class BehaviorTree : IXElement, ICollection<Behavior>
    {
        #region static Variables
        public static string Extension = ".sbt";// extension of editor file
        public static string FilterExtension = "Behavior Tree|*" + Extension;// filter that used by OpenFileDialog 
        #endregion

        #region Properties

        public System.Collections.Generic.Dictionary<string, AccessKey> AccessKeys { get; private set; }
        /// <summary> Root of tree </summary>
        public PrioritySelector Root { get; private set; }
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public BehaviorTree()
        {
            this.AccessKeys = new Dictionary<string, AccessKey>();
            this._Behaviors = new List<Behavior>();
            this.Name = "NewBehaviorTree";
            this.Root = new PrioritySelector();
            this.Root.Name = "Root";
            this.Add(Root);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check whether specyfied behavior is in hierarchy or unused
        /// </summary>
        /// <param name="behavior">Behavior to check</param>
        /// <returns>True if is in hierarchy, otherwise false</returns>
        public bool IsInHierarchy(Behavior behavior)
        {
            return IsInHierarchy(Root, behavior);
        }

        private bool IsInHierarchy(Behavior node, Behavior behavior)
        {
            if (behavior == node) return true;
            foreach (var item in node)
            {
                if (IsInHierarchy(item, behavior))
                    return true;
            }
            return false;
        }
        #endregion

        #region ICollection<Behavior>
        List<Behavior> _Behaviors;
        public void Add(Behavior item)
        {
            if (!Contains(item))
                _Behaviors.Add(item);
        }

        public void Clear()
        {
            _Behaviors.Clear();
        }

        public bool Contains(Behavior item)
        {
            return _Behaviors.Contains(item);
        }

        public void CopyTo(Behavior[] array, int arrayIndex)
        {
            _Behaviors.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Behaviors.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Behavior item)
        {
            return _Behaviors.Remove(item);
        }

        public IEnumerator<Behavior> GetEnumerator()
        {
            return _Behaviors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Behaviors as System.Collections.IEnumerable).GetEnumerator();
        }

        public Behavior Find(int id)
        {
            foreach (var item in this)
            {
                if (item.Id == id) return item;
            }
            return null;
        }
        #endregion

        #region AccessKey
        public void AddAccessKey(AccessKey key)
        {
            if (!string.IsNullOrEmpty(key.Key))
            {
                AccessKeys.Add(key.Key, key);
            }
        }
        public bool RemoveAccessKey(AccessKey key)
        {
            return AccessKeys.Remove(key.Key);
        }
        #endregion


        #region Save
        int _Ids = 0;
        void GenerateIds()
        {
            // set all nodes id to -1
            foreach (Behavior item in this) item.Id = -1;
            _Ids = 0;
            GenerateIds(Root); // set id for nodes that are in hierarchy
            //foreach (Behavior item in this) // then rest of nodes
            //    if (item.Id == -1)
            //        item.Id = _Ids++;
        }
        void GenerateIds(Behavior behavior)
        {
            if (behavior.Id == -1)
                behavior.Id = _Ids++;
            foreach (Behavior item in behavior) GenerateIds(item);
        }

        public XElement ToXElement()
        {
            GenerateIds(); // first regenerate ids for all behaviors
            XElement behaviorTree = new XElement("BehaviorTree");
            behaviorTree.SetAttributeValue("Name", Name);
            behaviorTree.SetAttributeValue("RootId", Root.Id);
            XElement nodes = new XElement("Nodes");
            nodes.SetAttributeValue("Count", Count);
            // write each behavior without children hierarchy
            // children will be writed in array of ids in selector and an id in decorator
            foreach (var item in this)
            {
                if (item.Id > -1)
                {
                    XElement n = item.ToXElement();
                    nodes.Add(n);
                }
            }
            behaviorTree.Add(nodes);

            XElement accessKeys = new XElement("AccessKeys");
            accessKeys.SetAttributeValue("Count", AccessKeys.Count);
            foreach (var item in AccessKeys)
            {
                XElement n = item.Value.ToXElement();
                accessKeys.Add(n);
            }

            behaviorTree.Add(accessKeys);

            return behaviorTree;
        }

        public void Save(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            string name = System.IO.Path.GetFileNameWithoutExtension(fileName);

            int tempPostfix = 0;
            string tempFile = System.IO.Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);

            while (System.IO.File.Exists(tempFile))
            {
                tempPostfix++;
                tempFile = System.IO.Path.Combine(dir, name + "_Temp" + tempPostfix + Extension);
            }

            System.IO.FileStream file = new System.IO.FileStream(tempFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            XDocument document = new XDocument();
            XElement btTtree = ToXElement();
            document.Add(btTtree);
            document.Save(file);
            file.Close();

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);
        }
        #endregion

        #region Load

        private static Behavior CreateBehaviorFrom(XElement behavior)
        {
            Behavior result = null;
            BehaviorType behaviorType;
            if (Enum.TryParse<BehaviorType>(behavior.Name.ToString(), false, out behaviorType))
            {
                switch (behaviorType)
                {
                    case BehaviorType.Action:
                        result = new Action();
                        break;
                    case BehaviorType.Condition:
                        result = new Condition();
                        break;
                    case BehaviorType.Decorator:

                        DecoratorType decoratorType = (DecoratorType)Enum.Parse(typeof(DecoratorType), behavior.GetAttributeValueAsString("DecoratorType", DecoratorType.Default.ToString()), false);
                        switch (decoratorType)
                        {
                            case DecoratorType.Default:
                                result = new Decorator();
                                break;
                            case DecoratorType.AccessLimit:
                                result = new AccessLimitDecorator();
                                break;
                            default:
                                break;
                        }

                        break;
                    case BehaviorType.Composite:
                        CompositeType selectorType = (CompositeType)Enum.Parse(typeof(CompositeType), behavior.GetAttributeValueAsString("CompositeType", ""), false);
                        switch (selectorType)
                        {
                            case CompositeType.Sequence:
                                result = new SequenceSelector();
                                break;
                            case CompositeType.Concurrent:
                                result = new ConcurrentSelector();
                                break;
                            case CompositeType.Random:
                                result = new RandomSelector();
                                break;
                            case CompositeType.Priority:
                                result = new PrioritySelector();
                                break;
                            case CompositeType.Loop:
                                result = new LoopSelector();
                                break;
                        }
                        break;
                }
            }
            return result;
        }

        private static AccessKey CreateAccessKeyFrom(XElement node)
        {
            AccessKey result = null;
            AccessKeyType accessKeyType;
            if (Enum.TryParse<AccessKeyType>(node.Name.ToString(), false, out accessKeyType))
            {
                switch (accessKeyType)
                {
                    case AccessKeyType.CounterLimit:
                        result = new CounterLimitAccessKey();
                        break;
                    case AccessKeyType.TimeLimit:
                        result = new TimeLimitAccessKey();
                        break;
                }
            }
            return result;
        }

        public void Load(XElement e)
        {
            var rootId = e.Attribute("RootId");
            XElement nodes = e.FindChildByName("Nodes");
            if (nodes != null)
            {
                int count = nodes.GetAttributeValueAsInt("Count", 0);
                Clear();
                foreach (var item in nodes.Elements())
                {
                    Behavior b = CreateBehaviorFrom(item);
                    if (b != null)
                    {
                        b.Load(item);
                        this.Add(b);
                    }
                }
            }

            XElement accessKeys = e.FindChildByName("AccessKeys");
            if (accessKeys != null)
            {
                int count = accessKeys.GetAttributeValueAsInt("Count", 0);
                AccessKeys.Clear();
                foreach (var item in accessKeys.Elements())
                {
                    AccessKey ak = CreateAccessKeyFrom(item);
                    if (ak != null)
                    {
                        ak.Load(item);
                        AddAccessKey(ak);
                    }
                }
            }


            // bind children
            if (rootId != null)
            {
                int id = int.Parse(rootId.Value);
                if (id >= 0)
                    Root = Find(id) as PrioritySelector;
            }

            foreach (var node in this)
            {
                switch (node.BehaviorType)
                {
                    case BehaviorType.Decorator:
                        Decorator decorator = node as Decorator;
                        if (decorator.LoadedChildId >= 0)
                            decorator.Add(Find(decorator.LoadedChildId));
                        break;
                    case BehaviorType.Composite:
                        Composite composite = node as Composite;
                        if (composite.LoadedChildrenIds != null)
                        {
                            foreach (var id in composite.LoadedChildrenIds)
                            {
                                Behavior child = Find(id);
                                composite.Add(child);
                            }
                        }
                        break;
                }
            }
        }

        public static BehaviorTree Load(string fileName)
        {
            System.IO.FileStream file = System.IO.File.OpenRead(fileName);
            XDocument document = XDocument.Load(file);
            BehaviorTree tree = new BehaviorTree();
            tree.Load(document.Elements().First());
            tree.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            file.Close();
            return tree;
        }
        #endregion

    }
    #endregion

    #region BehaviorTreeViewModel
    public class BehaviorTreeViewModel : INotifyPropertyChanged
    {
        #region Properties
        /// <summary> list of all actions in tree</summary>
        public ObservableCollection<BehaviorViewModel> Actions { get; private set; }
        /// <summary> list of all conditions in tree</summary>
        public ObservableCollection<BehaviorViewModel> Conditions { get; private set; }
        /// <summary> list of all decorators in tree</summary>
        public ObservableCollection<BehaviorViewModel> Decorators { get; private set; }
        /// <summary> list of all selectors in tree</summary>
        public ObservableCollection<BehaviorViewModel> Composites { get; private set; }

        /// <summary> this list contains root view model as root treeview item</summary>
        public ReadOnlyCollection<BehaviorViewModel> Nodes { get; private set; }
        /// <summary> root of tree</summary>
        public PrioritySelectorViewModel Root { get; private set; }
        /// <summary> BehaviorTree model</summary>
        public BehaviorTree Model { get; private set; }
        /// <summary> History to take care of undo and redo</summary>
        public UnDoRedo History { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of BehaviorTreeViewModel
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
        /// <param name="history">History of TabContent</param>
        public BehaviorTreeViewModel(BehaviorTree tree, UnDoRedo history)
        {
            this.Actions = new ObservableCollection<BehaviorViewModel>();
            this.Conditions = new ObservableCollection<BehaviorViewModel>();
            this.Decorators = new ObservableCollection<BehaviorViewModel>();
            this.Composites = new ObservableCollection<BehaviorViewModel>();
            this.Model = tree;
            this.History = history;
            Root = new PrioritySelectorViewModel(this, tree.Root);
            Nodes = new ReadOnlyCollection<BehaviorViewModel>(new BehaviorViewModel[] { Root });
        }
        #endregion

        #region Register view models


        private bool ContainsModel(ObservableCollection<BehaviorViewModel> list, BehaviorViewModel viewMode)
        {
            return list.Count(b => b.Model == viewMode.Model) > 0;
        }

        int _RegisterCount = -1;
        public void RegisterViewModel(BehaviorViewModel viewModel)
        {
            _RegisterCount++;// root behavior model already added to behavior tree model in constructor of behavior tree
            if (_RegisterCount < 1) return; // cause to do not register root node
            if (viewModel.Tree != this) return;

            //check if we donot register it before add it to list            
            switch (viewModel.Model.BehaviorType)
            {
                case BehaviorType.Action:
                    if (!ContainsModel(this.Actions, viewModel))
                        this.Actions.Add(viewModel);
                    break;
                case BehaviorType.Condition:
                    if (!ContainsModel(this.Conditions, viewModel))
                        this.Conditions.Add(viewModel);
                    break;
                case BehaviorType.Decorator:
                    if (!ContainsModel(this.Decorators, viewModel))
                        this.Decorators.Add(viewModel);
                    break;
                case BehaviorType.Composite:
                    if (!ContainsModel(this.Composites, viewModel))
                        this.Composites.Add(viewModel);
                    break;
            }
            if (!Model.Contains(viewModel.Model))
                Model.Add(viewModel.Model);
        }

        ///// <summary>
        ///// Just clean up view model from invalid data
        ///// </summary>
        ///// <param name="viewModel">viewmodel to unregister</param>
        //public void UnRegisterViewModel(BehaviorViewModel viewModel)
        //{
        //    if (viewModel.Tree != this) return;            

        //    switch (viewModel.Model.BehaviorType)
        //    {
        //        case BehaviorType.Action:
        //            this.Actions.Remove(viewModel);
        //            break;
        //        case BehaviorType.Condition:

        //            this.Conditions.Remove(viewModel);
        //            break;
        //        case BehaviorType.Decorator:

        //            this.Decorators.Remove(viewModel);
        //            break;
        //        case BehaviorType.Composite:
        //            this.Composites.Remove(viewModel);
        //            break;
        //    }
        //    Model.Remove(viewModel.Model);

        //}
        #endregion

        #region CreateNewName
        /// <summary>
        /// Create new name that is unique in tree
        /// </summary>
        /// <param name="behaviorVM">viewmodel to create name for it</param>
        public void CreateNewName(BehaviorViewModel behaviorVM)
        {
            switch (behaviorVM.Model.BehaviorType)
            {
                case BehaviorType.Action:
                    CreateNewName(behaviorVM, Actions);
                    break;
                case BehaviorType.Condition:
                    CreateNewName(behaviorVM, Conditions);
                    break;
                case BehaviorType.Decorator:
                    CreateNewName(behaviorVM, Decorators);
                    break;
                case BehaviorType.Composite:
                    CreateNewName(behaviorVM, Composites);
                    break;
            }
        }
        void CreateNewName(BehaviorViewModel behaviorVM, ObservableCollection<BehaviorViewModel> list)
        {
            int i = 1;
            string name = behaviorVM.Name;
            while (list.Where(b => b.Name == name).Count() > 0)
            {
                name = behaviorVM.Name + i++;
            }
            behaviorVM.Name = name;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion // INotifyPropertyChanged Members
    }
    #endregion
}
