using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework
{
    /// <summary>
    /// andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    /// </summary>
    public class CRSpline3D : Path3D
    {
        /// <summary> Keys of path </summary>
        [HideInInspector]
        public Vector3[] Keys;

        /// <summary> Times of keys </summary>
        [HideInInspector]
        public float[] Times;

        /// <summary> Number of keys in path </summary>
        public override int Length { get { return Keys != null ? Keys.Length : 0; } }
        /// <summary> lenght of path in time</summary>
        public override float TimeLength { get { return (Times != null && Times.Length > 0) ? Times[Times.Length - 1] : 0; } }

        private Vector3[] _PathControlPoints;
        /// <summary> Rebuild path after modify keys </summary>
        public override void Rebuild()
        {
            if (Keys == null) Keys = new Vector3[] { _Transform.position, _Transform.position + Vector3.forward };
            else if (Keys.Length == 1) Keys = new Vector3[] { Keys[0], Keys[0] + Vector3.forward };

            if (Times == null) Times = new float[] { 0, 1 };
            else if (Times.Length == 1) Times = new float[] { Times[0], Times[0] + 1 };

            _PathControlPoints = GeneratorPathControlPoints(Keys);
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="time"> Time</param>
        /// <returns>Evaluate position</returns>
        public override Vector3 Evaluate(float time)
        {
            time = ConvertToInterpolationTime(time);
            Vector3 result = Interpolate(_PathControlPoints, time);
            if (!UseWorldSpace)
                result = _Transform.TransformPoint(result);
            return result;
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="time"> Time</param>
        /// <returns>Evaluate position</returns>
        public Vector3 Evaluate01(float time)
        {            
            Vector3 result = Interpolate(_PathControlPoints, time);
            if (!UseWorldSpace)
                result = _Transform.TransformPoint(result);
            return result;
        }

        private float ValidateTime(float time)
        {
            if (time < 0)
            {
                switch (PreWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        time = 0.0f;
                        break;
                    case WrapMode.Loop:
                        time = Mathf.Abs(time);
                        time = TimeLength - (time - Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                    case WrapMode.PingPong:
                        time = Mathf.Abs(time);
                        time = time - (Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                }
            }
            else if (time > TimeLength)
            {
                switch (PostWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        time = TimeLength;
                        break;
                    case WrapMode.Loop:
                        time = time - (Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                    case WrapMode.PingPong:
                        time = TimeLength - (time - (Mathf.FloorToInt(time / TimeLength) * TimeLength));
                        break;
                }
            }
            return time;
        }

        /// <summary>
        /// Convert time to range (0.0f - 1.0f)
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>time between (0.0f - 1.0f)</returns>
        public float ConvertToInterpolationTime(float time)
        {
            time = ValidateTime(time);
            int index = FindTimeIndex(time);
            float timeStep = 1.0f / (Length - 1);
            return (index + (time - Times[index]) / (Times[index + 1] - Times[index])) * timeStep;
        }

        int FindTimeIndex(float time)
        {
            int minTimeIndex = 0;
            int maxTimeIndex = Length - 1;

            while (maxTimeIndex - minTimeIndex > 1)
            {
                // calculate the midpoint for roughly equal partition
                int middleIndex = (minTimeIndex + maxTimeIndex) / 2;                
                // determine which subarray to search
                if (Times[middleIndex] < time)
                    // change min index to search upper subarray
                    minTimeIndex = middleIndex;
                else
                    // change max index to search lower subarray
                    maxTimeIndex = middleIndex;
            }

            return minTimeIndex;
        }


        /// <summary>
        /// Retrieves time of path at specified point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght)</param>
        /// <returns>Time</returns>
        public override float GetTime(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Times[pointIndex];
        }



        /// <summary>
        /// Retrieves position of path control point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght - 1)</param>
        /// <returns>position of path control point</returns>
        public override Vector3 GetPoint(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Keys[pointIndex];
        }

        /// <summary>
        /// Interpolate between points at specified time
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="time">time (0.0f - 1.0f)</param>
        /// <returns></returns>
        public static Vector3 Interpolate(Vector3[] points, float time)
        {
            if (points.Length < 4) throw new ArgumentException("lenght of points must be at least 4");
            int numSections = points.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currPt;
            Vector3 a = points[currPt];
            Vector3 b = points[currPt + 1];
            Vector3 c = points[currPt + 2];
            Vector3 d = points[currPt + 3];
            return 0.5f * ((-a + 3.0f * b - 3.0f * c + d) * (u * u * u) + (2.0f * a - 5.0f * b + 4.0f * c - d) * (u * u) + (-a + c) * u + 2.0f * b);
        }


        /// <summary>
        /// Generate curve points
        /// </summary>
        /// <param name="pathPoints">points</param>
        /// <returns>curve points</returns>
        public static Vector3[] GeneratorPathControlPoints(Vector3[] pathPoints)
        {
            Vector3[] suppliedPath;
            Vector3[] vector3s;

            //create and store path points:
            suppliedPath = pathPoints;

            //populate calculate path;
            int offset = 2;
            vector3s = new Vector3[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

            //populate start and end control points:
            //vector3s[0] = vector3s[1] - vector3s[2];
            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector3[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
            }

            return (vector3s);
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color;
            if (ShowPoints && Keys != null)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawSphere(Keys[i], PointRadius);
                    else
                        Gizmos.DrawSphere(transform.TransformPoint(Keys[i]), PointRadius);
                }
            }
            if (ShowPath && Keys != null)
            {
                for (int i = 0; i < Keys.Length - 1; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawLine(Keys[i], Keys[i + 1]);
                    else
                        Gizmos.DrawLine(transform.TransformPoint(Keys[i]), transform.TransformPoint(Keys[i + 1]));
                }
            }
        }
    }
}