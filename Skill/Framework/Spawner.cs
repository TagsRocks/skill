using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.Managers;

namespace Skill.Framework
{
    /// <summary>
    /// Use this class to spawn object in scheduled time and with triggers
    /// </summary>
    [AddComponentMenu("Skill/Base/Spawner")]
    public class Spawner : DynamicBehaviour
    {
        /// <summary> GameObject to spawn </summary>
        public SpawnAsset SpawnObjects;
        /// <summary>
        /// A queue to place spawned objects.
        /// it is possible to sync some spawners togather with same queue
        /// </summary>
        public SpawnQueue Queue;
        /// <summary> where to spawn objects </summary>
        public Transform[] Locations;
        /// <summary> If true, the spawner will cycle through the spawn locations instead of spawning from a randomly chosen one </summary>
        public bool CycleLocations;
        /// <summary> Delta time between spawns </summary>
        public float Interval = 1f;
        /// <summary> The maximum number of agents alive at one time. If agents are destroyed, more will spawn to meet this number. </summary>
        public int AliveCount = 2;
        /// <summary> The maximum number of agents to spawn.when number of spawned object reach this value the spawner will not spawn anymore  </summary>
        public int SpawnCount = 1;
        /// <summary> Radius around spawn location to spawn agents. </summary>
        public float SpawnRadius = 0;
        /// <summary> If true, only spawn agents if player can't see spawn point </summary>
        public bool OnlySpawnHidden;
        /// <summary> If true, wait Interval after one spawned gameobject is dead </summary>
        public bool DeadInterval = false;

        /// <summary> List of all alive spawned objects </summary>
        public IEnumerable<GameObject> AliveObjects { get { return _AliveObjects; } }
        /// <summary> List of all alive spawned objects </summary>
        public IEnumerable<GameObject> DeadObjects { get { return _DeadObjects; } }
        /// <summary> Number of alive objects </summary>
        public int NumberOfAliveObjects { get { return _AliveObjects.Count; } }
        /// <summary> Number of dead objects </summary>
        public int NumberOfDeadObjects { get { return _DeadObjects.Count; } }

        // Holds the last SpawnLocation index used
        private int _LastSpawnLocationIndex;
        private TimeWatch _SpawnTW;
        private int _SpawnCounter;
        private int _DeadCounter;
        private float _TotalWeight;

        private SpawnObject[] _SpawnObjects;
        private List<GameObject> _AliveObjects;
        private List<GameObject> _DeadObjects;

        /// <summary>
        /// Call this method if you change SpawnObjects after the Spawner started.
        /// </summary>
        public void RecalculateWeights()
        {
            _TotalWeight = 0;
            if (SpawnObjects != null && SpawnObjects.Objects != null)
            {
                foreach (SpawnObject item in SpawnObjects.Objects)
                {
                    if (item != null)
                    {
                        if (item.Chance < 0.05f) item.Chance = 0.05f;
                        _TotalWeight += item.Chance;
                    }
                }
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
        /// Occurs when spawner complete and all spawned objects dead
        /// </summary>
        public event EventHandler AllDead;
        /// <summary>
        /// when spawner complete and all spawned objects dead
        /// </summary>
        protected virtual void OnAllDead()
        {
            if (AllDead != null) AllDead(this, EventArgs.Empty);
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _AliveObjects = new List<GameObject>(Mathf.Max(1, SpawnCount / 2));
            _DeadObjects = new List<GameObject>(Mathf.Max(1, SpawnCount / 2));
            _LastSpawnLocationIndex = -1;
            _SpawnCounter = 0;
            _DeadCounter = 0;
        }

        /// <summary>
        /// Select a random GameObject from SpawnObjects by chance
        /// subclass can change this behavior
        /// </summary>
        /// <returns>New GameObject from SpawnObjects to instantiate from</returns>
        protected virtual GameObject GetNextSpawnObject()
        {
            if (SpawnObjects == null) return null;
            if (SpawnObjects.Objects != null)
            {
                if (_SpawnObjects != SpawnObjects.Objects)
                {
                    RecalculateWeights();
                    _SpawnObjects = SpawnObjects.Objects;
                }
            }
            float rnd = UnityEngine.Random.Range(0.0f, _TotalWeight);
            float sum = 0;
            foreach (SpawnObject item in SpawnObjects.Objects)
            {
                sum += item.Chance;
                if (sum >= rnd) return item.Prefab;
            }
            return null;
        }

        /// <summary>
        /// Spawn new game objects
        /// </summary>
        /// <returns>True if success, otherwise false</returns>
        public bool Spawn()
        {
            if (!CanSpawn) return false;
            Vector3 position;
            Quaternion rotation;
            GetNextLocation(out position, out rotation);

            if (OnlySpawnHidden && Camera.main != null)
            {
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);
                bool isHidden = screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height;
                if (!isHidden)
                    return false;
            }

            var sp = GetNextSpawnObject();
            if (sp == null)
            {
                Debug.LogError("invalid object to spawn(No SpawnObject exist).");
                return false;
            }
            GameObject spawnedObj = Cache.Spawn(sp, position, rotation, Queue == null);
            _AliveObjects.Add(spawnedObj);
            _DeadObjects.Remove(spawnedObj);

            Controller controller = spawnedObj.GetComponent<Controller>();
            if (controller != null)
                controller.Spawner = this;

            if (Queue != null)
                Queue.Enqueue(spawnedObj);

            _SpawnTW.Begin(Interval);
            _SpawnCounter++;
            InitializeSpawnedObject(spawnedObj);

            if (_SpawnCounter >= SpawnCount)
            {
                OnComplete();
            }
            return true;
        }

        /// <summary>
        /// Get next location to spawn object
        /// </summary>
        /// <param name="position">Position of spawn object</param>
        /// <param name="rotation">Rotation of spawn object</param>
        protected virtual void GetNextLocation(out Vector3 position, out Quaternion rotation)
        {
            if (Locations != null && Locations.Length > 0) // get position and rotation from one of spawne locations
            {
                if (CycleLocations) // move to next spawn location
                    _LastSpawnLocationIndex = (_LastSpawnLocationIndex + 1) % Locations.Length;
                else // pick random spawn location
                    _LastSpawnLocationIndex = UnityEngine.Random.Range(0, Locations.Length);

                Transform location = Locations[_LastSpawnLocationIndex];
                rotation = location.rotation;
                position = location.position;
            }
            else
            {
                rotation = transform.rotation;
                position = transform.position;
            }

            if (SpawnRadius > 0)
            {
                Vector3 rndPosition = new Vector3();
                rndPosition.x = UnityEngine.Random.Range(0, SpawnRadius);
                position += rotation * rndPosition;
            }
        }



        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if ((_SpawnCounter < SpawnCount) && _SpawnTW.IsOver)
            {
                _SpawnTW.End();
                if (CanSpawn && _AliveObjects.Count < AliveCount)
                {
                    Spawn();
                }
            }
            base.Update();
        }

        /// <summary>
        /// when spawner destroyed (all spawned objects will destroyed too)
        /// </summary>
        protected override void OnDestroy()
        {
            _AliveObjects.Clear();
            _DeadObjects.Clear();
            base.OnDestroy();
        }

        /// <summary>
        /// Notify spawner that given object is dead but not destroyed yet ( the dead body is still visible )
        /// </summary>
        /// <param name="deadSpawnedObj">dead spawned object</param>
        public void NotifySpawnedObjectIsDead(GameObject deadSpawnedObj)
        {
            int index = _AliveObjects.IndexOf(deadSpawnedObj);
            if (index >= 0)
            {
                _AliveObjects.RemoveAt(index);
                if (!_DeadObjects.Contains(deadSpawnedObj))
                    _DeadObjects.Add(deadSpawnedObj);

                _DeadCounter++;
                if (_DeadCounter == SpawnCount)
                    OnAllDead();

                if (DeadInterval)
                    _SpawnTW.Begin(Interval);

                SpawnedObjectIsDead(deadSpawnedObj);
            }
        }

        protected virtual void SpawnedObjectIsDead(GameObject deadSpawnedObj)
        {
        }

        /// <summary>
        /// Destroy all alive objects
        /// </summary>
        public void KillAllAlives()
        {
            foreach (var obj in _AliveObjects)
            {
                Cache.DestroyCache(obj);
            }
        }
    }

}
