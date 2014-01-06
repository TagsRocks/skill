using System;
using UnityEngine;
using System.Collections.Generic;

namespace Skill.Framework.Managers
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
        public int CacheSize = 10;
        /// <summary> Is Growable?</summary>
        public bool Growable = true;

        /// <summary> Group </summary>
        public CacheGroup Group { get; private set; }
        /// <summary> Unique id (same as CacheBehavior.CacheId ) </summary>
        public int CacheId { get; private set; }

        private static int _CacheIdGenerator = 1;
        private Queue<GameObject> _DeactiveObjects;
        private List<GameObject> _Objects;
        private int _NextIndex;
        private int _FreeIndex;

        /// <summary>
        /// Initialize and instantiate objects
        /// </summary>
        /// <param name="group">Group</param>
        internal void Initialize(CacheGroup group)
        {
            this.Group = group;
            this.CacheId = _CacheIdGenerator++;
            SetCacheIdAndGroup(Prefab);
            if (CacheSize < 1) CacheSize = 1;
            _DeactiveObjects = new Queue<GameObject>(CacheSize);
            _Objects = new List<GameObject>(CacheSize);

            _FreeIndex = 0;
            // Instantiate the objects in the list and set them to be inactive
            for (_NextIndex = 0; _NextIndex < CacheSize; _NextIndex++)
            {
                GameObject obj = MonoBehaviour.Instantiate(Prefab) as GameObject;
                if (group.MakeAsChild)
                    obj.transform.parent = group.transform;

                SetCacheIdAndGroup(obj);
                obj.SetActive(false);

                EventManager eventManager = obj.GetComponent<EventManager>();
                if (eventManager != null)
                    eventManager.RaiseCached(null, new CacheEventArgs(this.CacheId));

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
                if (!item.activeSelf)
                    return item;
            }

            int index = _FreeIndex;
            _FreeIndex = (_FreeIndex + 1) % _Objects.Count;

            GameObject obj = _Objects[index];
            obj.SetActive(false);
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
                if (Group.MakeAsChild)
                    obj.transform.parent = Group.transform;
                SetCacheIdAndGroup(obj);
                obj.SetActive(false);
                EventManager eventManager = obj.GetComponent<EventManager>();
                if (eventManager != null)
                    eventManager.RaiseCached(null, new CacheEventArgs(this.CacheId));
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
            objToFree.SetActive(false);
            _DeactiveObjects.Enqueue(objToFree);
        }

        /// <summary>
        /// Destroy all instances
        /// </summary>
        public void Destroy()
        {
            foreach (var objectToDestroy in _Objects)
            {
                if (objectToDestroy != null) // not destroyed by engine
                    GameObject.Destroy(objectToDestroy);
            }
        }

        /// <summary>
        /// Clean
        /// </summary>
        public void Clean()
        {
            while (_DeactiveObjects.Count > 0)
            {
                if (_DeactiveObjects.Count <= CacheSize) break;
                GameObject objectToDestroy = _DeactiveObjects.Dequeue();
                if (objectToDestroy != null)
                {
                    _Objects.Remove(objectToDestroy);
                    GameObject.Destroy(objectToDestroy);
                }
            }
        }
    }

}