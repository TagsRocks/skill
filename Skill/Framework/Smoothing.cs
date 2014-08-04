using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Types of smoothing
    /// </summary>
    public enum SmoothType
    {
        /// <summary> Just using SmoothTime value of SmoothingParams</summary>
        Damp,
        /// <summary> using SmoothTime and MaxSpeed value of SmoothingParams</summary>
        DampSpeed,
        /// <summary> using SmoothTime, MaxSpeed and DeltaTimeFactor value of SmoothingParams</summary>
        DampSpeedAndTime
    }

    /// <summary>
    /// Parameters of Smoothing algorithm
    /// </summary>
    [System.Serializable]
    public class SmoothingParameters
    {
        /// <summary> Type of Smoothing </summary>
        public SmoothType SmoothType = SmoothType.Damp;
        /// <summary> Approximately the time it will take to reach the target. A smaller value will reach the target faster.</summary>
        public float SmoothTime = 0.3f;
        /// <summary> Optionally allows you to clamp the maximum speed. </summary>
        public float MaxSpeed = Mathf.Infinity;
        /// <summary> The factor of Time.deltaTime( The time since the last call to this function ). </summary>
        public float DeltaTimeFactor = 1;
    }

    /// <summary>
    /// Simplify using Mathf.SmoothDamp
    /// </summary>
    public struct Smoothing
    {

        private float _CurrentVelocity;
        private float _Current;

        /// <summary> Last result of call Update function </summary>
        public float Current { get { return _Current; } set { _Current = value; _CurrentVelocity = 0; } }

        /// <summary> The position we are trying to reach. </summary>
        public float Target { get; set; }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="sp">Parameters of Smoothing ( can be modified in inspector)</param>
        /// <returns>Smooth result</returns>
        public float Update(SmoothingParameters sp)
        {
            if (_Current != Target)
            {
                switch (sp.SmoothType)
                {
                    case SmoothType.Damp:
                        _Current = Mathf.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime);
                        break;
                    case SmoothType.DampSpeed:
                        _Current = Mathf.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime, sp.MaxSpeed);
                        break;
                    case SmoothType.DampSpeedAndTime:
                        _Current = Mathf.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime, sp.MaxSpeed, Time.deltaTime * sp.DeltaTimeFactor);
                        break;
                }
            }
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(float value)
        {
            Target = value;
            _Current = value;
            _CurrentVelocity = 0;
        }
    }

    /// <summary>
    /// Simplify using Mathf.SmoothDampAngle
    /// </summary>
    public struct SmoothingAngle
    {

        private float _CurrentAngleVelocity;
        private float _CurrentAngle;
        private float _TargetAngle;

        /// <summary> Last result of call Update function </summary>
        public float CurrentAngle { get { return _CurrentAngle; } set { _CurrentAngle = MathHelper.ClampAngle(value); _CurrentAngleVelocity = 0; } }

        /// <summary> The position we are trying to reach. </summary>
        public float TargetAngle { get { return _TargetAngle; } set { _TargetAngle = MathHelper.ClampAngle(value); } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="sp">Parameters of Smoothing ( can be modified in inspector)</param>
        /// <returns>Smooth result</returns>
        public float Update(SmoothingParameters sp)
        {
            if (_CurrentAngle != TargetAngle)
            {
                switch (sp.SmoothType)
                {
                    case SmoothType.Damp:
                        _CurrentAngle = Mathf.SmoothDampAngle(_CurrentAngle, TargetAngle, ref _CurrentAngleVelocity, sp.SmoothTime);
                        break;
                    case SmoothType.DampSpeed:
                        _CurrentAngle = Mathf.SmoothDampAngle(_CurrentAngle, TargetAngle, ref _CurrentAngleVelocity, sp.SmoothTime, sp.MaxSpeed);
                        break;
                    case SmoothType.DampSpeedAndTime:
                        _CurrentAngle = Mathf.SmoothDampAngle(_CurrentAngle, TargetAngle, ref _CurrentAngleVelocity, sp.SmoothTime, sp.MaxSpeed, Time.deltaTime * sp.DeltaTimeFactor);
                        break;
                }
            }
            return _CurrentAngle;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(float value)
        {
            TargetAngle = value;
            _CurrentAngle = value;
            _CurrentAngleVelocity = 0;
        }
    }

    /// <summary>
    /// Simplify using Mathf.SmoothDamp for two value
    /// </summary>
    public struct Smoothing2D
    {

        private Vector2 _CurrentVelocity;
        private Vector2 _Current;

        /// <summary> Last result of call Update function </summary>
        public Vector2 Current { get { return _Current; } set { _Current = value; _CurrentVelocity = Vector2.zero; } }

        /// <summary> The position we are trying to reach. </summary>
        public Vector2 Target { get; set; }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="sp">Parameters of Smoothing ( can be modified in inspector)</param>
        /// <returns>Smooth result</returns>
        public Vector2 Update(SmoothingParameters sp)
        {
            if (_Current != Target)
            {
                switch (sp.SmoothType)
                {
                    case SmoothType.Damp:
                        _Current.x = Mathf.SmoothDamp(_Current.x, Target.x, ref _CurrentVelocity.x, sp.SmoothTime);
                        _Current.y = Mathf.SmoothDamp(_Current.y, Target.y, ref _CurrentVelocity.y, sp.SmoothTime);
                        break;
                    case SmoothType.DampSpeed:
                        _Current.x = Mathf.SmoothDamp(_Current.x, Target.x, ref _CurrentVelocity.x, sp.SmoothTime, sp.MaxSpeed);
                        _Current.y = Mathf.SmoothDamp(_Current.y, Target.y, ref _CurrentVelocity.y, sp.SmoothTime, sp.MaxSpeed);
                        break;
                    case SmoothType.DampSpeedAndTime:
                        _Current.x = Mathf.SmoothDamp(_Current.x, Target.x, ref _CurrentVelocity.x, sp.SmoothTime, sp.MaxSpeed, Time.deltaTime * sp.DeltaTimeFactor);
                        _Current.y = Mathf.SmoothDamp(_Current.y, Target.y, ref _CurrentVelocity.y, sp.SmoothTime, sp.MaxSpeed, Time.deltaTime * sp.DeltaTimeFactor);
                        break;
                }
            }
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Vector2 value)
        {
            Target = value;
            _Current = value;
            _CurrentVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Simplify using Mathf.SmoothDamp for two value
    /// </summary>
    public struct Smoothing3D
    {
        private Vector3 _CurrentVelocity;
        private Vector3 _Current;

        /// <summary> Last result of call Update function </summary>
        public Vector3 Current { get { return _Current; } set { _Current = value; _CurrentVelocity = Vector3.zero; } }

        /// <summary> The position we are trying to reach. </summary>
        public Vector3 Target { get; set; }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="sp">Parameters of Smoothing ( can be modified in inspector)</param>
        /// <returns>Smooth result</returns>
        public Vector3 Update(SmoothingParameters sp)
        {
            if (_Current != Target)
            {
                switch (sp.SmoothType)
                {
                    case SmoothType.Damp:
                        _Current = Vector3.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime);
                        break;
                    case SmoothType.DampSpeed:
                        _Current = Vector3.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime, sp.MaxSpeed);
                        break;
                    case SmoothType.DampSpeedAndTime:
                        _Current = Vector3.SmoothDamp(_Current, Target, ref _CurrentVelocity, sp.SmoothTime, sp.MaxSpeed, Time.deltaTime * sp.DeltaTimeFactor);
                        break;
                }
            }
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Vector3 value)
        {
            Target = value;
            _Current = value;
            _CurrentVelocity = Vector3.zero;
        }
    }

}
