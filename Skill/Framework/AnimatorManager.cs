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
    public class AnimatorManager
    {

        private string[] _Layers;
        private Dictionary<string, AnimatorParameter> _Parameters;
        private List<AnimatorStateGroup> _UpdateGroups;
        private bool[] _IsInTransitions;


        public Animator Animator { get; private set; }
        public AnimatorStateInfo[] LayerStates { get; private set; }

        public string GetActiveGroupNames(int layerIndex)
        {
            string result = string.Empty;
            for (int i = 0; i < _UpdateGroups.Count; i++)
            {
                var group = _UpdateGroups[i];
                if (group.LayerIndex == layerIndex && group.IsActive)
                {
                    if (result != string.Empty) result += ", ";
                    result += group.Name;
                }
            }
            return result;
        }

        public AnimatorManager(Animator animator)
        {
            if (animator == null)
                throw new ArgumentNullException("Animator is null");

            this.Animator = animator;
            this.LayerStates = new AnimatorStateInfo[animator.layerCount];
            _UpdateGroups = new List<AnimatorStateGroup>();
            this._IsInTransitions = new bool[animator.layerCount];
            this._Layers = new string[Animator.layerCount];
            for (int i = 0; i < Animator.layerCount; i++)
            {
                this._Layers[i] = Animator.GetLayerName(i);
            }
            this._Parameters = new Dictionary<string, AnimatorParameter>();
        }

        public AnimatorParameter GetParameters(string parameterName)
        {
            AnimatorParameter result = null;
            _Parameters.TryGetValue(parameterName, out result);
            return result;
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < Animator.layerCount; i++)
            {
                LayerStates[i] = Animator.GetCurrentAnimatorStateInfo(i);
                _IsInTransitions[i] = Animator.IsInTransition(i);
            }

            foreach (var aug in _UpdateGroups)
            {
                aug.FixedUpdate(ref LayerStates[aug.LayerIndex], _IsInTransitions[aug.LayerIndex]);
            }
        }

        public void Update()
        {
            for (int i = 0; i < _UpdateGroups.Count; i++)
            {
                _UpdateGroups[i].Update();
            }
        }


        public AnimatorStateGroup CreateStateGroup(string name, int layerIndex)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid group name");
            if (layerIndex < 0 || layerIndex >= Animator.layerCount)
                throw new IndexOutOfRangeException("Invalid layerIndex");

            for (int i = 0; i < _UpdateGroups.Count; i++)
            {
                if (name == _UpdateGroups[i].Name)
                    throw new ArgumentException("Group name already exist");
            }

            AnimatorStateGroup group = new AnimatorStateGroup(name, Animator.GetLayerName(layerIndex), layerIndex);
            _UpdateGroups.Add(group);
            return group;
        }

        public BooleanParameter CreateBooleanParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            BooleanParameter parameter = new BooleanParameter(parameterName, Animator);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public IntegerParameter CreateIntegerParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            IntegerParameter parameter = new IntegerParameter(parameterName, Animator);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public FloatParameter CreateFloatParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            FloatParameter parameter = new FloatParameter(parameterName, Animator);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

        public TriggerParameter CreateTriggerParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Invalid parameter name");
            if (_Parameters.ContainsKey(parameterName))
                throw new ArgumentException("parameter name already exist");

            TriggerParameter parameter = new TriggerParameter(parameterName, Animator);
            _Parameters.Add(parameterName, parameter);
            return parameter;
        }

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

        public int LayerIndex { get; private set; }
        public string Name { get; private set; }
        public bool IsRelevant { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsRelevantNoTransition { get; private set; }
        /// <summary> default is false </summary>
        public bool OnlyUpdateInRelevantNoTransition { get; set; }

        public event EventHandler BecameRelevant;
        private void OnBecameRelevant()
        {
            if (BecameRelevant != null) BecameRelevant(this, EventArgs.Empty);
        }

        public event EventHandler BecameRelevantNoTransition;
        private void OnBecameRelevantNoTransition()
        {
            if (BecameRelevantNoTransition != null) BecameRelevantNoTransition(this, EventArgs.Empty);
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
            this.OnlyUpdateInRelevantNoTransition = false;
        }

        internal void Update()
        {
            if (IsActive)
                OnStayRelevant();
        }

        internal void FixedUpdate(ref AnimatorStateInfo info, bool isInTransition)
        {
            if (!_Sorted)
            {
                _Sorted = true;
                _NameMeshs.Sort();
            }
            bool preRelevant = this.IsRelevant;
            bool preRelevantNoTransition = this.IsRelevantNoTransition;

            if (this.IsRelevant || (!IsRelevant && !isInTransition))
                this.IsRelevant = _NameMeshs.BinarySearch(info.nameHash) >= 0;
            else
                this.IsRelevant = false;

            IsRelevantNoTransition = IsRelevant && !isInTransition;

            if (preRelevant && !this.IsRelevant) OnCeaseRelevant();
            else if (!preRelevant && this.IsRelevant) OnBecameRelevant();
            if (!preRelevantNoTransition && this.IsRelevantNoTransition) OnBecameRelevantNoTransition();

            if (OnlyUpdateInRelevantNoTransition)
            {
                IsActive = IsRelevantNoTransition;
            }
            else
            {
                IsActive = IsRelevant;
            }
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

            AddNewState(string.Format("{0}.{1}", subStateMachineName, stateName));
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

            int nameMesh = Animator.StringToHash(string.Format("{0}.{1}", subStateMachineName, stateName));
            return _NameMeshs.Remove(nameMesh);
        }
    }
    #endregion

    #region Parameters

    public enum AnimatorParameterType
    {
        Boolean,
        Integer,
        Float,
        Trigger
    }

    public abstract class AnimatorParameter
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public Animator Animator { get; private set; }

        internal AnimatorParameter(string name, Animator animator)
        {
            this.Name = name;
            this.Animator = animator;
            this.Id = Animator.StringToHash(this.Name);
        }

        public abstract AnimatorParameterType ParameterType { get; }
    }

    public class BooleanParameter : AnimatorParameter
    {
        private bool _Value;
        public bool Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    Animator.SetBool(Id, _Value);
                    OnChanged();
                }
            }
        }

        public event EventHandler Changed;
        protected virtual void OnChanged() { if (Changed != null) Changed(this, EventArgs.Empty); }

        internal BooleanParameter(string name, Animator animator) : base(name, animator) { }

        public override AnimatorParameterType ParameterType { get { return AnimatorParameterType.Boolean; } }

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
                if (_Value != value)
                {
                    _Value = value;
                    Animator.SetInteger(Id, _Value);
                    OnChanged();
                }
            }
        }
        public event EventHandler Changed;
        protected virtual void OnChanged() { if (Changed != null) Changed(this, EventArgs.Empty); }
        internal IntegerParameter(string name, Animator animator) : base(name, animator) { }
        public override AnimatorParameterType ParameterType { get { return AnimatorParameterType.Integer; } }

        public static implicit operator int(IntegerParameter i) { return i.Value; }
    }

    public class FloatParameter : AnimatorParameter
    {
        private float _Value;
        public float Value
        {
            get
            {
                if (IsControlledByCurve)
                    _Value = Animator.GetFloat(Id);
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

        internal FloatParameter(string name, Animator animator)
            : base(name, animator)
        {
            this.IsControlledByCurve = Animator.IsParameterControlledByCurve(Id);
        }

        public override AnimatorParameterType ParameterType { get { return AnimatorParameterType.Float; } }

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
            if (_Value != value)
            {
                _Value = value;
                OnChanged();
            }
            Animator.SetFloat(Id, _Value);
        }

        public void SetValue(float value, float dampTime, float deltaTime)
        {
            if (_Value != value)
            {
                _Value = value;
                OnChanged();
            }
            Animator.SetFloat(Id, _Value, dampTime, deltaTime);
        }
    }

    public class TriggerParameter : AnimatorParameter
    {
        internal TriggerParameter(string name, Animator animator) : base(name, animator) { }

        public override AnimatorParameterType ParameterType { get { return AnimatorParameterType.Trigger; } }

        public void Set() { Animator.SetTrigger(Id); }
        public void Reset() { Animator.ResetTrigger(Id); }
    }
    #endregion
}
