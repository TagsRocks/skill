using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;

namespace Skill.Studio.Compiler
{
    public class BehaviorTreeCompiler : DataCompiler
    {
        private List<Behavior> _Behaviors = new List<Behavior>();
        private BehaviorTree _Tree;

        public SharedAccessKeysCompiler AccessKeysCompiler { get; private set; }

        public BehaviorTreeCompiler(ICollection<CompileError> errors)
            : base(EntityType.BehaviorTree, errors)
        {
            AccessKeysCompiler = new SharedAccessKeysCompiler(errors);
        }

        protected override void Compile()
        {
            this._Tree = Node.SavedData as BehaviorTree;
            if (this._Tree == null) return;
            CreateBehaviorList();
            CheckDefaultState();
            SearchForBehaviorErrors();
            SearchForBehaviorWarnings();
            AccessKeysCompiler.Compile(_Tree.AccessKeys);
        }

        #region CheckDefaultState
        private void CheckDefaultState()
        {
            if (string.IsNullOrEmpty(this._Tree.DefaultState))
                AddError(string.Format("Invalid DefaultState for BehaviorTree : {0}.", _Tree.Name));
            else
            {
                bool found = false;
                foreach (var s in _Tree.States)
                {
                    if (s.Name == _Tree.DefaultState)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    AddError(string.Format("Invalid DefaultState for BehaviorTree : {0}.", _Tree.Name));
            }

        }
        #endregion

        #region Create list of behaviors
        private void CreateBehaviorList()
        {
            _Behaviors.Clear();
            foreach (var b in _Tree.States)
                CreateBehaviorList(b);
        }

        private void CreateBehaviorList(Behavior parent)
        {
            if (!_Behaviors.Contains(parent)) _Behaviors.Add(parent);
            foreach (var child in parent)
            {
                CreateBehaviorList(child);
            }
        }
        #endregion

        #region Search For Behavior Errors

        private void SearchForBehaviorErrors()
        {
            List<string> nameList = new List<string>(50);
            foreach (Behavior b in _Behaviors)
            {
                if (string.IsNullOrEmpty(b.Name))
                {
                    AddError("There is a Behavior node with empty name.");
                }
                else
                {
                    if (!nameList.Contains(b.Name))
                    {
                        int count = _Behaviors.Count(c => c.Name == b.Name);
                        if (count > 1)
                            AddError(string.Format("There are {0} behaviors node in BehaviorTree with same name ({1}).", count, b.Name));
                        nameList.Add(b.Name);
                    }

                    CheckParameterError(b);

                    if (b.BehaviorType == BehaviorType.Decorator)
                        CheckAccessKey(b as Decorator);
                    if (b.Weight <= 0)
                        AddError(string.Format("Weight of Behavior node {0} is invalid (must be greater than 0).", b.Name));
                }
            }
            nameList.Clear();
        }
        #endregion

        #region Check AccessKeys

        private void CheckAccessKey(Decorator decorator)
        {

            if (decorator.Type == DecoratorType.AccessLimit)
            {
                AccessLimitDecorator accessLimitDecorator = (AccessLimitDecorator)decorator;
                if (string.IsNullOrEmpty(accessLimitDecorator.AccessKey))
                {
                    AddError(string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.AccessKey, accessLimitDecorator.Name));
                    return;
                }

                if (string.IsNullOrEmpty(accessLimitDecorator.Address))
                {
                    if (_Tree.AccessKeys.Keys.Count(c => c.Key == accessLimitDecorator.AccessKey) < 1)
                    {
                        AddError(string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.AccessKey, accessLimitDecorator.Name));
                    }
                }
                else
                {
                    SharedAccessKeysNodeViewModel sharedAccessKeysVM = Node.Project.GetNode(accessLimitDecorator.Address) as SharedAccessKeysNodeViewModel;
                    if (sharedAccessKeysVM == null)
                        AddError(string.Format("The SharedAccessKeys address '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.Address, accessLimitDecorator.Name));
                    else
                    {
                        SharedAccessKeys model = sharedAccessKeysVM.SavedData as SharedAccessKeys;
                        if (model == null)
                            AddError(string.Format("The SharedAccessKeys address '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.Address, accessLimitDecorator.Name));
                        else if (model.Keys.Count(c => c.Key == accessLimitDecorator.AccessKey) < 1)
                            AddError(string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.AccessKey, accessLimitDecorator.Name));
                    }
                }
            }
        }
        #endregion

        #region Check Parameters

        private void CheckParameterError(Behavior b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                ParameterCollection parameters = b.GetParameters(i);
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        if (string.IsNullOrEmpty(b.Name))
                        {
                            AddError(string.Format("Invalid parameter of Behavior node {0} (can not be null or empty).", b[i].Name));
                        }
                        else
                        {
                            int count = parameters.Count(p => p.Name == item.Name);
                            if (count > 1)
                                AddError(string.Format("There are {0} parameters for behaviors node {1} with same name ({2}).", count, b[i].Name, item.Name));
                        }
                    }
                }
            }
        }

        #endregion

        #region Search For Behavior Warnings
        private void SearchForBehaviorWarnings()
        {
            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == BehaviorType.Composite)
                {
                    Composite composite = (Composite)b;
                    if (composite.Count == 0)
                    {
                        AddError(string.Format("Composite node {0} has not any children.", composite.Name));
                    }
                    if (composite.CompositeType == CompositeType.Priority || composite.CompositeType == CompositeType.Concurrent || composite.CompositeType == CompositeType.State) // check if a Decorator with NeverFaile property is child of PrioritySelector or ConcurrentSelector
                    {
                        foreach (var child in composite)
                        {
                            if (child != null && child.BehaviorType == BehaviorType.Decorator)
                            {
                                if (((Decorator)child).NeverFail)
                                {
                                    if (composite.CompositeType == CompositeType.Priority || composite.CompositeType == CompositeType.State)
                                        AddWarning(string.Format("Decorator '{0}' with 'NeverFail' property setted to 'true' is child of PrioritySelector '{1}'. This cause next children unreachable.", child.Name, b.Name));
                                    else if (composite.CompositeType == CompositeType.Concurrent)
                                    {
                                        if (((ConcurrentSelector)composite).SuccessPolicy == SuccessPolicy.SucceedOnOne)
                                            AddWarning(string.Format("Decorator '{0}' with 'NeverFail' property setted to 'true' is child of ConcurrentSelector '{1}' width 'SuccessPolicy' property setted to 'SucceedOnOne' . This cause ConcurrentSelector never fail.", child.Name, b.Name));
                                    }
                                }
                            }
                        }
                    }

                    if (composite.CompositeType != CompositeType.Random)
                    {
                        foreach (var child in composite)
                        {
                            if (child != null && child.BehaviorType == BehaviorType.ChangeState)
                            {
                                int index = composite.IndexOf(child);
                                if (index < composite.Count - 1)
                                    AddWarning(string.Format("There are unreachable behaviors in Composite '{0}', after ChangeState '{1}'", composite.Name, child.Name));
                            }
                        }
                    }
                }
                else if (b.BehaviorType == BehaviorType.Decorator)
                {
                    Decorator decorator = (Decorator)b;
                    if (decorator.Count == 0)
                    {
                        AddError(string.Format("Decorator node {0} has not any children.", decorator.Name));
                    }
                }
            }
        }
        #endregion
    }
}
