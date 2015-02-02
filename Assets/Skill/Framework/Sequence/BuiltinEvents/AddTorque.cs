using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you need to Apply physical torque to objects in your scene.
    /// The object that this event applies to must have a rigid body for this event to work.
    /// </summary>
    [CustomEvent("Torque", "Physics")]
    public class AddTorque : EventKey
    {
        [SerializeField]
        private Rigidbody _Body;
        [SerializeField]
        private Vector3 _Torque;
        [SerializeField]
        private ForceMode _ForceMode;
        [SerializeField]
        private bool _Relative;



        [ExposeProperty(101, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(102, "Torque", "Strength and Direction of force")]
        public Vector3 Torque { get { return _Torque; } set { _Torque = value; } }
        [ExposeProperty(103, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }
        [ExposeProperty(104, "Relative", "apply torque relative or not")]
        public bool Relative { get { return _Relative; } set { _Relative = value; } }

        public override void FireEvent()
        {
            if (_Body != null)
            {
                if (_Relative)
                {
                    _Body.AddRelativeTorque(_Torque, _ForceMode);
                }
                else
                {
                    _Body.AddTorque(_Torque, _ForceMode);
                }
            }
            else
                Debug.LogWarning("You must specify a valid Rigidbody for AddTorque event");
        }


    }
}