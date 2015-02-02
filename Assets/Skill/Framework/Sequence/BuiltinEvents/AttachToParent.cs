using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you want to manipulate the parent child relationship of two objects.
    /// </summary>
    [CustomEvent("AttachToParent", "Transform")]
    public class AttachToParent : EventKey
    {
        [SerializeField]
        private Transform _Child;
        [SerializeField]
        private Transform _Parent;


        [ExposeProperty(101, "Child", " child object to change it's parent ")]
        public Transform Child { get { return _Child; } set { _Child = value; } }

        [ExposeProperty(102, "Parent", "This is the object that you want to be the new parent. If you don't specify a parent, the object will be detached.")]
        public Transform Parent { get { return _Parent; } set { _Parent = value; } }


        public override void FireEvent()
        {
            if (_Child != null)
                _Child.parent = _Parent;
            else
                Debug.LogWarning("You must specify Child object for AttachToParent event");
        }


    }
}