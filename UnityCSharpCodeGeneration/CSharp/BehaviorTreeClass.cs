﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.CodeGeneration.CSharp
{



    /// <summary>
    /// generate C# code for a BehaviorTree class
    /// </summary>
    class BehaviorTreeClass : Class
    {
        #region BehaviorTreeStateNamesClass
        class BehaviorTreeStateNamesClass : Class
        {
            public BehaviorTreeStateNamesClass(BehaviorTree tree)
                : base("StateNames")
            {
                base.ClassModifier = ClassModifiers.Static;
                base.IsPartial = false;
                foreach (var s in tree.States)
                    Add(new Variable("string", s.Name, string.Format("\"{0}\"", s.Name)) { IsStatic = true, Modifier = Modifiers.Public });
            }
        }
        #endregion

        #region Variables
        private static string[] ActionResetEventHandlerParams = new string[] { "Skill.Framework.AI.Action action" };
        private static string[] ConditionHandlerParams = new string[] { "object sender", "Skill.Framework.AI.BehaviorParameterCollection parameters" };
        private static string[] DecoratorHandlerParams = ConditionHandlerParams;
        private static string[] ActionHandlerParams = ConditionHandlerParams;

        List<Behavior> _Behaviors;// list of behaviors in hierarchy
        BehaviorTree _Tree;// behavior tree model 
        StringBuilder _CreateTreeMethodBody;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a BehaviorTreeClass
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
        public BehaviorTreeClass(BehaviorTree tree)
            : base(tree.Name)
        {
            this._Tree = tree;
            this._Behaviors = new List<Behavior>();
            this._CreateTreeMethodBody = new StringBuilder();
            CreateBehaviorList();
            ProcessNodes();
            AddInherit("Skill.Framework.AI.BehaviorTree");

            if (tree.AccessKeys != null)
            {
                this.Add(new SharedAccessKeysClass(this._Tree.AccessKeys));
            }

            // states class
            BehaviorTreeStateNamesClass states = new BehaviorTreeStateNamesClass(tree);
            Add(states);

            if (!_Tree.ExpandMethods)
            {
                ClassModifier = ClassModifiers.Abstract;

                EnumClass actions = new EnumClass("Actions") { Modifier = Modifiers.Protected };
                EnumClass conditions = new EnumClass("Conditions") { Modifier = Modifiers.Protected };
                EnumClass decorators = new EnumClass("Decorators") { Modifier = Modifiers.Protected };

                foreach (var b in _Behaviors)
                {
                    switch (b.BehaviorType)
                    {
                        case BehaviorType.Action:
                            actions.Add(b.Name);
                            break;
                        case BehaviorType.Condition:
                            conditions.Add(b.Name);
                            break;
                        case BehaviorType.Decorator:
                            decorators.Add(b.Name);
                            break;
                    }
                }

                if (actions.Count > 0)
                {
                    Add(actions);
                    Add(new Method("Skill.Framework.AI.BehaviorResult", "OnAction", "", "Actions action", "object sender", "Skill.Framework.AI.BehaviorParameterCollection parameters") { SubMethod = SubMethod.Abstract, Modifier = Modifiers.Protected });
                    Add(new Method("void", "OnActionReset", "", "Actions action") { SubMethod = SubMethod.Abstract, Modifier = Modifiers.Protected });
                }
                if (conditions.Count > 0)
                {
                    Add(conditions);
                    Add(new Method("bool", "OnCondition", "", "Conditions condition", "object sender", "Skill.Framework.AI.BehaviorParameterCollection parameters") { SubMethod = SubMethod.Abstract, Modifier = Modifiers.Protected });
                }
                if (decorators.Count > 0)
                {
                    Add(decorators);
                    Add(new Method("bool", "OnDecorator", "", "Decorators decorator", "object sender", "Skill.Framework.AI.BehaviorParameterCollection parameters") { SubMethod = SubMethod.Abstract, Modifier = Modifiers.Protected });
                }

                IsPartial = false;
            }

            // add DefaultState property

            Property defaultState = new Property("string", "DefaultState", string.Format("\"{0}\"", _Tree.DefaultState), false) { Modifier = Modifiers.Public, SubMethod = CSharp.SubMethod.Override };
            Add(defaultState);

            //Method constructor = new Method("", Name, "", "");
            //constructor.Modifier = Modifiers.Public;
            //Add(constructor);

            Method createTree = new Method("Skill.Framework.AI.BehaviorTreeState[]", "CreateTree", this._CreateTreeMethodBody.ToString());
            createTree.IsPartial = false;
            createTree.SubMethod = SubMethod.Override;
            createTree.Modifier = Modifiers.Protected;
            Add(createTree);
        }

        #endregion

        #region CreateBehaviorList
        /// <summary>
        /// Create list of behaviors that are in hierarchy
        /// </summary>
        private void CreateBehaviorList()
        {
            foreach (var s in _Tree.States)
            {
                AddToBehaviors(s);
            }
        }
        private bool IsInBehavior(Behavior behavior)
        {
            foreach (var item in _Behaviors)
            {
                if (item.Id == behavior.Id) return true;
            }
            return false;
        }
        private void AddToBehaviors(Behavior behavior)
        {
            if (behavior == null) return;
            if (!IsInBehavior(behavior))
                _Behaviors.Add(behavior);

            foreach (var item in behavior)
            {
                AddToBehaviors(item);
            }
        }
        #endregion

        #region Process
        /// <summary>
        /// Get name of action handler method
        /// </summary>
        /// <param name="actionName">Name of action</param>
        /// <returns>name of action handler method</returns>
        private string GetActionHandlerName(string actionName)
        {
            return actionName + "_Action";
        }
        /// <summary>
        /// Get name of condition handler method
        /// </summary>                
        /// <param name="conditionName">name of condition</param>
        /// <returns>name of condition handler method</returns>
        private string GetConditionHandlerName(string conditionName)
        {
            return conditionName + "_Condition";
        }
        /// <summary>
        /// Get name of decorator handler method
        /// </summary>
        /// <param name="decoratorName">name of decorator</param>
        /// <returns>name of decorator handler method</returns>
        private string GetDecoratorHandlerName(string decoratorName)
        {
            return decoratorName + "_Decoration";
        }
        /// <summary>
        /// Find behavior by id
        /// </summary>
        /// <param name="id">Id of behavior</param>
        /// <returns>Behavior</returns>
        private Behavior Find(int id)
        {
            foreach (var item in _Behaviors)
            {
                if (item.Id == id) return item;
            }

            return null;
        }

        private string CreateParameters(Skill.DataModels.AI.ParameterCollection parameters)
        {
            StringBuilder result = new StringBuilder();

            result.Append("new Skill.Framework.AI.BehaviorParameterCollection( new Skill.Framework.AI.BehaviorParameter[] { ");

            foreach (var p in parameters)
            {
                switch (p.Type)
                {
                    case ParameterType.Int:
                        result.Append(string.Format("new Skill.Framework.AI.BehaviorParameter(\"{0}\",{1}),", p.Name, p.Value));
                        break;
                    case ParameterType.Bool:
                        result.Append(string.Format("new Skill.Framework.AI.BehaviorParameter(\"{0}\",{1}),", p.Name, p.Value.ToString().ToLower()));
                        break;
                    case ParameterType.Float:
                        result.Append(string.Format("new Skill.Framework.AI.BehaviorParameter(\"{0}\",{1}f),", p.Name, p.Value));
                        break;
                    case ParameterType.String:
                        result.Append(string.Format("new Skill.Framework.AI.BehaviorParameter(\"{0}\",\"{1}\"),", p.Name, p.Value));
                        break;
                }
            }

            result.Append("} )");
            return result.ToString();
        }

        /// <summary>
        /// Process behaviors and create variables, properties and methods
        /// </summary>
        private void ProcessNodes()
        {
            // first create variables and set their properties
            foreach (var b in _Behaviors)
            {
                switch (b.BehaviorType)
                {
                    case BehaviorType.Action:
                        CreateAction((Skill.DataModels.AI.Action)b);
                        break;
                    case BehaviorType.Condition:
                        CreateCondition((Condition)b);
                        break;
                    case BehaviorType.Decorator:
                        CreateDecorator((Decorator)b);
                        break;
                    case BehaviorType.Composite:
                        CreateComposite((Composite)b);
                        break;
                    case BehaviorType.ChangeState:
                        CreateChangeState((ChangeState)b);
                        break;
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            // add child of each behaviors(Decorator and Composites)
            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == BehaviorType.Decorator)
                {
                    for (int i = 0; i < b.Count; i++)
                    {
                        Behavior child = b[i];
                        if (child != null)
                        {
                            ParameterCollection parameters = b.GetParameters(i);
                            if (parameters != null && parameters.Count > 0)
                                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.SetChild({1},{2});", Variable.GetName(b.Name), Variable.GetName(child.Name), CreateParameters(parameters)));
                            else
                                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.SetChild({1},null);", Variable.GetName(b.Name), Variable.GetName(child.Name)));
                            break;
                        }
                    }
                }
                // Composite has multiple child
                else if (b.BehaviorType == BehaviorType.Composite)
                {
                    for (int i = 0; i < b.Count; i++)
                    {
                        Behavior child = b[i];
                        if (child != null)
                        {
                            ParameterCollection parameters = b.GetParameters(i);
                            if (parameters != null && parameters.Count > 0)
                                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Add({1},{2});", Variable.GetName(b.Name), Variable.GetName(child.Name), CreateParameters(parameters)));
                            else
                                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Add({1},null);", Variable.GetName(b.Name), Variable.GetName(child.Name)));

                        }
                    }
                }
            }

            // then return root of tree
            if (_Tree.States != null)
            {
                _CreateTreeMethodBody.AppendLine(string.Format("Skill.Framework.AI.BehaviorTreeState[] states = new Skill.Framework.AI.BehaviorTreeState[{0}];", _Tree.States.Length));
                for (int i = 0; i < _Tree.States.Length; i++)
                {
                    _CreateTreeMethodBody.AppendLine(string.Format("states[{0}] = {1};", i, Variable.GetName(_Tree.States[i].Name)));
                }

                _CreateTreeMethodBody.AppendLine("return states;");
            }
            else
                _CreateTreeMethodBody.AppendLine("return null;");
        }

        /// <summary>
        /// If weight of behavior is not default value write line of code to set value
        /// </summary>
        /// <param name="behavior">behavior</param>
        private void SetWeight(Behavior behavior)
        {
            if (behavior.Weight != 1)
            {
                _CreateTreeMethodBody.AppendLine(SetProperty(behavior.Name, "Weight", behavior.Weight.ToString() + "f"));
            }
        }

        private void CreateAction(Skill.DataModels.AI.Action action)
        {
            // create action variable
            Add(new Variable("Skill.Framework.AI.Action", action.Name, "null"));
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Action(\"{1}\", {2}, Skill.Framework.Posture.{3});", Variable.GetName(action.Name), action.Name, GetActionHandlerName(action.Name), action.ChangePosture));
            // set weight
            SetWeight(action);


            Method m = new Method("Skill.Framework.AI.BehaviorResult", GetActionHandlerName(action.Name),
                _Tree.ExpandMethods ? "return Skill.Framework.AI.BehaviorResult.Failure;" : string.Format("return OnAction( Actions.{0} , sender ,  parameters);", action.Name),
                ActionHandlerParams) { IsPartial = _Tree.ExpandMethods };
            Add(m);

            // create reset event handler and assign it to Reset event
            if (action.ResetEvent)
            {
                string resetName = action.Name + "_Reset";
                Method resetMethod = new Method("void", resetName,
                    _Tree.ExpandMethods ? "" : string.Format("return OnActionReset( Actions.{0} );", action.Name),
                    ActionResetEventHandlerParams) { IsPartial = _Tree.ExpandMethods };
                Add(resetMethod);
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Reset += new ActionResetEventHandler({1});", Variable.GetName(action.Name)
                    , resetName));
            }
        }

        private void CreateCondition(Condition condition)
        {
            // create condition variable
            Add(new Variable("Skill.Framework.AI.Condition", condition.Name, "null"));
            // new condition variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Condition(\"{1}\",{2});", Variable.GetName(condition.Name), condition.Name, GetConditionHandlerName(condition.Name)));
            // set weight
            SetWeight(condition);
            // create condition handler method
            Method m = new Method("bool", GetConditionHandlerName(condition.Name),
              _Tree.ExpandMethods ? "return false;" : string.Format("return OnCondition( Conditions.{0}, sender ,  parameters);", condition.Name), ConditionHandlerParams) { IsPartial = _Tree.ExpandMethods };
            Add(m);
        }

        private void CreateChangeState(ChangeState changeState)
        {
            // create condition variable
            Add(new Variable("Skill.Framework.AI.ChangeState", changeState.Name, "null"));
            // new condition variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.ChangeState(\"{1}\",\"{2}\");", Variable.GetName(changeState.Name), changeState.Name, changeState.DestinationState));
            // set weight
            SetWeight(changeState);
        }

        private void CreateDecorator(Decorator decorator)
        {
            switch (decorator.Type)
            {
                case DecoratorType.Default:
                    CreateDefaultDecorator(decorator);
                    break;
                case DecoratorType.AccessLimit:
                    CreateAccessLimitDecorator((AccessLimitDecorator)decorator);
                    break;
            }
        }

        private void CreateDefaultDecorator(Decorator decorator)
        {
            // create decorator variable
            Add(new Variable("Skill.Framework.AI.Decorator", decorator.Name, "null"));
            // new decorator variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Decorator(\"{1}\",{2});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name)));
            // set property SuccessOnFailHandler
            if (decorator.NeverFail != true) // default value is true, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetWeight(decorator);
            // create decorator handler method
            Method m = new Method("bool", GetDecoratorHandlerName(decorator.Name),
             _Tree.ExpandMethods ? "return false;" : string.Format("return OnDecorator( Decorators.{0}, sender ,  parameters);", decorator.Name),
             DecoratorHandlerParams) { IsPartial = _Tree.ExpandMethods };
            Add(m);
        }

        private void CreateAccessLimitDecorator(AccessLimitDecorator decorator)
        {
            // create decorator variable
            Add(new Variable("Skill.Framework.AI.AccessLimitDecorator", decorator.Name, "null"));

            // new decorator variable inside CreateTree method

            if (string.IsNullOrEmpty(decorator.Address) || decorator.Address.Equals("Internal", StringComparison.OrdinalIgnoreCase))
            {
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.AccessLimitDecorator(\"{1}\",{2},{3}.{4});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name), _Tree.AccessKeys.Name, decorator.AccessKey));
            }
            else
            {
                string className = System.IO.Path.GetFileNameWithoutExtension(decorator.Address);
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.AccessLimitDecorator(\"{1}\",{2},{3}.{4});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name), className, decorator.AccessKey));
            }
            // set property SuccessOnFailHandler
            if (decorator.NeverFail != true) // default value is true, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetWeight(decorator);
            // create decorator handler method
            Method m = new Method("bool", GetDecoratorHandlerName(decorator.Name),
                _Tree.ExpandMethods ? "return true;" : string.Format("return OnDecorator( Decorators.{0}, sender ,  parameters);", decorator.Name),
                DecoratorHandlerParams) { IsPartial = _Tree.ExpandMethods };
            Add(m);
        }

        private void CreateComposite(Composite composite)
        {
            switch (composite.CompositeType)
            {
                case CompositeType.Sequence:
                    CreateSequenceSelector((SequenceSelector)composite);
                    break;
                case CompositeType.Concurrent:
                    CreateConcurrentSelector((ConcurrentSelector)composite);
                    break;
                case CompositeType.Random:
                    CreateRandomSelector((RandomSelector)composite);
                    break;
                case CompositeType.Priority:
                    CreatePrioritySelector((PrioritySelector)composite);
                    break;
                case CompositeType.State:
                    CreateBehaviorTreeState((BehaviorTreeState)composite);
                    break;
                case CompositeType.Loop:
                    CreateLoopSelector((LoopSelector)composite);
                    break;
                default:
                    throw new InvalidOperationException("Invalid CompositeType");
            }
            // set weight
            SetWeight(composite);
        }

        private void CreateSequenceSelector(SequenceSelector sequenceSelector)
        {
            // create SequenceSelector variable
            Add(new Variable("Skill.Framework.AI.SequenceSelector", sequenceSelector.Name, "null"));
            // new SequenceSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.SequenceSelector(\"{1}\");", Variable.GetName(sequenceSelector.Name), sequenceSelector.Name));
        }

        private void CreateConcurrentSelector(ConcurrentSelector concurrentSelector)
        {
            // create ConcurrentSelector variable
            Add(new Variable("Skill.Framework.AI.ConcurrentSelector", concurrentSelector.Name, "null"));
            // new ConcurrentSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.ConcurrentSelector(\"{1}\");", Variable.GetName(concurrentSelector.Name), concurrentSelector.Name));

            // set BreakOnConditionFailure Property
            if (concurrentSelector.BreakOnConditionFailure != false) // default is false                
                _CreateTreeMethodBody.AppendLine(SetProperty(concurrentSelector.Name, "BreakOnConditionFailure", concurrentSelector.BreakOnConditionFailure.ToString().ToLower()));

            // set SuccessPolicy Property
            if (concurrentSelector.SuccessPolicy != SuccessPolicy.SucceedOnAll) // default is SucceedOnAll                
                _CreateTreeMethodBody.AppendLine(SetProperty(concurrentSelector.Name, "SuccessPolicy", string.Format("Skill.Framework.AI.SuccessPolicy.{0}", concurrentSelector.SuccessPolicy)));

            // set FailurePolicy Property
            if (concurrentSelector.FailurePolicy != FailurePolicy.FailOnAll) // default is FailOnAll                
                _CreateTreeMethodBody.AppendLine(SetProperty(concurrentSelector.Name, "FailurePolicy", string.Format("Skill.Framework.AI.FailurePolicy.{0}", concurrentSelector.FailurePolicy)));
        }

        private void CreateRandomSelector(RandomSelector randomSelector)
        {
            // create RandomSelector variable
            Add(new Variable("Skill.Framework.AI.RandomSelector", randomSelector.Name, "null"));
            // new RandomSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.RandomSelector(\"{1}\");", Variable.GetName(randomSelector.Name), randomSelector.Name));
        }

        private void CreatePrioritySelector(PrioritySelector prioritySelector)
        {
            // create PrioritySelector variable
            Add(new Variable("Skill.Framework.AI.PrioritySelector", prioritySelector.Name, "null"));
            // new PrioritySelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.PrioritySelector(\"{1}\");", Variable.GetName(prioritySelector.Name), prioritySelector.Name));

            // set Priority property
            if (prioritySelector.Priority != PriorityType.HighestPriority) // default is HighestPriority                
                _CreateTreeMethodBody.AppendLine(SetProperty(prioritySelector.Name, "Priority", string.Format("Skill.Framework.AI.PriorityType.{0}", prioritySelector.Priority)));
        }

        private void CreateBehaviorTreeState(BehaviorTreeState state)
        {
            // create PrioritySelector variable
            Add(new Variable("Skill.Framework.AI.BehaviorTreeState", state.Name, "null"));
            // new PrioritySelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.BehaviorTreeState(\"{1}\");", Variable.GetName(state.Name), state.Name));

            // set Priority property
            if (state.Priority != PriorityType.HighestPriority) // default is HighestPriority                
                _CreateTreeMethodBody.AppendLine(SetProperty(state.Name, "Priority", string.Format("Skill.Framework.AI.PriorityType.{0}", state.Priority)));
        }

        private void CreateLoopSelector(LoopSelector loopSelector)
        {
            // create LoopSelector variable
            Add(new Variable("Skill.Framework.AI.LoopSelector", loopSelector.Name, "null"));
            // new LoopSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.LoopSelector(\"{1}\");", Variable.GetName(loopSelector.Name), loopSelector.Name));

            // set LoopCount property
            if (loopSelector.LoopCount != -1) // default is -1 (infinity)                
                _CreateTreeMethodBody.AppendLine(SetProperty(loopSelector.Name, "LoopCount", loopSelector.LoopCount));
        }

        #endregion

    }
}
