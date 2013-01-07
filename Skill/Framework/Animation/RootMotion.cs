using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Specify how to update RootMotion
    /// </summary>
    public class RootMotionState
    {
        private bool _PositionX;
        /// <summary> PositionX is enable or not? </summary>
        public bool PositionX
        {
            get { return _PositionX; }
            set
            {
                if (_PositionX != value)
                {
                    _PositionX = value;
                    CheckEnable();
                }
            }
        }

        private bool _PositionY;
        /// <summary> PositionY is enable or not? </summary>
        public bool PositionY
        {
            get { return _PositionY; }
            set
            {
                if (_PositionY != value)
                {
                    _PositionY = value;
                    CheckEnable();
                }
            }
        }

        private bool _PositionZ;
        /// <summary> PositionZ is enable or not? </summary>
        public bool PositionZ
        {
            get { return _PositionZ; }
            set
            {
                if (_PositionZ != value)
                {
                    _PositionZ = value;
                    CheckEnable();
                }
            }
        }


        private void CheckEnable()
        {
            IsEnable = _PositionX | _PositionY | _PositionZ;
        }

        /// <summary>
        /// Whether at least one of parameters is enable
        /// </summary>
        public bool IsEnable { get; private set; }
    }


    /// <summary>
    /// RootMotion of AnimationSequence
    /// </summary>
    public class RootMotion
    {
        private AnimNodeSequence _Sequence;
        private AnimationCurve _XCurve;
        private AnimationCurve _YCurve;
        private AnimationCurve _ZCurve;

        private Vector3 _Motion;
        private Vector3 _Position;
        private Vector3 _PrePosition;

        private float _Time;
        private float _PreTime;

        private Vector3 _FirstPosition;
        private Vector3 _LastPosition;

        /// <summary> Motion in current update </summary>
        public Vector3 Motion { get { return _Motion; } }

        /// <summary> Whether RootMotion is enable or not? </summary>
        public RootMotionState State { get; private set; }

        /// <summary>
        /// Set Keyframes
        /// </summary>
        /// <param name="keys">Position Keyframes</param>
        public void SetKeyframes(Vector3Keyframes keys)
        {
            if (keys.XKeys != null && keys.XKeys.Length > 0)
            {
                _XCurve = new AnimationCurve(keys.XKeys);
                _FirstPosition.x = keys.XKeys[0].value;
                _LastPosition.x = keys.XKeys[keys.XKeys.Length - 1].value;
            }
            else
                _XCurve = null;

            if (keys.YKeys != null && keys.YKeys.Length > 0)
            {
                _YCurve = new AnimationCurve(keys.YKeys);
                _FirstPosition.y = keys.YKeys[0].value;
                _LastPosition.y = keys.YKeys[keys.YKeys.Length - 1].value;
            }
            else
                _YCurve = null;

            if (keys.ZKeys != null && keys.ZKeys.Length > 0)
            {
                _ZCurve = new AnimationCurve(keys.ZKeys);
                _FirstPosition.z = keys.ZKeys[0].value;
                _LastPosition.z = keys.ZKeys[keys.ZKeys.Length - 1].value;
            }
            else
                _ZCurve = null;
        }


        /// <summary>
        /// Create a RootMotion
        /// </summary>
        /// <param name="sequence">Owner Sequence</param>
        internal RootMotion(AnimNodeSequence sequence)
        {
            this._Sequence = sequence;
            this.State = new RootMotionState();
        }

        /// <summary>
        /// Begin calculatin RootMotion (when Sequence became relevant)
        /// </summary>
        internal void Begin()
        {
            _Time = _PreTime = 0;
            _PrePosition = _Position = _FirstPosition;
        }

        /// <summary>
        /// End calculatin RootMotion (when Sequence Cease relevant)
        /// </summary>
        internal void End()
        {
            _Time = _PreTime = _Sequence.Length;
            _PrePosition = _Position = _LastPosition;
        }

        /// <summary>
        /// Evaluate curves and calculate motion between two updates
        /// </summary>
        internal void Evaluate()
        {
            _Motion = Vector3.zero;
            float deltaTime = _Sequence.Speed * Time.deltaTime;

            while (Mathf.Abs(deltaTime) > 0.0001f)
            {
                float t = _Time + deltaTime;
                if (t > _Sequence.Length)
                    _Time = _Sequence.Length;
                else if (t < 0)
                    _Time = 0;
                else
                    _Time = t;

                if (_XCurve != null && State.PositionX)
                    _Position.x = _XCurve.Evaluate(_Time);
                if (_YCurve != null && State.PositionY)
                    _Position.y = _YCurve.Evaluate(_Time);
                if (_ZCurve != null && State.PositionZ)
                    _Position.z = _ZCurve.Evaluate(_Time);

                _Motion += _Position - _PrePosition;
                deltaTime -= _Time - _PreTime;
                _PreTime = _Time;
                _PrePosition = _Position;
                if (t > _Sequence.Length)
                    Begin();
                else if (t < 0)
                    End();
            }

        }
    }
}
