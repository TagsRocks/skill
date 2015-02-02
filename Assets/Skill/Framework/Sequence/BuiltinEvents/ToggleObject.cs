using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event to toggle a GameObject.
    /// </summary>
    [CustomEvent("ToggleObject", "Object")]
    public class ToggleObject : EventKey
    {
        [SerializeField]
        private bool _Active = true;
        [SerializeField]
        private GameObject[] _Objects;

        [ExposeProperty(101, "Active", "Should we acivate, or deactivate this GameObject")]
        public bool Active { get { return _Active; } set { _Active = value; } }

        [ExposeProperty(101, "Objects", "Objects to toggle")]
        public GameObject[] Objects { get { return _Objects; } set { _Objects = value; } }


        public override void FireEvent()
        {
            if (_Objects != null)
            {
                for (int i = 0; i < _Objects.Length; i++)
                {
                    if (_Objects[i] != null)
                        _Objects[i].SetActive(_Active);
                }
            }
        }
    }
}