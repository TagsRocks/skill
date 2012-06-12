using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Skill.DataModels.AI;
using System.Collections.ObjectModel;

namespace Skill.Studio.AI
{

    #region BehaviorViewModel
    /// <summary>
    /// View model for Behavior
    /// </summary>
    public abstract class BehaviorViewModel : TreeViewItemViewModel
    {
        #region Properties

        private ObservableCollection<ParameterCollectionViewModel> _Parameters;

        #endregion

        #region Properties
        /// <summary>
        /// Actual behavior data
        /// </summary>
        [Browsable(false)]
        public Behavior Model { get; private set; }

        /// <summary>
        /// BehaviorTree that contains this behavior
        /// </summary>
        [Browsable(false)]
        public BehaviorTreeViewModel Tree { get; private set; }


        private PathGeometry _ConnectionToP;
        [Browsable(false)]
        public PathGeometry ConnectionToP
        {
            get
            {
                if (_ConnectionToP == null)
                {
                    BehaviorViewModel parent = Parent as BehaviorViewModel;
                    if (parent != null)
                    {
                        int index = parent.IndexOf(this);
                        double h = 0;

                        for (int i = 0; i < parent.Count; i++)
                        {
                            BehaviorViewModel child = parent[i] as BehaviorViewModel;
                            if (child != null)
                            {
                                if (child == this)
                                {
                                    h += child.Height / 2;
                                    break;
                                }
                                else
                                {
                                    h += child.Height;
                                }
                            }
                        }

                        double y = (parent.Height / 2) - h - MarginBottom;
                        double x = -MarginLeft;
                        double offset = Height / 2;

                        _ConnectionToP = Skill.Studio.Diagram.BezierCurve.GetPathGeometry(new Point(0, offset), new Point(x, y + offset), Diagram.ConnectorOrientation.Left, Diagram.ConnectorOrientation.Right);
                    }
                    else
                        _ConnectionToP = new PathGeometry();
                }
                return _ConnectionToP;
            }
        }

        private static double MarginLeft = 25;
        private static double MarginBottom = 4;
        private static double UnitH = 33;

        private double _Height;
        [Browsable(false)]
        public double Height
        {
            get
            {
                if (_Height == 0)
                {
                    if (Count == 0)
                    {
                        _Height = UnitH;
                    }
                    else
                    {
                        _Height = MarginBottom;
                        foreach (BehaviorViewModel child in this)
                        {
                            if (child != null)
                                _Height += child.Height;
                        }
                    }
                }
                return _Height;
            }
        }

        public void UpdateConnection()
        {
            ResetHeight();
            ResetConnection();
        }

        private void ResetHeight()
        {
            _Height = 0;
            foreach (BehaviorViewModel child in this)
            {
                if (child != null)
                    child.ResetHeight();
            }
        }

        private void ResetConnection()
        {
            if (_ConnectionToP != null)
                _ConnectionToP.Clear();
            _ConnectionToP = null;
            foreach (BehaviorViewModel child in this)
            {
                if (child != null)
                    child.ResetConnection();
            }
            OnPropertyChanged(new PropertyChangedEventArgs("ConnectionToP"));
        }

        [Browsable(false)]
        public string DisplayName
        {
            get
            {
                if (ShowParameters && Parent != null && Model.BehaviorType != BehaviorType.Composite)
                    return string.Format("{0} {1}", Name, ((BehaviorViewModel)Parent).GetParameters(this).ToString());
                else
                    return Name;
            }
        }

        private bool _ShowParameters;
        [Browsable(false)]
        public bool ShowParameters
        {
            get { return _ShowParameters; }

            set
            {
                if (_ShowParameters != value)
                {
                    _ShowParameters = value;
                    foreach (BehaviorViewModel child in this)
                        child.ShowParameters = value;
                    if (Model.BehaviorType != BehaviorType.Composite)
                        OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
                }
            }

        }

        public void RaiseChangeDisplayName()
        {
            if (Model.BehaviorType != BehaviorType.Composite)
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Create an instance of Behavior View Model
        /// </summary>
        /// <param name="tree">Tree that contains behavior</param>
        /// <param name="behavior">behavior model</param>
        public BehaviorViewModel(BehaviorTreeViewModel tree, Behavior behavior)
            : this(tree, null, behavior)
        {
        }
        /// <summary>
        /// Create an instance of Behavior View Model. use Parent.BehaviorTree
        /// </summary>
        /// <param name="parent">parent behavior view model</param>
        /// <param name="behavior">nehavior model</param>
        public BehaviorViewModel(BehaviorViewModel parent, Behavior behavior)
            : this(parent.Tree, parent, behavior)
        {
        }
        private BehaviorViewModel(BehaviorTreeViewModel tree, BehaviorViewModel parent, Behavior behavior)
            : base(parent)
        {
            this._Parameters = new ObservableCollection<ParameterCollectionViewModel>();
            this.Model = behavior;
            this.Tree = tree;
            this.Tree.RegisterViewModel(this);
            LoadChildren();
        }

        /// <summary>
        /// create view models for all children
        /// </summary>
        private void LoadChildren()
        {
            //iterate throw children and create appropriate view model
            for (int i = 0; i < Model.Count; i++)
            {
                base.Add(CreateViewModel(Model[i]));
                _Parameters.Add(new ParameterCollectionViewModel(Model.GetParameters(i)));
            }
        }

        /// <summary>
        /// Create view model based on BehaviorType
        /// </summary>
        /// <param name="behavior">behavior data</param>
        /// <returns>Create view model</returns>
        public BehaviorViewModel CreateViewModel(Behavior behavior)
        {
            switch (behavior.BehaviorType)
            {
                case BehaviorType.Action:
                    return new ActionViewModel(this, (Skill.DataModels.AI.Action)behavior);
                case BehaviorType.Condition:
                    return new ConditionViewModel(this, (Skill.DataModels.AI.Condition)behavior);
                case BehaviorType.Decorator:
                    return CreateDecoratorViewModel((Skill.DataModels.AI.Decorator)behavior);
                case BehaviorType.Composite:
                    return CreateCompositeViewModel((Composite)behavior);
            }
            return null;
        }

        /// <summary>
        /// Create view model based on CompositeType
        /// </summary>
        /// <param name="behavior">selector data</param>
        /// <returns>Create view model</returns>
        DecoratorViewModel CreateDecoratorViewModel(Skill.DataModels.AI.Decorator decorator)
        {
            switch (decorator.Type)
            {
                case DecoratorType.Default:
                    return new DecoratorViewModel(this, decorator);
                case DecoratorType.AccessLimit:
                    return new AccessLimitDecoratorViewModel(this, (AccessLimitDecorator)decorator);
                default:
                    throw new InvalidCastException("Invalid DecoratorType");
            }
        }


        /// <summary>
        /// Create view model based on CompositeType
        /// </summary>
        /// <param name="behavior">selector data</param>
        /// <returns>Create view model</returns>
        CompositeViewModel CreateCompositeViewModel(Composite composite)
        {
            switch (composite.CompositeType)
            {
                case CompositeType.Sequence:
                    return new SequenceSelectorViewModel(this, (SequenceSelector)composite);
                case CompositeType.Concurrent:
                    return new ConcurrentSelectorViewModel(this, (ConcurrentSelector)composite);
                case CompositeType.Random:
                    return new RandomSelectorViewModel(this, (RandomSelector)composite);
                case CompositeType.Priority:
                    return new PrioritySelectorViewModel(this, (PrioritySelector)composite);
                case CompositeType.Loop:
                    return new LoopSelectorViewModel(this, (LoopSelector)composite);
                default:
                    throw new InvalidCastException("Invalid CompositeType");
            }
        }
        #endregion

        #region Parameters
        /// <summary>
        /// Retrieves parameters at specified index
        /// </summary>
        /// <param name="index">index of child behavior</param>
        /// <returns>parametres</returns>
        public ParameterCollectionViewModel GetParameters(int index)
        {
            return _Parameters[index];
        }

        /// <summary>
        /// Retrieves parameters for specified behavior
        /// </summary>
        /// <param name="childBehavior">child behavior</param>
        /// <returns>parameters</returns>
        public ParameterCollectionViewModel GetParameters(BehaviorViewModel childBehavior)
        {
            int index = IndexOf(childBehavior);
            if (index >= 0)
                return _Parameters[index];
            return null;
        }
        #endregion

        #region Browsable Properties
        [DisplayName("Name")]
        [Description("Name of Behavior.")]
        public string Name
        {
            get { return Model.Name; }
            set
            {
                if (value != Model.Name && !string.IsNullOrEmpty(value))
                {
                    if (!Validation.VariableNameValidator.IsValid(value))
                    {
                        MainWindow.Instance.ShowError("Invalid name");
                        return;
                    }

                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    Model.Name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                    if (Model.BehaviorType != BehaviorType.Composite)
                        this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
                }
            }
        }

        [DisplayName("Comment")]
        [Description("User comment for Behavior.")]
        public string Comment
        {
            get { return Model.Comment; }
            set
            {
                if (value != Model.Comment)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
                    Model.Comment = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Comment"));
                }
            }
        }

        [Category("Events")]
        [DisplayName("Success")]
        [Description("If true code generator create an method and hook it to success event")]
        public bool SuccessEvent
        {
            get { return Model.SuccessEvent; }
            set
            {
                if (value != Model.SuccessEvent)
                {
                    Model.SuccessEvent = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SuccessEvent"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "SuccessEvent", value, !value));
                }
            }
        }


        [Category("Events")]
        [DisplayName("Failure")]
        [Description("If true code generator create an method and hook it to failure event")]
        public bool FailureEvent
        {
            get { return Model.FailureEvent; }
            set
            {
                if (value != Model.FailureEvent)
                {
                    Model.FailureEvent = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("FailureEvent"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "FailureEvent", value, !value));
                }
            }
        }

        [Category("Events")]
        [DisplayName("Running")]
        [Description("If true code generator create an method and hook it to running event")]
        public bool RunningEvent
        {
            get { return Model.RunningEvent; }
            set
            {
                if (value != Model.RunningEvent)
                {
                    Model.RunningEvent = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("RunningEvent"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "RunningEvent", value, !value));
                }
            }
        }

        [Category("Events")]
        [DisplayName("Reset")]
        [Description("If true code generator create an method and hook it to reset event")]
        public bool ResetEvent
        {
            get { return Model.ResetEvent; }
            set
            {
                if (value != Model.ResetEvent)
                {
                    Model.ResetEvent = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ResetEvent"));
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "ResetEvent", value, !value));
                }
            }
        }

        [DisplayName("Weight")]
        [Description("Weight of node when behavior is child of a random selector")]
        public float Weight
        {
            get { return Model.Weight; }
            set
            {
                if (value != Model.Weight)
                {
                    if (Weight < 0) { MainWindow.Instance.ShowError("Negative weight is invalid"); return; }
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Weight", value, Model.Weight));
                    Model.Weight = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Weight"));
                }
            }
        }
        #endregion

        #region Editor methods

        /// <summary>
        /// Check whether this node can move up inside parent children
        /// </summary>
        [Browsable(false)]
        public bool CanMoveUp
        {
            get
            {
                if (Parent != null)
                {
                    if (Parent.Count > 0 && Parent[0] != this)
                        return true;
                    return false;

                }
                return false;
            }
        }

        /// <summary>
        /// Check whether this node can move down inside parent children
        /// </summary>
        [Browsable(false)]
        public bool CanMoveDown
        {
            get
            {
                if (Parent != null)
                {
                    if (Parent.Count > 0 && Parent[Parent.Count - 1] != this)
                        return true;
                    return false;

                }
                return false;
            }
        }

        /// <summary>
        /// Check where is there a child that contains given behavior
        /// </summary>
        /// <param name="behavior">Behavior</param>
        /// <returns>true if contains, otherwise false</returns>
        bool Contains(Behavior behavior)
        {
            foreach (BehaviorViewModel item in this)
                if (item.Model == behavior) return true;
            return false;
        }

        private bool CheckAddCauseLoop(BehaviorViewModel newBehavior)
        {
            TreeViewItemViewModel parent = this;
            if (CheckAddCauseLoop(parent, newBehavior))
                return true;
            foreach (BehaviorViewModel item in newBehavior)
            {
                if (CheckAddCauseLoop(item))
                    return true;
            }
            return false;
        }
        private bool CheckAddCauseLoop(TreeViewItemViewModel parent, BehaviorViewModel newBehavior)
        {
            while (parent != null)
            {
                if (((BehaviorViewModel)parent).Model == newBehavior.Model)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }


        public bool CanAddBehavior(BehaviorViewModel child, out string message)
        {
            // actions and conditions are leaves and can not have any child. also decorators can have only one child
            if (this.Model.BehaviorType != BehaviorType.Composite && !(this.Model.BehaviorType == BehaviorType.Decorator && Count == 0))
            {
                message = "Can not add child to this node anymore";
                return false;
            }

            // ignore it if already have this child. check actual model do to duplication
            if (Contains(child.Model))
            {
                message = "Already contains this child";
                return false;
            }

            // check to prevent loop in hierarchy. if a node be twise in hierarchy cause too loop in tree
            if (CheckAddCauseLoop(child))
            {
                message = "Adding this child cause to loop in tree";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Add a child behavior. use this method instead of direct Add method.
        /// this method check additional conditions and take care of history
        /// </summary>
        /// <param name="child">child to add</param>
        /// <param name="duplicate">whther add a duplicated child. when undo or redo we do not need to duplicate child</param>
        /// <param name="index">index to insert child. -1 for add at last</param>
        /// <returns>Actual added child </returns>
        /// <remarks>
        /// we can use one behavior more than once in behavior tree, so create a duplicate from view model
        /// but not from model and add it to tree
        /// </remarks>
        public BehaviorViewModel AddBehavior(BehaviorViewModel child, ParameterCollectionViewModel parameters = null, bool duplicate = true, int index = -1)
        {
            BehaviorViewModel toAdd = null;
            if (duplicate)
                toAdd = CreateViewModel(child.Model);
            else
                toAdd = child;

            if (index < 0 || index >= Count)
                index = Count;

            if (parameters == null)
                parameters = new ParameterCollectionViewModel(new ParameterCollection());

            Tree.CreateNewName(toAdd);

            this.Model.Insert(index, toAdd.Model, parameters.Model);
            this.Insert(index, toAdd);
            _Parameters.Insert(index, parameters);

            Tree.RegisterViewModel(toAdd);
            toAdd.ShowParameters = this.ShowParameters;
            Tree.History.Insert(new AddBehaviorUnDoRedo(toAdd, parameters, this, -1));
            return toAdd;
        }

        /// <summary>
        /// Create new child (action)
        /// </summary>        
        /// <returns>added child</returns>
        public BehaviorViewModel AddAction()
        {
            Behavior behavior = new Skill.DataModels.AI.Action();
            BehaviorViewModel behaviorVM = CreateViewModel(behavior);
            return AddBehavior(behaviorVM, null, false, -1);

        }

        /// <summary>
        /// Create new child (condition)
        /// </summary>        
        /// <returns>added child</returns>
        public BehaviorViewModel AddCondition()
        {
            Behavior behavior = new Skill.DataModels.AI.Condition();
            BehaviorViewModel behaviorVM = CreateViewModel(behavior);
            return AddBehavior(behaviorVM, null, false, -1);
        }


        /// <summary>
        /// Create new child (action,decorator or condition)
        /// </summary>
        /// <param name="behaviorType">type f child</param>
        /// <returns>added child</returns>
        public BehaviorViewModel AddDecorator(DecoratorType decoratorType)
        {
            Behavior behavior = null;
            switch (decoratorType)
            {
                case DecoratorType.Default:
                    behavior = new Skill.DataModels.AI.Decorator();
                    break;
                case DecoratorType.AccessLimit:
                    behavior = new AccessLimitDecorator();
                    break;
            }
            if (behavior != null)
            {
                BehaviorViewModel behaviorVM = CreateViewModel(behavior);
                return AddBehavior(behaviorVM, null, false, -1);
            }
            return null;
        }

        /// <summary>
        /// add new selector child
        /// </summary>
        /// <param name="compositeType">type of child</param>
        /// <returns>added child</returns>
        public BehaviorViewModel AddComposite(CompositeType compositeType)
        {
            Composite composite = null;
            switch (compositeType)
            {
                case CompositeType.Sequence:
                    composite = new SequenceSelector();
                    break;
                case CompositeType.Concurrent:
                    composite = new ConcurrentSelector();
                    break;
                case CompositeType.Random:
                    composite = new RandomSelector();
                    break;
                case CompositeType.Priority:
                    composite = new PrioritySelector();
                    break;
                case CompositeType.Loop:
                    composite = new LoopSelector();
                    break;
            }

            if (composite != null)
            {
                BehaviorViewModel selectorVM = CreateCompositeViewModel(composite);
                return AddBehavior(selectorVM, null, false, -1);
            }
            return null;
        }

        /// <summary>
        /// Move given child up (index - 1)
        /// </summary>
        /// <param name="child">child to move</param>
        public void MoveUp(BehaviorViewModel child)
        {
            if (child.CanMoveUp)
            {
                int index = IndexOf(child);
                if (index > 0 && index < Count)
                {
                    // decrease index one unit
                    this.Move(index, index - 1);
                    this._Parameters.Move(index, index - 1);
                    this.Model.Move(index, index - 1);
                    Tree.History.Insert(new MoveUpBehaviorUnDoRedo(child, this));
                }
            }
        }

        /// <summary>
        /// Move given child down (index + 1)
        /// </summary>
        /// <param name="child">child to move</param>
        public void MoveDown(BehaviorViewModel child)
        {
            if (child.CanMoveDown)
            {
                int index = IndexOf(child);
                if (index >= 0)
                {
                    this.Move(index, index + 1);
                    this._Parameters.Move(index, index + 1);
                    this.Model.Move(index, index + 1);
                    Tree.History.Insert(new MoveUpBehaviorUnDoRedo(child, this, true));
                }
            }
        }

        /// <summary>
        /// Remove specyfied child
        /// </summary>
        /// <param name="child">child to remove</param>
        /// <returns>true if sucess, otherwise false</returns>
        public bool RemoveBehavior(BehaviorViewModel child)
        {
            int index = this.IndexOf(child);
            if (this.Remove(child))
            {
                this.Model.Remove(child.Model);
                Tree.History.Insert(new AddBehaviorUnDoRedo(child, _Parameters[index], this, index, true));
                this._Parameters.RemoveAt(index);
                return true;
            }
            return false;
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return Name + " " + base.ToString();
        }
        #endregion
    }
    #endregion

    #region UnDoRedo helper classes
    class AddBehaviorUnDoRedo : IUnDoRedoCommand
    {
        int _Index;
        BehaviorViewModel _NewNode;
        ParameterCollectionViewModel _NewParameters;
        BehaviorViewModel _Parent;
        bool _Reverse;
        public AddBehaviorUnDoRedo(BehaviorViewModel newNode, ParameterCollectionViewModel newParameters, BehaviorViewModel parent, int index, bool reverse = false)
        {
            this._NewParameters = newParameters;
            this._NewNode = newNode;
            this._Parent = parent;
            this._Reverse = reverse;
            this._Index = index;
        }

        public void Undo()
        {
            if (_Reverse)
                _Parent.AddBehavior(_NewNode, _NewParameters, false, _Index);
            else
                _Parent.RemoveBehavior(_NewNode);
        }

        public void Redo()
        {
            if (_Reverse)
                _Parent.RemoveBehavior(_NewNode);
            else
                _Parent.AddBehavior(_NewNode, _NewParameters, false, _Index);
        }
    }

    class MoveUpBehaviorUnDoRedo : IUnDoRedoCommand
    {
        BehaviorViewModel _MoveNode;
        BehaviorViewModel _Parent;
        bool _Reverse;
        public MoveUpBehaviorUnDoRedo(BehaviorViewModel moveNode, BehaviorViewModel parent, bool reverse = false)
        {
            this._MoveNode = moveNode;
            this._Parent = parent;
            this._Reverse = reverse;
        }

        public void Undo()
        {
            if (_Reverse)
                _Parent.MoveUp(_MoveNode);
            else
                _Parent.MoveDown(_MoveNode);
        }

        public void Redo()
        {
            if (_Reverse)
                _Parent.MoveDown(_MoveNode);
            else
                _Parent.MoveUp(_MoveNode);
        }
    }

    #endregion
}
