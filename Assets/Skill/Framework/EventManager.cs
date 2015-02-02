using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{

    /// <summary>    
    /// This class designed to avoid using SendMessage. 
    /// </summary>    
    /// <remarks>
    /// i think SenMessage can be expensive because it must use reflection to know which behavior has requested method    
    /// so i use this way to call methods on MonoBehaviors. this needs more work to do
    /// besides you always know format of methods and what prameters to sent
    /// each game must defines it's own EventManager
    /// </remarks>
    /// <example> for example    
    /// <code>
    /// public class ThisGameEventManager : EventManager
    /// {
    ///     public delegate void OnHitHandler(UnityEngine.Collider other);   
    ///     public event OnHitHandler Hit;
    ///         
    ///     public void OnHit(UnityEngine.Collider other)
    ///     {
    ///         if (Hit != null)
    ///             Hit(other);
    ///     }
    /// }
    /// 
    /// public class PLayer : MonoBehaviour
    /// {    
    ///     void Awake()
    ///     {
    ///         HookEvents();
    ///     }    
    /// 
    ///     public void HookEvents()
    ///     {    
    ///         ThisGameEventManager eventManager = ThisGameEventManager.Get;ltThisGameEventManager;gt(this.gameObject);
    ///         if (eventManager != null)
    ///         {                
    ///             eventManager.Hit += OnHit;
    ///         }
    ///     }    
    ///         
    ///     private void OnHit(UnityEngine.Collider other)
    ///     {
    ///         // do something
    ///     }
    /// }    
    /// </code>
    /// 
    /// other behaviors could call OnHit method to notify interested behaviors
    /// </example>              
    public class EventManager : StaticBehaviour
    {
        
        /// <summary>
        /// Occurs when a ray or somthing Hit this GameObject
        /// </summary>
        public event HitEventHandler Hit;

        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
        public void RaiseHit(object sender, HitEventArgs args)
        {
            if (Hit != null)
                Hit(sender, args);
        }

        /// <summary>
        /// Occurs when this GameObject is dead
        /// </summary>
        public event EventHandler Die;

        /// <summary>
        /// Notify GameObject is dead
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>
        public void RaiseDie(object sender, EventArgs e)
        {
            if (Die != null)
                Die(sender, e);
        }

        /// <summary>
        /// Occurs when some damage imposed
        /// </summary>
        public event DamageEventHandler Damage;

        /// <summary>
        /// Handle imposed damage
        /// </summary>    
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"> An DamageEventArgs that contains damage event data.</param>
        public void RaiseDamage(object sender, DamageEventArgs args)
        {
            if (Damage != null)
                Damage(sender, args);
        }

        /// <summary>
        /// Occurs when some object cached
        /// </summary>
        public event Managers.CacheEventHandler Cached;

        /// <summary>
        ///  Occurs when some object cached (called by Managers.Cache)
        /// </summary>    
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"> An Managers.CacheEventArgs that contains cache event data.</param>
        public void RaiseCached(object sender, Managers.CacheEventArgs args)
        {
            if (Cached != null)
                Cached(sender, args);
        }        
    }    

    /// <summary>
    /// Handle a ray or somthing Hit this GameObject
    /// </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="args"> damage args </param>
    public delegate void HitEventHandler(object sender, HitEventArgs args);


    /// <summary>
    /// Handle imposed damage
    /// </summary>    
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> damage args </param>
    public delegate void DamageEventHandler(object sender, DamageEventArgs args);

}
