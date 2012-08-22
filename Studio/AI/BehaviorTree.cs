using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Skill.DataModels.AI;

namespace Skill.Studio.AI
{
    #region BehaviorTreeViewModel
    public class BehaviorTreeViewModel : INotifyPropertyChanged, IDataViewModel
    {
        #region Properties

        /// <summary> list of all behaviors in tree</summary>
        public ObservableCollection<BehaviorViewModel> Behaviors { get; private set; }

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
        public UnDoRedo History { get; set; }
        /// <summary> internal AccessKeys </summary>
        public SharedAccessKeysViewModel AccessKeys { get; private set; }
        /// <summary> Whether show parameters of behaviors or not </summary>
        public bool ShowParameters
        {
            get { return Root.ShowParameters; }
            set
            {
                if (Root.ShowParameters != value)
                {
                    Root.ShowParameters = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ShowParameters"));
                }
            }

        }

        /// <summary> Editor owner </summary>
        public Editor.BehaviorTreeEditor Editor { get; set; }

        private bool _IsDebuging;
        [Browsable(false)]
        public bool IsDebuging
        {
            get { return _IsDebuging; }
            set
            {
                if (_IsDebuging != value)
                {
                    _IsDebuging = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsDebuging"));
                }
            }
        }

        private TimeSpan _DebugTimer;
        [Browsable(false)]
        public TimeSpan DebugTimer
        {
            get { return _DebugTimer; }
            set
            {
                _DebugTimer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DebugTimerString"));
            }
        }

        public string DebugTimerString { get { return string.Format("Time : {0:D2}.{1:D2}.{2:D3}", _DebugTimer.Minutes, _DebugTimer.Seconds, _DebugTimer.Milliseconds); } }


        public double Scale
        {
            get { return Model.Scale; }
            set
            {
                if (Model.Scale != value)
                {
                    Model.Scale = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Scale"));
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
                    OnPropertyChanged(new PropertyChangedEventArgs("HorizontalOffset"));
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
                    OnPropertyChanged(new PropertyChangedEventArgs("VerticalOffset"));
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of BehaviorTreeViewModel
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
        /// <param name="history">History of TabContent</param>
        public BehaviorTreeViewModel(BehaviorTree tree)
        {
            this.Behaviors = new ObservableCollection<BehaviorViewModel>();
            this.Actions = new ObservableCollection<BehaviorViewModel>();
            this.Conditions = new ObservableCollection<BehaviorViewModel>();
            this.Decorators = new ObservableCollection<BehaviorViewModel>();
            this.Composites = new ObservableCollection<BehaviorViewModel>();
            this.Model = tree;
            this.Root = new PrioritySelectorViewModel(this, tree.Root);
            this.Nodes = new ReadOnlyCollection<BehaviorViewModel>(new BehaviorViewModel[] { Root });
            this.AccessKeys = new SharedAccessKeysViewModel(Model.AccessKeys);
        }

        #endregion

        #region Register view models

        public List<BehaviorViewModel> GetSharedModel(Behavior model)
        {
            List<BehaviorViewModel> list = new List<BehaviorViewModel>();
            GetSharedModel(list, Root, model);
            return list;
        }
        private void GetSharedModel(List<BehaviorViewModel> list, BehaviorViewModel Vm, Behavior model)
        {
            foreach (BehaviorViewModel item in Vm)
            {
                if (item.Model == model)
                    list.Add(item);
                GetSharedModel(list, item, model);
            }
        }

        private BehaviorViewModel FindViewModelByModel(ObservableCollection<BehaviorViewModel> list, Behavior model)
        {
            foreach (var vm in list)
            {
                if (vm.Model == model)
                    return vm;
            }
            return null;
        }

        int _AvoidRootRegister = 0;
        public void RegisterViewModel(BehaviorViewModel viewModel)
        {
            if (!this.Behaviors.Contains(viewModel))
                this.Behaviors.Add(viewModel);

            _AvoidRootRegister++;
            // root behavior model already added to behavior tree model in constructor of behavior tree
            if (_AvoidRootRegister == 1)
                return;

            if (viewModel.Tree != this) return;

            //check if we donot register it before add it to list            
            switch (viewModel.Model.BehaviorType)
            {
                case BehaviorType.Action:
                    if (FindViewModelByModel(this.Actions, viewModel.Model) == null)
                        this.Actions.Add(viewModel);
                    break;
                case BehaviorType.Condition:
                    if (FindViewModelByModel(this.Conditions, viewModel.Model) == null)
                        this.Conditions.Add(viewModel);
                    break;
                case BehaviorType.Decorator:
                    if (FindViewModelByModel(this.Decorators, viewModel.Model) == null)
                        this.Decorators.Add(viewModel);
                    break;
                case BehaviorType.Composite:
                    if (FindViewModelByModel(this.Composites, viewModel.Model) == null)
                        this.Composites.Add(viewModel);
                    break;
            }


        }

        public void UnRegisterViewModel(BehaviorViewModel viewModel)
        {
            if (!IsInHierarchy(viewModel))
                Behaviors.Remove(viewModel);
        }

        private bool IsInHierarchy(BehaviorViewModel viewModel)
        {
            return IsInHierarchy(Root, viewModel);
        }

        private bool IsInHierarchy(BehaviorViewModel node, BehaviorViewModel viewModel)
        {
            if (node == viewModel) return true;
            foreach (BehaviorViewModel child in node)
            {
                if (IsInHierarchy(child, viewModel))
                    return true;
            }
            return false;
        }
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
            while (list.Where(b => b != behaviorVM && b.Name == name).Count() > 0)
            {
                name = behaviorVM.Name + i++;
            }
            behaviorVM.Model.Name = name;
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

        #region IDataViewModel members
        public void NotifyEntityChange(EntityType type, string previousPath, string newPath)
        {
            bool historyEnable = History.IsEnable;
            History.IsEnable = false;

            // let AccessLimitDecorator know about change address of accesskeys
            foreach (DecoratorViewModel decorator in Decorators)
            {
                if (decorator.DecoratorType == DecoratorType.AccessLimit)
                {
                    AccessLimitDecoratorViewModel al = decorator as AccessLimitDecoratorViewModel;
                    if (al.Address == previousPath)
                        al.Address = newPath;
                }
            }
            History.IsEnable = historyEnable;
        }

        public object GetDataModel()
        {
            return Model;
        }

        public void CommiteChanges()
        {
            this.AccessKeys.CommiteChanges();
        }
        #endregion
    }
    #endregion
}
