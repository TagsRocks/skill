using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you need to Apply physical forces to objects in your scene.
    /// The object that this event applies to must have a rigid body for this event to work.
    /// </summary>
    [CustomEvent("Force", "Physics")]
    public class AddForce : EventKey
    {
        [SerializeField]
        private Rigidbody _Body;
        [SerializeField]
        private Transform _Position;
        [SerializeField]
        private Vector3 _Force;
        [SerializeField]
        private ForceMode _ForceMode;
        [SerializeField]
        private bool _Relative;



        [ExposeProperty(101, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(12, "Position", "Positon of force(optional)")]
        public Transform Position { get { return _Position; } set { _Position = value; } }
        [ExposeProperty(103, "Force", "Strength and Direction of force")]
        public Vector3 Force { get { return _Force; } set { _Force = value; } }
        [ExposeProperty(104, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }
        [ExposeProperty(105, "Relative", "apply force relative or not")]
        public bool Relative { get { return _Relative; } set { _Relative = value; } }

        public override void FireEvent()
        {
            if (_Body != null)
            {
                if (_Relative)
                {
                    _Body.AddRelativeForce(_Force, _ForceMode);
                }
                else
                {
                    if (_Position != null)
                        _Body.AddForceAtPosition(_Force, _Position.position, _ForceMode);
                    else
                        _Body.AddForce(_Force, _ForceMode);
                }
            }
            else
                Debug.LogWarning("You must specify a valid Rigidbody for AddForce event");
        }


    }
}