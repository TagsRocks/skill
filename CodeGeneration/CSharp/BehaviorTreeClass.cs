using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// generate code for a behavior tree class
    /// </summary>
    class BehaviorTreeClass : Class
    {
        #region Variables
        private static string[] BehaviorEventHandlerParams = new string[] { "Skill.AI.Behavior sender", "Skill.AI.BehaviorResult result", "Skill.AI.BehaviorTree tree" };
        private static string[] ConditionHandlerParams = new string[] { "Skill.AI.BehaviorTree tree", "Skill.AI.BehaviorParameterCollection parameters" };
        private static string[] DecoratorHandlerParams = ConditionHandlerParams;
        private static string[] ActionHandlerParams = ConditionHandlerParams;
        private static string AccessKeysVariableName = "AccessKeys";
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
            AddInherit("Skill.AI.BehaviorTree");


            CreateAccessKeys(tree);

            Method constructor = new Method("", Name, "", "Skill.Controllers.Controller controller");
            constructor.Modifiers = Modifiers.Public;
            constructor.BaseMethod = ":base(controller)";



            Add(constructor);

            Method createTree = new Method("Skill.AI.Behavior", "CreateTree", this._CreateTreeMethodBody.ToString());
            createTree.IsPartial = false;
            createTree.SubMethod = SubMethod.Override;
            createTree.Modifiers = Modifiers.Protected;
            Add(createTree);
        }

        private void CreateAccessKeys(BehaviorTree tree)
        {
            //if (tree.AccessKeys.Count > 0)
            //{
            //    Add(new Variable("System.Collections.Generic.Dictionary<string,Skill.AI.AccessKey>", AccessKeysVariableName, "null") { IsStatic = true });

            //    StringBuilder staticConstructorBody = new StringBuilder();
            //    staticConstructorBody.AppendLine(string.Format("{0} = new System.Collections.Generic.Dictionary<string,Skill.AI.AccessKey>();", Variable.GetName(AccessKeysVariableName)));
            //    foreach (var ak in tree.AccessKeys)
            //    {
            //        switch (ak.Value.Type)
            //        {
            //            case AccessKeyType.CounterLimit:
            //                staticConstructorBody.AppendLine(string.Format("{0}.Add( \"{1}\" , new CounterLimitAccessKey(\"{2}\",{3}) );", Variable.GetName(AccessKeysVariableName), ak.Key, ak.Key, ((CounterLimitAccessKey)ak.Value).MaxAccessCount));
            //                break;
            //            case AccessKeyType.TimeLimit:
            //                staticConstructorBody.AppendLine(string.Format("{0}.Add( \"{1}\" , new TimeLimitAccessKey(\"{2}\",{3}) );", Variable.GetName(AccessKeysVariableName), ak.Key, ak.Key, ((TimeLimitAccessKey)ak.Value).TimeInterval));
            //                break;
            //        }
            //    }

            //    Method staticConstructor = new Method(string.Empty, tree.Name, staticConstructorBody.ToString()) { IsStatic = true, Modifiers = Modifiers.None };
            //    Add(staticConstructor);
            //}
        }

        #endregion

        #region CreateBehaviorList
        /// <summary>
        /// Create list of behaviors that are in hierarchy
        /// </summary>
        private void CreateBehaviorList()
        {
            AddToBehaviors(_Tree.Root);
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
                }
            }

            // left one line empty
            _CreateTreeMethodBody.AppendLine();

            // add child of each behaviors(Decorator and Selectors)
            foreach (var b in _Behaviors)
            {
                // decorator has one child
                if (b.BehaviorType == BehaviorType.Decorator)
                {
                    int[] childId = Behavior.ConvertToIndices(b.GetChildrenString());
                    if (childId != null && childId.Length > 0)
                    {
                        Behavior child = Find(childId[0]);
                        if (child != null)
                            _CreateTreeMethodBody.AppendLine(string.Format("{0}.SetChild({1});", Variable.GetName(b.Name), Variable.GetName(child.Name)));
                    }
                }
                // selector has multiple child
                else if (b.BehaviorType == BehaviorType.Composite)
                {
                    int[] childrenIds = Behavior.ConvertToIndices(b.GetChildrenString());
                    if (childrenIds != null && childrenIds.Length > 0)
                    {
                        foreach (var childId in childrenIds)
                        {
                            Behavior child = Find(childId);
                            if (child != null)
                                _CreateTreeMethodBody.AppendLine(string.Format("{0}.Add({1});", Variable.GetName(b.Name), Variable.GetName(child.Name)));
                        }
                    }
                }
            }
            // then return root of tree
            var root = Find(_Tree.Root.Id);
            if (root != null)
            {
                _CreateTreeMethodBody.AppendLine(string.Format("return {0};", Variable.GetName(root.Name)));
            }
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

        private void CreateEvents(Behavior behavior)
        {
            // create failure event handler and assign it to failure event
            if (behavior.FailureEvent)
            {
                string eventName = behavior.Name + "_Failure";
                Add(new Method("void", eventName, "", BehaviorEventHandlerParams) { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Failure += new BehaviorEventHandler({1});", Variable.GetName(behavior.Name), eventName));
            }

            // create success event handler and assign it to success event
            if (behavior.SuccessEvent)
            {
                string eventName = behavior.Name + "_Success";
                Add(new Method("void", eventName, "", BehaviorEventHandlerParams) { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Success += new BehaviorEventHandler({1});", Variable.GetName(behavior.Name), eventName));
            }

            // create running event handler and assign it to running event
            if (behavior.RunningEvent)
            {
                string eventName = behavior.Name + "_Running";
                Add(new Method("void", eventName, "", BehaviorEventHandlerParams) { IsPartial = true });
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Running += new BehaviorEventHandler({1});", Variable.GetName(behavior.Name), eventName));
            }
        }

        private void CreateAction(Skill.DataModels.AI.Action action)
        {
            // create action variable
            Add(new Variable("Skill.AI.Action", action.Name, "null"));
            // new action inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.Action(\"{1}\",{2});", Variable.GetName(action.Name), action.Name, GetActionHandlerName(action.Name)));
            // set weight
            SetWeight(action);
            // create action handler method
            Method m = new Method("Skill.AI.BehaviorResult", GetActionHandlerName(action.Name), "return Skill.AI.BehaviorResult.Failure;", ActionHandlerParams);
            m.IsPartial = true;
            Add(m);
            // create events handlers (success, failure and Running)
            CreateEvents(action);
        }

        private void CreateCondition(Condition condition)
        {
            // create condition variable
            Add(new Variable("Skill.AI.Condition", condition.Name, "null"));
            // new condition variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.Condition(\"{1}\",{2});", Variable.GetName(condition.Name), condition.Name, GetConditionHandlerName(condition.Name)));
            // set weight
            SetWeight(condition);
            // create condition handler method
            Method m = new Method("bool", GetConditionHandlerName(condition.Name), "return false;", ConditionHandlerParams);
            m.IsPartial = true;
            Add(m);
            // create events handlers (success, failure and Running)
            CreateEvents(condition);
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
            Add(new Variable("Skill.AI.Decorator", decorator.Name, "null"));
            // new decorator variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.Decorator(\"{1}\",{2});", Variable.GetName(decorator.Name), decorator.Name, GetDecoratorHandlerName(decorator.Name)));
            // set property SuccessOnFailHandler
            if (decorator.NeverFail != true) // default value is true, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetWeight(decorator);
            // create decorator handler method
            Method m = new Method("bool", GetDecoratorHandlerName(decorator.Name), "return false;", DecoratorHandlerParams);
            m.IsPartial = true;
            Add(m);
            // create events handlers (success, failure and Running)
            CreateEvents(decorator);
        }

        private void CreateAccessLimitDecorator(AccessLimitDecorator decorator)
        {
            // create decorator variable
            Add(new Variable("Skill.AI.AccessLimitDecorator", decorator.Name, "null"));
            // new decorator variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.AccessLimitDecorator(\"{1}\",{2}[\"{3}\"]);", Variable.GetName(decorator.Name), decorator.Name, Variable.GetName(AccessKeysVariableName), decorator.AccessKey));
            // set property SuccessOnFailHandler
            if (decorator.NeverFail != true) // default value is true, so it is not necessary to set it
                _CreateTreeMethodBody.AppendLine(SetProperty(decorator.Name, "NeverFail", decorator.NeverFail.ToString().ToLower()));
            // set weight
            SetWeight(decorator);
            // create events handlers (success, failure and Running)
            CreateEvents(decorator);
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
                case CompositeType.Loop:
                    CreateLoopSelector((LoopSelector)composite);
                    break;
                default:
                    throw new InvalidOperationException("Invalid CompositeType");
            }
            // set weight
            SetWeight(composite);
            // create events handlers (success, failure and Running)
            CreateEvents(composite);
        }

        private void CreateSequenceSelector(SequenceSelector sequenceSelector)
        {
            // create SequenceSelector variable
            Add(new Variable("Skill.AI.SequenceSelector", sequenceSelector.Name, "null"));
            // new SequenceSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.SequenceSelector(\"{1}\");", Variable.GetName(sequenceSelector.Name), sequenceSelector.Name));
        }

        private void CreateConcurrentSelector(ConcurrentSelector concurrentSelector)
        {
            // create ConcurrentSelector variable
            Add(new Variable("Skill.AI.ConcurrentSelector", concurrentSelector.Name, "null"));
            // new ConcurrentSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.ConcurrentSelector(\"{1}\");", Variable.GetName(concurrentSelector.Name), concurrentSelector.Name));

            // set FirstConditions Property
            if (concurrentSelector.FirstConditions != true) // default is true
                _CreateTreeMethodBody.AppendLine(string.Format("{0}.FirstConditions = {1};", Variable.GetName(concurrentSelector.Name), concurrentSelector.FirstConditions));

            // set BreakOnConditionFailure Property
            if (concurrentSelector.BreakOnConditionFailure != false) // default is false
                _CreateTreeMethodBody.AppendLine(string.Format("{0}.BreakOnConditionFailure = {1};", Variable.GetName(concurrentSelector.Name), concurrentSelector.BreakOnConditionFailure));

            // set SuccessPolicy Property
            if (concurrentSelector.SuccessPolicy != SuccessPolicy.SucceedOnAll) // default is SucceedOnAll
                _CreateTreeMethodBody.AppendLine(string.Format("{0}.SuccessPolicy = Skill.AI.SuccessPolicy.{1};", Variable.GetName(concurrentSelector.Name), concurrentSelector.SuccessPolicy));

            // set FailurePolicy Property
            if (concurrentSelector.FailurePolicy != FailurePolicy.FailOnAll) // default is FailOnAll
                _CreateTreeMethodBody.AppendLine(string.Format("{0}.FailurePolicy = Skill.AI.FailurePolicy.{1};", Variable.GetName(concurrentSelector.Name), concurrentSelector.FailurePolicy));
        }

        private void CreateRandomSelector(RandomSelector randomSelector)
        {
            // create RandomSelector variable
            Add(new Variable("Skill.AI.RandomSelector", randomSelector.Name, "null"));
            // new RandomSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.RandomSelector(\"{1}\");", Variable.GetName(randomSelector.Name), randomSelector.Name));
        }

        private void CreatePrioritySelector(PrioritySelector prioritySelector)
        {
            // create PrioritySelector variable
            Add(new Variable("Skill.AI.PrioritySelector", prioritySelector.Name, "null"));
            // new PrioritySelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.PrioritySelector(\"{1}\");", Variable.GetName(prioritySelector.Name), prioritySelector.Name));

            // set Priority property
            if (prioritySelector.Priority != PriorityType.HighestPriority) // default is HighestPriority
                _CreateTreeMethodBody.AppendLine(string.Format("this.{0}.Priority = Skill.AI.PriorityType.{1};", Variable.GetName(prioritySelector.Name), prioritySelector.Priority));
        }

        private void CreateLoopSelector(LoopSelector loopSelector)
        {
            // create LoopSelector variable
            Add(new Variable("Skill.AI.LoopSelector", loopSelector.Name, "null"));
            // new LoopSelector variable inside CreateTree method
            _CreateTreeMethodBody.AppendLine(string.Format("this.{0} = new Skill.AI.LoopSelector(\"{1}\");", Variable.GetName(loopSelector.Name), loopSelector.Name));

            // set LoopCount property
            if (loopSelector.LoopCount != -1) // default is -1 (infinity)
                _CreateTreeMethodBody.AppendLine(string.Format("{0}.LoopCount = {1};", Variable.GetName(loopSelector.Name), loopSelector.LoopCount));
        }

        #endregion

    }
}
