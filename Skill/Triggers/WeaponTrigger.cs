using UnityEngine;
using System.Collections;

namespace Skill.Triggers
{

    //[AddComponentMenu("Game/Triggers/Weapon")]
    //public class G_WeaponTrigger : MonoBehaviour
    //{
    //    public G_WeaponBase Weapon { get; set; }

    //    void OnTriggerEnter(Collider other)
    //    {
    //        if (!enabled) return;
    //        if (other.tag == G_Tags.Player)
    //        {
    //            G_EventManager eventManager = G_EventManager.Get(other.transform.parent.gameObject);
    //            if (eventManager != null)
    //                eventManager.OnWeaponHit(Weapon);
    //        }
    //    }

    //    void OnTriggerExit(Collider other)
    //    {
    //        if (other.tag == G_Tags.Player)
    //        {
    //            G_EventManager eventManager = G_EventManager.Get(other.transform.parent.gameObject);
    //            if (eventManager != null)
    //                eventManager.OnWeaponUnHit(Weapon);
    //        }
    //    }
    //}
}