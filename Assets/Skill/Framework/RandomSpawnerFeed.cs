using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.Managers;

namespace Skill.Framework
{

    /// <summary>
    /// required data to spawn an object
    /// </summary>
    public class SpawnData
    {
        /// <summary> prefab to instantiate from </summary>
        public GameObject Prefab;
        /// <summary> position of spawned object </summary>
        public Vector3 Position;
        /// <summary> rotation of spawned object </summary>
        public Quaternion Rotation;
        /// <summary> user data </summary>
        public object UserData;
    }

    /// <summary>
    /// Use this class to spawn object in scheduled time and with triggers
    /// </summary>    
    public abstract class SpawnerFeed : StaticBehaviour
    {
        /// <summary> return a prefab to spawn from </summary>        
        /// <param name="spawner">spawner that request this feed</param>
        /// <returns> return a prefab to spawn from </returns>
        public abstract SpawnData GetNextSpawnObjectFor(Spawner spawner);
    }

    /// <summary>
    /// Contains data needed to spawn an object
    /// </summary>
    [System.Serializable]
    public class SpawnObject
    {
        /// <summary> Name of element</summary>
        public string Name;

        /// <summary> GameObject to spawn </summary>
        public GameObject Prefab;

        /// <summary> Chance to spawn </summary>
        public float Weight = 1.0f;
    }

    public class RandomSpawnerFeed : SpawnerFeed
    {
        /// <summary> GameObject to spawn </summary>
        public SpawnObject[] Objects;
        /// <summary> locations to spawn </summary>
        public Transform[] Locations;
        /// <summary> If true, the spawner will cycle through the spawn locations instead of spawning from a randomly chosen one </summary>
        public bool CycleLocations;
        /// <summary> Radius around spawn location to spawn agents. </summary>
        public float SpawnRadius = 0;

        private float _TotalWeight;
        private float[] _Weights;
        private int _LastSpawnLocationIndex; // Holds the last SpawnLocation index used

        /// <summary>
        /// Call this method if you change SpawnObjects after the Spawner started.
        /// </summary>
        public void RecalculateWeights()
        {
            if (Objects != null)
            {
                _Weights = new float[Objects.Length];
                _TotalWeight = 0;

                for (int i = 0; i < Objects.Length; i++)
                {
                    if (Objects[i] != null)
                    {
                        if (Objects[i].Weight < 0.0f) throw new ArgumentOutOfRangeException("weights", "element weight must be greater than or equal to zero");
                        _TotalWeight += Objects[i].Weight;
                        _Weights[i] = Objects[i].Weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("SpawnObject is null", "SpawnObject is null");
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            RecalculateWeights();
            _LastSpawnLocationIndex = -1;
            if (Locations == null || Locations.Length == 0)
                Locations = new Transform[] { transform };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RecalculateWeights();
            _LastSpawnLocationIndex = -1;
        }

        /// <summary>
        /// Select a random GameObject from SpawnObjects by chance
        /// subclass can change this behavior
        /// </summary>
        /// <returns>New GameObject from SpawnObjects to instantiate from</returns>
        public override SpawnData GetNextSpawnObjectFor(Spawner spawner)
        {
            SpawnData data = new SpawnData();

            Vector3 position;
            Quaternion rotation;
            GetNextLocation(out position, out rotation);

            data.Position = position;
            data.Rotation = rotation;
            data.Prefab = Objects[Utility.RandomByChance(_Weights, _TotalWeight)].Prefab;

            return data;
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
    }
}
