using UnityEngine;
using System.Collections;
using Skill.Framework.Managers;

namespace Skill.Framework.Dynamics
{

    /// <summary>
    /// Defines base class for ExplosiveObjects. usually this object has a Health and an exploded mesh. When Health dies, explodedMesh will be visible and an explosion effect spawned
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class ExplosiveObject : Explosive
    {
        /// <summary> Active after explosion </summary>
        public GameObject[] ExplodedObjects;
        /// <summary> to spread around after explosion </summary>
        public GameObject[] SpreadObjects;
        /// <summary> number of spread to select in random from SpreadObjects </summary>
        public int SpreadCount = 10;
        /// <summary> offset of spread objetcs </summary>
        public float SpreadOffset = 0.2f;
        /// <summary> how many far to spread objects </summary>
        public float ExplosionRadius = 0.3f;
        /// <summary> Froce to apply on spread object's Rigidbody </summary>
        public float ExplosionForce = 20.0f;
        /// <summary> upwardsModifier to use in Rigidbody.AddExplosionForce method </summary>
        public float UpwardsModifier = 3.0f;
        /// <summary> instantiate when health reach FireHealthThreshold </summary>
        public Transform[] FirePositions;
        /// <summary> instantiate when health reach FireHealthThreshold </summary>
        public GameObject FirePrefab;
        /// <summary> when health reach lower than this negative RegenerateSpeed begins </summary>
        public float FireHealthThreshold = 50f;
        /// <summary> speed of self-destruction when fire starts </summary>
        public float FireDamageSpeed = 5;

        /// <summary> Health </summary>
        public Health Health { get; private set; }

        private GameObject[] _DamageFireInstances;
        private bool _ReachThreshold;

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            Health = GetComponent<Health>();
            if (Health == null)
                Debug.LogWarning("ExplosiveObject should have Health component in same GameObject to work correctlly");
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (Health != null)
            {
                Health.RegenerateSpeed = 0;
                _ReachThreshold = false;
                Health.HealthChange += Health_HealthChange;
            }

            if (FirePositions == null)
                FirePositions = new Transform[] { _Transform };

            if (ExplodedObjects != null)
            {
                foreach (var item in ExplodedObjects)
                {
                    if (item != null)
                        item.SetActive(false);
                }
            }
        }

        void Health_HealthChange(Object sender, HealthChangeEventArgs args)
        {
            // check if health of object is lower than FireHealthThreshold then start fire and enable selfdamage
            if (!_ReachThreshold && Health.CurrentHealth <= FireHealthThreshold)
            {
                _ReachThreshold = true;
                if (FireDamageSpeed > 0)
                    Health.RegenerateSpeed = -FireDamageSpeed;
                else
                    Health.RegenerateSpeed = FireDamageSpeed;
                Health.enabled = true;
                if (FirePrefab != null)
                {
                    _DamageFireInstances = new GameObject[FirePositions.Length];
                    for (int i = 0; i < FirePositions.Length; i++)
                    {
                        _DamageFireInstances[i] = Cache.Spawn(FirePrefab, FirePositions[i].position, FirePositions[i].rotation);
                    }

                }
            }
        }

        /// <summary>
        /// The GameObject dies and explosion happened
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>   
        protected override void Events_Die(object sender, System.EventArgs e)
        {
            base.Events_Die(sender, e);

            // if there are spread objects then spread some objects
            if (SpreadObjects != null && SpreadObjects.Length > 0)
            {
                for (int i = 0; i < SpreadCount; i++)
                {
                    int rnd = Random.Range(0, SpreadObjects.Length);
                    SpawnSpreadObject(SpreadObjects[rnd]);
                }
            }

            // the object is exploded so destroy spawned fire
            if (_DamageFireInstances != null)
            {
                for (int i = 0; i < _DamageFireInstances.Length; i++)
                {
                    if (_DamageFireInstances[i] != null)
                        Cache.DestroyCache(_DamageFireInstances[i]);
                }
            }

            if (ExplodedObjects != null)
            {
                foreach (var item in ExplodedObjects)
                {
                    if (item != null && !item.activeSelf)
                        item.SetActive(true);
                }
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            // activate ExplodedObjects
            if (ExplodedObjects != null)
            {
                foreach (var item in ExplodedObjects)
                {
                    if (item != null && !item.activeSelf)
                        item.SetActive(true);
                }
            }
            base.OnDestroy();
        }

        /// <summary>
        /// Subclass can override this method to spawn SpreadObjects another way
        /// </summary>        
        /// <param name="prefab"> SpreadObject to spawn from </param>        
        protected virtual void SpawnSpreadObject(GameObject prefab)
        {
            Vector3 pos = _Transform.position + SpreadOffset * Random.onUnitSphere;
            GameObject so = Cache.Spawn(prefab, pos, prefab.transform.rotation);
            if (so.rigidbody != null)
                so.rigidbody.AddExplosionForce(ExplosionForce, _Transform.position, ExplosionRadius, UpwardsModifier);
        }
    }

}