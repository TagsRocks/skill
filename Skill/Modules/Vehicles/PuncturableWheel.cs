using UnityEngine;
using System.Collections;
using Skill.Managers;

namespace Skill.Modules.Vehicles
{
    /// <summary>
    /// This behaviour is useful for situations like when a static car placed besides of street and you want it's wheel be puncturable but without using RigidBodies.
    /// assign this components to each wheel and set valid reference to Chassis.
    /// </summary>
    [AddComponentMenu("Skill/Modules/Vehicles/PuncturableWheel")]
    public class PuncturableWheel : Health
    {
        /// <summary> Puncturable Chassis </summary>
        public PuncturableChassis Chassis;
        /// <summary> a prefab like 'wheel his' to spawn when wheel is punctured </summary>
        public GameObject PuncturePrefab;
        /// <summary> Rotation of PuncturePrefab relate to wheel </summary>
        public Vector3 PuncturePrefabDirection = new Vector3(0, 0, 1);
        /// <summary> delta height to apply wheel when wheel is punctured </summary>
        public float PunctureHeight = 0.2f;
        /// <summary> delta rotation to apply Chassis when wheel is punctured </summary>
        public Vector3 PunctureRotation = new Vector3(2, 0, 2);
        /// <summary> Type of hits that cause puncture </summary>
        public HitType PunctureHitType;

        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>
        protected override void OnHit(object sender, HitEventArgs args)
        {
            if ((args.Hit.Type & PunctureHitType) == 0) return;
            base.OnHit(sender, args);
        }

        /// <summary>
        /// When agent is dead
        /// </summary>        
        protected override void OnDie()
        {
            if (Chassis != null)
                Chassis.NotifyWheelPuncture(this);

            if (PuncturePrefab != null)
                CacheSpawner.Spawn(PuncturePrefab, transform.position, Quaternion.LookRotation(transform.TransformDirection(PuncturePrefabDirection)));

            Vector3 pos = transform.position;
            pos.y -= PunctureHeight;
            transform.position = pos;
            base.OnDie();
        }
    }

}