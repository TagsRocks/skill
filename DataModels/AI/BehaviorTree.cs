using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region BehaviorTree
    /// <summary>
    /// Defines a Behavior Tree
    /// </summary>
    public class BehaviorTree : IXElement
    {
        public const string DefaultDestinationState = "Default";

        #region Properties
        /// <summary> Internal Access keys  </summary>
        public SharedAccessKeys AccessKeys { get; private set; }

        /// <summary> Root of tree </summary>
        public Behavior[] States { get; set; }
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }

        /// <summary> Name of default state </summary>
        public string DefaultState { get; set; }

        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public BehaviorTree()
        {
            this.AccessKeys = new SharedAccessKeys();
            this.Name = "NewBehaviorTree";
            this.States = new Behavior[] { new PrioritySelector() { Name = DefaultDestinationState } };
            this.DefaultState = DefaultDestinationState;
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
            if (States != null)
            {
                foreach (var s in States)
                {
                    bool result = IsInHierarchy(s, behavior);
                    if (result) return result;
                }
            }
            return false;
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

        #region Save

        private void GenerateIds(List<Behavior> behaviors)
        {
            int id = 0;
            foreach (var b in behaviors)
            {
                b.Id = id++;
            }
        }

        /// <summary>
        /// add behaviors in hierarchy to given list
        /// </summary>
        /// <param name="list">List of Behaviors to fill</param>
        /// <param name="behavior">behavior to add to list</param>
        private void CreateList(List<Behavior> list, Behavior behavior)
        {
            if (!list.Contains(behavior))
                list.Add(behavior);
            foreach (Behavior b in behavior) CreateList(list, b);
        }

        public XElement ToXElement()
        {
            List<Behavior> list = new List<Behavior>();

            if (States != null)
            {
                foreach (var s in States)
                {
                    CreateList(list, s);
                }
            }
            GenerateIds(list); // first regenerate ids for all behaviors

            XElement behaviorTree = new XElement("BehaviorTree");
            behaviorTree.SetAttributeValue("Name", Name);
            behaviorTree.SetAttributeValue("DefaultState", DefaultState);

            behaviorTree.SetAttributeValue("HorizontalOffset", HorizontalOffset);
            behaviorTree.SetAttributeValue("VerticalOffset", VerticalOffset);

            XElement behaviorsElement = new XElement("Behaviors");

            // write each behavior without children hierarchy
            // children will be writed in hierarchy section
            foreach (var behavior in list)
            {
                XElement behaviorElement = behavior.ToXElement();
                behaviorsElement.Add(behaviorElement);

            }
            behaviorTree.Add(behaviorsElement);

            // write internal accesskeys
            XElement accessKeys = AccessKeys.ToXElement();
            behaviorTree.Add(accessKeys);

            // write hierarchy
            XElement hierarchy = new XElement("Hierarchy");

            foreach (var behavior in list)
            {
                if (behavior.Count > 0)
                {
                    XElement behaviorChildren = new XElement("BehaviorChildren");
                    behaviorChildren.SetAttributeValue("Id", behavior.Id);

                    for (int i = 0; i < behavior.Count; i++)
                    {
                        XElement container = new XElement("BehaviorContainer");
                        container.SetAttributeValue("ChildId", behavior[i].Id);
                        container.Add(behavior.GetParameters(i).ToXElement());
                        behaviorChildren.Add(container);
                    }

                    hierarchy.Add(behaviorChildren);
                }
            }

            behaviorTree.Add(hierarchy);


            return behaviorTree;
        }

        #endregion

        #region Load

        /// <summary>
        /// Detect Behavior data from Given XElement  and load it
        /// </summary>
        /// <param name="behavior">XElement contains Behavior data</param>
        /// <returns>Loaded Behavior</returns>
        public static Behavior CreateBehaviorFrom(XElement behavior)
        {
            Behavior result = null;
            BehaviorType behaviorType = BehaviorType.Action;
            bool isCorrect = false;
            try
            {
                behaviorType = (BehaviorType)Enum.Parse(typeof(BehaviorType), behavior.GetAttributeValueAsString("BehaviorType", ""), false);
                isCorrect = true;
            }
            catch (Exception)
            {
                isCorrect = false;
            }
            if (isCorrect)
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
                    case BehaviorType.ChangeState:
                        result = new ChangeState();
                        break;
                }
            }
            return result;
        }

        private static Behavior FindById(List<Behavior> list, int id)
        {
            foreach (var item in list)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.DefaultState = e.GetAttributeValueAsString("DefaultState", DefaultDestinationState);
            this.HorizontalOffset = e.GetAttributeValueAsDouble("HorizontalOffset", HorizontalOffset);
            this.VerticalOffset = e.GetAttributeValueAsDouble("VerticalOffset", VerticalOffset);


            List<Behavior> list = new List<Behavior>();
            XElement behaviorsElement = e.FindChildByName("Behaviors");
            if (behaviorsElement != null)
            {
                foreach (var behaviorElement in behaviorsElement.Elements())
                {
                    Behavior behavior = CreateBehaviorFrom(behaviorElement);
                    if (behavior != null)
                    {
                        behavior.Load(behaviorElement);
                        list.Add(behavior);
                    }
                }
            }

            List<Behavior> states = new List<Behavior>();
            foreach (var b in list) if (b.IsState) states.Add(b);
            if (states.Count > 0)
                this.States = states.ToArray();
            else
            {
                // try to load as previouse version format
                int rootId = e.GetAttributeValueAsInt("RootId", 0);
                Behavior root = FindById(list, rootId);
                if (root != null)
                {
                    root.Name = DefaultDestinationState;
                    this.States = new Behavior[] { root };
                }
                else
                {
                    this.States = new Behavior[] { new PrioritySelector() { Name = DefaultDestinationState } };
                }
            }

            XElement accessKeys = e.FindChildByName("AccessKeys");
            if (accessKeys != null)
                AccessKeys.Load(accessKeys);

            // load hierarchy            
            XElement hierarchy = e.FindChildByName("Hierarchy");
            if (hierarchy != null)
            {
                foreach (var behaviorChildrenElement in hierarchy.Elements())
                {
                    int behaviorId = behaviorChildrenElement.GetAttributeValueAsInt("Id", -2);
                    Behavior behavior = FindById(list, behaviorId);
                    if (behavior != null)
                    {
                        foreach (var containerElement in behaviorChildrenElement.Elements())
                        {
                            int childId = containerElement.GetAttributeValueAsInt("ChildId", -2);
                            Behavior child = FindById(list, childId);
                            if (child != null)
                            {
                                XElement parametersElement = containerElement.FindChildByName("Parameters");
                                if (parametersElement != null)
                                {
                                    ParameterCollection parameters = new ParameterCollection();
                                    parameters.Load(parametersElement);
                                    behavior.Add(child, parameters);
                                }
                                else
                                    behavior.Add(child);
                            }
                        }
                    }

                }
            }
        }
        #endregion

    }
    #endregion
}
