using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.Modules
{
    public class OrbitCamera : DynamicBehaviour
    {
        public float ResetTime = 0.5f;
        public float XSpeed = 10.0f;
        public float YSpeed = 10.0f;
        public float PanSpeed = 1.0f;

        public float YMinLimit = -20f;
        public float YMaxLimit = 80f;

        public float XMinLimit = -60f;
        public float XMaxLimit = 60f;

        public float DistanceMin = 10.5f;
        public float DistanceMax = 3f;

        private float _X = 0.0f;
        private float _Y = 0.0f;

        private Skill.Framework.IO.DragGestureDetector _DragGesture;
        private Skill.Framework.IO.ScaleGestureDetector _ScaleGesture;
        private Skill.Framework.IO.DragGestureDetector _PanDetector;
        private bool _Changed;
        private float _PreDistance;
        private float _Distance;
        private Vector3 _Target;
        private TimeWatch _ResetTW;
        private Vector3 _ResetPosition;
        private Quaternion _ResetRotation;
        private CameraLimit _CameraLimit;

        protected override void GetReferences()
        {
            base.GetReferences();
            _CameraLimit = GetComponent<CameraLimit>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_DragGesture != null)
            {
                _DragGesture.IsEnabled = true;
                _ScaleGesture.IsEnabled = true;
                _PanDetector.IsEnabled = true;
            }
        }

        public void ResetCamera(bool fast)
        {

            _Target = Vector3.zero;
            _Distance = (DistanceMin + DistanceMax) * 0.5f;
            _X = 0;
            _Y = YMinLimit;
            if (fast)
            {
                _ResetTW.End();
                Quaternion rotation = Quaternion.Euler(_Y, _X, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -_Distance);
                Vector3 position = rotation * negDistance + _Target;
                transform.rotation = rotation;
                transform.position = position;

            }
            else
            {
                _ResetTW.Begin(ResetTime);
            }

            _ResetPosition = transform.position;
            _ResetRotation = transform.rotation;
            _Changed = true;
        }

        private void OnDisable()
        {
            _DragGesture.IsEnabled = false;
            _ScaleGesture.IsEnabled = false;
            _PanDetector.IsEnabled = false;
        }

        protected override void Start()
        {
            base.Start();
            _Changed = true;
            Vector3 angles = transform.eulerAngles;
            _X = angles.y;
            _Y = angles.x;

            _DragGesture = new Skill.Framework.IO.DragGestureDetector() { Priority = 1 };
            _PanDetector = new Skill.Framework.IO.DragGestureDetector() { Priority = 2 };
            _ScaleGesture = new Skill.Framework.IO.ScaleGestureDetector() { Priority = 3, };

            Skill.Framework.IO.InputManager.Add(_DragGesture);
            Skill.Framework.IO.InputManager.Add(_ScaleGesture);
            Skill.Framework.IO.InputManager.Add(_PanDetector);

            _DragGesture.Drag += _DragGesture_Drag;
            _ScaleGesture.Scale += _ScaleGesture_Scale;
            _ScaleGesture.ScaleStart += _ScaleGesture_ScaleStart;
            _PanDetector.Drag += _PanDetector_Drag;

            UpdateGestureBounds();
            ResetCamera(true);
        }

        private void UpdateGestureBounds()
        {
            Rect half = Utility.ScreenRect;
            half.height *= 0.5f;
            _DragGesture.BoundaryFrame = half;

            half.y += half.height;
            _PanDetector.BoundaryFrame = half;
        }

        void _PanDetector_Drag(object sender, Skill.Framework.IO.DragGestureEventArgs args)
        {
            if (_ResetTW.IsEnabled) return;
            _Target += transform.right * (args.DeltaTranslation.x * PanSpeed * -0.001f * _Distance);
            _Target += transform.up * (args.DeltaTranslation.y * PanSpeed * -0.001f * _Distance);
            _Changed = true;
        }

        void _ScaleGesture_ScaleStart(object sender, Skill.Framework.IO.GestureEventArgs args)
        {
            _PreDistance = _Distance;
        }

        void _ScaleGesture_Scale(object sender, Skill.Framework.IO.ScaleGestureEventArgs args)
        {
            if (_ResetTW.IsEnabled) return;
            _Changed = true;
            _Distance = Mathf.Clamp(_PreDistance + (_PreDistance * (1.0f - args.TotalScale) * 0.3f), DistanceMin, DistanceMax);
        }

        void _DragGesture_Drag(object sender, Skill.Framework.IO.DragGestureEventArgs args)
        {
            if (_ResetTW.IsEnabled) return;
            _Changed = true;
            _X += args.DeltaTranslation.x * XSpeed * _Distance * 0.02f;
            _Y -= args.DeltaTranslation.y * YSpeed * 0.02f;
            _Y = ClampAngle(_Y, YMinLimit, YMaxLimit);
            _X = ClampAngle(_X, XMinLimit, XMaxLimit);
        }


        void LateUpdate()
        {
            UpdateGestureBounds();
            if (_Changed)
            {
                _Changed = false;
                _Target.y = 0.2f;
                if (_CameraLimit != null)
                    _CameraLimit.ApplyLimit(ref _Target, _ResetPosition);

                Quaternion rotation = Quaternion.Euler(_Y, _X, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -_Distance);
                Vector3 position = rotation * negDistance + _Target;

                if (_ResetTW.IsEnabledAndOver)
                    _ResetTW.End();

                if (_ResetTW.IsEnabled)
                {
                    transform.rotation = Quaternion.Slerp(_ResetRotation, rotation, _ResetTW.Percent);
                    transform.position = Vector3.Slerp(_ResetPosition, position, _ResetTW.Percent);
                    _Changed = true;
                }
                else
                {
                    transform.rotation = rotation;
                    transform.position = position;
                }
            }

        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}