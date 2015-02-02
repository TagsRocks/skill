using UnityEngine;
using System.Collections;
using Skill.Framework.Managers;

namespace Skill.Framework.Dynamics
{
    /// <summary>
    /// Base class for explosive objects. it explode OnDie.
    /// </summary>    
    public class Explosive : DynamicBehaviour
    {
        /// <summary> The GameObject to spawn on explosion </summary>
        public GameObject ExpPrefab;
        /// <summary> Positions to spawn ExpPrefab </summary>
        public Transform[] ExpPositions;
        /// <summary> use rotation of ExpPosition for ExpPrefab </summary>
        public bool OrientToExpPosition = false;
        /// <summary> Amount of self destruction delay after explosion (set it to negative to disable self destruction)</summary>
        public float DestroyDelay = 0;
        /// <summary> Shake camera on explosion </summary>
        public CameraShakeParams Shake;

        private TimeWatch _DestroyTW;

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
            {
                Events.Die += Events_Die;
            }
        }

        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (Events != null)
            {
                Events.Die -= Events_Die;
            }
        }


        /// <summary>
        /// The GameObject dies and explosion happened
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>        
        protected virtual void Events_Die(object sender, System.EventArgs e)
        {
            Skill.Framework.Global.RaiseCameraShake(this, Shake, transform.position);

            if (ExpPrefab != null) // spawn explosion prefab
            {
                foreach (var item in ExpPositions)
                    SpawnExplosionPrefab(item);
            }
            if (DestroyDelay >= 0)
            {
                _DestroyTW.Begin(DestroyDelay);
                enabled = true;
            }
            else
                enabled = false;
        }

        /// <summary>
        /// Subclass can override this method to spawn ExplosionPrefab another way
        /// </summary>
        /// <param name="position">Where to spawn</param>
        protected virtual void SpawnExplosionPrefab(Transform position)
        {
            Cache.Spawn(ExpPrefab, position.position, OrientToExpPosition ? position.rotation : ExpPrefab.transform.rotation);
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (ExpPositions == null || ExpPositions.Length == 0)
                ExpPositions = new Transform[] { _Transform };
            enabled = false;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (Global.Instance != null)
                Global.Instance.Initialize(this);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_DestroyTW.IsEnabledAndOver)
            {
                _DestroyTW.End();
                enabled = false;
                Cache.DestroyCache(gameObject);
            }
            base.Update();
        }
    }

}