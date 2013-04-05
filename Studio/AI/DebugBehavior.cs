using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.AI
{
    class DebugBehavior
    {
        private float _ExecutionTime;
        private TimeSpan _ExecutionTimeSpan;

        public Skill.Framework.AI.Behavior Behavior { get; private set; }

        public Skill.Studio.AI.BehaviorViewModel ViewModel { get; private set; }


        /// <summary> Action Execution Time (for actions)</summary>
        public float ExecutionTime
        {
            get { return _ExecutionTime; }
            set
            {
                _ExecutionTime = value;
                _ExecutionTimeSpan = TimeSpan.FromMilliseconds(value * 1000);
            }
        }
        /// <summary> Is condition or decorator is valid </summary>
        public bool IsValid { get; set; }

        /// <summary> Is this behavior visited during last update  </summary>
        public bool IsVisited { get; set; }

        public DebugBehavior(Skill.Studio.AI.BehaviorViewModel vm)
        {
            IsValid = true;
            this.ViewModel = vm;
            CreateBehavior();
            this.Behavior.Tag = this.ViewModel;
        }

        private bool _Started;
        private TimeSpan _EndTime;
        private Skill.Framework.AI.BehaviorResult ActionHandler(Skill.Framework.AI.BehaviorParameterCollection parameters)
        {
            IsVisited = true;

            if (_Started)
            {
                if (ViewModel.Tree.DebugTimer >= _EndTime)
                {
                    _Started = false;
                    return Skill.Framework.AI.BehaviorResult.Success;
                }
                else
                    return Skill.Framework.AI.BehaviorResult.Running;
            }
            else
            {
                _Started = true;
                _EndTime = ViewModel.Tree.DebugTimer + _ExecutionTimeSpan;
                return Skill.Framework.AI.BehaviorResult.Running;
            }
        }
        private bool ConditionHandler(Skill.Framework.AI.BehaviorParameterCollection parameters)
        {
            IsVisited = true;
            return IsValid;
        }

        private void CreateBehavior()
        {
            switch (ViewModel.Model.BehaviorType)
            {
                case Skill.DataModels.AI.BehaviorType.Action:
                    Behavior = new Skill.Framework.AI.Action(ViewModel.Name, this.ActionHandler);
                    ExecutionTime = ((Skill.DataModels.AI.Action)ViewModel.Model).ExecutionTime;
                    break;
                case Skill.DataModels.AI.BehaviorType.Condition:
                    Behavior = new Skill.Framework.AI.Condition(ViewModel.Name, this.ConditionHandler);
                    IsValid = ((Skill.DataModels.AI.Condition)ViewModel.Model).IsValid;
                    break;
                case Skill.DataModels.AI.BehaviorType.Decorator:
                    Behavior = new Skill.Framework.AI.Decorator(ViewModel.Name, this.ConditionHandler);
                    ((Skill.Framework.AI.Decorator)Behavior).NeverFail = ((Skill.Studio.AI.DecoratorViewModel)ViewModel).NeverFail;
                    IsValid = ((Skill.DataModels.AI.Decorator)ViewModel.Model).IsValid;
                    break;
                case Skill.DataModels.AI.BehaviorType.Composite:

                    switch (((Skill.DataModels.AI.Composite)ViewModel.Model).CompositeType)
                    {
                        case Skill.DataModels.AI.CompositeType.Sequence:
                            Behavior = new Skill.Framework.AI.SequenceSelector(ViewModel.Name);
                            break;
                        case Skill.DataModels.AI.CompositeType.Concurrent:
                            Behavior = new Skill.Framework.AI.ConcurrentSelector(ViewModel.Name);
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).BreakOnConditionFailure = ((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).BreakOnConditionFailure;
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).FailurePolicy = (Skill.Framework.AI.FailurePolicy)((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).FailurePolicy;
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).SuccessPolicy = (Skill.Framework.AI.SuccessPolicy)((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).SuccessPolicy;
                            break;
                        case Skill.DataModels.AI.CompositeType.Random:
                            Behavior = new Skill.Framework.AI.RandomSelector(ViewModel.Name);
                            break;
                        case Skill.DataModels.AI.CompositeType.Priority:
                            Behavior = new Skill.Framework.AI.PrioritySelector(ViewModel.Name);
                            ((Skill.Framework.AI.PrioritySelector)Behavior).Priority = (Skill.Framework.AI.PriorityType)((Skill.Studio.AI.PrioritySelectorViewModel)ViewModel).Priority;
                            break;
                        case Skill.DataModels.AI.CompositeType.Loop:
                            Behavior = new Skill.Framework.AI.LoopSelector(ViewModel.Name);
                            ((Skill.Framework.AI.LoopSelector)Behavior).LoopCount = ((Skill.Studio.AI.LoopSelectorViewModel)ViewModel).LoopCount;
                            break;
                    }
                    break;
            }
            Behavior.Weight = ViewModel.Weight;
            Behavior.Concurrency = (Framework.AI.ConcurrencyMode)((int)ViewModel.Concurrency);
        }

        public void UpdateParameters()
        {
            Behavior.Weight = ViewModel.Weight;
            Behavior.Concurrency = (Framework.AI.ConcurrencyMode)((int)ViewModel.Concurrency);

            switch (ViewModel.Model.BehaviorType)
            {
                case Skill.DataModels.AI.BehaviorType.Action:
                    ExecutionTime = ((Skill.DataModels.AI.Action)ViewModel.Model).ExecutionTime;
                    break;
                case Skill.DataModels.AI.BehaviorType.Condition:
                    IsValid = ((Skill.DataModels.AI.Condition)ViewModel.Model).IsValid;
                    break;
                case Skill.DataModels.AI.BehaviorType.Decorator:
                    ((Skill.Framework.AI.Decorator)Behavior).NeverFail = ((Skill.Studio.AI.DecoratorViewModel)ViewModel).NeverFail;
                    IsValid = ((Skill.DataModels.AI.Decorator)ViewModel.Model).IsValid;
                    break;
                case Skill.DataModels.AI.BehaviorType.Composite:

                    switch (((Skill.DataModels.AI.Composite)ViewModel.Model).CompositeType)
                    {
                        case Skill.DataModels.AI.CompositeType.Concurrent:
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).BreakOnConditionFailure = ((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).BreakOnConditionFailure;
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).FailurePolicy = (Skill.Framework.AI.FailurePolicy)((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).FailurePolicy;
                            ((Skill.Framework.AI.ConcurrentSelector)Behavior).SuccessPolicy = (Skill.Framework.AI.SuccessPolicy)((Skill.Studio.AI.ConcurrentSelectorViewModel)ViewModel).SuccessPolicy;                            
                            break;
                        case Skill.DataModels.AI.CompositeType.Priority:
                            ((Skill.Framework.AI.PrioritySelector)Behavior).Priority = (Skill.Framework.AI.PriorityType)((Skill.Studio.AI.PrioritySelectorViewModel)ViewModel).Priority;
                            break;
                        case Skill.DataModels.AI.CompositeType.Loop:
                            ((Skill.Framework.AI.LoopSelector)Behavior).LoopCount = ((Skill.Studio.AI.LoopSelectorViewModel)ViewModel).LoopCount;
                            break;
                        default:
                            break;
                    }

                    break;
            }
        }

        public void UpdateChildren()
        {
            _Started = false;
            if (Behavior.Type == Skill.Framework.AI.BehaviorType.Composite)
            {
                ((Skill.Framework.AI.Composite)Behavior).RemoveAll();
                foreach (BehaviorViewModel vm in ViewModel)
                {
                    ((Skill.Framework.AI.Composite)Behavior).Add(vm.Debug.Behavior);
                    vm.Debug.UpdateChildren();
                }

            }
            else if (Behavior.Type == Skill.Framework.AI.BehaviorType.Decorator)
            {
                if (ViewModel.Count > 0 && ViewModel[0] != null)
                {
                    ((Skill.Framework.AI.Decorator)Behavior).SetChild(((BehaviorViewModel)ViewModel[0]).Debug.Behavior);
                    ((BehaviorViewModel)ViewModel[0]).Debug.UpdateChildren();
                }
            }
        }


        public void ValidateBrush(bool isDebuging)
        {
            ViewModel.IsVisited = IsVisited;

            if (isDebuging)
            {

                ViewModel.BackBrush = IsValid ? Editor.BehaviorBrushes.EnableBrush : Editor.BehaviorBrushes.DisableBrush;

                if (IsVisited)
                {
                    switch (Behavior.Result)
                    {
                        case Skill.Framework.AI.BehaviorResult.Failure:
                            ViewModel.BorderBrush = Editor.BehaviorBrushes.FailedBrush;
                            ViewModel.ConnectionStroke = 3;
                            break;
                        case Skill.Framework.AI.BehaviorResult.Success:
                            ViewModel.BorderBrush = Editor.BehaviorBrushes.SuccessBrush;
                            ViewModel.ConnectionStroke = 3;
                            break;
                        case Skill.Framework.AI.BehaviorResult.Running:
                            ViewModel.BorderBrush = Editor.BehaviorBrushes.RunningBrush;
                            ViewModel.ConnectionStroke = 5;
                            break;
                    }
                }
                else
                {
                    ViewModel.BorderBrush = Editor.BehaviorBrushes.DefaultBorderBrush;
                    ViewModel.ConnectionStroke = 2;
                }
            }
            else
            {
                ViewModel.BackBrush = Editor.BehaviorBrushes.DefaultBackBrush;
                ViewModel.BorderBrush = ViewModel.IsSelected ? Editor.BehaviorBrushes.SelectedBrush : Editor.BehaviorBrushes.DefaultBorderBrush;
                ViewModel.ConnectionStroke = 2;
            }

            foreach (BehaviorViewModel vm in ViewModel)
                vm.Debug.ValidateBrush(isDebuging);
            IsVisited = false;
        }
    }
}
