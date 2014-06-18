using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Some helper methods for math
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Wether given angle is between (sourceAngle) and (sourceAngle - 180)
        /// </summary>
        /// <param name="angle">Angle</param>
        /// <param name="sourceAngle">Source angle</param>
        /// <returns>True if angle is left side, otherwise false</returns>
        public static bool IsAngleLeftOf(float angle, float sourceAngle)
        {
            float validAngle = ClampAngle(angle);
            float validSourceAngle = ClampAngle(sourceAngle);

            return Mathf.Abs(validSourceAngle - validAngle) < 180;
        }

        /// <summary>
        /// Calc angle rotation around y axis
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <returns>Angle</returns>
        public static float HorizontalAngle(Vector3 direction)
        {
            return HorizontalAngle(direction.x, direction.z);
        }

        /// <summary>
        /// Calc angle rotation around y axis
        /// </summary>
        /// <param name="x">Direction.x</param>
        /// <param name="z">Direction.z</param>
        /// <returns>Angle</returns>
        public static float HorizontalAngle(float x, float z)
        {
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
            ClampAngle(ref angle);
            return angle;
        }


        /// <summary>
        /// Keep angle between -180 to 180
        /// </summary>
        /// <param name="angle">Angle to validate</param>
        /// <returns>angle between -180 to 180</returns>
        public static float ClampAngle(float angle)
        {
            ClampAngle(ref angle);// angle is a value copy
            return angle;
        }

        /// <summary>
        /// Keep angle between -180 to 180
        /// </summary>
        /// <param name="angle">Angle to validate</param>        
        public static void ClampAngle(ref float angle)
        {
            while (angle < -180) angle += 360;
            while (angle > 180) angle -= 360;
        }


        /// <summary>
        /// The angle between dirA and dirB around axis
        /// </summary>
        /// <param name="dirA">Direction A</param>
        /// <param name="dirB">Direction B</param>
        /// <param name="axis">Axis</param>
        /// <returns>The angle between dirA and dirB around axis</returns>
        public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
        {
            // Project A and B onto the plane orthogonal target axis
            dirA = dirA - Vector3.Project(dirA, axis);
            dirB = dirB - Vector3.Project(dirB, axis);

            // Find (positive) angle between A and B
            float angle = Vector3.Angle(dirA, dirB);

            // Return angle multiplied with 1 or -1
            return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
        }


        /// <summary>
        /// Converts 'Meter Per Second' to 'Kilometer Per Hour'
        /// </summary>
        /// <param name="mps">value in meter per second</param>
        /// <returns></returns>
        public static float MPS_To_KPH(float mps)
        {
            return mps * 3.6f;
        }

        /// <summary>
        /// Sort array
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="array">Array to sort</param>
        /// <param name="comparer">Comparer to compare items</param>
        public static void QuickSort<T>(T[] array, IComparer<T> comparer)
        {
            if (array == null)
                throw new ArgumentNullException("Array is null");
            if (comparer == null)
                throw new ArgumentNullException("Comparer is null");
            if (array.Length > 1)
                QuickSort(array, 0, array.Length - 1, comparer);
        }

        private static void QuickSort<T>(T[] array, int left, int right, IComparer<T> comparer)
        {
            if (right <= left) return;
            int pivot = QuickSortPartition2(array, left, right, comparer);
            QuickSort(array, left, pivot - 1, comparer);
            QuickSort(array, pivot + 1, right, comparer);

        }

        private static int QuickSortPartition2<T>(T[] array, int left, int right, IComparer<T> comparer)
        {
            T pivot = array[left];
            while (left < right)
            {
                while ((left < right) && comparer.Compare(array[right], pivot) > 0) --right;
                if (left < right)
                {
                    array[left] = array[right];
                    ++left;
                }

                while ((left < right) && comparer.Compare(array[left], pivot) < 0) ++left;
                if (left < right)
                {
                    array[right] = array[left];
                    --right;
                }
            }
            array[left] = pivot;
            return left;

        }



        /// <summary>
        /// calc projection of a point on a line
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="lineStart">Start of line</param>
        /// <param name="lineDirection">Direction of line</param>
        /// <returns></returns>
        public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineDirection)
        {
            return Vector3.Project((point - lineStart), lineDirection) + lineStart;
        }

        /// <summary>
        /// Calculate 32bit integer layermask
        /// </summary>
        /// <param name="layerIndices">Index of layers</param>
        /// <returns>layermask</returns>
        public static int GetLayerMask(params int[] layerIndices)
        {
            int mask = 0;
            if (layerIndices != null)
            {
                foreach (var item in layerIndices)
                {
                    mask |= 1 << item;
                }
            }
            return mask;
        }

        /// <summary>
        /// Calculate distance to camera where height of frustum is specified value
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="frustumHeight">Height of frustum</param>
        /// <returns>distance to camera</returns>
        public static float FrameFrustumHeight(Camera camera, float frustumHeight)
        {
            return frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Calculate distance to camera where width of frustum is specified value
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="frustumWidth">Width of frustum</param>
        /// <returns>distance to camera</returns>
        public static float FrameFrustumWidth(Camera camera, float frustumWidth)
        {
            float frustumHeight = frustumWidth / camera.aspect;
            return frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Calculate height of frustum at specified distance
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="distance">Distance to camera</param>
        /// <returns>height of frustum</returns>
        public static float FrustumHeightAtDistance(Camera camera, float distance)
        {
            return distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f;
        }

        /// <summary>
        /// Calculate width of frustum at specified distance
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="distance">Distance to camera</param>
        /// <returns>width of frustum</returns>
        public static float FrustumWidthAtDistance(Camera camera, float distance)
        {
            return FrustumHeightAtDistance(camera, distance) * camera.aspect;
        }


        private static Dictionary<int, Vector3[]> _Circles;

        /// <summary>
        /// Calclate points on a xzplane circle with specified resolution
        /// </summary>
        /// <param name="resolution">number of points (at least 3)</param>
        /// <returns>Circle path</returns>
        public static Vector3[] GetCirclePath(int resolution)
        {
            if (resolution < 2)
                throw new ArgumentException("Invalid resolution - must be at least 2");

            if (_Circles == null) _Circles = new Dictionary<int, Vector3[]>();

            Vector3[] result = null;
            if (_Circles.TryGetValue(resolution, out result))
                return result;

            result = new Vector3[resolution];
            float angle = 0;
            float deltaAngel = (Mathf.PI * 2) / resolution;
            int i = 0;
            for (i = 0; i < resolution; i++)
            {
                result[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                angle += deltaAngel;
            }

            _Circles.Add(resolution, result);
            return result;
        }


        /// <summary>
        /// Normalize vector3 and calc magnitude
        /// </summary>
        /// <param name="v">Vector3</param>
        /// <returns>Magnitude of Vector3</returns>
        public static float NormalizeAndMagnitude(ref Vector3 v)
        {
            float magnitude = Mathf.Sqrt((v.x * v.x) + (v.y * v.y) + (v.z * v.z));
            if (magnitude > 0.00001f)
            {
                float f = 1.0f / magnitude;
                v.x *= f; v.y *= f; v.z *= f;
            }
            else
            {
                v = Vector3.zero;
                magnitude = 0;
            }
            return magnitude;
        }
    }
}
