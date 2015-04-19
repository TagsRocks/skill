using UnityEngine;
using System.Collections;



namespace Skill.Framework
{
    public class TPSCamera : DynamicBehaviour
    {
        [System.Serializable]
        public class TPSMode
        {
            public string Name = "Default";
            public float MinPitch = -80;
            public float MaxPitch = 80;
            public Transform Target;
            public Vector3 TargetOffset = new Vector3(0, 1, 0);
            public float Offset = 1.5f;
            public float RotationSpeedX = 3;
            public float RotationSpeedY = 3;
            public float FieldOfView = 60;
            public float ChangeTime = 0.1f;
        }

        public TPSMode[] Modes = new TPSMode[1] { new TPSMode() };


        private TPSMode _SelectedMode;
        private TPSMode _PreSelectedMode;
        private TimeWatch _ChangeModeTW;
        private Camera _Camera;

        public bool InvertY { get; set; }
        public bool InvertX { get; set; }

        public TPSMode SelectedMode
        {
            get
            {
                return _SelectedMode;
            }
            set
            {
                if (_SelectedMode != value)
                {
                    _PreSelectedMode = _SelectedMode;
                    _SelectedMode = value;
                    _PrePitch = _Pitch;
                    if (_SelectedMode != null && _PreSelectedMode != null)
                    {
                        if (_ChangeModeTW.IsEnabled)
                            _ChangeModeTW.Begin(Mathf.Max(0, _SelectedMode.ChangeTime * _ChangeModeTW.Percent));
                        else
                            _ChangeModeTW.Begin(Mathf.Max(0, _SelectedMode.ChangeTime));
                    }
                    else
                        _ChangeModeTW.End();
                }
            }
        }

        private float _PrePitch;
        private float _Pitch;
        public float Pitch
        {
            get
            {
                return _Pitch;
            }
            set
            {
                if (_SelectedMode != null)
                    value = Mathf.Clamp(value, _SelectedMode.MinPitch, _SelectedMode.MaxPitch);
                _Pitch = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (_SelectedMode == null && Modes != null && Modes.Length > 0)
                _SelectedMode = Modes[0];
            _PreSelectedMode = _SelectedMode;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            _Camera = GetComponent<Camera>();
        }


        protected virtual Vector3 CovertToTargetSpace(Vector3 direction)
        {
            if (_SelectedMode != null && _SelectedMode.Target != null)
                return _SelectedMode.Target.TransformDirection(direction);
            else
                return direction;
        }
        protected virtual float GetDeltaX() { return Input.GetAxis("Mouse X"); }
        protected virtual float GetDeltaY() { return Input.GetAxis("Mouse Y"); }
        protected virtual void LateUpdate()
        {
            if (Modes != null)
            {
                if (_SelectedMode != null && _SelectedMode.Target != null)
                {
                    if (_ChangeModeTW.IsEnabledAndOver)
                    {
                        _PreSelectedMode = _SelectedMode;
                        _ChangeModeTW.End();
                    }


                    float horizontal = GetDeltaX() * _SelectedMode.RotationSpeedX * ((InvertX) ? -1 : 1);
                    Pitch += GetDeltaY() * _SelectedMode.RotationSpeedY * ((InvertY) ? 1 : -1);
                    RotateTarget(horizontal);

                    float pitch = Pitch;
                    Vector3 targetPosition = _SelectedMode.Target.position + CovertToTargetSpace(_SelectedMode.TargetOffset);
                    Vector3 dir = new Vector3(0, 0, _SelectedMode.Offset);
                    _Camera.fieldOfView = _SelectedMode.FieldOfView;

                    if (_ChangeModeTW.IsEnabled && _PreSelectedMode != null && _PreSelectedMode.Target != null && _PreSelectedMode != _SelectedMode)
                    {
                        _PrePitch += GetDeltaY() * _PreSelectedMode.RotationSpeedY * ((InvertY) ? 1 : -1);

                        float lerpFactor = _ChangeModeTW.Percent;
                        pitch = Mathf.SmoothStep(_PrePitch, pitch, lerpFactor);
                        targetPosition = MathHelper.SmoothStep(_PreSelectedMode.Target.position + CovertToTargetSpace(_PreSelectedMode.TargetOffset), targetPosition, lerpFactor);
                        dir.z = Mathf.SmoothStep(_PreSelectedMode.Offset, dir.z, lerpFactor);
                        _Camera.fieldOfView = Mathf.SmoothStep(_PreSelectedMode.FieldOfView, _SelectedMode.FieldOfView, lerpFactor);
                    }

                    float desiredAngle = GetTargetRotation();
                    Quaternion rotation = Quaternion.Euler(pitch, desiredAngle, 0);

                    transform.position = targetPosition - (rotation * dir);
                    transform.rotation = rotation;

                }
            }
        }

        protected virtual void RotateTarget(float deltaRotation)
        {
            if (_SelectedMode != null && _SelectedMode.Target != null)
                _SelectedMode.Target.Rotate(0, deltaRotation, 0, Space.Self);
        }
        protected virtual float GetTargetRotation()
        {
            if (_SelectedMode != null && _SelectedMode.Target != null)
                return _SelectedMode.Target.eulerAngles.y;
            else
                return 0;
        }

    }
}