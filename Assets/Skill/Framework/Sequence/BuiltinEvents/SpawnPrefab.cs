using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you would like to dynamically spawn an object.
    /// </summary>
    [CustomEvent("SpawnPrefab", "Spawn")]
    public class SpawnPrefab : EventKey
    {
        [SerializeField]
        private GameObject _Prefab;
        [SerializeField]
        private Transform _Position;


        [ExposeProperty(101, "Prefab", "The prefab to spawn")]
        public GameObject Prefab { get { return _Prefab; } set { _Prefab = value; } }
        [ExposeProperty(102, "Position", "The transform to use when spawning this prefab.")]
        public Transform Position { get { return _Position; } set { _Position = value; } }

        public override void FireEvent()
        {
            if (_Prefab != null && _Position != null)
            {
                Skill.Framework.Managers.Cache.Spawn(_Prefab, _Position.position, _Position.rotation);
            }
            else
            {
                Debug.LogWarning("You must specify a valid Prefab and Position for SpawnPrefab event");
            }
        }


    }
}