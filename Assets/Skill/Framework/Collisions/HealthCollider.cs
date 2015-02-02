using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.Collisions
{
    /// <summary>
    /// when a Prefab is made of more than one collider, but you want use single Health for entire Prefab assign this behaiour to each collider
    /// and set reference to main health
    /// </summary>
    [RequireComponent(typeof(EventManager))]    
    public class HealthCollider : StaticBehaviour
    {
        /// <summary> Main Health</summary>
        public EventManager Health;
        /// <summary> Notify main Health about incomming hits</summary>
        public bool Hit = true;
        /// <summary> Notify main Health about incomming damages</summary>
        public bool Damage = false;

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
            {
                Events.Hit += Events_Hit;
                Events.Damage += Events_Damage;
            }
            else
                Debug.LogWarning("Miss EventManager behaviour for HealthCollider");
        }

        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (Events != null)
            {
                Events.Hit -= Events_Hit;
                Events.Damage -= Events_Damage;
            }
        }

        /// <summary>
        /// Handle imposed damage
        /// </summary>    
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"> An DamageEventArgs that contains damage event data.</param>
        protected virtual void Events_Damage(object sender, DamageEventArgs args)
        {
            if (Damage && Health != null)
                Health.RaiseDamage(sender, args);
        }



        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
        protected virtual void Events_Hit(object sender, HitEventArgs args)
        {
            if (Hit && Health != null)
                Health.RaiseHit(sender, args);
        }
    }
}