using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Managers
{
    [Serializable]
    public class ObjectCache
    {
        public GameObject Prefab;
        public int InitialCacheSize = 10;
        public string NamePatternFormat;
        public bool Growable = true;
        public float CleanInterval = 0;// after this time try to destroy some inactive objects to reach InitialCacheSize. (0 means never)

        public int CacheId { get; private set; }
        private static int _CacheIdGenerator = 1;

        private Queue<GameObject> _DeactiveObjects;
        private List<GameObject> _Objects;
        private int _NextIndex;
        private int _FreeIndex;
        private Skill.TimeWatch _CleanTW;


        public void Initialize()
        {
            this.CacheId = _CacheIdGenerator++;
            SetCacheId(Prefab);
            if (InitialCacheSize < 1) InitialCacheSize = 1;
            _DeactiveObjects = new Queue<GameObject>(InitialCacheSize);
            _Objects = new List<GameObject>(InitialCacheSize);

            _FreeIndex = 0;
            // Instantiate the objects in the list and set them to be inactive
            for (_NextIndex = 0; _NextIndex < InitialCacheSize; _NextIndex++)
            {
                GameObject obj = MonoBehaviour.Instantiate(Prefab) as GameObject;
                obj.transform.parent = CacheSpawner.Parent; 
                if (string.IsNullOrEmpty(NamePatternFormat))
                    NamePatternFormat = obj.name + "_{0}";

                SetCacheId(obj);
                obj.SetActiveRecursively(false);
                obj.name = string.Format(NamePatternFormat, _NextIndex);
                _DeactiveObjects.Enqueue(obj);
                _Objects.Add(obj);
            }
        }

        private void SetCacheId(GameObject obj)
        {
            CacheBehavior cacheable = obj.GetComponent<CacheBehavior>();
            if (cacheable != null)
                cacheable.CacheId = this.CacheId;
            else
                throw new Exception("The prefab you set for cache does not have CacheBehavior component");
        }

        private GameObject GetFirstInactive()
        {
            foreach (var item in _Objects)
            {
                if (!item.active)
                    return item;
            }

            int index = _FreeIndex;
            _FreeIndex = (_FreeIndex + 1) % _Objects.Count;

            GameObject obj = _Objects[index];
            obj.SetActiveRecursively(false);
            return obj;
        }

        public GameObject Next()
        {
            if (_DeactiveObjects.Count > 0)
            {
                return _DeactiveObjects.Dequeue();
            }
            if (Growable)
            {
                GameObject obj = MonoBehaviour.Instantiate(Prefab) as GameObject;
                obj.transform.parent = CacheSpawner.Parent; 
                obj.name = string.Format(NamePatternFormat, _NextIndex++);
                SetCacheId(obj);
                _Objects.Add(obj);
                return obj;
            }
            else
            {
                return GetFirstInactive();
            }
        }

        public void Free(GameObject objToFree)
        {
            objToFree.SetActiveRecursively(false);
            _DeactiveObjects.Enqueue(objToFree);
        }

        public void Destroy()
        {
            foreach (var objectToDestroy in _Objects)
            {
                GameObject.Destroy(objectToDestroy);
            }
        }

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
                            if (_DeactiveObjects.Count <= InitialCacheSize) break;
                            GameObject objectToDestroy = _DeactiveObjects.Dequeue();
                            _Objects.Remove(objectToDestroy);
                            GameObject.Destroy(objectToDestroy);
                        }
                        _CleanTW.End();
                    }
                }
                else
                    _CleanTW.Begin(CleanInterval);
            }
        }
    }

    [AddComponentMenu("Skill/Managers/CacheSpawner")]
    public class CacheSpawner : MonoBehaviour
    {
        public float CleanInterval = 20;
        public Transform ParentOfCache; // all gameobjects will be child of this to avoid mess in "Hierarchy" window in editor
        public ObjectCache[] Caches;

        private static CacheSpawner _Instance = null;
        private Skill.TimeWatch _CleanTW;



        void Awake()
        {
            _Instance = this;
            // Loop through the caches
            for (var i = 0; i < Caches.Length; i++)
            {
                // Initialize each cache
                Caches[i].Initialize();
            }
        }

        /// <summary>
        /// all cached GameObjects will be child of this to avoid mess in "Hierarchy" window in editor
        /// </summary>
        public static Transform Parent { get { return _Instance.ParentOfCache; } }


        private static ObjectCache GetObjectCache(GameObject prefab)
        {
            if (_Instance != null)
            {
                CacheBehavior cacheBehavior = prefab.GetComponent<CacheBehavior>();
                if (cacheBehavior != null)
                {
                    foreach (var item in _Instance.Caches)
                    {
                        if (item.CacheId == cacheBehavior.CacheId)
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, bool enabled = true)
        {
            ObjectCache cache = GetObjectCache(prefab);
            // If there's no cache for this prefab type, just instantiate normally
            if (cache == null)
            {
                return GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }

            // Find the next object in the cache
            GameObject obj = cache.Next();

            try
            {
                // Set the position and rotation of the object
                obj.transform.position = position;
            }
            catch (MissingReferenceException)
            {
                Debug.Log("prefab.name : " + prefab.name + " catchId : " + cache.CacheId);
                throw;
            }

            obj.transform.rotation = rotation;

            // Set the object to be active
            obj.SetActiveRecursively(enabled);
            return obj;
        }

        public static void DestroyCache(GameObject objectToDestroy)
        {
            ObjectCache cache = GetObjectCache(objectToDestroy);
            if (cache != null)
            {
                cache.Free(objectToDestroy);
            }
            else
            {
                GameObject.Destroy(objectToDestroy);
            }
        }

        public void OnDestroy()
        {
            foreach (var item in Caches)
            {
                item.Destroy();
            }
        }

        public void Update()
        {
            if (CleanInterval > 0)
            {
                if (_CleanTW.Enabled)
                {
                    if (_CleanTW.IsOver)
                    {
                        foreach (var item in Caches)
                        {
                            item.Clean();
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