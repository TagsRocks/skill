using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// Defines rotation of curve projectile when spawned by weapon
    /// </summary>
    public enum InitialCurveProjectileRotation
    {
        /// <summary> Quaternion.identity </summary>
        Identity,
        /// <summary> Forward of spawn point </summary>
        Forward,
        /// <summary> custom rotation </summary>
        AbsoluteCustom,
        /// <summary> custom local rotation </summary>
        RelativeCustom,

    }

    /// <summary>
    /// Parameters needed by weapon when shooting Curve projectiles
    /// </summary>
    [Serializable]
    public class CurveProjectileParams
    {
        /// <summary> Angle relative to direction to throw bullets. </summary>
        public float ThrowAngle = 45;

        /// <summary> Rotation of bulet when spawn</summary>
        public InitialCurveProjectileRotation InitialRotation;

        /// <summary> Custom Rotation of bulet when InitialRotation is Custom</summary>
        public Quaternion Rotation;


        private float _PreThrowAngle = float.MinValue;
        //private float _SinThrowAngle;
        //private float _CosThrowAngle;
        private float _Sin2ThrowAngle;
        //private float _Cos2ThrowAngle;
        private float _TangentThrowAngle;

        private void CalcSinCosThrowAngle()
        {
            if (_PreThrowAngle != ThrowAngle)
            {                
                _PreThrowAngle = ThrowAngle;
                //_SinThrowAngle = Mathf.Sin(ThrowAngle * Mathf.Deg2Rad);
                //_CosThrowAngle = Mathf.Cos(ThrowAngle * Mathf.Deg2Rad);
                _Sin2ThrowAngle = Mathf.Sin(ThrowAngle * 2 * Mathf.Deg2Rad);
                //_Cos2ThrowAngle = Mathf.Cos(ThrowAngle * 2 * Mathf.Deg2Rad);
                _TangentThrowAngle = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad);
            }
        }

        /// <summary> Retrieves precalculated Mathf.Sin(ThrowAngle) </summary>
        //public float SinThrowAngle
        //{
        //    get
        //    {
        //        CalcSinCosThrowAngle();
        //        return _SinThrowAngle;
        //    }
        //}

        /// <summary> Retrieves precalculated Mathf.Cos(ThrowAngle) </summary>
        //public float CosThrowAngle
        //{
        //    get
        //    {
        //        CalcSinCosThrowAngle();
        //        return _CosThrowAngle;
        //    }
        //}

        public float Sin2ThrowAngle
        {
            get
            {
                CalcSinCosThrowAngle();
                return _Sin2ThrowAngle;
            }
        }

        /// <summary> Retrieves precalculated Mathf.Cos(ThrowAngle) </summary>
        //public float Cos2ThrowAngle
        //{
        //    get
        //    {
        //        CalcSinCosThrowAngle();
        //        return _Cos2ThrowAngle;
        //    }
        //}

        /// <summary> Retrieves precalculated Mathf.Tan(ThrowAngle) </summary>
        public float TangentThrowAngle
        {
            get
            {
                CalcSinCosThrowAngle();
                return _TangentThrowAngle;
            }
        }
    }
}
