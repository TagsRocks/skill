using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Managers
{

    /// <summary>    
    /// This class designed to avoid using SendMessage. 
    /// </summary>    
    /// <remarks>
    /// i think SenMessage can be expensive because it must use reflection to know which behavior has requested method
    /// so i use this way to call methods on MonoBehaviors. this needs more work to do
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
    /// public class PLayer : Skill.Controllers.Controller, Skill.Managers.IEventManagerHooker
    /// {
    /// 
    ///     void Awake()
    ///     {
    ///         HookEvents();
    ///     }
    /// 
    ///     void OnDestroy()
    ///     {
    ///         UnhookEvents();
    ///     }
    /// 
    ///     public void HookEvents()
    ///     {
    ///         _OnHitHandler = OnHit;
    /// 
    ///         ThisGameEventManager eventManager = ThisGameEventManager.Get<ThisGameEventManager>(this.gameObject);
    ///         if (eventManager != null)
    ///         {                
    ///             eventManager.Hit += _OnHitHandler;
    ///         }
    ///     }
    /// 
    ///     public void UnhookEvents()
    ///     {
    ///         ThisGameEventManager eventManager = ThisGameEventManager.Get;gt<ThisGameEventManager>(this.gameObject);
    ///         if (eventManager != null)
    ///         {
    ///             eventManager.Hit -= _OnHitHandler;
    ///         }
    ///     }
    ///     
    ///     private ThisGameEventManager.OnHitHandler _OnHitHandler;
    ///     private void OnHit(UnityEngine.Collider other)
    ///     {
    ///         // do something
    ///     }
    /// }
    ///     
    /// </code>
    /// 
    /// other behaviors could call OnHit method to notify interested behaviors
    /// </example>       
    [UnityEngine.AddComponentMenu("Skill/Managers/EventManager")]
    public class EventManager : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// Get specified EventManager
        /// </summary>
        /// <typeparam name="T">type of EventManager</typeparam>
        /// <param name="go">GameObject to retrieve EventManager</param>
        /// <returns>EventManager</returns>
        public static T Get<T>(UnityEngine.GameObject go) where T : EventManager
        {
            return go.GetComponent<T>();
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            enabled = false;
        }
    }

    /// <summary>
    /// Generalize using EventManager. if a MonoBehavior wants to use EventManager methods should implement this interface
    /// </summary>
    public interface IEventManagerHooker
    {
        /// <summary>
        /// Call this method in Awake and hook desired events of EventManager
        /// </summary>
        void HookEvents();
        /// <summary>
        /// Call this method in OnDestroy and unhook hooked events from EventManager
        /// </summary>
        void UnhookEvents();
    }
}
