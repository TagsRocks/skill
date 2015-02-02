using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.AI
{
    #region BehaviorTree
    /// <summary>
    /// Defines a Behavior Tree
    /// </summary>
    public class BehaviorTreeData : IXmlElementSerializable
    {
        public const string DefaultDestinationState = "Default";

        #region Properties
        /// <summary> Root of tree </summary>
        public BehaviorData[] States { get; set; }
        public BehaviorData[] ExtraBehaviors { get; set; }
        /// <summary> Name of tree. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }

        /// <summary> Name of default state </summary>
        public string DefaultState { get; set; }

        /// <summary> Used by code generation to decide how implement methods</summary>
        public bool ExpandMethods { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of Behavior Tree
        /// </summary>
        public BehaviorTreeData()
        {
            this.Name = "NewBehaviorTree";
            this.States = new BehaviorData[] { new BehaviorTreeStateData() { Name = DefaultDestinationState } };
            this.ExtraBehaviors = new BehaviorData[0];
            this.DefaultState = DefaultDestinationState;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check whether specyfied behavior is in hierarchy or unused
        /// </summary>
        /// <param name="behavior">Behavior to check</param>
        /// <returns>True if is in hierarchy, otherwise false</returns>
        public bool IsInHierarchy(BehaviorData behavior)
        {
            if (States != null)
            {
                foreach (var s in States)
                {
                    if (IsInHierarchy(s, behavior))
                        return true;
                }
            }
            return false;
        }

        private bool IsInHierarchy(BehaviorData node, BehaviorData behavior)
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

        private void GenerateIds(List<BehaviorData> behaviors)
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
        private void CreateList(List<BehaviorData> list, BehaviorData behavior)
        {
            if (!list.Contains(behavior))
                list.Add(behavior);
            foreach (BehaviorData b in behavior) CreateList(list, b);
        }

        public XmlElement ToXmlElement()
        {
            List<BehaviorData> list = new List<BehaviorData>();

            if (States != null)
            {
                foreach (var s in States)
                {
                    CreateList(list, s);
                }
            }
            if (ExtraBehaviors != null)
            {// add extra behaviors
                foreach (var b in ExtraBehaviors)
                {
                    if (!list.Contains(b))
                        list.Add(b);
                }
            }

            GenerateIds(list); // first regenerate ids for all behaviors

            XmlElement behaviorTree = new XmlElement("BehaviorTree");
            behaviorTree.SetAttributeValue("Name", Name);
            behaviorTree.SetAttributeValue("DefaultState", DefaultState);
            behaviorTree.SetAttributeValue("ExpandMethods", ExpandMethods);
            XmlElement behaviorsElement = new XmlElement("Behaviors");

            // write each behavior without children hierarchy
            // children will be writed in hierarchy section
            foreach (var behavior in list)
            {
                XmlElement behaviorElement = behavior.ToXmlElement();
                behaviorsElement.AppendChild(behaviorElement);

            }
            behaviorTree.AppendChild(behaviorsElement);

            // write hierarchy
            XmlElement hierarchy = new XmlElement("Hierarchy");

            foreach (var behavior in list)
            {
                if (behavior.Count > 0)
                {
                    XmlElement behaviorChildren = new XmlElement("BehaviorChildren");
                    behaviorChildren.SetAttributeValue("Id", behavior.Id);

                    for (int i = 0; i < behavior.Count; i++)
                    {
                        XmlElement container = new XmlElement("BehaviorContainer");
                        container.SetAttributeValue("ChildId", behavior[i].Id);
                        container.AppendChild(behavior.GetParameters(i).ToXmlElement());
                        behaviorChildren.AppendChild(container);
                    }
                    hierarchy.AppendChild(behaviorChildren);
                }
            }

            behaviorTree.AppendChild(hierarchy);


            return behaviorTree;
        }

        #endregion

        #region Load

        /// <summary>
        /// Detect Behavior data from Given XmlElement  and load it
        /// </summary>
        /// <param name="behavior">XmlElement contains Behavior data</param>
        /// <returns>Loaded Behavior</returns>
        public static BehaviorData CreateBehaviorFrom(XmlElement behavior)
        {
            BehaviorData result = null;
            Skill.Framework.AI.BehaviorType behaviorType = Skill.Framework.AI.BehaviorType.Action;
            bool isCorrect = false;
            try
            {
                behaviorType = behavior.GetAttributeValueAsEnum<Skill.Framework.AI.BehaviorType>("BehaviorType", Skill.Framework.AI.BehaviorType.Condition);
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
                    case Skill.Framework.AI.BehaviorType.Action:
                        result = new ActionData();
                        break;
                    case Skill.Framework.AI.BehaviorType.Condition:
                        result = new ConditionData();
                        break;
                    case Skill.Framework.AI.BehaviorType.Decorator:
                        Skill.Framework.AI.DecoratorType decoratorType = behavior.GetAttributeValueAsEnum<Skill.Framework.AI.DecoratorType>("DecoratorType", Skill.Framework.AI.DecoratorType.Default);
                        switch (decoratorType)
                        {
                            case Skill.Framework.AI.DecoratorType.Default:
                                result = new DecoratorData();
                                break;
                            case Skill.Framework.AI.DecoratorType.AccessLimit:
                                result = new AccessLimitDecoratorData();
                                break;
                            default:
                                break;
                        }

                        break;
                    case Skill.Framework.AI.BehaviorType.Composite:
                        Skill.Framework.AI.CompositeType selectorType = behavior.GetAttributeValueAsEnum<Skill.Framework.AI.CompositeType>("CompositeType", Skill.Framework.AI.CompositeType.Sequence);
                        switch (selectorType)
                        {
                            case Skill.Framework.AI.CompositeType.Sequence:
                                result = new SequenceSelectorData();
                                break;
                            case Skill.Framework.AI.CompositeType.Concurrent:
                                result = new ConcurrentSelectorData();
                                break;
                            case Skill.Framework.AI.CompositeType.Random:
                                result = new RandomSelectorData();
                                break;
                            case Skill.Framework.AI.CompositeType.Priority:
                                result = new PrioritySelectorData();
                                break;
                            case Skill.Framework.AI.CompositeType.Loop:
                                result = new LoopSelectorData();
                                break;
                            case Skill.Framework.AI.CompositeType.State:
                                result = new BehaviorTreeStateData();
                                break;
                        }
                        break;
                    case Skill.Framework.AI.BehaviorType.ChangeState:
                        result = new ChangeStateData();
                        break;
                }
            }
            return result;
        }

        private static BehaviorData FindById(List<BehaviorData> list, int id)
        {
            foreach (var item in list)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.DefaultState = e.GetAttributeValueAsString("DefaultState", DefaultDestinationState);
            this.ExpandMethods = e.GetAttributeValueAsBoolean("ExpandMethods", false);
            this.ExtraBehaviors = null;

            List<BehaviorData> list = new List<BehaviorData>();
            XmlElement behaviorsElement = e["Behaviors"];
            if (behaviorsElement != null)
            {
                foreach (var behaviorElement in behaviorsElement)
                {
                    BehaviorData behavior = CreateBehaviorFrom(behaviorElement);
                    if (behavior != null)
                    {
                        behavior.Load(behaviorElement);
                        list.Add(behavior);
                    }
                }
            }

            List<BehaviorTreeStateData> states = new List<BehaviorTreeStateData>();
            foreach (var b in list) if (b.BehaviorType == Skill.Framework.AI.BehaviorType.Composite && ((CompositeData)b).CompositeType == Skill.Framework.AI.CompositeType.State) states.Add(b as BehaviorTreeStateData);

            // load hierarchy            
            XmlElement hierarchy = e["Hierarchy"];
            if (hierarchy != null)
            {
                foreach (var behaviorChildrenElement in hierarchy)
                {
                    int behaviorId = behaviorChildrenElement.GetAttributeValueAsInt("Id", -2);
                    BehaviorData behavior = FindById(list, behaviorId);
                    if (behavior != null)
                    {
                        foreach (var containerElement in behaviorChildrenElement)
                        {
                            int childId = containerElement.GetAttributeValueAsInt("ChildId", -2);
                            BehaviorData child = FindById(list, childId);
                            if (child != null)
                            {
                                XmlElement parametersElement = containerElement[ParameterDataCollection.ElementName];
                                if (parametersElement != null)
                                {
                                    ParameterDataCollection parameters = new ParameterDataCollection();
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

            foreach (var b in list) b.FixParameters();

            if (states.Count > 0)
                this.States = states.ToArray();
            else
            {
                // try to load as previouse version format
                int rootId = e.GetAttributeValueAsInt("RootId", 0);
                BehaviorData root = FindById(list, rootId);
                if (root != null && root.BehaviorType == Skill.Framework.AI.BehaviorType.Composite && ((CompositeData)root).CompositeType == Skill.Framework.AI.CompositeType.Priority)
                {
                    PrioritySelectorData ps = root as PrioritySelectorData;
                    BehaviorTreeStateData state = new BehaviorTreeStateData();

                    state.Comment = ps.Comment;
                    state.Concurrency = ps.Concurrency;
                    state.Id = ps.Id;
                    state.Name = ps.Name;
                    state.Priority = ps.Priority;
                    state.Weight = ps.Weight;

                    foreach (var child in ps)
                        state.Add(child);

                    this.States = new BehaviorTreeStateData[] { state };

                    DefaultState = state.Name;
                }
                else
                {
                    this.States = new BehaviorTreeStateData[] { new BehaviorTreeStateData() { Name = DefaultDestinationState } };
                }
            }

            List<BehaviorData> extra = new List<BehaviorData>();
            foreach (var b in list)
            {
                if (!IsInHierarchy(b))
                    extra.Add(b);
            }
            if (extra.Count > 0)
                ExtraBehaviors = extra.ToArray();
        }

        #endregion


        public void MatchParameters()
        {
            List<BehaviorData> list = new List<BehaviorData>();

            if (States != null)
            {
                foreach (var s in States)
                {
                    CreateList(list, s);
                }
            }
            if (ExtraBehaviors != null)
            {// add extra behaviors
                foreach (var b in ExtraBehaviors)
                {
                    if (!list.Contains(b))
                        list.Add(b);
                }
            }

            foreach (var b in list)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (b[i] is IParameterData)
                    {
                        ParameterDataCollection parameters = b.GetParameters(i);
                        if (parameters != null)
                        {
                            ParameterDataCollection difinitions = ((IParameterData)b[i]).ParameterDifinition;
                            parameters.Match(difinitions);
                        }
                    }
                }
            }
        }
    }
    #endregion
}
