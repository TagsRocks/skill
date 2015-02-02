using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Simplify using Mathf.Lerp
    /// </summary>
    public struct LerpSmoothing
    {

        private float _Current;
        private float _Target;

        /// <summary> Last result of call Update function </summary>
        public float Current { get { return _Current; } set { _Current = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public float Target { get { return _Target; } set { _Target = value; } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public float Update(float smoothing)
        {
            if (_Current != Target)
                _Current = Mathf.Lerp(_Current, _Target, smoothing * Time.deltaTime);
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(float value)
        {
            _Target = value;
            _Current = value;
        }
    }

    /// <summary>
    /// Simplify using Vector2.Lerp
    /// </summary>
    public struct LerpSmoothing2D
    {

        private Vector2 _Current;
        private Vector2 _Target;

        /// <summary> Last result of call Update function </summary>
        public Vector2 Current { get { return _Current; } set { _Current = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public Vector2 Target { get { return _Target; } set { _Target = value; } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public Vector2 Update(float smoothing)
        {
            if (_Current != Target)
                _Current = Vector2.Lerp(_Current, _Target, smoothing * Time.deltaTime);
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Vector2 value)
        {
            _Target = value;
            _Current = value;
        }
    }

    /// <summary>
    /// Simplify using Vector3.Lerp
    /// </summary>
    public struct LerpSmoothing3D
    {

        private Vector3 _Current;
        private Vector3 _Target;

        /// <summary> Last result of call Update function </summary>
        public Vector3 Current { get { return _Current; } set { _Current = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public Vector3 Target { get { return _Target; } set { _Target = value; } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public Vector3 Update(float smoothing)
        {
            if (_Current != Target)
                _Current = Vector3.Lerp(_Current, _Target, smoothing * Time.deltaTime);
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Vector3 value)
        {
            _Target = value;
            _Current = value;
        }
    }


    // <summary>
    /// Simplify using Vector4.Lerp
    /// </summary>
    public struct LerpSmoothing4D
    {

        private Vector4 _Current;
        private Vector4 _Target;

        /// <summary> Last result of call Update function </summary>
        public Vector4 Current { get { return _Current; } set { _Current = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public Vector4 Target { get { return _Target; } set { _Target = value; } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public Vector4 Update(float smoothing)
        {
            if (_Current != Target)
                _Current = Vector4.Lerp(_Current, _Target, smoothing * Time.deltaTime);
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Vector4 value)
        {
            _Target = value;
            _Current = value;
        }
    }


    /// <summary>
    /// Simplify using Color.Lerp
    /// </summary>
    public struct LerpSmoothingColor
    {

        private Color _Current;
        private Color _Target;

        /// <summary> Last result of call Update function </summary>
        public Color Current { get { return _Current; } set { _Current = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public Color Target { get { return _Target; } set { _Target = value; } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public Color Update(float smoothing)
        {
            if (_Current != Target)
                _Current = Color.Lerp(_Current, _Target, smoothing * Time.deltaTime);
            return _Current;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(Color value)
        {
            _Target = value;
            _Current = value;
        }
    }


    /// <summary>
    /// Simplify using Mathf.Lerp
    /// </summary>
    public struct LerpSmoothingAngle
    {

        private float _CurrentAngle;
        private float _TargetAngle;

        /// <summary> Last result of call Update function </summary>
        public float CurrentAngle { get { return _CurrentAngle; } set { _CurrentAngle = value; } }

        /// <summary> The value we are trying to reach. </summary>
        public float TargetAngle { get { return _TargetAngle; } set { _TargetAngle = MathHelper.ClampAngle(value); } }

        /// <summary>
        /// Update one step and return result
        /// </summary>
        /// <param name="smoothing">Smoothing</param>
        /// <returns>Smooth result</returns>
        public float Update(float smoothing)
        {
            if (_CurrentAngle != _TargetAngle)
                _CurrentAngle = MathHelper.ClampAngle(Mathf.LerpAngle(_CurrentAngle, _TargetAngle, smoothing * Time.deltaTime));
            return _CurrentAngle;
        }

        /// <summary>
        /// Reset current and target
        /// </summary>
        /// <param name="value">Value</param>
        public void Reset(float value)
        {
            _TargetAngle = MathHelper.ClampAngle(value);
            _CurrentAngle = value;
        }
    }

}
