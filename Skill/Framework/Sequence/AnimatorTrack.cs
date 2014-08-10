using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{

    public class AnimatorTrack : EventOrientedTrack
    {
        [System.Serializable]
        public class Parameter
        {
            public string Name;
            public AnimatorParameterType ParameterType;
            public bool DefaultBool;
            public int DefaultInt;
            public float DefaultFloat;

        }

        [HideInInspector]
        [SerializeField]
        public Animator _Animator;

        [HideInInspector]
        [SerializeField]
        public string _StartStateName = "Idle";

        [HideInInspector]
        [SerializeField]
        public bool _Preview = true;

        [HideInInspector]
        [SerializeField]
        public AnimatorTrack.Parameter[] _Parameters;


        [ExposeProperty(0, "Animator")]
        public Animator Animator { get { return _Animator; } set { _Animator = value; } }

        [ExposeProperty(1, "Start State", "Name of state to start with")]
        public string StartStateName { get { return _StartStateName; } set { _StartStateName = value; } }

        [ExposeProperty(2, "Preview", "Preview animations in editor")]
        public bool Preview { get { return _Preview; } set { _Preview = value; } }

        public override TrackType Type { get { return TrackType.Animator; } }

        public override void Evaluate(float time)
        {
            base.Evaluate(time);
            if (_Animator != null && (!Application.isPlaying && Preview))
            {
                if (DeltaTime != 0)
                    _Animator.Update(DeltaTime);
            }
        }

        public override void Seek(float time)
        {
            base.Seek(time);
            if (_Animator != null && (Application.isPlaying || Preview))
            {
                ResetParameters();
                float updateTime = 0;

                if (Keys != null)
                {
                    if (Index >= 0)
                    {
                        for (int i = 0; i <= Index; i++)
                        {
                            AnimatorKey key = (AnimatorKey)Keys[i];
                            float preTime = updateTime;
                            updateTime = key.FireTime;
                            float deltaTime = updateTime - preTime;
                            if (deltaTime > 0) _Animator.Update(deltaTime);
                            InitializeEvent(key);
                            if (!key.IsSingleExecution && (updateTime + key.Length >= time))
                            {
                                key.StartEvent();
                                key.UpdateEvent(time);
                            }
                            else
                            {
                                key.FireEvent();
                            }
                        }

                    }
                }
                if (time > updateTime)
                {
                    float deltaTime = (time - updateTime) * 0.5f;
                    _Animator.Update(deltaTime);
                    _Animator.Update(deltaTime);

                    // i don't know why is should update twice to make animator update correctly
                }
            }
        }

        private void ResetParameters()
        {
            if (_Animator != null)
            {
                if (_Parameters != null)
                {
                    foreach (var p in _Parameters)
                    {
                        switch (p.ParameterType)
                        {
                            case AnimatorParameterType.Float:
                                _Animator.SetFloat(p.Name, p.DefaultFloat);
                                break;
                            case AnimatorParameterType.Int:
                                _Animator.SetInteger(p.Name, p.DefaultInt);
                                break;
                            case AnimatorParameterType.Bool:
                                _Animator.SetBool(p.Name, p.DefaultBool);
                                break;
                            case AnimatorParameterType.Trigger:
                                _Animator.ResetTrigger(p.Name);
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(_StartStateName))
                    _Animator.CrossFade(_StartStateName, 0, 0, 0);
            }
        }

        protected override void InitializeEvent(EventOrientedKey key)
        {
            AnimatorKey aKey = (AnimatorKey)key;
            aKey.Animator = this._Animator;
        }
    }


    #region AnimatorKeys
    [System.Serializable]
    public abstract class AnimatorKey : EventOrientedKey
    {
        public override bool IsSingleExecution { get { return true; } }
        internal Animator Animator { get; set; }
    }

    [System.Serializable]
    public abstract class ParameterAnimatorKey : AnimatorKey
    {
        public string _ParameterName;

        [ExposeProperty(1, "Parameter Name", "Name of parameter")]
        public string ParameterName { get { return this._ParameterName; } set { this._ParameterName = value; } }
    }

    [System.Serializable]
    public class BooleanAnimatorKey : ParameterAnimatorKey
    {
        public bool _Value;

        [ExposeProperty(3, "Value")]
        public bool BoolValue { get { return this._Value; } set { this._Value = value; } }

        public override void FireEvent()
        {
            if (Animator != null)
                Animator.SetBool(_ParameterName, this._Value);
        }
    }

    [System.Serializable]
    public class IntengerAnimatorKey : ParameterAnimatorKey
    {
        public int _Value;

        [ExposeProperty(3, "Value")]
        public int IntValue { get { return this._Value; } set { this._Value = value; } }
        public override void FireEvent()
        {
            if (Animator != null)
                Animator.SetInteger(_ParameterName, this._Value);
        }
    }

    [System.Serializable]
    public class FloatAnimatorKey : ParameterAnimatorKey
    {
        public float _Value;
        public float _Duration;

        [ExposeProperty(3, "Value", "The new value for the parameter.")]
        public float FloatValue { get { return this._Value; } set { this._Value = value; } }

        [ExposeProperty(4, "Duration", "The time allowed to parameter to reach the value.")]
        public float Duration { get { return this._Duration; } set { this._Duration = Mathf.Max(0.001f, value); } }
        public override float Length { get { return _Duration; } }
        public override bool IsSingleExecution { get { return false; } }

        public override void FireEvent()
        {
            FinishEvent();
        }

        private float _StartValue;

        public override void StartEvent()
        {
            if (Animator != null)
                _StartValue = Animator.GetFloat(ParameterName);
        }

        public override void UpdateEvent(float time)
        {
            if (Animator != null)
            {
                float value = Mathf.Lerp(_StartValue, _Value, (time - FireTime) / _Duration);
                Animator.SetFloat(_ParameterName, value);
            }
        }

        public override void FinishEvent()
        {
            if (Animator != null)
                Animator.SetFloat(_ParameterName, this._Value);
        }
    }

    [System.Serializable]
    public class TriggerAnimatorKey : ParameterAnimatorKey
    {
        public override void FireEvent()
        {
            if (Animator != null)
                Animator.SetTrigger(_ParameterName);
        }
    }

    [System.Serializable]
    public abstract class IkPositionAnimatorKey : AnimatorKey
    {
        public AvatarIKGoal _Goal;
        public Vector3 _GoalPosition;
        public float _Weight;

        [ExposeProperty(1, "Goal", "The AvatarIKGoal that is set.")]
        public AvatarIKGoal Goal { get { return this._Goal; } set { this._Goal = value; } }

        [ExposeProperty(2, "Goal Position", "The position in world space.")]
        public Vector3 GoalPosition { get { return this._GoalPosition; } set { this._GoalPosition = value; } }

        [ExposeProperty(3, "Weight", "The translative weight")]
        public float Weight { get { return this._Weight; } set { this._Weight = value; } }

        public override void FireEvent()
        {
            if (Animator != null)
            {
                Animator.SetIKPosition(_Goal, _GoalPosition);
                Animator.SetIKPositionWeight(_Goal, _Weight);
            }
        }
    }

    [System.Serializable]
    public abstract class IkRotationAnimatorKey : AnimatorKey
    {
        public AvatarIKGoal _Goal;
        public Vector3 _GoalRotation;
        public float _Weight;

        [ExposeProperty(1, "Goal", "The AvatarIKGoal that is set.")]
        public AvatarIKGoal Goal { get { return this._Goal; } set { this._Goal = value; } }

        [ExposeProperty(2, "Goal Rotation", "The rotation in world space.")]
        public Vector3 GoalRotation { get { return this._GoalRotation; } set { this._GoalRotation = value; } }

        [ExposeProperty(3, "Weight", "The translative weight")]
        public float Weight { get { return this._Weight; } set { this._Weight = value; } }

        public override void FireEvent()
        {
            if (Animator != null)
            {
                Animator.SetIKRotation(_Goal, Quaternion.Euler(_GoalRotation));
                Animator.SetIKRotationWeight(_Goal, _Weight);
            }
        }
    }

    [System.Serializable]
    public abstract class LookAtAnimatorKey : AnimatorKey
    {
        public Vector3 _LookAtPosition;
        public float _Weight;
        public float _BodyWeight;
        public float _HeadWeight;
        public float _EyesWeight;
        public float _ClampWeight;

        [ExposeProperty(1, "Position", "The position to lookAt.")]
        public Vector3 LookAtPosition { get { return this._LookAtPosition; } set { this._LookAtPosition = value; } }

        [ExposeProperty(2, "Weight", "(0-1) the global weight of the LookAt, multiplier for other parameters.")]
        public float Weight { get { return this._Weight; } set { this._Weight = value; } }

        [ExposeProperty(3, "Body Weight", "(0-1) determines how much the body is involved in the LookAt.")]
        public float BodyWeight { get { return this._BodyWeight; } set { this._BodyWeight = value; } }

        [ExposeProperty(4, "Head Weight", "(0-1) determines how much the head is involved in the LookAt.")]
        public float HeadWeight { get { return this._HeadWeight; } set { this._HeadWeight = value; } }

        [ExposeProperty(5, "Eyes Weight", "(0-1) determines how much the eyes are involved in the LookAt")]
        public float EyesWeight { get { return this._EyesWeight; } set { this._EyesWeight = value; } }

        [ExposeProperty(6, "Clamp Weight", "(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
        public float ClampWeight { get { return this._ClampWeight; } set { this._ClampWeight = value; } }

        public override void FireEvent()
        {
            if (Animator != null)
            {
                Animator.SetLookAtPosition(_LookAtPosition);
                Animator.SetLookAtWeight(_Weight, _BodyWeight, _HeadWeight, _EyesWeight, _ClampWeight);
            }
        }
    }

    [System.Serializable]
    public abstract class LayerWeightAnimatorKey : AnimatorKey
    {
        public int _LayerIndex;
        public float _Weight;
        public float _Duration;

        [ExposeProperty(1, "Layer Index", "The layer's index.")]
        public int LayerIndex { get { return this._LayerIndex; } set { this._LayerIndex = value; } }

        [ExposeProperty(2, "Weight", "The weight of the layer.")]
        public float Weight { get { return this._Weight; } set { this._Weight = value; } }

        [ExposeProperty(3, "Duration", "Duration of change layer weight")]
        public float Duration { get { return this._Duration; } set { this._Duration = Mathf.Max(0.001f, value); } }
        public override float Length { get { return _Duration; } }
        public override bool IsSingleExecution { get { return false; } }

        private float _StartWeight;

        public override void StartEvent()
        {
            if (Animator != null)
                _StartWeight = Animator.GetLayerWeight(_LayerIndex);
            else
                _StartWeight = _Weight > 0.5f ? 0 : 1.0f;
        }

        public override void UpdateEvent(float time)
        {

            if (Animator != null)
            {
                float w = Mathf.Lerp(_StartWeight, _Weight, (time - FireTime) / _Duration);
                Animator.SetLayerWeight(_LayerIndex, w);
            }

        }
        public override void FireEvent() { FinishEvent(); }

        public override void FinishEvent()
        {
            if (Animator != null)
                Animator.SetLayerWeight(_LayerIndex, _Weight);
        }
    }

    [System.Serializable]
    public class CrossFadeAnimatorKey : AnimatorKey
    {
        public string _StateName = "Idle";
        public float _TransitionDuration = 0.3f;
        public float _NormalizedTime = 0;
        public int _Layer = 0;

        [ExposeProperty(2, "State Name", "The name of the destination state.")]
        public string StateName { get { return this._StateName; } set { this._StateName = value; } }

        [ExposeProperty(3, "Transition Duration", "The duration of the transition. Value is in source state normalized time.")]
        public float TransitionDuration { get { return this._TransitionDuration; } set { this._TransitionDuration = value; } }

        [ExposeProperty(4, "Normalized Time", "Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
        public float NormalizedTime { get { return this._NormalizedTime; } set { this._NormalizedTime = value; } }

        [ExposeProperty(5, "Layer", "Layer index containing the destination state.")]
        public int Layer { get { return this._Layer; } set { this._Layer = value; } }

        public override void FireEvent()
        {
            if (Animator != null)
                Animator.CrossFade(_StateName, _TransitionDuration, _Layer, _NormalizedTime);
        }
    }
    #endregion

}
