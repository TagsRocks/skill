using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// This bullet goes in a straight direction.
    /// </summary>    
    public class StraightLineBullet : Bullet
    {
        /// <summary>
        /// Amount of damage fall of per unit.
        /// </summary>
        public float DamageFallOf = 0;

        /// <summary>
        /// Damage of bullet do not be lower that this do to fallof
        /// </summary>
        public float MinDamage = 0;

        /// <summary>
        /// Gravity of bullet when rigidbody.isKinematic
        /// </summary>
        public float Gravity = 0;

        /// <summary>
        /// How to rotate towards direction. (when rigidbody.isKinematic and Gravity != 0)
        /// </summary>
        public float RotationFactor = 0.02f;

        /// <summary>
        /// The travelled distance of bullet after spawn.
        /// </summary>
        public float TravelledDistance { get { return _TravelledDistance; } }

        private float _TravelledDistance;
        private float _V0SinAlpha;
        private float _V0CosAlpha;
        private Vector3 _PrePosition;
        private float _StartY;
        private float _Time;
        private Vector3 _XZDirection;

        /// <summary>
        /// Layers to raycast. (should be setted by weapon)
        /// </summary>
        public int LayerMask { get; internal set; }

        /// <summary>
        /// Amount of damage caused by this bullet.
        /// </summary>
        public override float Damage
        {
            get
            {
                if (DamageFallOf > 0)
                    return Mathf.Max(MinDamage, base.Damage - (DamageFallOf * _TravelledDistance));
                else
                    return base.Damage;
            }
            set
            {
                base.Damage = value;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            float deltaMove = _V0CosAlpha * Time.deltaTime;
            _TravelledDistance += deltaMove;

            if (rigidbody == null || rigidbody.isKinematic)
            {
                if (Gravity != 0)
                {
                    _Time += Time.deltaTime;

                    Vector3 pos = _Transform.position + (_XZDirection * deltaMove);
                    pos.y = _StartY + ((0.5f * Gravity * _Time) + _V0SinAlpha) * _Time;

                    _Transform.position = pos;
                    _Transform.forward = Direction = Vector3.RotateTowards(_Transform.forward, (pos - _PrePosition).normalized, RotationFactor, 1);
                    _PrePosition = pos;
                }
                else
                {
                    _Transform.position += Direction * deltaMove;
                }
            }


            if (_TravelledDistance >= Range)
            {
                OnDie(null);
            }
            base.Update();
        }

        /// <summary>
        /// Called by weapon when initialize bullet
        /// </summary>
        public override void StartJourney()
        {
            base.StartJourney();
            _Time = 0;
            _TravelledDistance = 0;
            _PrePosition = transform.position;
            _StartY = _PrePosition.y;

            if ((rigidbody == null || rigidbody.isKinematic) && Gravity != 0)
            {
                _XZDirection = Direction;
                _XZDirection.y = 0;
                _XZDirection.Normalize();

                float startAngle = Vector3.Angle(_XZDirection, Direction) * Mathf.Sign(Direction.y);
                _V0SinAlpha = Speed * Mathf.Sin(startAngle * Mathf.Deg2Rad);
                _V0CosAlpha = Speed * Mathf.Cos(startAngle * Mathf.Deg2Rad);
            }
            else
            {
                _V0SinAlpha = 0.0f;
                _V0CosAlpha = Speed;
            }

        }
    }
}
