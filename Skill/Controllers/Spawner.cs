using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Skill.Managers;

namespace Skill.Controllers
{
    /// <summary>
    /// Use this class to spawn object in scheduled time and with triggers
    /// </summary>
    [AddComponentMenu("Skill/Controllers/Spawner")]
    public class Spawner : MonoBehaviour
    {
        /// <summary> GameObject to spawn </summary>
        public GameObject SpawnObject;
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
        /// Spawn new game objects
        /// </summary>
        public void Spawn()
        {
            Spawn(null);
        }

        private void Spawn(GameObject spawnedObj)
        {
            if (!CanSpawn) return;
            Vector3 position;
            Quaternion rotation;
            GetNewLocation(out position, out rotation);

            if (spawnedObj == null)
            {
                spawnedObj = CacheSpawner.Spawn(SpawnObject, position, rotation);
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
            spawnedObj.SetActiveRecursively(true);
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
        protected virtual void Awake()
        {
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
        protected virtual void Update()
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
            if (_SpawnCount >= MaxSpawnCount && DisableAfterAll)
            {
                enabled = false;
                DestroyRespawnedObjects();
            }
        }

        /// <summary>
        /// when spawner destroyed (all spawned objects will destroyed too)
        /// </summary>
        protected virtual void OnDestroy()
        {
            DestroySpawnedObjects();
            DestroyRespawnedObjects();
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

        //public void OnDieSpawnedObject(GameObject spawnedObj)
        //{
        //    if (SpawnedObjects.Contains(spawnedObj))
        //    {
        //        _DeadCount++;
        //    }
        //}
    }
}
