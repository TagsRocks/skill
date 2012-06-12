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
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of BehaviorTreeViewModel
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
        /// <param name="history">History of TabContent</param>
        public BehaviorTreeViewModel(BehaviorTree tree)
        {
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

        private bool ContainsModel(ObservableCollection<BehaviorViewModel> list, BehaviorViewModel viewMode)
        {
            return list.Count(b => b.Model == viewMode.Model) > 0;
        }

        public void RegisterViewModel(BehaviorViewModel viewModel)
        {
            // root behavior model already added to behavior tree model in constructor of behavior tree
            if (viewModel == Root)
                return;

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

        }
        #endregion
    }
    #endregion
}
