using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.Modules
{
    public class OrbitCamera : DynamicBehaviour
    {
        public Transform Target;
        public float Damping = 3;
        public float XSpeed = 10.0f;
        public float YSpeed = 10.0f;
        public float Distance = 5.0f;
        public Vector2 MinLimit = new Vector2(-20, 0);
        public Vector2 MaxLimit = new Vector2(80, 360);
        public Vector3 Offset = Vector3.zero;

        private CameraLimit _CameraLimit;
        private LerpSmoothing _AngleX;
        private LerpSmoothing _AngleY;
        private Vector3 _PrePosition;
        protected override void GetReferences()
        {
            base.GetReferences();
            _CameraLimit = GetComponent<CameraLimit>();
        }

        protected override void Start()
        {
            base.Start();
            Reset((MinLimit.x + MaxLimit.x) * 0.5f, (MinLimit.y + MaxLimit.y) * 0.5f, true);
            LateUpdate();
            _PrePosition = transform.position;

        }

        public void Reset(float rotationX, float rotationY, bool fast = false)
        {
            rotationX = Mathf.Clamp(rotationX, MinLimit.x, MaxLimit.x);
            rotationY = Mathf.Clamp(rotationY, MinLimit.y, MaxLimit.y);

            if (fast)
            {
                _AngleX.Reset(rotationX);
                _AngleY.Reset(rotationY);
            }
            else
            {
                _AngleX.Target = rotationX;
                _AngleY.Target = rotationY;
            }

        }

        public void RotateX(float deltaRotation) { Rotate(ref _AngleX, deltaRotation, MinLimit.x, MaxLimit.x); }
        public void RotateY(float deltaRotation) { Rotate(ref _AngleY, deltaRotation, MinLimit.y, MaxLimit.y); }

        public void Rotate(float deltaRotationX, float deltaRotationY)
        {
            Rotate(ref _AngleX, deltaRotationX, MinLimit.x, MaxLimit.x);
            Rotate(ref _AngleY, deltaRotationY, MinLimit.y, MaxLimit.y);
        }

        private static void Rotate(ref LerpSmoothing angle, float deltaRotation, float min, float max)
        {
            angle.Target = Mathf.Clamp(angle.Target + deltaRotation, min, max);
        }

        void LateUpdate()
        {
            if (Target != null)
            {
                _AngleX.Update(Damping);
                _AngleY.Update(Damping);

                Quaternion rotation = Quaternion.Euler(_AngleY.Current, _AngleX.Current, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
                Vector3 position = (rotation * negDistance) + Target.position;

                position += rotation * Offset;

                if (_CameraLimit != null)
                    _CameraLimit.ApplyLimit(ref position, _PrePosition);

                transform.rotation = rotation;
                transform.position = position;
                _PrePosition = position;
            }
        }
    }
}