using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Managers
{

    [AddComponentMenu("Skill/Managers/CacheSpawner")]
    public class CacheSpawner : MonoBehaviour
    {
        public CacheGroup[] Groups;

        private static CacheSpawner _Instance = null;


        void Awake()
        {
            if (_Instance != null)
                Debug.LogError("More that one CacheSpawner found in scene");
            _Instance = this;
        }

        private static CacheObject GetCacheObject(GameObject prefab)
        {
            try
            {
                CacheBehavior cacheBehavior = prefab.GetComponent<CacheBehavior>();
                if (cacheBehavior != null)
                {
                    if (cacheBehavior.Group != null)
                    {
                        foreach (var item in cacheBehavior.Group.Caches)
                        {
                            if (item.CacheId == cacheBehavior.CacheId)
                            {
                                return item;
                            }
                        }
                    }
                }
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning(prefab.name);
                throw;
            }
            return null;
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, bool enabled = true)
        {
            CacheObject cache = GetCacheObject(prefab);
            // If there's no cache for this prefab type, just instantiate normally
            if (cache == null)
            {
                return GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }

            // Find the next object in the cache
            GameObject obj = cache.Next();
            CacheBehavior cacheBehavior = obj.GetComponent<CacheBehavior>();
            cacheBehavior.IsCollected = false;

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
            if (objectToDestroy == null) return; // maybe engine destroy it before
            CacheObject cache = GetCacheObject(objectToDestroy);
            if (cache != null)
            {
                CacheBehavior cacheBehavior = objectToDestroy.GetComponent<CacheBehavior>();
                if (!cacheBehavior.IsCollected)
                {
                    cacheBehavior.IsCollected = true;
                    cache.Free(objectToDestroy);
                }
            }
            else
            {
                GameObject.Destroy(objectToDestroy);
            }
        }

        void Start()
        {
            enabled = false;
        }
        void Update()
        {
            enabled = false;
        }
    }
}