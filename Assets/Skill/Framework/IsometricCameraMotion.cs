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

        /// <summary> SmoothStep </summary>
        public bool SmoothStep = true;

        // Private memeber data        
        private LerpAngle _AroundAngle;
        private LerpAngle _LookAngle;
        private LerpAngle _Fov;
        private Lerp _Preview;
        private Lerp _ZoomIn;
        private Lerp _ZoomOut;
        private Lerp3D _CustomOffset;

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
            if (finalFov != Camera.Fov)
            {
                _Fov.Begin(Camera.Fov, finalFov, motionTime);
                enabled = true;
            }
        }

        public void MotionAroundAngle(float finalAroundAngle, float motionTime)
        {
            //finalAroundAngle = Mathf.Repeat(finalAroundAngle, 360);
            if (finalAroundAngle != Camera.AroundAngle)
            {
                _AroundAngle.Begin(Camera.AroundAngle, finalAroundAngle, motionTime);
                enabled = true;
            }
        }

        public void MotionLookAngle(float finalLookAngle, float motionTime)
        {
            //finalLookAngle = Mathf.Repeat(finalLookAngle, 90);
            if (finalLookAngle != Camera.LookAngle)
            {
                _LookAngle.Begin(Camera.LookAngle, finalLookAngle, motionTime);
                enabled = true;
            }
        }

        public void MotionPreview(float finalPreview, float motionTime)
        {
            if (finalPreview != Camera.CameraPreview)
            {
                _Preview.SmoothStep = this.SmoothStep;
                _Preview.Begin(Camera.CameraPreview, finalPreview, motionTime);
                enabled = true;
            }
        }
        public void MotionZoomIn(float finalZoomIn, float motionTime)
        {
            if (finalZoomIn != Camera.ZoomIn)
            {
                _ZoomIn.SmoothStep = this.SmoothStep;
                _ZoomIn.Begin(Camera.ZoomIn, finalZoomIn, motionTime);
                enabled = true;
            }
        }

        public void MotionZoomOut(float finalZoomOut, float motionTime)
        {
            if (finalZoomOut != Camera.ZoomOut)
            {
                _ZoomOut.SmoothStep = this.SmoothStep;
                _ZoomOut.Begin(Camera.ZoomOut, finalZoomOut, motionTime);
                enabled = true;
            }
        }

        public void MotionCustomOffset(Vector3 finalCustomOffset, float motionTime)
        {
            if (finalCustomOffset != Camera.CustomOffset)
            {
                _CustomOffset.SmoothStep = this.SmoothStep;
                _CustomOffset.Begin(Camera.CustomOffset, finalCustomOffset, motionTime);
                enabled = true;
            }
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

                if (this._AroundAngle.IsEnabled)
                {
                    if (this._AroundAngle.IsOver)
                        this._AroundAngle.End();
                    else
                        updating = true;
                    this.Camera.AroundAngle = this._AroundAngle.Value;
                }

                if (this._LookAngle.IsEnabled)
                {
                    if (this._LookAngle.IsOver)
                        this._LookAngle.End();
                    else
                        updating = true;
                    this.Camera.LookAngle = this._LookAngle.Value;
                }

                if (this._Fov.IsEnabled)
                {
                    if (this._Fov.IsOver)
                        this._Fov.End();
                    else
                        updating = true;
                    this.Camera.Fov = this._Fov.Value;
                }

                if (this._Preview.IsEnabled)
                {
                    if (this._Preview.IsOver)
                        this._Preview.End();
                    else
                        updating = true;
                    this.Camera.CameraPreview = this._Preview.Value;
                }

                if (this._ZoomIn.IsEnabled)
                {
                    if (this._ZoomIn.IsOver)
                        this._ZoomIn.End();
                    else
                        updating = true;
                    this.Camera.ZoomIn = this._ZoomIn.Value;
                }

                if (this._ZoomOut.IsEnabled)
                {
                    if (this._ZoomOut.IsOver)
                        this._ZoomOut.End();
                    else
                        updating = true;
                    this.Camera.ZoomOut = this._ZoomOut.Value;
                }

                if (this._CustomOffset.IsEnabled)
                {
                    if (this._CustomOffset.IsOver)
                        this._CustomOffset.End();
                    else
                        updating = true;
                    this.Camera.CustomOffset = this._CustomOffset.Value;
                }
                enabled = updating;
            }

            base.Update();
        }
    }
}
