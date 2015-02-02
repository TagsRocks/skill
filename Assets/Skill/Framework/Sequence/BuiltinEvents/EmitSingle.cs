using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you would like to Emit a single particle with given parameters.
    /// </summary>
    [CustomEvent("Emit Single", "Particle")]
    public class EmitSingle : EventKey
    {
        [SerializeField]
        private ParticleEmitter _Emmiter;
        [SerializeField]
        private Transform _Position; // The position of the particle.
        [SerializeField]
        private Vector3 _Velocity = Vector3.up;	 // The velocity of the particle.
        [SerializeField]
        private float _Size;         // The size of the particle.
        [SerializeField]
        private float _Energy;       //	The remaining lifetime of the particle.
        [SerializeField]
        private Color _Color;        //	The color of the particle.        

        [SerializeField]
        private bool _UseRotation;
        [SerializeField]
        private float _Rotation;         //	The initial rotation of the particle in degrees.
        [SerializeField]
        private float _AngularVelocity;  //The angular velocity of the particle in degrees per second.


        [ExposeProperty(101, "Emmiter", "Particle Emitter")]
        public ParticleEmitter Emmiter { get { return _Emmiter; } set { _Emmiter = value; } }
        [ExposeProperty(102, "Position", "The position of the particle.  ")]
        public Transform Position { get { return _Position; } set { _Position = value; } }
        [ExposeProperty(103, "Velocity", "The velocity of the particle.")]
        public Vector3 Velocity { get { return _Velocity; } set { _Velocity = value; } }
        [ExposeProperty(104, "Size", "The size of the particle.")]
        public float Size { get { return _Size; } set { _Size = value; } }
        [ExposeProperty(105, "Energy", "The remaining lifetime of the particle.")]
        public float Energy { get { return _Energy; } set { _Energy = value; } }
        [ExposeProperty(106, "Color", "The color of the particle.")]
        public Color Color { get { return _Color; } set { _Color = value; } }


        [ExposeProperty(107, "Use Rotation", "Use rotation parameters")]
        public bool UseRotation { get { return _UseRotation; } set { _UseRotation = value; } }
        [ExposeProperty(108, "Rotation", "The initial rotation of the particle in degrees.")]
        public float Rotation { get { return _Rotation; } set { _Rotation = value; } }
        [ExposeProperty(109, "Angular Velocity", "The angular velocity of the particle in degrees per second.")]
        public float AngularVelocity { get { return _AngularVelocity; } set { _AngularVelocity = value; } }


        public override void FireEvent()
        {
            if (_Emmiter != null)
            {
                if (_Position != null)
                {
                    if (_UseRotation)
                        _Emmiter.Emit(_Position.position, _Velocity, _Size, _Energy, _Color, _Rotation, _AngularVelocity);
                    else
                        _Emmiter.Emit(_Position.position, _Velocity, _Size, _Energy, _Color);
                }
                else
                    Debug.LogWarning("You must set a valid Position for EmitSingle event");

            }
            else
                Debug.LogWarning("You must set a valid ParticleEmitter for EmitSingle event");
        }
    }
}