using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.AI;
using Skill.Framework.AI;

namespace Skill.Editor.CodeGeneration
{



    /// <summary>
    /// generate C# code for a BehaviorTree class
    /// </summary>
    class BehaviorTreeClass : Class
    {
        #region BehaviorTreeStateNamesClass
        class BehaviorTreeStateNamesClass : Class
        {
            public BehaviorTreeStateNamesClass(BehaviorTreeData tree)
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

        List<BehaviorData> _Behaviors;// list of behaviors in hierarchy
        BehaviorTreeData _Tree;// behavior tree model 
        StringBuilder _CreateTreeMethodBody;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a BehaviorTreeClass
        /// </summary>
        /// <param name="tree">BehaviorTree model</param>
        public BehaviorTreeClass(BehaviorTreeData tree)
            : base(tree.Name)
        {
            this._Tree = tree;
            this._Behaviors = new List<BehaviorData>();
            this._CreateTreeMethodBody = new StringBuilder();
            CreateBehaviorList();
            ProcessNodes();
            AddInherit("Skill.Framework.AI.BehaviorTree");

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

            Property defaultState = new Property("string", "DefaultState", string.Format("\"{0}\"", _Tree.DefaultState), false) { Modifier = Modifiers.Public, SubMethod = SubMethod.Override };
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
        private bool IsInBehavior(BehaviorData behavior)
        {
            foreach (var item in _Behaviors)
            {
                if (item.Id == behavior.Id) return true;
            }
            return false;
        }
        private void AddToBehaviors(BehaviorData behavior)
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
        private BehaviorData Find(int id)
        {
            foreach (var item in _Behaviors)
            {
                if (item.Id == id) return item;
            }

            return null;
        }

        private string CreateParameters(Skill.Editor.AI.ParameterDataCollection parameters)
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
                        CreateAction((Skill.Editor.AI.ActionData)b);
                        break;
                    case BehaviorType.Condition:
                        CreateCondition((ConditionData)b);
                        break;
                    case BehaviorType.Decorator:
                        CreateDecorator((DecoratorData)b);
                        break;
                    case BehaviorType.Composite:
                        CreateComposite((CompositeData)b);
                        break;
                    case BehaviorType.ChangeState:
                        CreateChangeState((ChangeStateData)b);
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
                        BehaviorData child = b[i];
                        if (child != null)
                        {
                            ParameterDataCollection parameters = b.GetParameters(i);
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
                        BehaviorData child = b[i];
                        if (child != null)
                        {
                            ParameterDataCollection parameters = b.GetParameters(i);
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
        private void SetBehaviorParameters(BehaviorData behavior)
        {
            if (behavior.Weight != 1)            
                _CreateTreeMethodBody.AppendLine(SetProperty(behavior.Name, "Weight", behavior.Weight.ToString() + "f"));
            if (behavior.Concurrency != ConcurrencyMode.Unlimit )
                _CreateTreeMethodBody.AppendLine(SetProperty(behavior.Name, "Concurrency", "Skill.Framework.AI.ConcurrencyMode." + behavior.Concurrency.ToString()));            
        }

        private BehaviorData FindOneParent(BehaviorData child)
        {
            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == BehaviorType.Decorator || b.BehaviorType == BehaviorType.Composite)
                {
                    if (b.Contains(child))
                        return b;
                }
            }
            return null;
        }

        private string CreateMethodBody(BehaviorData b, string append)
        {
            StringBuilder resule = new StringBuilder();
            if (b is IParameterData)
            {
                var parameters = ((IParameterData)b).ParameterDifinition;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var p in parameters)
                    {
                        string type = p.Type.ToString().ToLower();
                        string pName = p.Name.Substring(0, 1).ToLower();
                        if (p.Name.Length > 1)
                            pName += p.Name.Substring(1);

                        resule.AppendLine(string.Format("// {0} {1} = ({0})parameters[\"{2}\"];", type, pName, p.Name));
                    }

                }
            }
            if (!string.IsNullOrEmpty(append))
                resule.AppendLine(append);
            return resule.ToString();
        }

        private void CreateAction(Skill.Editor.AI.ActionData action)
        {
            // create action variable
            Add(new Variable("Skill.Framework.AI.Action", action.Name, "null"));
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Action(\"{1}\", {2}, Skill.Framework.Posture.{3});", Variable.GetName(action.Name), action.Name, GetActionHandlerName(action.Name), action.ChangePosture));
            // set weight
            SetBehaviorParameters(action);


            Method m = new Method("Skill.Framework.AI.BehaviorResult", GetActionHandlerName(action.Name), string.Empty, ActionHandlerParams) { IsPartial = _Tree.ExpandMethods };
            if (_Tree.ExpandMethods)
                m.Body = CreateMethodBody(action, "return Skill.Framework.AI.BehaviorResult.Failure;");
            else
                m.Body = string.Format("return OnAction( Actions.{0} , sender ,  parameters);", action.Name);
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

        private void CreateCondition(ConditionData condition)
        {
            // create condition variable
            Add(new Variable("Skill.Framework.AI.Condition", condition.Name, "null"));
            // new condition variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Condition(\"{1}\",{2});", Variable.GetName(condition.Name), condition.Name, GetConditionHandlerName(condition.Name)));
            // set weight
            SetBehaviorParameters(condition);
            // create condition handler method
            Method m = new Method("bool", GetConditionHandlerName(condition.Name), string.Empty, ConditionHandlerParams) { IsPartial = _Tree.ExpandMethods };

            if (_Tree.ExpandMethods)
                m.Body = CreateMethodBody(condition, "return false;");
            else
                m.Body = string.Format("return OnCondition( Conditions.{0}, sender ,  parameters);", condition.Name);
            Add(m);
        }

        private void CreateChangeState(ChangeStateData changeState)
        {
            // create condition variable
            Add(new Variable("Skill.Framework.AI.ChangeState", changeState.Name, "null"));
            // new condition variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.ChangeState(\"{1}\",\"{2}\");", Variable.GetName(changeState.Name), changeState.Name, changeState.DestinationState));
            // set weight
            SetBehaviorParameters(changeState);
        }

        private void CreateDecorator(DecoratorData decorator)
        {
            switch (decorator.Type)
            {
                case DecoratorType.Default:
                    CreateDefaultDecorator(decorator);
                    break;
                case DecoratorType.AccessLimit:
                    CreateAccessLimitDecorator((AccessLimitDecoratorData)decorator);
                    break;
            }
        }

        private void CreateDefaultDecorator(DecoratorData decorator)
        {
            // create decorator variable
            Add(new Variable("Skill.Framework.AI.Decorator", decorator.Name, "null"));
            // new decorator variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.Decorator(\"{1}\",{2});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name)));
            // set property SuccessOnFailHandler
            if (decorator.NeverFail == true) // default value is false, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetBehaviorParameters(decorator);
            // create decorator handler method
            Method m = new Method("bool", GetDecoratorHandlerName(decorator.Name), string.Empty, DecoratorHandlerParams) { IsPartial = _Tree.ExpandMethods };

            if (_Tree.ExpandMethods)
                m.Body = CreateMethodBody(decorator, "return false;");
            else
                m.Body = string.Format("return OnDecorator( Decorators.{0}, sender ,  parameters);", decorator.Name);
            Add(m);
        }

        private void CreateAccessLimitDecorator(AccessLimitDecoratorData decorator)
        {
            // create decorator variable
            Add(new Variable("Skill.Framework.AI.AccessLimitDecorator", decorator.Name, "null"));

            // new decorator variable inside CreateTree method            
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.AccessLimitDecorator(\"{1}\",{2},{3}.{4});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name), decorator.ClassName, decorator.AccessKey));
            // set property SuccessOnFailHandler
            if (decorator.NeverFail == true) // default value is false, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetBehaviorParameters(decorator);
            // create decorator handler method
            Method m = new Method("bool", GetDecoratorHandlerName(decorator.Name), string.Empty, DecoratorHandlerParams) { IsPartial = _Tree.ExpandMethods };
            if (_Tree.ExpandMethods)
                m.Body = CreateMethodBody(decorator, "return true;");
            else
                m.Body = string.Format("return OnDecorator( Decorators.{0}, sender ,  parameters);", decorator.Name);
            Add(m);
        }

        private void CreateComposite(CompositeData composite)
        {
            switch (composite.CompositeType)
            {
                case CompositeType.Sequence:
                    CreateSequenceSelector((SequenceSelectorData)composite);
                    break;
                case CompositeType.Concurrent:
                    CreateConcurrentSelector((ConcurrentSelectorData)composite);
                    break;
                case CompositeType.Random:
                    CreateRandomSelector((RandomSelectorData)composite);
                    break;
                case CompositeType.Priority:
                    CreatePrioritySelector((PrioritySelectorData)composite);
                    break;
                case CompositeType.State:
                    CreateBehaviorTreeState((BehaviorTreeStateData)composite);
                    break;
                case CompositeType.Loop:
                    CreateLoopSelector((LoopSelectorData)composite);
                    break;
                default:
                    throw new InvalidOperationException("Invalid CompositeType");
            }
            // set weight
            SetBehaviorParameters(composite);
        }

        private void CreateSequenceSelector(SequenceSelectorData sequenceSelector)
        {
            // create SequenceSelector variable
            Add(new Variable("Skill.Framework.AI.SequenceSelector", sequenceSelector.Name, "null"));
            // new SequenceSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.SequenceSelector(\"{1}\");", Variable.GetName(sequenceSelector.Name), sequenceSelector.Name));
        }

        private void CreateConcurrentSelector(ConcurrentSelectorData concurrentSelector)
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

        private void CreateRandomSelector(RandomSelectorData randomSelector)
        {
            // create RandomSelector variable
            Add(new Variable("Skill.Framework.AI.RandomSelector", randomSelector.Name, "null"));
            // new RandomSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.RandomSelector(\"{1}\");", Variable.GetName(randomSelector.Name), randomSelector.Name));
        }

        private void CreatePrioritySelector(PrioritySelectorData prioritySelector)
        {
            // create PrioritySelector variable
            Add(new Variable("Skill.Framework.AI.PrioritySelector", prioritySelector.Name, "null"));
            // new PrioritySelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.PrioritySelector(\"{1}\");", Variable.GetName(prioritySelector.Name), prioritySelector.Name));

            // set Priority property
            if (prioritySelector.Priority != PriorityType.HighestPriority) // default is HighestPriority                
                _CreateTreeMethodBody.AppendLine(SetProperty(prioritySelector.Name, "Priority", string.Format("Skill.Framework.AI.PriorityType.{0}", prioritySelector.Priority)));
        }

        private void CreateBehaviorTreeState(BehaviorTreeStateData state)
        {
            // create PrioritySelector variable
            Add(new Variable("Skill.Framework.AI.BehaviorTreeState", state.Name, "null"));
            // new PrioritySelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.Framework.AI.BehaviorTreeState(\"{1}\");", Variable.GetName(state.Name), state.Name));

            // set Priority property
            if (state.Priority != PriorityType.HighestPriority) // default is HighestPriority                
                _CreateTreeMethodBody.AppendLine(SetProperty(state.Name, "Priority", string.Format("Skill.Framework.AI.PriorityType.{0}", state.Priority)));
        }

        private void CreateLoopSelector(LoopSelectorData loopSelector)
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
