using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you need one object to teleport to the location of another.
    /// </summary>
    [CustomEvent("WarpToObject", "Transform")]
    public class WarpToObject : EventKey
    {
        [SerializeField]
        private Transform _WrappingObject;
        [SerializeField]
        private Transform _ObjectToWarpTo;
        [SerializeField]
        private bool _UseObjectRotation;

        [ExposeProperty(101, "Wrapping Object", " child object to change it's parent ")]
        public Transform WrappingObject { get { return _WrappingObject; } set { _WrappingObject = value; } }

        [ExposeProperty(102, "Object To Warp To", "The transform, or object in the scene, whose position we will use to complete this warp.")]
        public Transform ObjectToWarpTo { get { return _ObjectToWarpTo; } set { _ObjectToWarpTo = value; } }

        [ExposeProperty(103, "Target Rotation?", "This defines if, upon warping, we should use the target objects orientation or not.")]
        public bool UseObjectRotation { get { return _UseObjectRotation; } set { _UseObjectRotation = value; } }

        public override void FireEvent()
        {
            if (_WrappingObject != null && _ObjectToWarpTo != null)
            {
                _WrappingObject.position = _ObjectToWarpTo.position;
                if (_UseObjectRotation)
                    _WrappingObject.rotation = _ObjectToWarpTo.rotation;
            }
            else
                Debug.LogWarning("You must specify WrappingObject and ObjectToWarpTo for WarpToObject event");
        }
    }
}