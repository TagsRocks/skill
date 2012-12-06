using Skill.Managers;
using System;
using UnityEngine;

namespace Skill.Weapons
{
    /// <summary>
    /// Base class for weapons that shoot bullets in straight directrion.
    /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.
    /// </summary>
    public abstract class DirectWeapon : BaseWeapon
    {
        /// <summary> Retrieves direction of weapon to shoot bullets</summary>
        public Vector3 BulletDirection { get { return _BulletDirection; } }

        /// <summary> Retrieves rotation of bullets </summary>
        public Quaternion BulletRotation { get { return _BulletRotation; } }
               
        private Vector3 _BulletDirection;
        private Quaternion _BulletRotation;

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;
            Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);

            Vector3 dir;
            if (Target != null)
                dir = (Target.position - CurrentProjectile.SpawnPoint.position).normalized;
            else
                dir = _Transform.forward;

            _BulletRotation = Quaternion.LookRotation(spreadRot * dir);
            _BulletDirection = (_BulletRotation * Vector3.forward).normalized;

            base.Update();
        }        
    }
}
