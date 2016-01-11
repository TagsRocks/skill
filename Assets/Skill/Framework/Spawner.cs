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
    public class Spawner : DynamicBehaviour
    {
        /// <summary> Feed to spawn </summary>
        public SpawnerFeed Feed;
        /// <summary> Delta time between spawns </summary>
        public float Interval = 1f;
        /// <summary> The maximum number of agents alive at one time. If agents are destroyed, more will spawn to meet this number. </summary>
        public int MaxAlive = 2;
        /// <summary> The maximum number of agents to spawn.when number of spawned object reach this value the spawner will not spawn anymore  </summary>
        public int SpawnCount = 1;
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


        private TimeWatch _SpawnTW;
        private int _SpawnCounter;
        private int _DeadCounter;
        private float _TotalWeight;

        private List<GameObject> _AliveObjects;
        private List<GameObject> _DeadObjects;

        /// <summary>
        /// Subclasses can avoid spawning for some reason at specific times
        /// </summary>
        protected virtual bool CanSpawn { get { return true; } }

        /// <summary>
        /// let inherited class modify spawned object right after spawn time
        /// </summary>
        /// <param name="spawnedObj">Spawned Object</param>
        protected virtual void InitializeSpawnedObject(GameObject spawnedObj, object userData) { }

        /// <summary> is spawner spawned all objects? </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Occurs when spawner spawned all objects
        /// </summary>
        public event EventHandler Complete;
        /// <summary>
        /// when spawner spawned all objects
        /// </summary>
        protected virtual void OnComplete()
        {
            IsCompleted = true;
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
            _AliveObjects = new List<GameObject>();
            _DeadObjects = new List<GameObject>();
            _SpawnCounter = 0;
            _DeadCounter = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // reset spawner
            _AliveObjects.Clear();
            _DeadObjects.Clear();
            _SpawnCounter = 0;
            _DeadCounter = 0;
            IsCompleted = false;
        }

        /// <summary>
        /// Spawn new game objects
        /// </summary>
        /// <returns>True if success, otherwise false</returns>
        public bool Spawn()
        {
            if (!CanSpawn || Feed == null) return false;

            var sp = Feed.GetNextSpawnObjectFor(this);
            if (sp == null)
            {
                Debug.LogError("invalid object to spawn(No SpawnObject exist).");
                return false;
            }

            if (OnlySpawnHidden && Camera.main != null)
            {
                if (Utility.IsVisible(sp.Position))
                    return false;
            }



            GameObject spawnedObj = Cache.Spawn(sp.Prefab, sp.Position, sp.Rotation);
            _AliveObjects.Add(spawnedObj);
            _DeadObjects.Remove(spawnedObj);

            Controller controller = spawnedObj.GetComponent<Controller>();
            if (controller != null)
                controller.Spawner = this;

            _SpawnTW.Begin(Interval);
            _SpawnCounter++;
            InitializeSpawnedObject(spawnedObj, sp.UserData);

            if (_SpawnCounter >= SpawnCount)
            {
                OnComplete();
            }
            return true;
        }





        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if ((_SpawnCounter < SpawnCount) && _SpawnTW.IsOver)
            {
                _SpawnTW.Begin(Interval);
                if (CanSpawn && _AliveObjects.Count < MaxAlive)
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
