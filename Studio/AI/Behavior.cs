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
        #region Brush Properties

        private Brush _BackBrush;
        [Browsable(false)]
        public Brush BackBrush
        {
            get { return _BackBrush != null ? _BackBrush : Editor.BehaviorBrushes.DefaultBackBrush; }
            set
            {
                if (_BackBrush != value)
                {
                    _BackBrush = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("BackBrush"));
                }
            }
        }

        private Brush _BorderBrush;
        [Browsable(false)]
        public Brush BorderBrush
        {
            get
            {
                return _BorderBrush;
            }
            set
            {
                if (_BorderBrush != value)
                {
                    _BorderBrush = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("BorderBrush"));
                }
            }
        }

        private double _ConnectionStroke;
        [Browsable(false)]
        public double ConnectionStroke
        {
            get { return _ConnectionStroke; }
            set
            {
                if (_ConnectionStroke != value)
                {
                    _ConnectionStroke = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ConnectionStroke"));
                }
            }
        }

        private Brush _TextBrush;
        [Browsable(false)]
        public Brush TextBrush
        {
            get
            {
                return _TextBrush;
            }
            set
            {
                if (_TextBrush != value)
                {
                    _TextBrush = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TextBrush"));
                }
            }
        }

        #endregion

        #region Debug

        [Browsable(false)]
        internal DebugBehavior Debug { get; private set; }

        [Browsable(false)]
        internal bool IsDebuging { get { return Tree.IsDebuging; } }

        private bool _IsVisited;
        [Browsable(false)]
        public bool IsVisited
        {
            get { return _IsVisited; }
            set
            {
                if (_IsVisited != value)
                {
                    _IsVisited = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsVisited"));
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsValidable { get { return false; } }

        #endregion

        #region Properties

        [Browsable(false)]
        public virtual double CornerRadius { get { return 8; } }

        [Browsable(false)]
        public virtual double MinHeight { get { return 10; } }

        [Browsable(false)]
        public override bool IsSelected
        {
            get { return base.IsSelected; }
            set
            {
                base.IsSelected = value;
                if (!IsDebuging)
                {
                    BorderBrush = value ? Editor.BehaviorBrushes.SelectedBrush : Editor.BehaviorBrushes.DefaultBorderBrush;
                }
            }
        }

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


        /// <summary> Is this behavior root of state? </summary>
        [Browsable(false)]
        public bool IsState
        {
            get { return Model.IsState; }
            set { Model.IsState = value; }
        }

        private bool _IsDefaultState;
        [Browsable(false)]
        public bool IsDefaultState
        {
            get { return _IsDefaultState; }
            set
            {
                if (_IsDefaultState != value)
                {
                    _IsDefaultState = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsDefaultState"));
                }
            }
        }

        private bool _IsSelectedState;
        [Browsable(false)]
        public bool IsSelectedState
        {
            get { return _IsSelectedState; }
            set
            {
                if (_IsSelectedState != value)
                {
                    _IsSelectedState = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsSelectedState"));
                }
            }
        }


        private PathGeometry _ConnectionToParent;
        [Browsable(false)]
        public PathGeometry ConnectionToParent
        {
            get { return _ConnectionToParent; }
            set
            {
                if (_ConnectionToParent != value)
                {
                    _ConnectionToParent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ConnectionToP"));
                }
            }
        }

        private PathFigure _PathFigure;
        private BezierSegment _BezierSegment;
        private void UpdateConnectionToParent()
        {

            if (_ConnectionToParent == null)
            {
                _PathFigure = new PathFigure();
                _BezierSegment = new BezierSegment();
                _BezierSegment.IsStroked = true;
                _PathFigure.Segments.Add(_BezierSegment);
            }
            BehaviorViewModel parent = Parent as BehaviorViewModel;
            if (parent != null)
            {
                double deltaX = parent.X - this.X + parent.Width;
                double deltaY = (parent.Y + parent.Height * 0.5) - (this.Y + this.Height * 0.5);

                Point targetPosition = new Point(OFFSET, Height * 0.5);
                Point sourcePosition = new Point(deltaX - OFFSET, deltaY + this.Height * 0.5);

                deltaX = System.Math.Abs(targetPosition.X - sourcePosition.X) * 0.5;
                deltaY = System.Math.Abs(targetPosition.Y - sourcePosition.Y) * 0.5;

                Point startBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Right, sourcePosition);
                Point endBezierPoint = Skill.Studio.Diagram.BezierCurve.GetBezierPoint(deltaX, deltaY, Diagram.ConnectorOrientation.Left, targetPosition);

                _PathFigure.StartPoint = sourcePosition;
                _BezierSegment.Point1 = startBezierPoint;
                _BezierSegment.Point2 = endBezierPoint;
                _BezierSegment.Point3 = targetPosition;

                if (_ConnectionToParent == null)
                {
                    PathGeometry pathGeometry = new System.Windows.Media.PathGeometry();
                    pathGeometry.Figures.Add(_PathFigure);
                    this.ConnectionToParent = pathGeometry;
                }
            }

        }

        private static double MINWIDTH = 80;
        private static double MINHEIGHT = 30;
        private static double MARGINLEFT = 50;
        private static double MARGINBOTTOM = 10;
        private static double OFFSET = 2;


        public void UpdatePosition()
        {
            UpdatePosition(MARGINLEFT, 10);
            UpdateConnection();
        }

        private void UpdateConnection()
        {
            this.UpdateConnectionToParent();
            foreach (BehaviorViewModel child in this) if (child != null) child.UpdateConnection();
        }

        private double UpdatePosition(double x, double y)
        {

            X = x;

            double delta;
            if (Count == 0) // this is a leaf node
            {
                delta = Height;
            }
            else
            {
                delta = 0;
                int i = 0;
                foreach (BehaviorViewModel child in this)
                {
                    if (child != null)
                        delta += child.UpdatePosition(x + Width + MARGINLEFT, y + delta) + MARGINBOTTOM;
                    i++;
                }

                if (Count > 1) delta -= MARGINBOTTOM;
            }
            Y = y + (delta - Height) * 0.5;
            return delta;
        }

        [Browsable(false)]
        public string DisplayName
        {
            get
            {
                if (Parent != null && Model.BehaviorType != BehaviorType.Composite)
                    return string.Format("{0} {1}", Name, ((BehaviorViewModel)Parent).GetParametersString(this));
                else
                    return Name;
            }
        }

        public void RaiseChangeDisplayName()
        {
            if (Model.BehaviorType != BehaviorType.Composite)
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
        }

        private Editor.BehaviorBorder _Border;
        [Browsable(false)]
        public Editor.BehaviorBorder Border
        {
            get { return _Border; }
            set
            {
                if (_Border != value)
                {
                    _Border = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Border"));
                }
            }
        }

        [Browsable(false)]
        public double Width
        {
            get
            {
                return _Border != null ? _Border.DesiredSize.Width : MINWIDTH;
            }
        }


        [Browsable(false)]
        public double Height
        {
            get
            {
                return _Border != null ? _Border.DesiredSize.Height : MINHEIGHT;
            }
        }

        private double _X;
        /// <summary> Canvas.Left </summary>        
        [Browsable(false)]
        public double X { get { return _X; } set { if (_X != value) { _X = value; OnPropertyChanged(new PropertyChangedEventArgs("X")); } } }

        private double _Y;
        /// <summary> Canvas.Top </summary>        
        [Browsable(false)]
        public double Y { get { return _Y; } set { if (_Y != value) { _Y = value; OnPropertyChanged(new PropertyChangedEventArgs("Y")); } } }
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
            this._BorderBrush = Editor.BehaviorBrushes.DefaultBorderBrush;
            this._ConnectionStroke = 2;
            this.Model = behavior;
            this.Tree = tree;
            this.Tree.RegisterViewModel(this);
            this.Debug = new DebugBehavior(this);
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
                BehaviorViewModel child = CreateViewModel(Model[i]);
                base.Add(child);
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
                case BehaviorType.ChangeState:
                    return new ChangeStateViewModel(this, (Skill.DataModels.AI.ChangeState)behavior);
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
        public ParameterCollection GetParameters(int index)
        {
            return Model.GetParameters(index);
        }

        /// <summary>
        /// Retrieves parameters for specified behavior
        /// </summary>
        /// <param name="childBehavior">child behavior</param>
        /// <returns>parameters</returns>
        public ParameterCollection GetParameters(BehaviorViewModel childBehavior)
        {
            int index = IndexOf(childBehavior);
            if (index >= 0)
                return GetParameters(index);
            return null;
        }

        /// <summary>
        /// Retrieves parameters for specified behavior
        /// </summary>
        /// <param name="childBehavior">child behavior</param>
        /// <returns>parameters</returns>
        public string GetParametersString(BehaviorViewModel childBehavior)
        {
            int index = IndexOf(childBehavior);
            if (index >= 0)
                return this.Model.GetParameters(index).ToString();
            return "";
        }
        #endregion

        #region Browsable Properties
        [DisplayName("Name")]
        [Description("Name of Behavior.")]
        public virtual string Name
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
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));

                    foreach (var vm in Tree.GetSharedModel(Model))
                    {
                        if (vm != this)
                        {
                            vm.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                            vm.OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
                        }
                    }
                }
            }
        }

        [DefaultValue("")]
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


        [DefaultValue(1)]
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
                    Debug.Behavior.Weight = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Weight"));
                }
            }
        }

        [DefaultValue(false)]
        [DisplayName("Concurrency")]
        [Description("If true : when a condition child fails, return failure")]
        public ConcurrencyMode Concurrency
        {
            get { return Model.Concurrency; }
            set
            {
                if (value != Model.Concurrency)
                {
                    Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Concurrency", value, Model.Concurrency));
                    Model.Concurrency = value;
                    Debug.Behavior.Concurrency = (Framework.AI.ConcurrencyMode)((int)value);
                    OnPropertyChanged(new PropertyChangedEventArgs("Concurrency"));
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
        public BehaviorViewModel AddBehavior(BehaviorViewModel child, ParameterCollection parameters, bool duplicate, int index = -1)
        {
            BehaviorViewModel toAdd = null;
            if (duplicate)
                toAdd = CreateViewModel(child.Model);
            else
                toAdd = child;

            if (index < 0 || index >= Count)
                index = Count;

            if (parameters == null)
                parameters = new ParameterCollection();

            this.Model.Insert(index, toAdd.Model, parameters);
            this.Insert(index, toAdd);

            Tree.RegisterViewModel(toAdd);

            foreach (var vm in Tree.GetSharedModel(Model))
            {
                if (vm != this)
                {
                    BehaviorViewModel newVM = CreateViewModel(child.Model);
                    vm.Insert(index, newVM);
                }
            }

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
            Tree.CreateNewName(behaviorVM);
            return AddBehavior(behaviorVM, null, false, -1);

        }

        /// <summary>
        /// Create new child (ChangeState)
        /// </summary>        
        /// <returns>added child</returns>
        public BehaviorViewModel AddChangeState()
        {
            Skill.DataModels.AI.ChangeState behavior = new Skill.DataModels.AI.ChangeState() { Name = "GoTo State" };
            if (Tree.States.Count > 0)
            {
                behavior.Name = ChangeStateViewModel.CreateName(Tree.States[0].Name);
                behavior.DestinationState = Tree.States[0].Name;
            }
            BehaviorViewModel behaviorVM = CreateViewModel(behavior);
            Tree.CreateNewName(behaviorVM);
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
            Tree.CreateNewName(behaviorVM);
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
                Tree.CreateNewName(behaviorVM);
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
                Tree.CreateNewName(selectorVM);
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
                    this.Model.Move(index, index - 1);
                    foreach (var vm in Tree.GetSharedModel(Model))
                    {
                        if (vm != this)
                        {
                            vm.Move(index, index - 1);
                        }
                    }

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
                    this.Model.Move(index, index + 1);
                    foreach (var vm in Tree.GetSharedModel(Model))
                    {
                        if (vm != this)
                        {
                            vm.Move(index, index + 1);
                        }
                    }
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
            ParameterCollection parameters = null;
            int index = this.IndexOf(child);
            if (this.Remove(child))
            {
                parameters = this.Model.GetParameters(index);
                this.Model.Remove(child.Model);
                foreach (var vm in Tree.GetSharedModel(Model)) // remove child from same behaviors
                {
                    if (vm != this)
                    {
                        foreach (BehaviorViewModel item in vm)
                        {
                            if (item.Model == child.Model)
                            {
                                vm.Remove(item);
                                break;
                            }
                        }
                    }
                }
                Tree.UnRegisterViewModel(child);
                Tree.History.Insert(new AddBehaviorUnDoRedo(child, parameters, this, index, true));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Match parameters when parameters of another viewmodel with shared Behavior model edited by user
        /// </summary>
        /// <param name="parameters">new parameters</param>
        public void MatchParameters(ParameterCollection parameters)
        {
            ParameterCollection myParams = ((BehaviorViewModel)Parent).GetParameters(this);
            foreach (var p in parameters)
            {
                if (myParams.Count(mp => mp.Name == p.Name) == 0)
                    myParams.Add(new Parameter() { Name = p.Name, Type = p.Type, Value = p.Value });
            }

            int index = 0;
            while (index < myParams.Count)
            {
                var mp = myParams[index];
                if (parameters.Count(p => p.Name == mp.Name) == 0)
                {
                    myParams.Remove(mp);
                    continue;
                }
                index++;
            }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return Name;
        }
        #endregion


    }
    #endregion

    #region UnDoRedo helper classes
    class AddBehaviorUnDoRedo : IUnDoRedoCommand
    {
        int _Index;
        BehaviorViewModel _NewNode;
        ParameterCollection _NewParameters;
        BehaviorViewModel _Parent;
        bool _Reverse;
        public AddBehaviorUnDoRedo(BehaviorViewModel newNode, ParameterCollection newParameters, BehaviorViewModel parent, int index, bool reverse = false)
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
