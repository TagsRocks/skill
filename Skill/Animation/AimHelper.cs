using System;
using UnityEngine;

namespace Skill.Animation
{
    /// <summary>
    /// Helper class to calculate anim for AnimNodeAimOffset
    /// </summary>
    public class AimHelper
    {
        private Quaternion _CurentAim;
        private Vector2 _Aim;

        /// <summary>
        /// Calculated Aim vector
        /// </summary>
        public Vector2 Aim { get { return _Aim; } }

        /// <summary> Gets or sets minimum value of pitch </summary>
        public float PitchMin { get; set; }
        /// <summary> Gets or sets maximum value of pitch </summary>
        public float PitchMax { get; set; }
        /// <summary> Gets or sets minimum value of yaw </summary>
        public float YawMin { get; set; }
        /// <summary> Gets or sets maximum value of pitch </summary>
        public float YawMax { get; set; }

        /// <summary> Local direction of aim relative to forward direction of actor </summary>
        public Vector3 Direction { get; set; }
        /// <summary> Speed of aim </summary>
        public float AimSpeed { get; set; }        

        /// <summary>
        /// Update AimHelper to calculate new aim vector
        /// </summary>
        public void Update()
        {
            Quaternion desiredAim = Quaternion.Euler(Direction);
            Vector3 euler = desiredAim.eulerAngles;
            euler.y = Mathf.Clamp(euler.y, PitchMin, PitchMax);
            euler.x = Mathf.Clamp(euler.x, YawMin, YawMax);
            desiredAim.eulerAngles = euler;

            if (_CurentAim != desiredAim)
                _CurentAim = Quaternion.Lerp(_CurentAim, desiredAim, AimSpeed * Time.deltaTime);

            euler = _CurentAim.eulerAngles;
            // Adjust the pitch
            if (euler.y < 0)
            {
                _Aim.y = Mathf.Abs(euler.y / PitchMax);
            }
            else if (euler.y > 0)
            {
                _Aim.y = euler.y / PitchMin;
            }
            else
            {
                _Aim.y = 0.0f;
            }

            // Adjust the yaw
            if (euler.x > 0)
            {
                _Aim.x = euler.x / YawMax;
            }
            else if (euler.x < 0)
            {
                _Aim.x = Mathf.Abs(euler.x / YawMin) * -1.0f;
            }
            else
            {
                _Aim.x = 0.0f;
            }
        }
    }
}
