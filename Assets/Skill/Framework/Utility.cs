using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    public static class Utility
    {

        /// <summary>
        /// Change specified components of Vector3, null components remains unchanged
        /// </summary>
        /// <param name="vector">Vector3 to modify</param>
        /// <param name="x"> new value of x, null for unchanged </param>
        /// <param name="y"> new value of y, null for unchanged </param>
        /// <param name="z"> new value of z, null for unchanged</param>
        /// <returns> Modified vector </returns>
        /// <remarks>
        /// exp: useful when u want change y of transform.position
        /// use :
        ///     transform.position = Utility.Modify(transform.position,null,newY,null);
        /// Instead of :
        ///     Vector3 pos = transform.position;
        ///     pos.y = newY;
        ///     transform.position = pos;
        /// 
        /// </remarks>
        public static Vector3 Modify(Vector3 vector, float? x, float? y, float? z)
        {
            if (x != null && x.HasValue) vector.x = x.Value;
            if (y != null && y.HasValue) vector.y = y.Value;
            if (z != null && z.HasValue) vector.z = z.Value;
            return vector;
        }

        /// <summary> Returns rect of entire screen </summary>
        public static Rect ScreenRect { get { return new Rect(0, 0, Screen.width, Screen.height); } }


        /// <summary>
        /// Activate or deactivate objects
        /// </summary>
        /// <param name="objects">list of objects</param>
        /// <param name="active"> active? </param>
        public static void Activate(IEnumerable<GameObject> objects, bool active)
        {
            if (objects == null) throw new ArgumentNullException("Invalid objects");
            foreach (var obj in objects)
            {
                if (obj != null)
                    obj.SetActive(active);
            }
        }


        /// <summary>
        /// Is specified Transform(child) somewhere in hierarchy of root
        /// </summary>
        /// <param name="root">Root transform</param>
        /// <param name="child">Child transform to search</param>
        /// <returns> true is is in hierarchy, otherwise false</returns>
        public static bool IsInHierarchy(Transform root, Transform child)
        {
            if (root == child) return true;
            for (int i = 0; i < root.childCount; i++)
            {
                bool result = IsInHierarchy(root.GetChild(i), child);
                if (result) return true;
            }
            return false;
        }

        /// <summary>
        /// Get path os child in hierarchy of root (make sure child is in hierarchy)
        /// </summary>
        /// <param name="root">Root transform </param>
        /// <param name="child"> Child transform in hierarchy </param>
        /// <returns>Path</returns>
        public static string GetPath(Transform root, Transform child)
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder();
            path.Insert(0, child.gameObject.name);

            Transform parent = child.parent;
            while (parent != null)
            {
                if (parent == root) break;
                path.Insert(0, "/");
                path.Insert(0, parent.gameObject.name);
                parent = parent.parent;
            }

            return path.ToString();
        }


        #region QuickSort
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
        #endregion
    }
}
