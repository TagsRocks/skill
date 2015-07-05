using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework
{
    /// <summary>
    /// Manage parameters and states of Animator
    /// </summary>
    /// <remarks>
    /// This class designed to manage states and parameters of Animator and make easy access to them
    /// Normally user must inherit from this class and defines AnimatorStateGroups and AnimatorParameters as Properties of class and initialize them
    /// in constructor, then another class can instantiate one and set parameters and hook events of AnimatorStateGroups
    /// </remarks>
    [RequireComponent(typeof(Animator))]
    public class AnimatorManager : Skill.Framework.DynamicBehaviour
    {

        private string[] _Layers;
        private Dictionary<string, AnimatorParameter> _Parameters;
        private List<AnimatorStateGroup> _StateGroups;
        private bool[] _IsInTransitions;


        public Animator Animator { get; private set; }
        public AnimatorStateInfo[] CurrentLayerStates { get; private set; }
        public AnimatorStateInfo[] NextLayerStates { get; private set; }

        public string GetActiveGroupNames(int layerIndex)
        {
            string result = string.Empty;
            for (int i = 0; i < _StateGroups.Count; i++)
            {
                var group = _StateGroups[i];
                if (group.LayerIndex == layerIndex && group.IsRelevant)
                {
                    if (result != string.Empty) result += ", ";
                    result += group.Name;
                }
            }
            return result;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            this.Animator = GetComponent<Animator>();
            if (Animator == null)
                throw new ArgumentNullException("Animator is null");
        }

        protected override void Awake()
        {
            base.Awake();

            this.CurrentLayerStates = new AnimatorStateInfo[Animator.layerCount];
            this.NextLayerStates = new AnimatorStateInfo[Animator.layerCount];
            _StateGroups = new List<AnimatorStateGroup>();
            this._IsInTransitions = new bool[Animator.layerCount];
            this._Layers = new string[Animator.layerCount];
            for (int i = 0; i < Animator.layerCount; i++)
            {
                this._Layers[i] = Animator.GetLayerName(i);
            }
            this._Parameters = new Dictionary<string, AnimatorParameter>();

            CreateParameters();
            CreateStates();
        }

        protected virtual void CreateParameters() { }
        protected virtual void CreateStates() { }

        public AnimatorParameter GetParameters(string parameterName)
        {
            AnimatorParameter result = null;
            _Parameters.TryGetValue(parameterName, out result);
            return result;
        }

        protected virtual void FixedUpdate()
        {
            for (int i = 0; i < Animator.layerCount; i++)
            {
                CurrentLayerStates[i] = Animator.GetCurrentAnimatorStateInfo(i);
                _IsInTransitions[i] = Animator.IsInTransition(i);
                NextLayerStates[i] = Animator.GetNextAnimatorStateInfo(i);

            }

            foreach (var aug in _StateGroups)
            {
                aug.FixedUpdate(ref CurrentLayerStates[aug.LayerIndex], ref NextLayerStates[aug.LayerIndex], _IsInTransitions[aug.LayerIndex]);
            }
        }

        protected override void Update()
        {
            for (int i = 0; i < _StateGroups.Count; i++)
            {
                _StateGroups[i].Update();
            }
            base.Update();
        }


        public AnimatorStateGroup CreateStateGroup(string name, int layerIndex)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid group name");
            if (layerIndex < 0 || layerIndex >= Animator.layerCount)
                throw new IndexOutOfRangeException("Invalid layerIndex");

            for (int i = 0; i < _StateGroups.Count; i++)
            {
                if (name == _StateGroups[i].Name)
                    throw new ArgumentException("Group name already exist");
            }

            AnimatorStateGroup group = new AnimatorStateGroup(name, Animator.GetLayerName(layerIndex), layerIndex);
            _StateGroups.Add(group);
            return group;
        }

        public BooleanParameter CreateBooleanParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            BooleanParameter parameter = new BooleanParameter(parameterName, this);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public IntegerParameter CreateIntegerParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            IntegerParameter parameter = new IntegerParameter(parameterName, this);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public FloatParameter CreateFloatParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            FloatParameter parameter = new FloatParameter(parameterName, this);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public TriggerParameter CreateTriggerParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            TriggerParameter parameter = new TriggerParameter(parameterName, this);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        #region StateGroup
        /// <summary>
        /// it groups some states and allow user to do somethings when any of this is states BecameRelevant, CeaseRelevant and StayRelevant
        /// </summary>
        public class AnimatorStateGroup
        {
            private bool _Sorted;
            private List<int> _NameMeshs;
            private string _LayerName;
            private int _SideEvents;

            public int LayerIndex { get; private set; }
            public string Name { get; private set; }
            public bool IsRelevant { get; private set; }


            public event EventHandler BecameRelevant;
            private void OnBecameRelevant()
            {
                if (BecameRelevant != null) BecameRelevant(this, EventArgs.Empty);
            }

            public event EventHandler StayRelevant;
            private void OnStayRelevant() { if (StayRelevant != null) StayRelevant(this, EventArgs.Empty); }
            public event EventHandler CeaseRelevant;
            private void OnCeaseRelevant() { if (CeaseRelevant != null) CeaseRelevant(this, EventArgs.Empty); }

            internal AnimatorStateGroup(string name, string layerName, int layerIndex)
            {
                this._NameMeshs = new List<int>();
                this.LayerIndex = layerIndex;
                this.Name = name;
                this._LayerName = layerName;
            }

            internal void Update()
            {
                if (_SideEvents == 2)
                    OnCeaseRelevant();
                else if (_SideEvents == 1)
                    OnBecameRelevant();
                _SideEvents = 0;

                if (IsRelevant)
                    OnStayRelevant();
            }

            internal void FixedUpdate(ref AnimatorStateInfo currentState, ref AnimatorStateInfo nextState, bool isInTransition)
            {
                if (!_Sorted)
                {
                    _Sorted = true;
                    _NameMeshs.Sort();
                }
                bool preRelevant = this.IsRelevant;

                if (isInTransition)
                    this.IsRelevant = _NameMeshs.BinarySearch(nextState.fullPathHash) >= 0;
                else
                    this.IsRelevant = _NameMeshs.BinarySearch(currentState.fullPathHash) >= 0;


                if (preRelevant && !this.IsRelevant)
                    _SideEvents = 2;
                else if (!preRelevant && this.IsRelevant)
                    _SideEvents = 1;
            }

            public void AddState(string stateName)
            {
                if (string.IsNullOrEmpty(stateName))
                    throw new ArgumentException("Invalid state name");
                AddNewState(string.Format("{0}.{1}", _LayerName, stateName));
            }

            public void AddState(string stateName, string subStateMachineName)
            {
                if (string.IsNullOrEmpty(stateName))
                    throw new ArgumentException("Invalid state name");
                if (string.IsNullOrEmpty(subStateMachineName))
                    throw new ArgumentException("Invalid Sub-State Machine name");

                AddNewState(string.Format("{0}.{1}.{2}", _LayerName, subStateMachineName, stateName));// unity 5 needs layername too
                _Sorted = false;
            }

            private void AddNewState(string fullName)
            {
                int nameMesh = Animator.StringToHash(fullName);
                if (_NameMeshs.Contains(nameMesh))
                    throw new ArgumentException("State name already exist");

                _NameMeshs.Add(nameMesh);
                _Sorted = false;
            }

            public bool RemoveState(string stateName)
            {
                if (string.IsNullOrEmpty(stateName))
                    throw new ArgumentException("Invalid state name");

                int nameMesh = Animator.StringToHash(string.Format("{0}.{1}", _LayerName, stateName));
                return _NameMeshs.Remove(nameMesh);
            }

            public bool RemoveState(string stateName, string subStateMachineName)
            {
                if (string.IsNullOrEmpty(stateName))
                    throw new ArgumentException("Invalid state name");
                if (string.IsNullOrEmpty(subStateMachineName))
                    throw new ArgumentException("Invalid Sub-State Machine name");

                int nameMesh = Animator.StringToHash(string.Format("{0}.{1}.{2}", _LayerName, subStateMachineName, stateName));
                return _NameMeshs.Remove(nameMesh);
            }
        }
        #endregion

        #region Parameters

        public abstract class AnimatorParameter
        {
            public string Name { get; private set; }
            public int Id { get; private set; }
            public AnimatorManager Owner { get; private set; }

            internal AnimatorParameter(string name, AnimatorManager owner)
            {
                this.Name = name;
                this.Owner = owner;
                this.Id = Animator.StringToHash(this.Name);
            }

            public abstract UnityEngine.AnimatorControllerParameterType ParameterType { get; }
        }

        public class BooleanParameter : AnimatorParameter
        {
            private bool _Value;
            public bool Value
            {
                get { return _Value; }
                set
                {
                    if (!Owner.IsDestroyed && _Value != value)
                    {
                        _Value = value;
                        Owner.Animator.SetBool(Id, _Value);
                        OnChanged();
                    }
                }
            }

            public event EventHandler Changed;
            protected virtual void OnChanged() { if (Changed != null) Changed(this, EventArgs.Empty); }

            internal BooleanParameter(string name, AnimatorManager owner) : base(name, owner) { }

            public override UnityEngine.AnimatorControllerParameterType ParameterType { get { return UnityEngine.AnimatorControllerParameterType.Bool; } }

            public static implicit operator bool(BooleanParameter b)
            {
                return b.Value;
            }
        }

        public class IntegerParameter : AnimatorParameter
        {
            private int _Value;
            public int Value
            {
                get { return _Value; }
                set
                {
                    if (!Owner.IsDestroyed && _Value != value)
                    {
                        _Value = value;
                        Owner.Animator.SetInteger(Id, _Value);
                        OnChanged();
                    }
                }
            }
            public event EventHandler Changed;
            protected virtual void OnChanged() { if (Changed != null) Changed(this, EventArgs.Empty); }
            internal IntegerParameter(string name, AnimatorManager owner) : base(name, owner) { }
            public override UnityEngine.AnimatorControllerParameterType ParameterType { get { return UnityEngine.AnimatorControllerParameterType.Int; } }

            public static implicit operator int(IntegerParameter i) { return i.Value; }
        }

        public class FloatParameter : AnimatorParameter
        {
            private float _Value;
            public float Value
            {
                get
                {
                    if (!Owner.IsDestroyed && IsControlledByCurve)
                        _Value = Owner.Animator.GetFloat(Id);
                    return _Value;
                }
                set
                {
                    SetValue(value);
                }
            }

            public bool IsControlledByCurve { get; private set; }

            public event EventHandler Changed;

            protected virtual void OnChanged() { if (Changed != null) Changed(this, EventArgs.Empty); }

            internal FloatParameter(string name, AnimatorManager owner)
                : base(name, owner)
            {
                this.IsControlledByCurve = Owner.Animator.IsParameterControlledByCurve(Id);
            }

            public override UnityEngine.AnimatorControllerParameterType ParameterType { get { return UnityEngine.AnimatorControllerParameterType.Float; } }

            public void UpdateValueFromAnimator(Animator animator)
            {
                float value = animator.GetFloat(Id);
                if (_Value != value)
                {
                    _Value = value;
                    OnChanged();
                }
            }

            public static implicit operator float(FloatParameter f)
            {
                return f.Value;
            }

            public void SetValue(float value)
            {
                if (!Owner.IsDestroyed)
                {
                    if (_Value != value)
                    {
                        _Value = value;
                        OnChanged();
                    }
                    Owner.Animator.SetFloat(Id, _Value);
                }
            }

            public void SetValue(float value, float dampTime, float deltaTime)
            {
                if (!Owner.IsDestroyed)
                {
                    if (_Value != value)
                    {
                        _Value = value;
                        OnChanged();
                    }
                    Owner.Animator.SetFloat(Id, _Value, dampTime, deltaTime);
                }
            }
        }

        public class TriggerParameter : AnimatorParameter
        {
            internal TriggerParameter(string name, AnimatorManager owner) : base(name, owner) { }

            public override UnityEngine.AnimatorControllerParameterType ParameterType { get { return UnityEngine.AnimatorControllerParameterType.Trigger; } }

            public void Set()
            {
                if (!Owner.IsDestroyed)
                    Owner.Animator.SetTrigger(Id);
            }
            public void Reset()
            {
                if (!Owner.IsDestroyed)
                    Owner.Animator.ResetTrigger(Id);
            }
        }
        #endregion
    }




}
