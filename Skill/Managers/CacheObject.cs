using System;
using UnityEngine;
using System.Collections.Generic;

namespace Skill.Managers
{
    [Serializable]
    public class CacheObject
    {
        public GameObject Prefab;
        public int InitialCacheSize = 10;
        public int MaxSize = 10;
        public string NamePatternFormat;
        public bool Growable = true;

        public float CleanInterval = 0;// after this time try to destroy some inactive objects to reach InitialCacheSize. (0 means never)

        public CacheGroup Group { get; private set; }
        public int CacheId { get; private set; }
        private static int _CacheIdGenerator = 1;

        private Queue<GameObject> _DeactiveObjects;
        private List<GameObject> _Objects;
        private int _NextIndex;
        private int _FreeIndex;
        private Skill.TimeWatch _CleanTW;


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
                throw new Exception("The prefab you set for cache does not have CacheBehavior component : " +  obj.name);
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
                            if (_DeactiveObjects.Count <= InitialCacheSize || (MaxSize > InitialCacheSize && _DeactiveObjects.Count <= MaxSize)) break;
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

}