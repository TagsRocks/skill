using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Modules
{
    [RequireComponent(typeof(Renderer))]
    public class ActiveOnVisible : StaticBehaviour
    {
        /// <summary> Root of object </summary>
        public Transform Root;
        /// <summary> deactive OnBecameInvisible </summary>        
        public bool Deactive = true;


        private StaticBehaviour[] _Behaviours; // Behaviours to toggle
        private Collider[] _Colliders; // Colliders to toggle

        private bool _IsActivated;
        private bool _IsKinematic;
        private bool _UseGravity;


        protected override void Awake()
        {
            base.Awake();
            if (Root != null)
            {
                _Behaviours = Root.GetComponentsInChildren<StaticBehaviour>();
                _Colliders = Root.GetComponentsInChildren<Collider>();
                _Rigidbody = Root.GetComponentInChildren<Rigidbody>();

                if (_Rigidbody != null)
                {
                    _IsKinematic = _Rigidbody.isKinematic;
                    _UseGravity = _Rigidbody.useGravity;
                }
            }
            EnableBehaviors(false);
            _IsActivated = false;
        }

        void OnBecameVisible()
        {
            if (!_IsActivated)
            {
                EnableBehaviors(true);
                _IsActivated = true;
            }
        }
        void OnBecameInvisible()
        {
            if (_IsActivated && Deactive)
            {
                EnableBehaviors(false);
                _IsActivated = false;
            }
        }


        private void EnableBehaviors(bool enable)
        {
            if (_Behaviours != null && !IsDestroyed)
            {
                for (int i = 0; i < _Colliders.Length; i++)
                {
                    _Colliders[i].enabled = enable;
                }
                if (_Rigidbody != null)
                {
                    if (enable)
                    {
                        _Rigidbody.isKinematic = _IsKinematic;
                        _Rigidbody.useGravity = _UseGravity;
                    }
                    else
                    {
                        _Rigidbody.isKinematic = true;
                        _Rigidbody.useGravity = false;
                    }
                }

                for (int i = 0; i < _Behaviours.Length; i++)
                {
                    if (_Behaviours[i] != this && !_Behaviours[i].IsDestroyed)
                        _Behaviours[i].enabled = enable;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _Behaviours = null;
            _Colliders = null;
            _Rigidbody = null;
        }
    }
}