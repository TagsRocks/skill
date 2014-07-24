using UnityEngine;
using System.Collections;
namespace Skill.Framework.Sequence
{
    #region Animation

    /// <summary>
    /// Fades the animation with name animation in over a period of time seconds and fades other animations out.
    /// </summary>
    [CustomEvent("Play Animation", "Animation")]
    public class PlayAnimationEvent : EventKey
    {
        [SerializeField]
        private UnityEngine.Animation _Animation;
        [SerializeField]
        private string _AnimationName = "Idle";
        [SerializeField]
        private float _FadeLength = 0.3f;
        [SerializeField]
        private PlayMode _PlayMode = PlayMode.StopAll;

        [ExposeProperty(11, "Animation", "Animation component")]
        public UnityEngine.Animation Animation { get { return _Animation; } set { _Animation = value; } }
        [ExposeProperty(12, "Animation Name", "Animation clip name")]
        public string AnimationName { get { return _AnimationName; } set { _AnimationName = value; } }
        [ExposeProperty(13, "Fade Length", "")]
        public float FadeLength { get { return _FadeLength; } set { _FadeLength = value; } }
        [ExposeProperty(14, "PlayMode", "")]
        public PlayMode PlayMode { get { return _PlayMode; } set { _PlayMode = value; } }

        public override void Fire()
        {
            if (Animation != null && !string.IsNullOrEmpty(AnimationName))
                Animation.CrossFade(AnimationName, _FadeLength, _PlayMode);
            else
                Debug.LogWarning("Invalid PlayAnimationEvent data");
        }
    }
    #endregion

    #region Transform
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


        [ExposeProperty(11, "Child", " child object to change it's parent ")]
        public Transform Child { get { return _Child; } set { _Child = value; } }

        [ExposeProperty(12, "Parent", "This is the object that you want to be the new parent. If you don't specify a parent, the object will be detached.")]
        public Transform Parent { get { return _Parent; } set { _Parent = value; } }


        public override void Fire()
        {
            if (_Child != null)
                _Child.parent = _Parent;
            else
                Debug.LogWarning("You must specify Child object for AttachToParent event");
        }


    }

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

        [ExposeProperty(11, "Wrapping Object", " child object to change it's parent ")]
        public Transform WrappingObject { get { return _WrappingObject; } set { _WrappingObject = value; } }

        [ExposeProperty(12, "Object To Warp To", "The transform, or object in the scene, whose position we will use to complete this warp.")]
        public Transform ObjectToWarpTo { get { return _ObjectToWarpTo; } set { _ObjectToWarpTo = value; } }

        [ExposeProperty(13, "Object To Warp To", "This defines if, upon warping, we should use the target objects orientation or not.")]
        public bool UseObjectRotation { get { return _UseObjectRotation; } set { _UseObjectRotation = value; } }

        public override void Fire()
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
    #endregion

    #region Audio

    /// <summary>
    /// This event will allow you to Pause or Resume playback on an already playing AudioClip.
    /// </summary>
    [CustomEvent("PauseOrResume", "Audio")]
    public class PauseOrResumeAudio : EventKey
    {

        [SerializeField]
        private AudioSource _Source;

        [SerializeField]
        private bool _Pause;


        [ExposeProperty(11, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        [ExposeProperty(12, "Pause", "Are we pausing or resuming this Audio Clip")]
        public bool Pause { get { return _Pause; } set { _Pause = value; } }

        public override void Fire()
        {
            if (_Source != null)
            {
                if (_Pause)
                    _Source.Pause();
                else
                    _Source.Play();
            }
            else
                Debug.LogWarning("Invalid AudioSource for PauseOrResume event");
        }


    }

    /// <summary>
    /// Add this event when you want to play an Audio Clip
    /// </summary>
    [CustomEvent("Play", "Audio")]
    public class PlayAudio : EventKey
    {

        [SerializeField]
        private AudioSource _Source;

        [SerializeField]
        private AudioClip _Clip;

        [SerializeField]
        private bool _Loop;


        [ExposeProperty(11, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        [ExposeProperty(12, "Clip", "The Audio Clip to play.")]
        public AudioClip Clip { get { return _Clip; } set { _Clip = value; } }

        [ExposeProperty(13, "Loop", "Should we loop this Audio Clip.")]
        public bool Loop { get { return _Loop; } set { _Loop = value; } }

        public override void Fire()
        {
            if (_Source != null && Clip != null)
            {
                if (_Source.isPlaying)
                    _Source.Stop();
                _Source.clip = _Clip;
                _Source.loop = _Loop;
                _Source.Play();
            }
            else
                Debug.LogWarning("Invalid AudioSource and AudioClip for PlayAudio event");
        }


    }

    /// <summary>
    ///  Add this Event when you would like to stop the playback of audio.
    /// </summary>
    [CustomEvent("Stop", "Audio")]
    public class StopAudio : EventKey
    {
        [SerializeField]
        private AudioSource _Source;


        [ExposeProperty(11, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        public override void Fire()
        {
            if (_Source != null)
                _Source.Stop();
            else
                Debug.LogWarning("Invalid AudioSource for StopAudio event");
        }

    }
    #endregion

    #region Debug
    /// <summary>
    /// Add this event when you would like to write a debug message to the Unity Console.
    /// </summary>
    [CustomEvent("Log", "Debug")]
    public class LogMessage : EventKey
    {
        [SerializeField]
        private string _Message;

        [ExposeProperty(11, "Message", "Log message")]
        public string Message { get { return _Message; } set { _Message = value; } }

        public override void Fire()
        {
            if (!string.IsNullOrEmpty(_Message))
                Debug.Log(_Message);
        }


    }
    #endregion

    #region Object
    /// <summary>
    /// Add this event to toggle a GameObject.
    /// </summary>
    [CustomEvent("ToggleObject", "Object")]
    public class ToggleObject : EventKey
    {
        [SerializeField]
        private bool _Active = true;
        [SerializeField]
        private GameObject _Object1;
        [SerializeField]
        private GameObject _Object2;
        [SerializeField]
        private GameObject _Object3;
        [SerializeField]
        private GameObject _Object4;
        [SerializeField]
        private GameObject _Object5;

        [ExposeProperty(11, "Active", "Should we acivate, or deactivate this GameObject")]
        public bool Active { get { return _Active; } set { _Active = value; } }

        [ExposeProperty(12, "Object1", "Object to toggle")]
        public GameObject Object1 { get { return _Object1; } set { _Object1 = value; } }
        [ExposeProperty(13, "Object2", "Object to toggle")]
        public GameObject Object2 { get { return _Object2; } set { _Object2 = value; } }
        [ExposeProperty(14, "Object3", "Object to toggle")]
        public GameObject Object3 { get { return _Object3; } set { _Object3 = value; } }
        [ExposeProperty(15, "Object4", "Object to toggle")]
        public GameObject Object4 { get { return _Object4; } set { _Object4 = value; } }
        [ExposeProperty(16, "Object5", "Object to toggle")]
        public GameObject Object5 { get { return _Object5; } set { _Object5 = value; } }


        public override void Fire()
        {
            if (_Object1 != null) _Object1.SetActive(_Active);
            if (_Object2 != null) _Object2.SetActive(_Active);
            if (_Object3 != null) _Object3.SetActive(_Active);
            if (_Object4 != null) _Object4.SetActive(_Active);
            if (_Object5 != null) _Object5.SetActive(_Active);
        }
    }
    #endregion

    #region ParticleSystem

    /// <summary>
    /// Add this event when you would like to start or stop emission a Particle System Emitter.
    /// </summary>
    [CustomEvent("Start/Stop Emitter", "ParticleSystem")]
    public class StartStopEmitter : EventKey
    {
        [SerializeField]
        private ParticleEmitter _Emmiter;
        [SerializeField]
        private bool _Emit = true;

        [ExposeProperty(11, "Emmiter", "Particle Emitter")]
        public ParticleEmitter Emmiter { get { return _Emmiter; } set { _Emmiter = value; } }

        [ExposeProperty(12, "Emit", "start emit or stop emit")]
        public bool Emit { get { return _Emit; } set { _Emit = value; } }

        public override void Fire()
        {
            if (_Emmiter != null)
                _Emmiter.emit = _Emit;
            else
                Debug.LogWarning("You must set a valid ParticleEmitter for StartStopEmitter event");
        }
    }


    /// <summary>
    /// Add this event when you would like to Emit a number of particles.
    /// </summary>
    [CustomEvent("Emit", "ParticleSystem")]
    public class Emit : EventKey
    {
        [SerializeField]
        private ParticleEmitter _Emmiter;
        [SerializeField]
        private int _Count;          // number of particles.        
        [SerializeField]
        private bool _ClearParticles = false;


        [ExposeProperty(11, "Emmiter", "Particle Emitter")]
        public ParticleEmitter Emmiter { get { return _Emmiter; } set { _Emmiter = value; } }
        [ExposeProperty(12, "Count", "Number of particles")]
        public int Count { get { return _Count; } set { _Count = value; } }
        [ExposeProperty(13, "Emit", "Removes all particles from the particle emitter before emit.")]
        public bool ClearParticles { get { return _ClearParticles; } set { _ClearParticles = value; } }

        public override void Fire()
        {
            if (_Emmiter != null)
            {
                if (_ClearParticles) _Emmiter.ClearParticles();
                if (_Count > 0)
                    _Emmiter.Emit(_Count);
            }
            else
                Debug.LogWarning("You must set a valid ParticleEmitter for Emit event");
        }
    }


    /// <summary>
    /// Add this event when you would like to Emit a single particle with given parameters.
    /// </summary>
    [CustomEvent("Emit Single", "ParticleSystem")]
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


        [ExposeProperty(11, "Emmiter", "Particle Emitter")]
        public ParticleEmitter Emmiter { get { return _Emmiter; } set { _Emmiter = value; } }
        [ExposeProperty(12, "Position", "The position of the particle.  ")]
        public Transform Position { get { return _Position; } set { _Position = value; } }
        [ExposeProperty(13, "Velocity", "The velocity of the particle.")]
        public Vector3 Velocity { get { return _Velocity; } set { _Velocity = value; } }
        [ExposeProperty(14, "Size", "The size of the particle.")]
        public float Size { get { return _Size; } set { _Size = value; } }
        [ExposeProperty(15, "Energy", "The remaining lifetime of the particle.")]
        public float Energy { get { return _Energy; } set { _Energy = value; } }
        [ExposeProperty(16, "Color", "The color of the particle.")]
        public Color Color { get { return _Color; } set { _Color = value; } }


        [ExposeProperty(17, "Use Rotation", "Use rotation parameters")]
        public bool UseRotation { get { return _UseRotation; } set { _UseRotation = value; } }
        [ExposeProperty(18, "Rotation", "The initial rotation of the particle in degrees.")]
        public float Rotation { get { return _Rotation; } set { _Rotation = value; } }
        [ExposeProperty(19, "Angular Velocity", "The angular velocity of the particle in degrees per second.")]
        public float AngularVelocity { get { return _AngularVelocity; } set { _AngularVelocity = value; } }


        public override void Fire()
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

    #endregion

    #region Physics

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



        [ExposeProperty(11, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(12, "Position", "Positon of force(optional)")]
        public Transform Position { get { return _Position; } set { _Position = value; } }
        [ExposeProperty(13, "Force", "Strength and Direction of force")]
        public Vector3 Force { get { return _Force; } set { _Force = value; } }
        [ExposeProperty(14, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }
        [ExposeProperty(15, "Relative", "apply force relative or not")]
        public bool Relative { get { return _Relative; } set { _Relative = value; } }

        public override void Fire()
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



        [ExposeProperty(11, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(12, "Position", "Positon of explosion")]
        public Transform Position { get { return _ExpPosition; } set { _ExpPosition = value; } }
        [ExposeProperty(13, "Strength", "Strength of force")]
        public float Strength { get { return _Strength; } set { _Strength = value; } }
        [ExposeProperty(14, "Radius", "Radius of explosion")]
        public float Radius { get { return _Radius; } set { _Radius = value; } }
        [ExposeProperty(15, "_UpwardsModifier", "UpwardsModifier applies the force as if it was applied from beneath the object")]
        public float UpwardsModifier { get { return _UpwardsModifier; } set { _UpwardsModifier = value; } }
        [ExposeProperty(16, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }

        public override void Fire()
        {
            if (_Body != null)
                _Body.AddExplosionForce(_Strength, _ExpPosition.position, _Radius, _UpwardsModifier, _ForceMode);
            else
                Debug.LogWarning("You must specify a valid Rigidbody for AddExplosionForce event");
        }


    }


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



        [ExposeProperty(11, "Body", "The Rigidbody to apply force")]
        public Rigidbody Body { get { return _Body; } set { _Body = value; } }
        [ExposeProperty(12, "Torque", "Strength and Direction of force")]
        public Vector3 Torque { get { return _Torque; } set { _Torque = value; } }
        [ExposeProperty(13, "ForceMode", "Option for how to apply a force")]
        public ForceMode ForceMode { get { return _ForceMode; } set { _ForceMode = value; } }
        [ExposeProperty(14, "Relative", "apply torque relative or not")]
        public bool Relative { get { return _Relative; } set { _Relative = value; } }

        public override void Fire()
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

    #endregion

    #region Signal

    public enum SendMessageParameter
    {
        None,
        Float,
        Int,
        Bool,
        ObjectReference,
        String,
    }

    /// <summary>
    /// Add this event if you would like to call a function on an object.
    /// </summary>
    [CustomEvent("SendMessage", "Signal")]
    public class SendMessage : EventKey
    {
        [SerializeField]
        private GameObject _Receiver;
        [SerializeField]
        private string _FunctionName;
        [SerializeField]
        private SendMessageParameter _ParameterType;
        [SerializeField]
        private SendMessageOptions _MessageOptions;
        [SerializeField]
        private float _FloatParameter;
        [SerializeField]
        private int _IntParameter;
        [SerializeField]
        private bool _BoolParameter;
        [SerializeField]
        private Object _ObjectReferenceParameter;
        [SerializeField]
        private string _StringParameter;



        [ExposeProperty(11, "Receiver", " The object to receive this message.")]
        public GameObject Receiver { get { return _Receiver; } set { _Receiver = value; } }
        [ExposeProperty(12, "Function Name", "Name of function to call")]
        public string FunctionName { get { return _FunctionName; } set { _FunctionName = value; } }
        [ExposeProperty(13, "MessageOptions", "Options")]
        public SendMessageOptions MessageOptions { get { return _MessageOptions; } set { _MessageOptions = value; } }
        [ExposeProperty(14, "Parameter Type", "Parameter type")]
        public SendMessageParameter ParameterType { get { return _ParameterType; } set { _ParameterType = value; } }
        [ExposeProperty(15, "Float", "Float parameter")]
        public float FloatParameter { get { return _FloatParameter; } set { _FloatParameter = value; } }
        [ExposeProperty(16, "Int", "Int parameter")]
        public int IntParameter { get { return _IntParameter; } set { _IntParameter = value; } }
        [ExposeProperty(17, "Bool", "Boolean parameter")]
        public bool BoolParameter { get { return _BoolParameter; } set { _BoolParameter = value; } }
        [ExposeProperty(18, "Object", "Object reference parameter")]
        public Object ObjectReferenceParameter { get { return _ObjectReferenceParameter; } set { _ObjectReferenceParameter = value; } }
        [ExposeProperty(19, "String", "String parameter")]
        public string StringParameter { get { return _StringParameter; } set { _StringParameter = value; } }

        public override void Fire()
        {
            if (_Receiver != null)
            {
                if (!string.IsNullOrEmpty(_FunctionName))
                {
                    switch (_ParameterType)
                    {
                        case SendMessageParameter.None:
                            _Receiver.SendMessage(_FunctionName, _MessageOptions);
                            break;
                        case SendMessageParameter.Float:
                            _Receiver.SendMessage(_FunctionName, _FloatParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.Int:
                            _Receiver.SendMessage(_FunctionName, _IntParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.Bool:
                            _Receiver.SendMessage(_FunctionName, _BoolParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.ObjectReference:
                            _Receiver.SendMessage(_FunctionName, _ObjectReferenceParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.String:
                            _Receiver.SendMessage(_FunctionName, _StringParameter, _MessageOptions);
                            break;
                    }
                }
                else
                    Debug.LogWarning("You must specify a valid FunctionName for SendMessage event");
            }
            else
                Debug.LogWarning("You must specify a valid Receiver for SendMessage event");
        }
    }
    #endregion

    #region Spawn

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


        [ExposeProperty(11, "Prefab", "The prefab to spawn")]
        public GameObject Prefab { get { return _Prefab; } set { _Prefab = value; } }
        [ExposeProperty(12, "Position", "The transform to use when spawning this prefab.")]
        public Transform Position { get { return _Position; } set { _Position = value; } }

        public override void Fire()
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
    #endregion

}