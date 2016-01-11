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


        /// <summary>
        /// Find index of object in array
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="array">Array</param>
        /// <param name="obj">Object to find</param>
        /// <returns>found : index of object in array, else -1</returns>
        public static int IndexOf<T>(T[] array, T obj) where T : class
        {
            int index = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == obj)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Detach and destroy children
        /// </summary>
        /// <param name="t">Transform to Detach and destroy it's children</param>
        public static void DestroyChildren(Transform t)
        {
            Transform[] childs = new Transform[t.childCount];
            for (int i = 0; i < childs.Length; i++)
                childs[i] = t.GetChild(i);
            t.DetachChildren();

#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                for (int i = 0; i < childs.Length; i++)
                    GameObject.Destroy(childs[i].gameObject);
            }
            else
            {
                for (int i = 0; i < childs.Length; i++)
                    GameObject.DestroyImmediate(childs[i].gameObject);
            }
#else
            for (int i = 0; i < childs.Length; i++)
                GameObject.Destroy(childs[i].gameObject);
#endif


        }



        /// <summary>
        /// Destroy array of objects
        /// </summary>
        /// <param name="objects">Objects to destroy</param>
        public static void Destroy(GameObject[] objects)
        {
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] != null)
                    {
                        StaticBehaviour sb = objects[i].GetComponent<StaticBehaviour>();
                        if (sb != null && sb.IsDestroyed)
                            continue;
                        GameObject.Destroy(objects[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Is specified point inside view area of main camera
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <returns>true if visible: otherwise false</returns>
        public static bool IsVisible(Vector3 point)
        {
            return IsVisible(point, Camera.main);
        }

        /// <summary>
        /// Is specified point inside view area of camera
        /// </summary>
        /// <param name="point">Point in world space</param>        
        /// <param name="camera">Camera</param>
        /// <returns>true if visible: otherwise false</returns>
        public static bool IsVisible(Vector3 point, Camera camera)
        {
            Vector3 viewPoint = Camera.main.WorldToViewportPoint(point);
            return (viewPoint.z >= 0) && (viewPoint.x >= 0 && viewPoint.x <= 1.0f) && (viewPoint.y >= 0 && viewPoint.y <= 1.0f);
        }

        /// <summary>
        /// This will return true if the renderer is within the camera's view frustrum
        /// </summary>
        /// <param name="renderer">renderer</param>
        /// <param name="camera">camera</param>
        /// <returns>return true if the renderer is within the camera's view frustrum</returns>
        public static bool IsVisible(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }


        private static System.Security.Cryptography.MD5CryptoServiceProvider _MD5;
        private static System.Text.UTF8Encoding _UTF8Encoding;
        /// <summary>
        /// Generates an MD5 hash of the given text.
        /// WARNING. Not safe for storing passwords
        /// </summary>
        /// <returns>MD5 Hashed string</returns>
        /// <param name="text">The text to hash</param>
        public static string GenerateMD5(string text)
        {
            if (_MD5 == null)
                _MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            if (_UTF8Encoding == null)
                _UTF8Encoding = new System.Text.UTF8Encoding();

            byte[] inputBytes = _UTF8Encoding.GetBytes(text);
            byte[] hash = _MD5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }



        /// <summary>
        /// select random item by chance
        /// </summary>
        /// <param name="weights">weight of each item</param>
        /// <returns>index of item</returns>
        public static int RandomByChance(float[] weights)
        {
            float totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] < 0.0f)
                    throw new System.ArgumentOutOfRangeException(string.Format("chances[{0}]", i), "chance must be greater than zero");
                totalWeight += weights[i];
            }
            return RandomByChance(weights, totalWeight);
        }

        /// <summary>
        /// select random item by chance
        /// </summary>
        /// <param name="weights">weight of each item</param>        
        /// <param name="totalWeight">precalculated sun of all weights</param>
        /// <returns>index of item</returns>
        public static int RandomByChance(float[] weights, float totalWeight)
        {
            float rnd = UnityEngine.Random.Range(0.0f, totalWeight);
            for (int i = 0; i < weights.Length; i++)
            {
                if (rnd < weights[i])
                    return i;
                rnd -= weights[i];
            }
            return -1;
        }


        public class GameObjectNameComparer : IComparer<GameObject>
        {
            private static GameObjectNameComparer _Instance;
            public static GameObjectNameComparer Instance
            {
                get
                {
                    if (_Instance == null)
                        _Instance = new GameObjectNameComparer();
                    return _Instance;
                }
            }
            public bool Descending { get; set; }

            public int Compare(GameObject x, GameObject y)
            {
                if (Descending)
                    return y.name.CompareTo(x.name);
                else
                    return x.name.CompareTo(y.name);
            }
        }
        public class ComponentNameComparer : IComparer<Component>
        {
            private static ComponentNameComparer _Instance;
            public static ComponentNameComparer Instance
            {
                get
                {
                    if (_Instance == null)
                        _Instance = new ComponentNameComparer();
                    return _Instance;
                }
            }
            public bool Descending { get; set; }

            public int Compare(Component x, Component y)
            {
                if (Descending)
                    return y.name.CompareTo(x.name);
                else
                    return x.name.CompareTo(y.name);
            }
        }


        public static class Array
        {
            public static void Add<T>(ref T[] array, T item)
            {
                System.Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = item;
            }

            public static void AddRange<T>(ref T[] array, T[] items)
            {
                int length = array.Length;
                System.Array.Resize(ref array, array.Length + items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    array[length + i] = items[i];
                }
            }

            public static bool ArrayEquals<T>(T[] lhs, T[] rhs)
            {
                if (lhs.Length != rhs.Length)
                {
                    return false;
                }
                for (int i = 0; i < lhs.Length; i++)
                {
                    if (!lhs[i].Equals(rhs[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public static void Clear<T>(ref T[] array)
            {
                System.Array.Clear(array, 0, array.Length);
                System.Array.Resize<T>(ref array, 0);
            }

            public static bool Contains<T>(T[] array, T item)
            {
                List<T> list = new List<T>(array);
                return list.Contains(item);
            }

            public static T Find<T>(T[] array, Predicate<T> match)
            {
                List<T> list = new List<T>(array);
                return list.Find(match);
            }

            public static List<T> FindAll<T>(T[] array, Predicate<T> match)
            {
                List<T> list = new List<T>(array);
                return list.FindAll(match);
            }

            public static int FindIndex<T>(T[] array, Predicate<T> match)
            {
                List<T> list = new List<T>(array);
                return list.FindIndex(match);
            }

            public static int IndexOf<T>(T[] array, T value)
            {
                List<T> list = new List<T>(array);
                return list.IndexOf(value);
            }

            public static void Insert<T>(ref T[] array, int index, T item)
            {
                System.Collections.ArrayList list = new System.Collections.ArrayList();
                list.AddRange(array);
                list.Insert(index, item);
                array = list.ToArray(typeof(T)) as T[];
            }

            public static int LastIndexOf<T>(T[] array, T value)
            {
                List<T> list = new List<T>(array);
                return list.LastIndexOf(value);
            }

            public static void Remove<T>(ref T[] array, T item)
            {
                List<T> list = new List<T>(array);
                list.Remove(item);
                array = list.ToArray();
            }

            public static void RemoveAt<T>(ref T[] array, int index)
            {
                List<T> list = new List<T>(array);
                list.RemoveAt(index);
                array = list.ToArray();
            }
        }
    }
}
