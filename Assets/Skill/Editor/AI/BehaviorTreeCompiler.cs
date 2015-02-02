using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skill.Editor.AI
{
    public static class BehaviorTreeCompiler
    {
        private static bool _ErrorFound;
        private static BehaviorTreeData _Tree;
        private static SharedAccessKeysData[] _AccessKeys;
        private static List<BehaviorData> _Behaviors;

        public static bool Compile(BehaviorTreeData data, SharedAccessKeysData[] accessKeys)
        {
            _ErrorFound = false;
            _Tree = data;
            _AccessKeys = accessKeys;

            if (_Behaviors == null)
                _Behaviors = new List<BehaviorData>();

            CreateBehaviorList();
            CheckDefaultState();
            SearchForBehaviorErrors();
            SearchForBehaviorWarnings();

            _Tree = null;
            _AccessKeys = null;
            return !_ErrorFound;
        }

        #region CheckDefaultState
        private static void CheckDefaultState()
        {
            if (string.IsNullOrEmpty(_Tree.DefaultState))
            {
                Debug.LogError(string.Format("Invalid DefaultState for BehaviorTree : {0}.", _Tree.Name));
                _ErrorFound = true;
            }
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
                {
                    Debug.LogError(string.Format("Invalid DefaultState for BehaviorTree : {0}.", _Tree.Name));
                    _ErrorFound = true;
                }
            }

        }
        #endregion

        #region Create list of behaviors
        private static void CreateBehaviorList()
        {
            _Behaviors.Clear();
            foreach (var b in _Tree.States)
                CreateBehaviorList(b);
        }

        private static void CreateBehaviorList(BehaviorData parent)
        {
            if (!_Behaviors.Contains(parent)) _Behaviors.Add(parent);
            foreach (var child in parent)
            {
                CreateBehaviorList(child);
            }
        }
        #endregion

        #region Search For Behavior Errors

        private static void SearchForBehaviorErrors()
        {
            List<string> nameList = new List<string>(50);
            foreach (BehaviorData b in _Behaviors)
            {
                if (string.IsNullOrEmpty(b.Name))
                {
                    Debug.LogError("There is a Behavior node with empty name.");
                    _ErrorFound = true;
                }
                else
                {
                    if (!nameList.Contains(b.Name))
                    {
                        int count = _Behaviors.Count(c => c.Name == b.Name);
                        if (count > 1)
                        {
                            Debug.LogError(string.Format("There are {0} behaviors node in BehaviorTree with same name ({1}).", count, b.Name));
                            _ErrorFound = true;
                        }
                        nameList.Add(b.Name);
                    }

                    CheckParameterError(b);

                    if (b.BehaviorType == Framework.AI.BehaviorType.Decorator)
                    {
                        DecoratorData decorator = (DecoratorData)b;
                        if (decorator.Count == 0)
                        {
                            Debug.LogError(string.Format("Decorator node {0} has not any children.", decorator.Name));
                            _ErrorFound = true;
                        }
                        CheckAccessKey(decorator);
                    }
                    if (b.Weight <= 0)
                    {
                        Debug.LogError(string.Format("Weight of Behavior node {0} is invalid (must be greater than 0).", b.Name));
                        _ErrorFound = true;
                    }
                }
            }
            nameList.Clear();
        }
        #endregion

        #region Check AccessKeys

        private static void CheckAccessKey(DecoratorData decorator)
        {

            if (decorator.Type == Framework.AI.DecoratorType.AccessLimit)
            {
                AccessLimitDecoratorData accessLimitDecorator = (AccessLimitDecoratorData)decorator;
                if (string.IsNullOrEmpty(accessLimitDecorator.AccessKey))
                {
                    Debug.LogError(string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.AccessKey, accessLimitDecorator.Name));
                    _ErrorFound = true;
                    return;
                }

                if (string.IsNullOrEmpty(accessLimitDecorator.ClassName))
                {
                    Debug.LogError(string.Format(" Invalid AccessKey for behavior node '{0}'", accessLimitDecorator.Name));
                    _ErrorFound = true;
                }
                else
                {
                    SharedAccessKeysData ac = null;
                    foreach (var item in _AccessKeys)
                    {
                        if (item != null)
                        {
                            if (item.Name == accessLimitDecorator.ClassName)
                            {
                                ac = item;
                                break;
                            }
                        }
                    }
                    if (ac == null)
                    {
                        Debug.LogError(string.Format(" Invalid AccessKey for behavior node '{0}'", accessLimitDecorator.Name));
                        _ErrorFound = true;
                    }
                    else
                    {
                        if (ac.Keys.Count(c => c.Key == accessLimitDecorator.AccessKey) < 1)
                        {
                            Debug.LogError(string.Format("The provided AccessKey '{0}' for behavior node '{1}' does not exist.", accessLimitDecorator.AccessKey, accessLimitDecorator.Name));
                            _ErrorFound = true;
                        }
                    }
                }

            }
        }
        #endregion

        #region Check Parameters

        private static void CheckParameterError(BehaviorData b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                ParameterDataCollection parameters = b.GetParameters(i);
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        if (string.IsNullOrEmpty(b.Name))
                        {
                            Debug.LogError(string.Format("Invalid parameter of Behavior node {0} (can not be null or empty).", b[i].Name));
                            _ErrorFound = true;
                        }
                        else
                        {
                            int count = parameters.Count(p => p.Name == item.Name);
                            if (count > 1)
                            {
                                Debug.LogError(string.Format("There are {0} parameters for behaviors node {1} with same name ({2}).", count, b[i].Name, item.Name));
                                _ErrorFound = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Search For Behavior Warnings
        private static void SearchForBehaviorWarnings()
        {
            foreach (var b in _Behaviors)
            {
                if (b.BehaviorType == Framework.AI.BehaviorType.Composite)
                {
                    CompositeData composite = (CompositeData)b;
                    if (composite.Count == 0)
                    {
                        Debug.LogError(string.Format("Composite node {0} has not any children.", composite.Name));
                        _ErrorFound = true;
                    }
                    if (composite.CompositeType == Framework.AI.CompositeType.Priority || composite.CompositeType == Framework.AI.CompositeType.Concurrent || composite.CompositeType == Framework.AI.CompositeType.State) // check if a Decorator with NeverFaile property is child of PrioritySelector or ConcurrentSelector
                    {
                        foreach (var child in composite)
                        {
                            if (child != null && child.BehaviorType == Framework.AI.BehaviorType.Decorator)
                            {
                                if (((DecoratorData)child).NeverFail)
                                {
                                    if (composite.CompositeType == Framework.AI.CompositeType.Priority || composite.CompositeType == Framework.AI.CompositeType.State)
                                        Debug.LogWarning(string.Format("Decorator '{0}' with 'NeverFail' property setted to 'true' is child of PrioritySelector '{1}'. This cause next children unreachable.", child.Name, b.Name));
                                    else if (composite.CompositeType == Framework.AI.CompositeType.Concurrent)
                                    {
                                        if (((ConcurrentSelectorData)composite).SuccessPolicy == Framework.AI.SuccessPolicy.SucceedOnOne)
                                            Debug.LogWarning(string.Format("Decorator '{0}' with 'NeverFail' property setted to 'true' is child of ConcurrentSelector '{1}' width 'SuccessPolicy' property setted to 'SucceedOnOne' . This cause ConcurrentSelector never fail.", child.Name, b.Name));
                                    }
                                }
                            }
                        }
                    }

                    if (composite.CompositeType != Framework.AI.CompositeType.Random)
                    {
                        foreach (var child in composite)
                        {
                            if (child != null && child.BehaviorType == Framework.AI.BehaviorType.ChangeState)
                            {
                                int index = composite.IndexOf(child);
                                if (index < composite.Count - 1)
                                    Debug.LogWarning(string.Format("There are unreachable behaviors in Composite '{0}', after ChangeState '{1}'", composite.Name, child.Name));
                            }
                        }
                    }
                }
                else if (b.BehaviorType == Framework.AI.BehaviorType.Decorator)
                {
                    DecoratorData decorator = (DecoratorData)b;
                    if (decorator.Type == Framework.AI.DecoratorType.AccessLimit)
                    {
                        AccessLimitDecoratorData al = (AccessLimitDecoratorData)decorator;

                        if (decorator.Child.BehaviorType == Framework.AI.BehaviorType.ChangeState && al.KeyType == Framework.AI.AccessKeyType.CounterLimit)
                        {
                            Debug.LogWarning(string.Format("AccessLimitDecorator '{0} with CounterLimit accesskey' has a ChangeState child '{1}'. CounterLimit key will be unlocked after change state.", decorator.Name, decorator.Child.Name));
                        }
                    }
                }
            }
        }
        #endregion
    }

}