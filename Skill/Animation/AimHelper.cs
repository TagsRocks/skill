using System;
using UnityEngine;

namespace Skill.Animation
{
    public class AimHelper
    {
        private Quaternion _CurentAim;
        private Vector2 _Aim;
        public Vector2 Aim { get { return _Aim; } }

        public float PitchMin { get; set; }
        public float PitchMax { get; set; }
        public float YawMin { get; set; }
        public float YawMax { get; set; }

        public Vector3 Direction { get; set; }
        public float AimSpeed { get; set; }

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
