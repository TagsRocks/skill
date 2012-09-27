using System;
using UnityEngine;
using System.Collections.Generic;

namespace Skill.Managers
{
    /// <summary>
    /// Information about an GameObject by CacheBehavior component
    /// </summary>
    [Serializable]
    public class CacheObject
    {
        /// <summary> Cacheable GameObject</summary>
        public GameObject Prefab;
        /// <summary> Number of instances to create at initialize time</summary>
        public int InitialCacheSize = 10;
        /// <summary> If this object is Growable, grow until what size? </summary>
        public int MaxSize = 10;
        /// <summary> C# string format use for names of CacheBehavior objects </summary>
        public string NamePatternFormat;
        /// <summary> Is Growable?</summary>
        public bool Growable = true;

        /// <summary> After this time try to destroy some inactive objects to reach InitialCacheSize. (0 means never) </summary>
        public float CleanInterval = 0;

        /// <summary> Group </summary>
        public CacheGroup Group { get; private set; }
        /// <summary> Unique id (same as CacheBehavior.CacheId ) </summary>
        public int CacheId { get; private set; }

        private static int _CacheIdGenerator = 1;
        private Queue<GameObject> _DeactiveObjects;
        private List<GameObject> _Objects;
        private int _NextIndex;
        private int _FreeIndex;
        private Skill.TimeWatch _CleanTW;

        /// <summary>
        /// Initialize and instantiate objects
        /// </summary>
        /// <param name="group"></param>
        public void Initialize(CacheGroup group)
        {
            this.Group = group;
            this.CacheId = _CacheIdGenerator++;
            SetCacheIdAndGroup(Prefab);
            if (InitialCacheSize < 1) InitialCacheSize = 1;
            _DeactiveObjects = new Queue<GameObject>(InitialCacheSize);
            _Objects = new List<GameObject>(InitialCacheSize);

            _FreeIndex = 0;
            // Instantiate the objects in the list and set them to be inactive
            for (_NextIndex = 0; _NextIndex < InitialCacheSize; _NextIndex++)
            {
                GameObject obj = MonoBehaviour.Instantiate(Prefab) as GameObject;
                obj.transform.parent = group.transform;
                if (string.IsNullOrEmpty(NamePatternFormat))
                    NamePatternFormat = obj.name + "_{0}";

                SetCacheIdAndGroup(obj);
                obj.SetActiveRecursively(false);
                obj.name = string.Format(NamePatternFormat, _NextIndex);
                _DeactiveObjects.Enqueue(obj);
                _Objects.Add(obj);
            }
        }

        private void SetCacheIdAndGroup(GameObject obj)
        {
            CacheBehavior cacheable = obj.GetComponent<CacheBehavior>();
            if (cacheable != null)
            {
                cacheable.CacheId = this.CacheId;
                cacheable.Group = Group;
            }
            else
                throw new Exception("The prefab you set for cache does not have CacheBehavior component : " + obj.name);
        }

        private GameObject GetFirstInactive()
        {
            for (int i = 0; i < _Objects.Count; i++)
            {
                GameObject item = _Objects[i];
                if (!item.active)
                    return item;
            }

            int index = _FreeIndex;
            _FreeIndex = (_FreeIndex + 1) % _Objects.Count;

            GameObject obj = _Objects[index];
            obj.SetActiveRecursively(false);
            return obj;
        }

        /// <summary>
        /// Get next available and deactive object to reuse
        /// </summary>
        /// <returns></returns>
        public GameObject Next()
        {
            if (_DeactiveObjects.Count > 0)
            {
                return _DeactiveObjects.Dequeue();
            }
            if (Growable)
            {
                GameObject obj = MonoBehaviour.Instantiate(Prefab) as GameObject;
                obj.transform.parent = Group.transform;
                obj.name = string.Format(NamePatternFormat, _NextIndex++);
                SetCacheIdAndGroup(obj);
                _Objects.Add(obj);
                return obj;
            }
            else
            {
                return GetFirstInactive();
            }
        }

        /// <summary>
        /// Add unused GameObject to free list
        /// </summary>
        /// <param name="objToFree"> unused GameObject </param>
        public void Free(GameObject objToFree)
        {
            objToFree.SetActiveRecursively(false);
            _DeactiveObjects.Enqueue(objToFree);
        }

        /// <summary>
        /// Destroy all instances
        /// </summary>
        public void Destroy()
        {
            foreach (var objectToDestroy in _Objects)
            {
                GameObject.Destroy(objectToDestroy);
            }
        }

        /// <summary>
        /// Clean
        /// </summary>
        public void Clean()
        {
            if (CleanInterval > 0)
            {
                if (_CleanTW.Enabled)
                {
                    if (_CleanTW.IsOver)
                    {
                        while (_DeactiveObjects.Count > 0)
                        {
                            if (_DeactiveObjects.Count <= InitialCacheSize || (MaxSize > InitialCacheSize && _DeactiveObjects.Count <= MaxSize)) break;
                            GameObject objectToDestroy = _DeactiveObjects.Dequeue();
                            if (objectToDestroy != null)
                            {
                                _Objects.Remove(objectToDestroy);
                                GameObject.Destroy(objectToDestroy);
                            }
                        }
                        _CleanTW.End();
                    }
                }
                else
                    _CleanTW.Begin(CleanInterval);
            }
        }
    }

}