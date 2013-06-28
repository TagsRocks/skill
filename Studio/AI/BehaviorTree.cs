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

        /// <summary>
        /// Default state of BehaviorTree
        /// </summary>
        public string DefaultState
        {
            get { return Model.DefaultState; }
            set
            {
                if (Model.DefaultState != value)
                {
                    foreach (var s in States)
                    {
                        if (s.Name == Model.DefaultState)
                        {
                            s.IsDefaultState = false;
                            break;
                        }
                    }
                    Model.DefaultState = value;
                    foreach (var s in States)
                    {
                        if (s.Name == Model.DefaultState)
                        {
                            s.IsDefaultState = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary> list of all states in tree</summary>
        public ObservableCollection<BehaviorViewModel> States { get; private set; }
        /// <summary> list of all behaviors in tree</summary>
        public ObservableCollection<BehaviorViewModel> Behaviors { get; private set; }
        /// <summary> list of all behaviors used in current state</summary>
        public ObservableCollection<BehaviorViewModel> SelectedBehaviors { get; private set; }
        /// <summary> list of all actions in tree</summary>
        public ObservableCollection<BehaviorViewModel> Actions { get; private set; }
        /// <summary> list of all conditions in tree</summary>
        public ObservableCollection<BehaviorViewModel> Conditions { get; private set; }
        /// <summary> list of all decorators in tree</summary>
        public ObservableCollection<BehaviorViewModel> Decorators { get; private set; }
        /// <summary> list of all selectors in tree</summary>
        public ObservableCollection<BehaviorViewModel> Composites { get; private set; }
        /// <summary> list of all changeStates in tree</summary>
        public ObservableCollection<BehaviorViewModel> ChangeStates { get; private set; }
        /// <summary> this list contains root view model as root treeview item</summary>
        public ObservableCollection<BehaviorViewModel> Nodes { get; private set; }

        /// <summary> Root of current state</summary>
        public BehaviorViewModel Root { get { return (Nodes.Count > 0) ? Nodes[0] : null; } }

        /// <summary> BehaviorTree model</summary>
        public BehaviorTree Model { get; private set; }
        /// <summary> History to take care of undo and redo</summary>
        public UnDoRedo History { get; set; }
        /// <summary> internal AccessKeys </summary>
        public SharedAccessKeysViewModel AccessKeys { get; private set; }

        private BehaviorViewModel _PreState;
        public bool ChangeState(string destinationState)
        {
            if (string.IsNullOrEmpty(destinationState) || (Root != null && Root.Name == destinationState)) return false;
            foreach (var b in States)
            {
                if (b.Name == destinationState)
                {
                    if (Nodes.Count > 0)
                    {
                        _PreState = Nodes[0];
                        _PreState.IsSelectedState = false;
                    }
                    Nodes.Clear();
                    Nodes.Add(b);
                    Nodes[0].IsSelectedState = true;

                    if (GraphView != null)
                    {
                        GraphView.RefreshGraph();
                    }
                    if (!IsDebuging && _PreState != null)
                        this.History.Insert(new ChangeStateUnDoRedo(this, Root.Name, (_PreState != null) ? _PreState.Name : string.Empty));
                    return true;
                }
            }
            return false;
        }

        public void NotifyChangeDestinationState(string preDestinationState, string newDestinationState)
        {
            History.IsEnable = false;

            if (DefaultState == preDestinationState)
                DefaultState = newDestinationState;

            foreach (ChangeStateViewModel cs in ChangeStates)
            {
                if (cs.DestinationState == preDestinationState)
                    cs.DestinationState = newDestinationState;
            }
            History.IsEnable = true;
        }

        internal void UpdateSelectedBehaviors()
        {
            SelectedBehaviors.Clear();
            AddSelectedBehaviors(Root);
        }

        private void AddSelectedBehaviors(BehaviorViewModel node)
        {
            if (node != null)
            {
                if (!SelectedBehaviors.Contains(node))
                    SelectedBehaviors.Add(node);
                foreach (BehaviorViewModel vm in node)
                {
                    AddSelectedBehaviors(vm);
                }
            }
        }

        public void SelectDefaultState()
        {
            foreach (var s in States)
            {
                if (s.IsDefaultState)
                {
                    ChangeState(s.Name);
                    return;
                }
            }

            if (States.Count > 0)
            {
                ChangeState(States[0].Name);
            }

        }

        /// <summary> Editor owner </summary>
        public Editor.IBehaviorTreeGraphView GraphView { get; set; }

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

        public string DebugTimerString { get { return string.Format("{0:D2}.{1:D2}.{2:D3}", _DebugTimer.Minutes, _DebugTimer.Seconds, _DebugTimer.Milliseconds); } }


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
            this.States = new ObservableCollection<BehaviorViewModel>();
            this.Behaviors = new ObservableCollection<BehaviorViewModel>();
            this.SelectedBehaviors = new ObservableCollection<BehaviorViewModel>();
            this.Actions = new ObservableCollection<BehaviorViewModel>();
            this.Conditions = new ObservableCollection<BehaviorViewModel>();
            this.Decorators = new ObservableCollection<BehaviorViewModel>();
            this.Composites = new ObservableCollection<BehaviorViewModel>();
            this.ChangeStates = new ObservableCollection<BehaviorViewModel>();
            this.Model = tree;
            this.Nodes = new ObservableCollection<BehaviorViewModel>();

            foreach (var s in tree.States)
            {
                BehaviorViewModel svm = new PrioritySelectorViewModel(this, (PrioritySelector)s);
                // each state register itself
            }
            this.SelectDefaultState();
            this.AccessKeys = new SharedAccessKeysViewModel(Model.AccessKeys);
        }

        #endregion

        #region Register view models

        public List<BehaviorViewModel> GetSharedModel(Behavior model)
        {
            List<BehaviorViewModel> list = new List<BehaviorViewModel>();
            foreach (var s in States)
                GetSharedModel(list, s, model);
            return list;
        }
        private void GetSharedModel(List<BehaviorViewModel> list, BehaviorViewModel Vm, Behavior model)
        {
            foreach (BehaviorViewModel item in Vm)
            {
                if (item.Model == model && !list.Contains(item))
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

        private bool _IgnoreRegister = false;
        public void RegisterViewModel(BehaviorViewModel viewModel)
        {
            if (_IgnoreRegister || viewModel.Tree != this) return;

            if (viewModel.IsState)
            {
                if (FindViewModelByModel(this.States, viewModel.Model) == null)
                    this.States.Add(viewModel);
            }
            else
            {
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
                    case BehaviorType.ChangeState:
                        if (FindViewModelByModel(this.ChangeStates, viewModel.Model) == null)
                            this.ChangeStates.Add(viewModel);
                        break;
                }
            }

            if (FindViewModelByModel(this.Behaviors, viewModel.Model) == null)
                this.Behaviors.Add(viewModel);
        }

        public void UnRegisterViewModel(BehaviorViewModel viewModel)
        {
            UnRegisterRecursive(viewModel);
        }

        private bool IsInHierarchy(BehaviorViewModel viewModel)
        {
            foreach (var s in States)
            {
                if (IsInHierarchy(s, viewModel))
                    return true;
            }
            return false;
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

        //private void RegisterRecursive(BehaviorViewModel viewModel)
        //{
        //    if (!this.Behaviors.Contains(viewModel))
        //    {
        //        this.Behaviors.Add(viewModel);
        //        foreach (BehaviorViewModel child in viewModel)
        //        {
        //            if (child != null)
        //                RegisterRecursive(child);
        //        }
        //    }
        //}

        private void UnRegisterRecursive(BehaviorViewModel viewModel)
        {
            if (viewModel.IsState)
            {
                this.States.Remove(viewModel);
            }
            else if (!IsInHierarchy(viewModel))
            {
                Behaviors.Remove(viewModel);
                foreach (BehaviorViewModel child in viewModel)
                {
                    if (child != null)
                        UnRegisterRecursive(child);
                }
            }
        }
        #endregion

        #region CreateNewName
        /// <summary>
        /// Create new name that is unique in tree
        /// </summary>
        /// <param name="behaviorVM">viewmodel to create name for it</param>
        public void CreateNewName(BehaviorViewModel behaviorVM)
        {
            CreateNewName(behaviorVM, Behaviors);
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

            List<Behavior> states = new List<Behavior>();
            foreach (var s in States)
                states.Add(s.Model);
            this.Model.States = states.ToArray();
        }

        private void OptimizeChangeStates(BehaviorViewModel behavior, List<DataModels.AI.ChangeState> changestateList)
        {
            for (int i = 0; i < behavior.Count; i++)
            {
                BehaviorViewModel bvm = behavior[i] as BehaviorViewModel;

                if (bvm.Model.BehaviorType == BehaviorType.ChangeState)
                {
                    ChangeState cs = (ChangeState)bvm.Model;
                    ChangeState existCS = changestateList.Find(p => p.DestinationState == cs.DestinationState);
                    if (existCS != null)
                        behavior.Model.Replace(cs, existCS);
                    else
                        changestateList.Add(cs);

                }
                else if (bvm.Model.BehaviorType == BehaviorType.Composite || bvm.Model.BehaviorType == BehaviorType.Decorator)
                {
                    OptimizeChangeStates(bvm, changestateList);
                }
            }

        }
        #endregion

        #region State

        public void AddState(BehaviorViewModel state)
        {
            RegisterViewModel(state);
            ChangeState(state.Name);
            History.Insert(new AddStateUnDoRedo(state, false));
        }

        public void RemoveState(BehaviorViewModel state)
        {
            UnRegisterViewModel(state);
            if (_PreState != null)
                ChangeState(_PreState.Name);
            else
                SelectDefaultState();
            History.Insert(new AddStateUnDoRedo(state, true));
        }

        /// <summary>
        /// add new state
        /// </summary>                
        public BehaviorViewModel AddNewState()
        {
            PrioritySelector state = new PrioritySelector();

            _IgnoreRegister = true;
            PrioritySelectorViewModel stateVM = new PrioritySelectorViewModel(this, state) { IsState = true, Name = "NewState" };
            _IgnoreRegister = false;
            CreateNewName(stateVM);
            AddState(stateVM);
            return stateVM;
        }

        class AddStateUnDoRedo : IUnDoRedoCommand
        {
            private bool _Reverse;
            BehaviorViewModel _State;

            public AddStateUnDoRedo(BehaviorViewModel state, bool reverse)
            {
                this._Reverse = reverse;
                this._State = state;
            }

            public void Undo()
            {
                if (this._Reverse)
                    _State.Tree.AddState(_State);
                else
                    _State.Tree.RemoveState(_State);
            }

            public void Redo()
            {
                if (this._Reverse)
                    _State.Tree.RemoveState(_State);
                else
                    _State.Tree.AddState(_State);
            }
        }

        class ChangeStateUnDoRedo : IUnDoRedoCommand
        {

            private BehaviorTreeViewModel _Tree;
            private string _NewState;
            private string _PreState;

            public ChangeStateUnDoRedo(BehaviorTreeViewModel tree, string newState, string preState)
            {
                this._Tree = tree;
                this._NewState = newState;
                this._PreState = preState;
            }

            public void Undo()
            {
                this._Tree.ChangeState(_PreState);
            }

            public void Redo()
            {
                this._Tree.ChangeState(_NewState);
            }
        }

        #endregion
    }
    #endregion
}
