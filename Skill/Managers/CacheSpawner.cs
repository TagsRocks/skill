using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Managers
{
    /// <summary>
    /// Manage spawning cache objects
    /// </summary>
    /// <remarks>
    /// Only one instance of this object should be in scene
    /// </remarks>
    [AddComponentMenu("Skill/Managers/CacheSpawner")]
    public class CacheSpawner : MonoBehaviour
    {
        /// <summary> Groups to manage </summary>
        public CacheGroup[] Groups;

        private static CacheSpawner _Instance = null;

        /// <summary>
        /// Awake
        /// </summary>
        protected void Awake()
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

        /// <summary>
        /// Spawn a cache object
        /// </summary>
        /// <param name="prefab">GameObject with CacheBehavior component</param>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="enabled">Enable at spawn time or let caller enable it itself</param>
        /// <returns>Spawned GameObject</returns>
        /// <remarks>
        /// If GameObject has not a CacheBehavior component, spawner instantiate it normally ( by GameObject.Instantiate method )
        /// </remarks>
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

        /// <summary>
        /// Destroy cache GameObject and add to free list
        /// </summary>
        /// <param name="objectToDestroy">GameObject with CacheBehavior component</param>
        ///  /// <remarks>
        /// If GameObject has not a CacheBehavior component, spawner destroy it normally ( by GameObject.Destroy method )
        /// </remarks>
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
    }
}