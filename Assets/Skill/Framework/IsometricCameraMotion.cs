using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Motion Isometric camera smoothly. Add this component to 'UnityEngine.Camera' with 'Skill.Framework.IsometricCamera' component attached.
    /// </summary>    
    [RequireComponent(typeof(IsometricCamera))]
    public class IsometricCameraMotion : DynamicBehaviour
    {
        /// <summary> Isometric Camera </summary>
        public IsometricCamera Camera { get; private set; }

        // Private memeber data        
        private SmoothingAngle _AroundAngle;
        private SmoothingAngle _LookAngle;
        private SmoothingAngle _Fov;
        private SmoothingParameters _AroundAngleSmoothing;
        private SmoothingParameters _LookAngleSmoothing;
        private SmoothingParameters _FovSmoothing;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            _AroundAngleSmoothing = new SmoothingParameters() { SmoothType = SmoothType.Damp };
            _LookAngleSmoothing = new SmoothingParameters() { SmoothType = SmoothType.Damp };
            _FovSmoothing = new SmoothingParameters() { SmoothType = SmoothType.Damp };

            if (Camera != null)
            {
                _AroundAngle.Reset(Camera.AroundAngle);
                _LookAngle.Reset(Camera.LookAngle);
                _Fov.Reset(Camera.Fov);
            }

            enabled = false;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            Camera = GetComponent<IsometricCamera>();
            if (Camera == null)
                throw new MissingComponentException("IsometricCamera missed");
        }

        public void MotionFov(float finalFov, float motionTime)
        {
            finalFov = Mathf.Repeat(finalFov, 180);
            _Fov.Reset(Camera.Fov);
            _Fov.TargetAngle = finalFov;
            _FovSmoothing.SmoothTime = motionTime;
            enabled = true;
        }
        public void MotionDeltaFov(float deltaFov, float motionTime)
        {
            MotionFov(Camera.Fov + deltaFov, motionTime);
        }

        public void MotionAroundAngle(float finalAroundAngle, float motionTime)
        {
            finalAroundAngle = Mathf.Repeat(finalAroundAngle, 360);
            _AroundAngle.Reset(Camera.AroundAngle);
            _AroundAngle.TargetAngle = finalAroundAngle;
            _AroundAngleSmoothing.SmoothTime = motionTime;
            enabled = true;
        }
        public void MotionDeltaAroundAngle(float deltaAroundAngle, float motionTime)
        {
            MotionAroundAngle(Camera.AroundAngle + deltaAroundAngle, motionTime);
        }

        public void MotionLookAngle(float finalLookAngle, float motionTime)
        {
            finalLookAngle = Mathf.Repeat(finalLookAngle, 90);
            _LookAngle.Reset(Camera.LookAngle);
            _LookAngle.TargetAngle = finalLookAngle;
            _LookAngleSmoothing.SmoothTime = motionTime;
            enabled = true;
        }
        public void MotionDeltaLookAngle(float deltaLookAngle, float motionTime)
        {
            MotionLookAngle(Camera.LookAngle + deltaLookAngle, motionTime);
        }

        public void Motion(float finalFov, float finalAroundAngle, float finalLookAngle, float motionTime)
        {
            MotionFov(finalFov, motionTime);
            MotionAroundAngle(finalAroundAngle, motionTime);
            MotionLookAngle(finalLookAngle, motionTime);
        }

        public void MotionDelta(float deltaFov, float deltaAroundAngle, float deltaLookAngle, float motionTime)
        {
            MotionDeltaFov(deltaFov, motionTime);
            MotionDeltaAroundAngle(deltaAroundAngle, motionTime);
            MotionDeltaLookAngle(deltaLookAngle, motionTime);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Camera == null) return;

            if (!Global.IsGamePaused) // do not modify camera if game is in cutscene mode
            {
                bool updating = false;

                if (this.Camera.AroundAngle != this._AroundAngle.TargetAngle)
                {
                    if (Mathf.Abs(this.Camera.AroundAngle - this._AroundAngle.TargetAngle) < 0.01f)
                    {
                        this.Camera.AroundAngle = this._AroundAngle.TargetAngle;
                    }
                    else
                    {
                        updating = true;
                        this._AroundAngle.Update(_AroundAngleSmoothing);
                        this.Camera.AroundAngle = this._AroundAngle.CurrentAngle;
                    }
                }

                if (this.Camera.LookAngle != this._LookAngle.TargetAngle)
                {
                    if (Mathf.Abs(this.Camera.LookAngle - this._LookAngle.TargetAngle) < 0.01f)
                    {
                        this.Camera.LookAngle = this._LookAngle.TargetAngle;
                    }
                    else
                    {
                        updating = true;
                        this._LookAngle.Update(_LookAngleSmoothing);
                        this.Camera.LookAngle = this._LookAngle.CurrentAngle;
                    }
                }

                if (this.Camera.Fov != this._Fov.TargetAngle)
                {
                    if (Mathf.Abs(this.Camera.Fov - this._Fov.TargetAngle) < 0.01f)
                    {
                        this.Camera.Fov = this._Fov.TargetAngle;
                    }
                    else
                    {
                        updating = true;
                        this._Fov.Update(_FovSmoothing);
                        this.Camera.Fov = this._Fov.CurrentAngle;
                    }
                }

                enabled = updating;
            }

            base.Update();
        }
    }
}
