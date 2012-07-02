using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Skill.Managers;

namespace Skill.Controllers
{
    public class Spawner : MonoBehaviour
    {
        // GameObject to spawn
        public GameObject SpawnObject;
        // where to spawn objects
        public Transform[] SpawnLocations;
        // If true, use CacheSpawner.Instance to create objects
        public bool UseCacheSpawner = false;
        // If true, the spawner will cycle through the spawn locations instead of spawning from a randomly chosen one
        public bool CycleSpawnLocations;
        // Delta time between spawns
        public float SpawnInterval = 1f;
        // The maximum number of agents alive at one time. If agents are destroyed, more will spawn to meet this number.
        public int AliveCount = 2;
        // The maximum number of agents to spawn.when number of spawned object reach this value the spawner will not spawn anymore 
        public int MaxSpawnCount = 1;
        // If true, agents that are totally removed (ie blown up) are respawned
        public bool RespawnDeadAgents;
        // Radius around spawn location to spawn agents.
        public float SpawnRadius = 0;
        // If true, only spawn agents if player can't see spawn point
        //public bool OnlySpawnHidden;
        // If true, controls whether we are actively spawning agents
        //public bool SpawnActivate = false;

        /// <summary> List of all alive spawned objects </summary>
        public List<GameObject> SpawnedObjects { get; private set; }

        private List<GameObject> _RespawnObjects;

        // Holds the last SpawnLocation index used
        private int _LastSpawnLocationIndex;
        private float _LastSpawnTime;
        private int _SpawnCount;
        private int _DeadCount;

        protected virtual bool CanSpawn { get { return true; } }

        public Spawner()
        {
            SpawnedObjects = new List<GameObject>();
            _RespawnObjects = new List<GameObject>();
            _LastSpawnLocationIndex = -1;
            _LastSpawnTime = -1000;
            _SpawnCount = 0;
            _DeadCount = 0;
        }

        // let inherited class modify spawned object
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
                if (UseCacheSpawner)
                    spawnedObj = CacheSpawner.Spawn(SpawnObject, position, rotation);
                else
                    spawnedObj = (GameObject)GameObject.Instantiate(SpawnObject, position, rotation);
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

        public virtual void Awake()
        {
            enabled = false;
        }

        public virtual void Update()
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
                enabled = false;
                DestroyRespawnedObjects();
            }
        }

        public virtual void OnDestroy()
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
            if (UseCacheSpawner)
                CacheSpawner.DestroyCache(SpawnedObj);
            else
                GameObject.Destroy(SpawnedObj);
        }

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

        public void OnDieSpawnedObject(GameObject spawnedObj)
        {
            if (SpawnedObjects.Contains(spawnedObj))
            {
                _DeadCount++;
            }
        }
    }
}
