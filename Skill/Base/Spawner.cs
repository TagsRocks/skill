using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Skill.Managers;

namespace Skill
{
    /// <summary>
    /// Use this class to spawn object in scheduled time and with triggers
    /// </summary>
    [AddComponentMenu("Skill/Spawner")]
    public class Spawner : DynamicBehaviour
    {
        /// <summary> GameObject to spawn </summary>
        public SpawnAsset SpawnObjects;
        /// <summary> where to spawn objects </summary>
        public Transform[] SpawnLocations;
        /// <summary> If true, the spawner will cycle through the spawn locations instead of spawning from a randomly chosen one </summary>
        public bool CycleSpawnLocations;
        /// <summary> Delta time between spawns </summary>
        public float SpawnInterval = 1f;
        /// <summary> The maximum number of agents alive at one time. If agents are destroyed, more will spawn to meet this number. </summary>
        public int AliveCount = 2;
        /// <summary> The maximum number of agents to spawn.when number of spawned object reach this value the spawner will not spawn anymore  </summary>
        public int MaxSpawnCount = 1;
        /// <summary> If true, agents that are totally removed (ie blown up) are respawned </summary>
        public bool RespawnDeadAgents;
        /// <summary> Radius around spawn location to spawn agents. </summary>
        public float SpawnRadius = 0;
        /// <summary> Spawn On Awake </summary>
        public bool SpawnOnAwake = false;

        // If true, only spawn agents if player can't see spawn point
        //public bool OnlySpawnHidden;
        // If true, controls whether we are actively spawning agents
        //public bool SpawnActivate = false;

        /// <summary> Spawner will disabled when (number of spawned object) reach MaxSpawnCount, in other words, when spawned all objects. </summary>
        public bool DisableAfterAll { get; protected set; }

        /// <summary> List of all alive spawned objects </summary>
        public List<GameObject> SpawnedObjects { get; private set; }

        private List<GameObject> _RespawnObjects;
        // Holds the last SpawnLocation index used
        private int _LastSpawnLocationIndex;
        private float _LastSpawnTime;
        private int _SpawnCount;
        private int _DeadCount;
        private float _TotalWeight = 0;

        /// <summary>
        /// Call this method if you change SpawnObjects after the Spawner started.
        /// </summary>
        public void RecalculateWeights()
        {
            _TotalWeight = 0;
            foreach (SpawnObject item in SpawnObjects.Objects)
            {
                if (item.Chance < 0.05f) item.Chance = 0.05f;
                else if (item.Chance > 1.0f) item.Chance = 1.0f;
                _TotalWeight += item.Chance;
            }
        }

        /// <summary>
        /// Subclasses can avoid spawning for some reason at specific times
        /// </summary>
        protected virtual bool CanSpawn { get { return true; } }

        /// <summary>
        /// let inherited class modify spawned object right after spawn time
        /// </summary>
        /// <param name="spawnedObj">Spawned Object</param>
        protected virtual void InitializeSpawnedObject(GameObject spawnedObj) { }

        /// <summary>
        /// Occurs when spawner spawned all objects
        /// </summary>
        public event EventHandler Complete;
        /// <summary>
        /// when spawner spawned all objects
        /// </summary>
        protected virtual void OnComplete()
        {
            if (Complete != null) Complete(this, EventArgs.Empty);
        }

        /// <summary>
        /// Spawn new game objects
        /// </summary>
        public void Spawn()
        {
            Spawn(null);
        }

        private GameObject GetRandomSpawnObject()
        {
            if (_TotalWeight <= 0) RecalculateWeights();
            float rnd = UnityEngine.Random.Range(0.0f, _TotalWeight);
            float sum = 0;
            foreach (SpawnObject item in SpawnObjects.Objects)
            {
                sum += item.Chance;
                if (sum >= rnd) return item.Prefab;
            }
            return null;
        }


        private void Spawn(GameObject spawnedObj)
        {
            if (!CanSpawn) return;
            Vector3 position;
            Quaternion rotation;
            GetNewLocation(out position, out rotation);

            if (spawnedObj == null)
            {
                var sp = GetRandomSpawnObject();
                if (sp == null)
                {
                    Debug.LogError("invalid object to spawn(No SpawnObject exist).");
                    return;
                }
                spawnedObj = CacheSpawner.Spawn(sp, position, rotation);
                SpawnedObjects.Add(spawnedObj);
            }
            else
            {
                spawnedObj.transform.position = position;
                spawnedObj.transform.rotation = rotation;
            }
            Controller controller = spawnedObj.GetComponent<Controller>();
            if (controller != null)
                controller.Spawner = this;

            _LastSpawnTime = Time.time;
            InitializeSpawnedObject(spawnedObj);
            spawnedObj.SetActive(true);
            _SpawnCount++;
        }

        private void GetNewLocation(out Vector3 position, out Quaternion rotation)
        {
            if (SpawnLocations != null && SpawnLocations.Length > 0) // get position and rotation from one of spawne locations
            {
                if (CycleSpawnLocations) // move to next spawn location
                    _LastSpawnLocationIndex = (_LastSpawnLocationIndex + 1) % SpawnLocations.Length;
                else // pick random spawn location
                    _LastSpawnLocationIndex = UnityEngine.Random.Range(0, SpawnLocations.Length);

                Transform location = SpawnLocations[_LastSpawnLocationIndex];
                rotation = location.rotation;
                position = location.position;
            }
            else
            {
                rotation = transform.rotation;
                position = transform.position;
            }

            if (SpawnRadius != 0)
            {
                Vector3 rndPosition = new Vector3();
                rndPosition.x = UnityEngine.Random.Range(0, SpawnRadius);
                position = position + rotation * rndPosition;
            }
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            SpawnedObjects = new List<GameObject>();
            _RespawnObjects = new List<GameObject>();
            _LastSpawnLocationIndex = -1;
            _LastSpawnTime = -1000;
            _SpawnCount = 0;
            _DeadCount = 0;
            DisableAfterAll = true;
            enabled = SpawnOnAwake;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {            
            if ((_SpawnCount < MaxSpawnCount) &&
                (Time.time > SpawnInterval + _LastSpawnTime))
            {
                if (_RespawnObjects.Count > 0 && CanSpawn)
                {
                    int index = _RespawnObjects.Count - 1;
                    GameObject obj = _RespawnObjects[index];
                    _RespawnObjects.RemoveAt(index);
                    Spawn(obj);
                }
                else if (SpawnedObjects.Count < AliveCount)
                {
                    Spawn();
                }
                else if (_DeadCount > 0)
                {
                    _DeadCount--;
                    Spawn();
                }
            }
            if (_SpawnCount >= MaxSpawnCount)
            {
                if (DisableAfterAll)
                    enabled = false;
                OnComplete();
                DestroyRespawnedObjects();
            }

            base.Update();
        }

        /// <summary>
        /// when spawner destroyed (all spawned objects will destroyed too)
        /// </summary>
        protected override void OnDestroy()
        {
            DestroySpawnedObjects();
            DestroyRespawnedObjects();
            base.OnDestroy();
        }

        private void DestroySpawnedObjects()
        {
            foreach (var item in SpawnedObjects)
            {
                Destroy(item);
            }
            _RespawnObjects.Clear();
        }

        private void DestroyRespawnedObjects()
        {
            foreach (var item in _RespawnObjects)
            {
                Destroy(item);
            }
            _RespawnObjects.Clear();
        }

        private void Destroy(GameObject SpawnedObj)
        {
            if (SpawnedObj != null) // maybe engine destroy it before
                CacheSpawner.DestroyCache(SpawnedObj);
        }

        /// <summary>
        /// Destroy Spawned Object by this spawner
        /// </summary>
        /// <param name="spawnedObj">GameObject or destroy</param>
        public void DestroySpawnedObject(GameObject spawnedObj)
        {
            if (SpawnedObjects.Contains(spawnedObj))
            {
                SpawnedObjects.Remove(spawnedObj);
                if (RespawnDeadAgents)
                {
                    _RespawnObjects.Add(spawnedObj);
                }
                else
                {
                    Destroy(spawnedObj);
                }
            }
            else
                GameObject.Destroy(spawnedObj);
        }

        /// <summary>
        /// Notify spawner that given object is dead but not destroyed yet ( the dead body is still visible )
        /// </summary>
        /// <param name="deadSpawnedObj">dead spawned object</param>
        public void NotifySpawnedObjectIsDead(GameObject deadSpawnedObj)
        {
            if (SpawnedObjects.Contains(deadSpawnedObj))
            {
                _DeadCount++;
            }
        }
    }
}
