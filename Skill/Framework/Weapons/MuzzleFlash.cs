using UnityEngine;
using System.Collections;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A muzzle flash in front of weapon at fire time.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/MuzzleFlash")]
    public class MuzzleFlash : DynamicBehaviour
    {
        /// <summary> Weapon that own's this muzzle flash </summary>
        public Weapon Weapon;
        /// <summary> Time of muzzle flash </summary>
        public float LifeTime = 0.2f;
        /// <summary> Minmum value of random scale </summary>
        public float MinScale = 0.9f;
        /// <summary> Maxmum value of random scale </summary>
        public float MaxScale = 1.1f;

        


        private TimeWatch _LifeTimeTW;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            _LifeTimeTW.Begin(LifeTime);
            float randomScale = Random.Range(MinScale, MaxScale);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.IsEnabledAndOver)
            {
                gameObject.SetActive(false);
            }
            base.Update();
        }

        /// <summary>
        /// Hoot required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if(Weapon != null) Weapon.Shoot += Weapon_Shoot;
        }

        /// <summary>
        /// Unhoot hooked events
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (Weapon != null) Weapon.Shoot -= Weapon_Shoot;
        }

        void Weapon_Shoot(object sender, System.EventArgs e)
        {
            gameObject.SetActive(true);
        }
    }

}