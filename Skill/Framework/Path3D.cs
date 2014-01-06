using System;
using UnityEngine;
using System.Collections.Generic;
namespace Skill.Framework
{
    /// <summary>
    /// base class for path in 3d space
    /// </summary>
    public abstract class Path3D : Skill.Framework.StaticBehaviour
    {
        /// <summary> How to evaluate when time exit from range of path </summary>
        public WrapMode PostWrapMode = WrapMode.ClampForever;
        /// <summary> How to evaluate when time exit from range of path </summary>
        public WrapMode PreWrapMode = WrapMode.ClampForever;
        /// <summary> If enabled, the curve are defined in world space.</summary>
        public bool UseWorldSpace = true;
        /// <summary> Show gizmo for control points.</summary>
        public bool ShowPoints = true;
        /// <summary> How to show smooth in editor.</summary>
        public int SmoothAmount = 20;
        /// <summary> color of path in editor.</summary>
        public Color Color = Color.blue;
        /// <summary> radius of gizmo points.</summary>
        public float PointRadius = 0.1f;
        /// <summary> use in Editor </summary>
        [HideInInspector]
        public float PathTime;
        /// <summary> use in Editor </summary>
        [HideInInspector]
        public int SelectedIndex = 0;

        /// <summary> Show lowpoly path in editor(for internal use).</summary>
        public bool ShowPath { get; set; }

        protected override void Awake()
        {
            base.Awake();
            ShowPath = true; Rebuild();
        }

        /// <summary>
        /// Get direction along path at specified time
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="deltaTime">deltaTime</param>
        /// <returns>Direction</returns>
        public Vector3 GetDirection(float time, float deltaTime = 0.01f)
        {
            Vector3 direction;
            GetDirection(time, deltaTime, out direction);
            return direction;
        }
        /// <summary>
        /// Get direction along path at specified time
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="deltaTime">deltaTime</param>    
        /// <param name="direction">Direction</param>
        public void GetDirection(float time, float deltaTime, out Vector3 direction)
        {
            if (deltaTime < 0.0001f) deltaTime = 0.0001f;
            direction = (Evaluate(time + deltaTime) - Evaluate(time)).normalized;

        }

        /// <summary> Number of keys in path </summary>
        public abstract int Length { get; }
        /// <summary> lenght of path in time</summary>
        public abstract float TimeLength { get; }
        /// <summary> Rebuild path after modify keys </summary>
        public abstract void Rebuild();
        /// <summary> Evaluate path at specified time </summary>
        /// <param name="time">time (0 - TimeLength)</param>
        /// <returns>Evaluated point</returns>
        public abstract Vector3 Evaluate(float time);

        /// <summary>
        /// Retrieves time of path at specified point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght - 1)</param>
        /// <returns>Time</returns>
        public abstract float GetTime(int pointIndex);

        /// <summary>
        /// Retrieves position of path control point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght - 1)</param>
        /// <returns>position of path control point</returns>
        public abstract Vector3 GetPoint(int pointIndex);

        protected int GetValidIndex(int pointIndex)
        {
            if (pointIndex < 0)
            {
                switch (PreWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        pointIndex = 0;
                        break;
                    case WrapMode.Loop:
                        pointIndex = Length - Mathf.Abs(pointIndex) % Length - 1;
                        break;
                    case WrapMode.PingPong:
                        pointIndex = Mathf.Abs(pointIndex) % Length;
                        break;
                }
            }
            else if (pointIndex >= Length)
            {
                switch (PostWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        pointIndex = Length - 1;
                        break;
                    case WrapMode.Loop:
                        pointIndex = Mathf.Abs(pointIndex) % Length;
                        break;
                    case WrapMode.PingPong:
                        pointIndex = Length - (pointIndex % Length) - 1;
                        break;
                }
            }

            return pointIndex;
        }

        /// <summary>
        /// Calculate distance of path points relative to first point
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="precision"> precision of calculation</param>
        /// <returns>distances points relative to first point</returns>
        public static float[] CalcDistances(Path3D path, int precision = 60)
        {
            if (path != null)
            {
                precision = Mathf.Max(precision, 10);
                float maxDistance = 0;
                float[] distances = new float[path.Length];
                distances[0] = 0;

                for (int i = 1; i < path.Length; i++)
                {
                    maxDistance += CalcDistance(path, path.GetTime(i - 1), path.GetTime(i), precision);
                    distances[i] = maxDistance;
                }

                return distances;
            }
            return null;
        }

        /// <summary>
        /// Calculate distance of path points relative to previous point
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="precision"> precision of calculation</param>
        /// <returns>distances points relative to previous point</returns>
        public static float[] CalcDeltaDistances(Path3D path, int precision = 60)
        {
            if (path != null)
            {
                precision = Mathf.Max(precision, 10);
                float[] distances = new float[path.Length];
                distances[0] = 0;

                for (int i = 1; i < path.Length; i++)
                    distances[i] = CalcDistance(path, path.GetTime(i - 1), path.GetTime(i), precision);

                return distances;
            }
            return null;
        }


        /// <summary>
        /// Calculate distance between 2 times in path
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="precision"> Precision of calculation</param>
        /// <returns>distance between 2 times in path</returns>
        public static float CalcDistance(Path3D path, float startTime, float endTime, int precision = 60)
        {
            float timeStep = (endTime - startTime) / precision;
            float distance = 0;
            float time = startTime;
            for (int i = 0; i < precision; i++)
            {
                distance += Vector3.Distance(path.Evaluate(time), path.Evaluate(time + timeStep));
                time += timeStep;
            }

            return distance;
        }
    }

}