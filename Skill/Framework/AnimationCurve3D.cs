using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Skill.Framework
{

    /// <summary>
    /// AnimationCurve in 3D space
    /// </summary>
    public class AnimationCurve3D
    {
        private AnimationCurve _CurveX;
        private AnimationCurve _CurveY;
        private AnimationCurve _CurveZ;

        /// <summary> The number of keys in the curve. (Read Only) </summary>
        public int Length { get { return _CurveX.length; } }

        /// <summary> The behaviour of the animation after the last keyframe. </summary>
        public WrapMode PostWrapMode
        {
            get { return _CurveX.postWrapMode; }
            set { _CurveX.postWrapMode = _CurveY.postWrapMode = _CurveZ.postWrapMode = value; }
        }
        /// <summary> The behaviour of the animation before the first keyframe. </summary>
        public WrapMode PreWrapMode
        {
            get { return _CurveX.preWrapMode; }
            set { _CurveX.preWrapMode = _CurveY.preWrapMode = _CurveZ.preWrapMode = value; }
        }

        /// <summary> Retrieves the key at index. (Read Only) </summary>
        /// <param name="index">Index of key</param>
        /// <returns>Key</returns>
        public Keyframe3D this[int index]
        {
            get
            {
                Keyframe3D key = new Keyframe3D();
                key.Time = _CurveX[index].time;
                key.Value = new Vector3(_CurveX[index].value, _CurveY[index].value, _CurveZ[index].value);
                key.InTangent = new Vector3(_CurveX[index].inTangent, _CurveY[index].inTangent, _CurveZ[index].inTangent);
                key.OutTangent = new Vector3(_CurveX[index].outTangent, _CurveY[index].outTangent, _CurveZ[index].outTangent);
                return key;
            }
        }

        public AnimationCurve3D()
            : this(null)
        {
        }
        public AnimationCurve3D(Keyframe3D[] keys)
        {
            _CurveX = new AnimationCurve();
            _CurveY = new AnimationCurve();
            _CurveZ = new AnimationCurve();

            if (keys != null)
            {
                Keyframe keyframe = new Keyframe();
                foreach (var k in keys)
                {
                    keyframe.time = k.Time;

                    keyframe.inTangent = k.InTangent.x;
                    keyframe.outTangent = k.OutTangent.x;
                    keyframe.value = k.Value.x;
                    _CurveX.AddKey(keyframe);

                    keyframe.inTangent = k.InTangent.y;
                    keyframe.outTangent = k.OutTangent.y;
                    keyframe.value = k.Value.y;
                    _CurveY.AddKey(keyframe);

                    keyframe.inTangent = k.InTangent.z;
                    keyframe.outTangent = k.OutTangent.z;
                    keyframe.value = k.Value.z;
                    _CurveZ.AddKey(keyframe);

                }
            }
        }

        public void AddKey(Keyframe3D key)
        {

            Keyframe keyframe = new Keyframe();

            keyframe.time = key.Time;

            keyframe.inTangent = key.InTangent.x;
            keyframe.outTangent = key.OutTangent.x;
            keyframe.value = key.Value.x;
            _CurveX.AddKey(keyframe);

            keyframe.inTangent = key.InTangent.y;
            keyframe.outTangent = key.OutTangent.y;
            keyframe.value = key.Value.y;
            _CurveY.AddKey(keyframe);

            keyframe.inTangent = key.InTangent.z;
            keyframe.outTangent = key.OutTangent.z;
            keyframe.value = key.Value.z;
            _CurveZ.AddKey(keyframe);
        }

        /// <summary>
        /// Removes the keyframe at index and inserts key.
        /// </summary>
        /// <param name="index">Index of key</param>
        /// <param name="key">Keyframe to insert</param>
        /// <returns>Returns the index of the keyframe after moving it.</returns>
        /// <remarks>
        /// If a keyframe already exists at /key.time/ the time of the old keyframe's position /key[index].time/ will be used instead. This is the desired behaviour for dragging keyframes in a curve editor.
        /// </remarks>
        public int MoveKey(int index, Keyframe3D key)
        {
            _CurveX.MoveKey(index, new Keyframe() { time = key.Time, value = key.Value.x, inTangent = key.InTangent.x, outTangent = key.OutTangent.x });
            _CurveY.MoveKey(index, new Keyframe() { time = key.Time, value = key.Value.y, inTangent = key.InTangent.y, outTangent = key.OutTangent.y });
            return _CurveZ.MoveKey(index, new Keyframe() { time = key.Time, value = key.Value.z, inTangent = key.InTangent.z, outTangent = key.OutTangent.z });
        }

        /// <summary>
        /// Removes a key.
        /// </summary>
        /// <param name="index"> Index of key</param>
        public void RemoveKey(int index)
        {
            _CurveX.RemoveKey(index);
            _CurveY.RemoveKey(index);
            _CurveZ.RemoveKey(index);
        }

        /// <summary>
        /// Evaluate the curve at time.
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns></returns>
        public Vector3 Evaluate(float time)
        {
            return new Vector3(_CurveX.Evaluate(time), _CurveY.Evaluate(time), _CurveZ.Evaluate(time));
        }

        /// <summary>
        /// Smooth the in and out tangents of the keyframe at index.
        /// </summary>
        /// <param name="index">Index of key</param>
        /// <param name="weight">Weight</param>
        /// <remarks>
        /// A weight of 0 evens out tangents
        /// </remarks>
        public void SmoothTangents(int index, float weight)
        {
            _CurveX.SmoothTangents(index, weight);
            _CurveY.SmoothTangents(index, weight);
            _CurveZ.SmoothTangents(index, weight);
        }

        /// <summary> Retrieves a copy of all keys defined in the animation curve. (Read Only) </summary>
        public Keyframe3D[] GetKeys()
        {
            Keyframe[] xKeys = _CurveX.keys;
            Keyframe[] yKeys = _CurveY.keys;
            Keyframe[] zKeys = _CurveZ.keys;

            Keyframe3D[] keys = new Keyframe3D[Length];
            for (int i = 0; i < Length; i++)
            {
                keys[i] = new Keyframe3D();
                keys[i].Time = xKeys[i].time;
                keys[i].Value = new Vector3(xKeys[i].value, yKeys[i].value, zKeys[i].value);
                keys[i].InTangent = new Vector3(xKeys[i].inTangent, yKeys[i].inTangent, zKeys[i].inTangent);
                keys[i].OutTangent = new Vector3(xKeys[i].outTangent, yKeys[i].outTangent, zKeys[i].outTangent);
            }
            return keys;
        }

        /// <summary> Retrieves a copy of all keys defined in the animation curve x axis. (Read Only) </summary>
        public Keyframe[] GetXKeys() { return _CurveX.keys; }
        /// <summary> Retrieves a copy of all keys defined in the animation curve y axis. (Read Only) </summary>
        public Keyframe[] GetYKeys() { return _CurveY.keys; }
        /// <summary> Retrieves a copy of all keys defined in the animation curve z axis. (Read Only) </summary>
        public Keyframe[] GetZKeys() { return _CurveZ.keys; }


        /// <summary>
        /// An ease-in and out curve starting at timeStart, valueStart and ending at timeEnd, valueEnd.
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="valueStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="valueEnd"></param>
        /// <returns></returns>
        public static AnimationCurve3D EaseInOut(float timeStart, Vector3 valueStart, float timeEnd, Vector3 valueEnd)
        {
            AnimationCurve xCurve = AnimationCurve.EaseInOut(timeStart, valueStart.x, timeEnd, valueEnd.x);
            AnimationCurve yCurve = AnimationCurve.EaseInOut(timeStart, valueStart.y, timeEnd, valueEnd.y);
            AnimationCurve zCurve = AnimationCurve.EaseInOut(timeStart, valueStart.z, timeEnd, valueEnd.z);

            return Create(xCurve, yCurve, zCurve);
        }

        public static AnimationCurve3D Linear(float timeStart, Vector3 valueStart, float timeEnd, Vector3 valueEnd)
        {
            AnimationCurve xCurve = AnimationCurve.Linear(timeStart, valueStart.x, timeEnd, valueEnd.x);
            AnimationCurve yCurve = AnimationCurve.Linear(timeStart, valueStart.y, timeEnd, valueEnd.y);
            AnimationCurve zCurve = AnimationCurve.Linear(timeStart, valueStart.z, timeEnd, valueEnd.z);

            return Create(xCurve, yCurve, zCurve);
        }

        public static AnimationCurve3D Create(AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve zCurve)
        {
            Keyframe[] xKeys = xCurve.keys;
            Keyframe[] yKeys = yCurve.keys;
            Keyframe[] zKeys = zCurve.keys;

            Keyframe3D[] keys = new Keyframe3D[xKeys.Length];
            for (int i = 0; i < xKeys.Length; i++)
            {
                keys[i].Time = xKeys[i].time;
                keys[i].Value = new Vector3(xKeys[i].value, yKeys[i].value, zKeys[i].value);
                keys[i].InTangent = new Vector3(xKeys[i].inTangent, yKeys[i].inTangent, zKeys[i].inTangent);
                keys[i].OutTangent = new Vector3(xKeys[i].outTangent, yKeys[i].outTangent, zKeys[i].outTangent);
            }
            return new AnimationCurve3D(keys);
        }



    }


    /// <summary>
    /// Keyframe in 3D space
    /// </summary>
    [System.Serializable]
    public class Keyframe3D : IComparer<Keyframe3D>
    {
        /// <summary> The time of the keyframe. </summary>
        public float Time;
        /// <summary> The value of the curve at keyframe.</summary>
        public Vector3 Value;
        /// <summary> Describes the tangent when approaching this point from the previous point in the curve. </summary>
        public Vector3 InTangent;
        /// <summary> Describes the tangent when leaving this point towards the next point in the curve. </summary>
        public Vector3 OutTangent;


        /// <summary>
        /// Create a Keyframe3D
        /// </summary>
        public Keyframe3D()
            : this(0, Vector3.zero, Vector3.zero, Vector3.zero)
        {
        }

        /// <summary>
        /// Create a Keyframe3D
        /// </summary>
        /// <param name="time">The time of the keyframe.</param>
        /// <param name="value">The value of the curve at keyframe.</param>
        public Keyframe3D(float time, Vector3 value)
            : this(time, value, Vector3.zero, Vector3.zero)
        {
        }

        /// <summary>
        /// Create a Keyframe3D
        /// </summary>
        /// <param name="time">The time of the keyframe.</param>
        /// <param name="value">The value of the curve at keyframe.</param>
        /// <param name="inTangent">Describes the tangent when approaching this point from the previous point in the curve.</param>
        /// <param name="outTangent">Describes the tangent when leaving this point towards the next point in the curve.</param>
        public Keyframe3D(float time, Vector3 value, Vector3 inTangent, Vector3 outTangent)
        {
            this.Time = time;
            this.Value = value;
            this.InTangent = inTangent;
            this.OutTangent = outTangent;
        }

        /// <summary>
        /// Create a copy of Keyframe3D
        /// </summary>
        /// <param name="other">Other Keyframe3D to copy from</param>
        public Keyframe3D(Keyframe3D other)
            : this(other.Time, other.Value, other.InTangent, other.OutTangent)
        {
        }

        /// <summary>
        /// Compare keyframes by time
        /// </summary>
        /// <param name="x">Keyframe3D</param>
        /// <param name="y">Keyframe3D</param>
        /// <returns></returns>
        public int Compare(Keyframe3D x, Keyframe3D y) { return x.Time.CompareTo(y.Time); }
    }
}