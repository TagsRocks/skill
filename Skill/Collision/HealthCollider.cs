using UnityEngine;
using System.Collections;
using Skill;

/// <summary>
/// when a Prefab is made of more than one collider, but you want use single Health for entire Prefab assign this behaiour to each collider
/// and set reference to main health
/// </summary>
[RequireComponent(typeof(EventManager))]
[AddComponentMenu("Skill/Collision/HealthCollider")]
public class HealthCollider : StaticBehaviour
{
    /// <summary> Main Health</summary>
    public Health Health;
    /// <summary> Notify main Health about incomming hits</summary>
    public bool HitEnable = true;
    /// <summary> Notify main Health about incomming damages</summary>
    public bool DamageEnable = false;

    /// <summary>
    /// Hook required events
    /// </summary>
    protected override void HookEvents()
    {
        base.HookEvents();
        if (Events != null)
        {
            if (HitEnable)
                Events.Hit += OnHit;
            if (DamageEnable)
                Events.Damage += OnDamage;
        }
        else
            Debug.LogWarning("Miss EventManager behaviour for HealthCollider");
    }

    /// <summary>
    /// Handle imposed damage
    /// </summary>    
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> An DamageEventArgs that contains damage event data.</param>
    protected virtual void OnDamage(object sender, DamageEventArgs args)
    {
        Health.Events.OnDamage(sender, args);
    }



    /// <summary>
    /// Handle a ray or somthing Hit this GameObject
    /// </summary>
    /// <param name="sender"> sender </param>
    /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
    protected virtual void OnHit(object sender, HitEventArgs args)
    {
        Health.Events.OnHit(sender, args);
    }
}
