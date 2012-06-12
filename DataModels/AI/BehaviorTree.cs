﻿using System;
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
        #region Properties
        /// <summary> Internal Access keys  </summary>
        public SharedAccessKeys AccessKeys { get; private set; }
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
            this.AccessKeys = new SharedAccessKeys();
            this.Name = "NewBehaviorTree";
            this.Root = new PrioritySelector();
            this.Root.Name = "Root";
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
            CreateList(list, Root);
            GenerateIds(list); // first regenerate ids for all behaviors

            XElement behaviorTree = new XElement("BehaviorTree");
            behaviorTree.SetAttributeValue("Name", Name);
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
            hierarchy.Add(CreateHierarchy(Root));
            behaviorTree.Add(hierarchy);


            return behaviorTree;
        }

        private XElement CreateHierarchy(Behavior behavior)
        {
            XElement behaviorElement = new XElement("Behavior");
            behaviorElement.SetAttributeValue("Id", behavior.Id);


            for (int i = 0; i < behavior.Count; i++)
            {
                XElement bcElement = new XElement("BehaviorContainer");

                XElement bElement = CreateHierarchy(behavior[i]);
                bcElement.Add(bElement);

                bcElement.Add(behavior.GetParameters(i).ToXElement());

                behaviorElement.Add(bcElement);
            }

            return behaviorElement;
        }


        #endregion

        #region Load

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

            XElement accessKeys = e.FindChildByName("AccessKeys");
            if (accessKeys != null)
                AccessKeys.Load(accessKeys);

            // load hierarchy            

            XElement hierarchy = e.FindChildByName("Hierarchy");
            if (hierarchy != null)
            {
                Root = LoadHierarchy(list, hierarchy.Elements().First()) as PrioritySelector;
            }

        }

        private Behavior LoadHierarchy(List<Behavior> list, XElement behaviorElement)
        {
            int id = behaviorElement.GetAttributeValueAsInt("Id", -1);
            Behavior behavior = FindById(list, id);

            if (behavior != null)
            {
                foreach (var bcElement in behaviorElement.Elements())
                {
                    XElement bElement = bcElement.FindChildByName("Behavior");
                    Behavior childBehavior = LoadHierarchy(list, bElement);

                    XElement parametersElement = bcElement.FindChildByName("Parameters");
                    if (parametersElement != null)
                    {
                        ParameterCollection parameters = new ParameterCollection();
                        parameters.Load(parametersElement);
                        behavior.Add(childBehavior, parameters);
                    }
                    else
                        behavior.Add(childBehavior);
                }
            }
            return behavior;
        }
        #endregion

    }
    #endregion
}
