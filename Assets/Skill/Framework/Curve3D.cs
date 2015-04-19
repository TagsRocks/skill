using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    /// <summary>
    /// Path3D use tangents
    /// </summary>
    public class Curve3D : Path3D
    {
        /// <summary> use in Editor </summary>
        public bool ShowInTangent;
        /// <summary> use in Editor </summary>
        public bool ShowOutTangent;

        /// <summary> Keys of path </summary>
        [HideInInspector]
        public Keyframe3D[] Keys;
        /// <summary> use in Editor </summary>
        [HideInInspector]
        public float SmoothValue = 0.5f;

        private AnimationCurve3D _Curve;
        /// <summary> lenght of path in time</summary>
        public override float TimeLength { get { return (Keys != null && Keys.Length > 0) ? Keys[Keys.Length - 1].Time : 0; } }
        /// <summary> Number of keys in path </summary>
        public override int Length { get { return Keys.Length; } }
        /// <summary> Rebuild path after modify keys </summary>
        public override void Rebuild()
        {
            if (Keys != null && Keys.Length > 0)
            {
                Skill.Framework.Utility.QuickSort(Keys, Keys[0]);
                _Curve = new AnimationCurve3D(Keys) { PostWrapMode = PostWrapMode, PreWrapMode = PreWrapMode };
            }
        }
        /// <summary> Evaluate path at specified time </summary>
        /// <param name="time">time (0 - TimeLength)</param>
        /// <returns>Evaluated point</returns>
        public override Vector3 Evaluate(float time)
        {            
            if (_Curve != null)
            {
                if (UseWorldSpace)
                    return _Curve.Evaluate(time);
                else
                    return transform.TransformPoint(_Curve.Evaluate(time));
            }
            else
            {
                return transform.position;
            }
        }

        /// <summary>
        /// Retrieves time of path at specified point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght)</param>
        /// <returns>Time</returns>
        public override float GetTime(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Keys[pointIndex].Time;
        }

        /// <summary>
        /// Retrieves position of path control point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght - 1)</param>
        /// <returns>position of path control point</returns>
        public override Vector3 GetPoint(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Keys[pointIndex].Value;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color;
            if (ShowPoints && Keys != null)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawSphere(Keys[i].Value, PointRadius);
                    else
                        Gizmos.DrawSphere(transform.TransformPoint(Keys[i].Value), PointRadius);
                }
            }
            if (ShowPath && Keys != null)
            {
                for (int i = 0; i < Keys.Length - 1; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawLine(Keys[i].Value, Keys[i + 1].Value);
                    else
                        Gizmos.DrawLine(transform.TransformPoint(Keys[i].Value), transform.TransformPoint(Keys[i + 1].Value));
                }
            }
        }        
    }
}