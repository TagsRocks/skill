using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Skill.Editor.AI
{
    #region BehaviorType
    /// <summary>
    /// Defines types of behaviors
    /// </summary>
    public enum BehaviorType
    {
        Action,
        Condition,
        Decorator,
        Composite,
    }
    #endregion

    #region Behavior
    /// <summary>
    /// Base class for all behaviors
    /// </summary>
    public abstract class Behavior : IXElement, ICollection<Behavior>
    {
        #region Properties
        /// <summary> Returns type of behavior. all subclass must implement this properties </summary>
        public abstract BehaviorType BehaviorType { get; }

        /// <summary> Name of behavior </summary>
        public virtual string Name { get; set; }

        /// <summary> Id of behavior </summary>
        public int Id { get; set; }

        /// <summary> If true code generator create an method and hook it to success event </summary>
        public bool SuccessEvent { get; set; }

        /// <summary> If true code generator create an method and hook it to failure event </summary>
        public bool FailureEvent { get; set; }

        /// <summary> If true code generator create an method and hook it to running event </summary>
        public bool RunningEvent { get; set; }

        /// <summary> Weight of node when behavior is child of a random selector </summary>
        public float Weight { get; set; }

        /// <summary> User comment for this behavior </summary>
        public string Comment { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creatre an instance of behavior
        /// </summary>
        /// <param name="name">Name of behavior</param>
        public Behavior(string name)
        {
            _Children = new List<Behavior>();
            this.Name = name;
            this.Weight = 1;
        }
        #endregion

        #region Helper save methods
        /// <summary>
        /// convert back children index string to array of int. used in saving and loading behavior
        /// </summary>
        public static int[] ConvertToIndices(string childrenString)
        {
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(childrenString))
            {
                string[] splip = childrenString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splip.Length; i++)
                {
                    list.Add(int.Parse(splip[i]));
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// convert index of children to string that seperated with ,
        /// </summary>
        /// <returns>Index of chilren</returns>
        public string GetChildrenString()
        {
            string childrenString = "";
            for (int i = 0; i < Count; i++)
            {
                childrenString += this[i].Id.ToString("D");
                if (i < Count - 1)
                    childrenString += ",";
            }
            return childrenString;
        }
        #endregion

        #region Save
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        /// <summary>
        /// Create a XElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement behavior = new XElement(BehaviorType.ToString());
            behavior.SetAttributeValue("Name", Name);
            behavior.SetAttributeValue("Id", Id);
            XElement events = new XElement("Events");
            events.SetAttributeValue("Failure", FailureEvent);
            events.SetAttributeValue("Success", SuccessEvent);
            events.SetAttributeValue("Running", RunningEvent);

            if (!string.IsNullOrEmpty(Comment))
            {
                XElement comment = new XElement("Comment");
                comment.SetValue(Comment);
                behavior.Add(comment);
            }
            behavior.Add(events);
            WriteAttributes(behavior); // allow subclass to add additional data
            return behavior;
        }
        #endregion

        #region Load

        protected static XElement FindChild(XElement e, string name)
        {
            foreach (var item in e.Elements().Where(p => p.Name == name))
                return item;
            return null;
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XElement e) { }

        /// <summary>
        /// Load behavior data
        /// </summary>
        /// <param name="e">contains behavior data</param>
        public void Load(XElement e)
        {
            Name = e.Attribute("Name").Value;
            Id = int.Parse(e.Attribute("Id").Value);

            XElement events = FindChild(e, "Events");
            if (events != null)
            {
                var failure = events.Attribute("Failure");
                var success = events.Attribute("Success");
                var running = events.Attribute("Running");
                if (failure != null) FailureEvent = bool.Parse(failure.Value);
                if (success != null) SuccessEvent = bool.Parse(success.Value);
                if (running != null) RunningEvent = bool.Parse(running.Value);
            }

            XElement comment = FindChild(e, "Comment");
            if (comment != null)
            {
                Comment = comment.Value;
            }

            ReadAttributes(e);// allow subclass to read additional data
        }
        #endregion

        #region ICollection<Behavior> methods

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            var item = this._Children[oldIndex];
            this._Children.RemoveAt(oldIndex);
            this._Children.Insert(newIndex, item);
        }

        /// <summary>
        /// Retrieves child by index
        /// </summary>
        /// <param name="index">index of child</param>
        /// <returns>child at given index</returns>
        public Behavior this[int index]
        {
            get { return _Children[index]; }
        }

        private List<Behavior> _Children; // list of children

        /// <summary>
        /// Add a child behavior.
        /// </summary>
        /// <param name="item">behavior to add</param>
        /// <remarks>we don't control type of behavior here since controlled in ViewModel</remarks>
        public void Add(Behavior item)
        {
            _Children.Add(item);
        }

        /// <summary>
        /// Remove all children
        /// </summary>
        public void Clear()
        {
            _Children.Clear();
        }

        /// <summary>
        /// Return true if contains given child
        /// </summary>
        /// <param name="item">child to check</param>
        /// <returns>true if contains, otherwise false</returns>
        public bool Contains(Behavior item)
        {
            return _Children.Contains(item);
        }

        /// <summary>
        /// Copy children to an array
        /// </summary>
        /// <param name="array">array to fill</param>
        /// <param name="arrayIndex">start index in array to fill</param>
        public void CopyTo(Behavior[] array, int arrayIndex)
        {
            _Children.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Count of children
        /// </summary>
        public int Count
        {
            get { return _Children.Count; }
        }

        /// <summary>
        /// Collection is not readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove given child
        /// </summary>
        /// <param name="item">child to remove</param>
        /// <returns></returns>
        public bool Remove(Behavior item)
        {
            return _Children.Remove(item);
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        public IEnumerator<Behavior> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }
        #endregion
    }
    #endregion


    #region BehaviorViewModel
    /// <summary>
    /// View model for Behavior
    /// </summary>
    public abstract class BehaviorViewModel : TreeViewItemViewModel
    {
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

                        _ConnectionToP = Skill.Editor.Diagram.BezierCurve.GetPathGeometry(new Point(0, offset), new Point(x, y + offset), Diagram.ConnectorOrientation.Left, Diagram.ConnectorOrientation.Right);
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
            foreach (var child in Model)
            {
                base.Add(CreateViewModel(child));
            }
        }
        /// <summary>
        /// Create view model based on BehaviorType
        /// </summary>
        /// <param name="behavior">behavior data</param>
        /// <returns>Create view model</returns>
        BehaviorViewModel CreateViewModel(Behavior behavior)
        {
            switch (behavior.BehaviorType)
            {
                case BehaviorType.Action:
                    return new ActionViewModel(this, (Action)behavior);
                case BehaviorType.Condition:
                    return new ConditionViewModel(this, (Condition)behavior);
                case BehaviorType.Decorator:
                    return new DecoratorViewModel(this, (Decorator)behavior);
                case BehaviorType.Composite:
                    return CreateSelectorViewModel((Composite)behavior);
            }
            return null;
        }
        /// <summary>
        /// Create view model based on CompositeType
        /// </summary>
        /// <param name="behavior">selector data</param>
        /// <returns>Create view model</returns>
        CompositeViewModel CreateSelectorViewModel(Composite selector)
        {
            switch (selector.CompositeType)
            {
                case CompositeType.Sequence:
                    return new SequenceSelectorViewModel(this, (SequenceSelector)selector);
                case CompositeType.Concurrent:
                    return new ConcurrentSelectorViewModel(this, (ConcurrentSelector)selector);
                case CompositeType.Random:
                    return new RandomSelectorViewModel(this, (RandomSelector)selector);
                case CompositeType.Priority:
                    return new PrioritySelectorViewModel(this, (PrioritySelector)selector);
                case CompositeType.Loop:
                    return new LoopSelectorViewModel(this, (LoopSelector)selector);
                default:
                    throw new InvalidCastException("Invalid CompositeType");
            }
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
                    if (Parent[0] != this)
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
                    if (Parent[Parent.Count - 1] != this)
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
        public BehaviorViewModel AddBehavior(BehaviorViewModel child, bool duplicate = true, int index = -1)
        {
            // actions and conditions are leaves and can not have any child. also decorators can have only one child
            if (this.Model.BehaviorType != BehaviorType.Composite && !(this.Model.BehaviorType == BehaviorType.Decorator && Count == 0))
                throw new Exception("Can not add child to this node anymore");

            // ignore it if already have this child. check actual model do to duplication
            if (Contains(child.Model))
                throw new Exception("Already contains this child");

            // check to prevent loop in hierarchy. if a node be twise in hierarchy cause too loop in tree
            if (CheckAddCauseLoop(child))
                throw new Exception("Adding this child cause to loop in tree");


            BehaviorViewModel toAdd = null;
            if (duplicate)
                toAdd = CreateViewModel(child.Model);
            else
                toAdd = child;
            this.Model.Add(toAdd.Model);
            if (index >= 0 && index < Count)
                this.Insert(index, toAdd);
            else
                this.Add(toAdd);

            Tree.History.Insert(new AddBehaviorUnDoRedo(toAdd, this, -1));
            return toAdd;
        }

        /// <summary>
        /// Create new child (action,decorator or condition)
        /// </summary>
        /// <param name="behaviorType">type f child</param>
        /// <returns>added child</returns>
        public BehaviorViewModel AddNonSelector(BehaviorType behaviorType)
        {
            Behavior behavior = null;
            switch (behaviorType)
            {
                case BehaviorType.Action:
                    behavior = new Action();
                    break;
                case BehaviorType.Condition:
                    behavior = new Condition();
                    break;
                case BehaviorType.Decorator:
                    behavior = new Decorator();
                    break;
            }
            if (behavior != null)
            {
                BehaviorViewModel behaviorVM = CreateViewModel(behavior);
                return AddBehavior(behaviorVM, false, -1);
            }
            return null;
        }

        /// <summary>
        /// add new selector child
        /// </summary>
        /// <param name="selectorType">type of child</param>
        /// <returns>added child</returns>
        public BehaviorViewModel AddSelector(CompositeType selectorType)
        {
            Composite selector = null;
            switch (selectorType)
            {
                case CompositeType.Sequence:
                    selector = new SequenceSelector();
                    break;
                case CompositeType.Concurrent:
                    selector = new ConcurrentSelector();
                    break;
                case CompositeType.Random:
                    selector = new RandomSelector();
                    break;
                case CompositeType.Priority:
                    selector = new PrioritySelector();
                    break;
                case CompositeType.Loop:
                    selector = new LoopSelector();
                    break;
            }

            if (selector != null)
            {
                BehaviorViewModel selectorVM = CreateSelectorViewModel(selector);
                return AddBehavior(selectorVM, false, -1);
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
                Tree.History.Insert(new AddBehaviorUnDoRedo(child, this, index, true));
                Tree.UnRegisterViewModel(child);
                return true;
            }
            return false;
        }

        #endregion

        public override string ToString()
        {
            return Name + " " + base.ToString();
        }
    }
    #endregion



    #region UnDoRedo helper classes
    class AddBehaviorUnDoRedo : IUnDoRedoCommand
    {
        int _Index;
        BehaviorViewModel _NewNode;
        BehaviorViewModel _Parent;
        bool _Reverse;
        public AddBehaviorUnDoRedo(BehaviorViewModel newNode, BehaviorViewModel parent, int index, bool reverse = false)
        {
            this._NewNode = newNode;
            this._Parent = parent;
            this._Reverse = reverse;
            this._Index = index;
        }

        public void Undo()
        {
            if (_Reverse)
                _Parent.AddBehavior(_NewNode, false, _Index);
            else
                _Parent.RemoveBehavior(_NewNode);
        }

        public void Redo()
        {
            if (_Reverse)
                _Parent.RemoveBehavior(_NewNode);
            else
                _Parent.AddBehavior(_NewNode, false, _Index);
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
