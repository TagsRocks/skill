﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.Managers
{

    /// <summary>
    /// containing cache event data.
    /// </summary>
    public class CacheEventArgs : EventArgs
    {
        /// <summary> Cache id </summary>
        public int CacheId { get; private set; }


        /// <summary>
        /// Create CacheEventArgs
        /// </summary>
        /// <param name="cacheId"> cache id </param>
        public CacheEventArgs(int cacheId)
        {
            this.CacheId = cacheId;
        }
    }

    /// <summary>
    /// Handle when GameObject is cached
    /// </summary>
    /// <param name="sender"> The source of the event. (null)</param>
    /// <param name="args"> A CacheEventArgs containing cache data </param>
    public delegate void CacheEventHandler(object sender, CacheEventArgs args);


    /// <summary>
    /// Manage spawning cache objects
    /// </summary>
    public static class Cache
    {
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
                Debug.LogWarning("MissingReference of " + prefab.name);
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
        /// <param name="active">Activate at spawn time or let caller activate it self</param>
        /// <returns>Spawned GameObject</returns>
        /// <remarks>
        /// If GameObject has not a CacheBehavior component, spawner instantiate it normally ( by GameObject.Instantiate method )
        /// </remarks>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, bool active = true)
        {
            if (prefab == null)
            {
                Debug.LogError("Invalid GameObject to spawn");
                return null;
            }

            GameObject obj = null;
            CacheObject cache = GetCacheObject(prefab);
            // If there's no cache for this prefab type, just instantiate normally
            if (cache == null)
            {
                obj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }
            else
            {
                // Find the next object in the cache
                obj = cache.Next();
                CacheBehavior cacheBehavior = obj.GetComponent<CacheBehavior>();
                cacheBehavior.IsCached = false;

                try
                {
                    // Set the position and rotation of the object
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                }
                catch (MissingReferenceException)
                {
                    Debug.LogWarning(string.Format("MissingReferenceException when spawn prefab.name : {0}, catchId : {1}.", prefab.name, cache.CacheId));
                    throw;
                }
            }            

            if (obj != null && active)
            {// Set the object to be active
                obj.SetActive(active);                
            }
            return obj;
        }

        /// <summary>
        /// Destroy cache GameObject and add to free list
        /// </summary>
        /// <param name="objectToDestroy">GameObject with CacheBehavior component</param>
        /// <remarks>
        /// If GameObject has not a CacheBehavior component, spawner destroy it normally ( by GameObject.Destroy method )
        /// </remarks>
        public static void DestroyCache(GameObject objectToDestroy)
        {
            if (objectToDestroy == null) return; // maybe engine destroy it before
            CacheObject cache = GetCacheObject(objectToDestroy);
            if (cache != null)
            {
                CacheBehavior cacheBehavior = objectToDestroy.GetComponent<CacheBehavior>();
                if (!cacheBehavior.IsCached)
                {
                    cacheBehavior.IsCached = true;
                    cache.Free(objectToDestroy);

                    EventManager eventManager = objectToDestroy.GetComponent<EventManager>();
                    if (eventManager != null)
                        eventManager.RaiseCached(null, new CacheEventArgs(cache.CacheId));
                }
            }
            else
            {
                GameObject.Destroy(objectToDestroy);
            }
        }
    }
}