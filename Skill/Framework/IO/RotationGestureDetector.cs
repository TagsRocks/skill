using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// RotationGestureEventArgs
    /// </summary>
    public class RotationGestureEventArgs : GestureEventArgs
    {
        /// <summary> Delta rotation since last rotation </summary>
        public float DeltaRotation { get; private set; }
        /// <summary> Total rotation relative to initial rotation </summary>
        public float TotalRotation { get; private set; }

        /// <summary>
        /// Create a GestureDetectorEventArgs
        /// </summary>    
        /// <param name="fingerCount"> Number of fingers </param>
        /// <param name="positions">Position of touches when event happened  </param>    
        /// <param name="deltaRotation"> Delta rotation since last rotation </param>
        /// <param name="totalRotation"> Total rotation relative to initial rotation  </param>
        public RotationGestureEventArgs(int fingerCount, Vector2[] positions, float deltaRotation, float totalRotation)
            : base(fingerCount, positions)
        {
            this.DeltaRotation = deltaRotation;
            this.TotalRotation = totalRotation;
        }
    }

    /// <summary>
    /// Handle rotation gesture events
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Arguments</param>
    public delegate void RotationGestureEventHandler(object sender, RotationGestureEventArgs args);

    /// <summary>
    /// Detect rotation gesture
    /// </summary>
    public class RotationGestureDetector : LockerGestureDetector
    {
        private float _PreviousRotation;
        private float _DeltaRotation;
        private float _TotalRotation;
        private bool _IsRotationStarted;

        /// <summary> Minimum rotation to start detecting </summary>
        public float MinimumRotation { get; set; }

        /// <summary> Occurs when a rotation gesture detected </summary>
        public event RotationGestureEventHandler Rotate;
        /// <summary> Occurs when a rotation gesture detected </summary>
        protected virtual void OnRotate()
        {
            if (Rotate != null) Rotate(this, new RotationGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), _DeltaRotation, _TotalRotation));
        }

        /// <summary> Occurs when a rotation gesture started </summary>
        public event GestureEventHandler RotateStart;
        /// <summary> Occurs when a rotation gesture started </summary>
        protected virtual void OnRotateStart()
        {
            if (RotateStart != null)
            {
                RotateStart(this, new GestureEventArgs(FingerCount, GetPositionOfTrackingTouches()));
            }
        }

        /// <summary>
        /// Center of rotation on screen.
        /// null : detector use two finger that rotate relative together
        /// not null : detector use single finger that rotates around origin
        /// </summary>
        public Vector2? Origin { get; set; }

        /// <summary> Number of fingers </summary>
        public override int FingerCount
        {
            get { return (Origin != null && Origin.HasValue) ? 1 : 2; }
            set { }
        }

        /// <summary>
        /// Create a RotationGestureDetector
        /// </summary>        
        public RotationGestureDetector()            
        {
            _TotalRotation = _DeltaRotation = 0;
            MinimumRotation = 0.0f;
        }

        // calc angles between points
        private float AngleBetweenPoints()
        {
            Vector2 point1, point2;
            if (Origin != null && Origin.HasValue)
            {
                point1 = Origin.Value;
                point2 = GetTrackingToucheByIndex(0).Position;
            }
            else
            {
                point1 = GetTrackingToucheByIndex(0).Position;
                point2 = GetTrackingToucheByIndex(1).Position;
            }

            var fromLine = point2 - point1;
            var toLine = new Vector2(1, 0);

            var angle = Vector2.Angle(fromLine, toLine);
            var cross = Vector3.Cross(fromLine, toLine);

            // did we wrap around?
            if (cross.z > 0)
                angle = 360f - angle;

            return angle;
        }

        /// <summary>
        /// Begin detection
        /// </summary>
        protected override void BeginDetection()
        {
            _IsRotationStarted = false;
            _TotalRotation = _DeltaRotation = 0;
            _PreviousRotation = AngleBetweenPoints();
        }

        protected override GestureDetectionResult Detection()
        {
            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;

            if (!_IsRotationStarted && LockTouches)
                UnlockTrackingTouches(); // because no rotation hapened yet

            if (IsAnyTrackingTouchesMoved())
            {
                // find delta rotation at currect frame
                float currentRotation = AngleBetweenPoints();
                _DeltaRotation = Mathf.DeltaAngle(currentRotation, _PreviousRotation);

                if (!_IsRotationStarted)
                {
                    if (Mathf.Abs(_DeltaRotation) >= MinimumRotation)
                    {
                        if (LockTouches) // because we unlock them in few lines before
                            LockTrackingTouches();
                        _IsRotationStarted = true;
                        OnRotateStart();
                    }
                }
                if (_IsRotationStarted)
                {
                    _TotalRotation += _DeltaRotation;
                    _PreviousRotation = currentRotation;
                }
            }
            else
                _DeltaRotation = 0;

            if (IsAnyTrackingTouchesEnded())
            {
                if (_TotalRotation != 0)
                    return GestureDetectionResult.Detected;
                else
                    return GestureDetectionResult.Failed;
            }

            if (_DeltaRotation != 0)
                OnRotate();
            if (_TotalRotation != 0)
                return GestureDetectionResult.Detecting;

            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _IsRotationStarted = false;
            _TotalRotation = _DeltaRotation = 0;
            base.Reset();
        }
    }

}