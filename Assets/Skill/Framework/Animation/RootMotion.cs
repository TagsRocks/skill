using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// RootMotion of AnimationSequence
    /// </summary>
    public class RootMotion
    {
        private AnimNodeSequence _Sequence;
        private AnimationCurve _XCurve;
        private AnimationCurve _YCurve;
        private AnimationCurve _ZCurve;
        private AnimationCurve _RXCurve;
        private AnimationCurve _RYCurve;
        private AnimationCurve _RZCurve;


        private Vector3 _DeltaPosition;
        private Vector3 _DeltaRotation;
        private Vector3 _PrePosition;
        private Vector3 _PreRotation;

        private float _PreTime;

        /// <summary> delta position in current update </summary>
        public Vector3 DeltaPosition { get { return _DeltaPosition; } }

        /// <summary> delta rotation in current update </summary>
        public Vector3 DeltaRotation { get { return _DeltaRotation; } }

        /// <summary> Whether RootMotion contains position data </summary>
        public bool HasPosition { get; private set; }

        /// <summary> Whether RootMotion contains rotation data </summary>
        public bool HasRotation { get; private set; }

        /// <summary>
        /// Set Keyframes
        /// </summary>
        /// <param name="keys">Position Keyframes</param>
        public void SetKeyframes(RootMotionKeyframes keys)
        {
            if (keys == null)
            {
                _XCurve = _YCurve = _ZCurve = null;
                HasPosition = false;
                _RXCurve = _RYCurve = _RZCurve = null;
                HasRotation = false;
            }
            else
            {
                if (keys.XKeys != null && keys.XKeys.Length > 0)
                    _XCurve = new AnimationCurve(keys.XKeys);
                else
                    _XCurve = null;

                if (keys.YKeys != null && keys.YKeys.Length > 0)
                    _YCurve = new AnimationCurve(keys.YKeys);
                else
                    _YCurve = null;

                if (keys.ZKeys != null && keys.ZKeys.Length > 0)
                    _ZCurve = new AnimationCurve(keys.ZKeys);
                else
                    _ZCurve = null;

                HasPosition = _XCurve != null || _YCurve != null || _ZCurve != null;


                if (keys.RXKeys != null && keys.RXKeys.Length > 0)
                    _RXCurve = new AnimationCurve(keys.RXKeys);
                else
                    _RXCurve = null;

                if (keys.RYKeys != null && keys.RYKeys.Length > 0)
                    _RYCurve = new AnimationCurve(keys.RYKeys);
                else
                    _RYCurve = null;

                if (keys.RZKeys != null && keys.RZKeys.Length > 0)
                    _RZCurve = new AnimationCurve(keys.RZKeys);
                else
                    _RZCurve = null;

                HasRotation = _RXCurve != null || _RYCurve != null || _RZCurve != null;
            }
        }


        /// <summary>
        /// Create a RootMotion
        /// </summary>
        /// <param name="sequence">Owner Sequence</param>
        internal RootMotion(AnimNodeSequence sequence)
        {
            this._Sequence = sequence;
        }

        /// <summary>
        /// Begin calculatin RootMotion (when Sequence became relevant)
        /// </summary>
        internal void Begin()
        {
            if (_Sequence.State != null)
            {
                _PreTime = _Sequence.State.time;
                if (_XCurve != null)
                    _PrePosition.x = _XCurve.Evaluate(_PreTime);
                if (_YCurve != null)
                    _PrePosition.y = _YCurve.Evaluate(_PreTime);
                if (_ZCurve != null)
                    _PrePosition.z = _ZCurve.Evaluate(_PreTime);

                if (_RXCurve != null)
                    _PreRotation.x = _RXCurve.Evaluate(_PreTime);
                if (_RYCurve != null)
                    _PreRotation.y = _RYCurve.Evaluate(_PreTime);
                if (_RZCurve != null)
                    _PreRotation.z = _RZCurve.Evaluate(_PreTime);
            }
        }

        /// <summary>
        /// Evaluate curves and calculate motion between two updates
        /// </summary>
        internal void Evaluate()
        {
            _DeltaPosition = _DeltaRotation = Vector3.zero;

            if (_Sequence.State != null)
            {
                float time = _Sequence.State.time;
                float length = _Sequence.State.length;
                if (time > length)
                {
                    if (_Sequence.WrapMode == WrapMode.Loop)
                        while (time > length) time -= length;
                    else
                        time = length;
                }

                if (HasPosition)
                    _DeltaPosition = Evaluate(_XCurve, _YCurve, _ZCurve, time, _PreTime,length, ref _PrePosition);

                if (HasRotation)
                    _DeltaRotation = Evaluate(_RXCurve, _RYCurve, _RZCurve, time, _PreTime, length, ref _PreRotation);

                _PreTime = time;
            }
        }


        private static Vector3 Evaluate(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float time, float pretime, float length, ref Vector3 prePosition)
        {
            Vector3 motion = Vector3.zero;
            Vector3 position = motion;
            if (time != pretime)
            {
                if (time < pretime)
                {
                    if (curveX != null)
                    {
                        position.x = curveX.Evaluate(length);
                        motion.x += position.x - prePosition.x;
                        position.x = curveX.Evaluate(0);
                    }
                    if (curveY != null)
                    {
                        position.y = curveY.Evaluate(length);
                        motion.y += position.y - prePosition.y;
                        position.y = curveY.Evaluate(0);
                    }
                    if (curveZ != null)
                    {
                        position.z = curveZ.Evaluate(length);
                        motion.z += position.z - prePosition.z;
                        position.z = curveZ.Evaluate(0);
                    }

                    pretime = 0;
                    prePosition = position;
                }

                if (time > pretime)
                {
                    if (curveX != null)
                    {
                        position.x = curveX.Evaluate(time);
                        motion.x += position.x - prePosition.x;
                    }
                    if (curveY != null)
                    {
                        position.y = curveY.Evaluate(time);
                        motion.y += position.y - prePosition.y;
                    }
                    if (curveZ != null)
                    {
                        position.z = curveZ.Evaluate(time);
                        motion.z += position.z - prePosition.z;
                    }

                    prePosition = position;
                }
            }
            return motion;
        }
    }
}
