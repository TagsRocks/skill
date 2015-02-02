using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you need to Apply explosion forces to objects in your scene.
    /// The object that this event applies to must have a rigid body for this event to work.
    /// </summary>
    [CustomEvent("Explosion Force", "Physics")]
    public class AddExplosionForce : EventKey
    {
        [SerializeField]
        private Rigidbody _Body;
        [SerializeField]
        private Transform _ExpPosition;
        [SerializeField]
        private float _Strength;
        [SerializeField]
        private float _Radius;
        [SerializeField]
        private float _UpwardsModifier;
        [SerializeField]
        private ForceMode _ForceMode;



        [ExposeProperty(101, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(102, "Position", "Positon of explosion")]
        public Transform Position { get { return _ExpPosition; } set { _ExpPosition = value; } }
        [ExposeProperty(103, "Strength", "Strength of force")]
        public float Strength { get { return _Strength; } set { _Strength = value; } }
        [ExposeProperty(014, "Radius", "Radius of explosion")]
        public float Radius { get { return _Radius; } set { _Radius = value; } }
        [ExposeProperty(105, "_UpwardsModifier", "UpwardsModifier applies the force as if it was applied from beneath the object")]
        public float UpwardsModifier { get { return _UpwardsModifier; } set { _UpwardsModifier = value; } }
        [ExposeProperty(106, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }

        public override void FireEvent()
        {
            if (_Body != null)
                _Body.AddExplosionForce(_Strength, _ExpPosition.position, _Radius, _UpwardsModifier, _ForceMode);
            else
                Debug.LogWarning("You must specify a valid Rigidbody for AddExplosionForce event");
        }


    }
}