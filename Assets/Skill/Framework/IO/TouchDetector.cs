using System;
using System.Collections.Generic;
using UnityEngine;
namespace Skill.Framework.IO
{

    /// <summary>
    /// Touch gesture detector 
    /// </summary>
    public class TouchGestureDetector : LockerGestureDetector
    {
        private Vector2 _PreviousLocation;
        private Vector2 _DeltaTranslation;
        private Vector2 _TotalTranslation;
        private bool _IsTouchStarted;

        /// <summary> position of pointers when touch started</summary>
        public Vector2[] TouchStartPoints { get; private set; }

        /// <summary> the time when touch started</summary>
        public float TouchStartTime { get; private set; }

        /// <summary> the time when touch ended</summary>
        public float TouchEndTime { get; private set; }


        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        /// <summary> Occurs when a drag gesture event detected </summary>
        public event DragGestureEventHandler Drag;
        /// <summary> Occurs when a drag gesture event detected </summary>
        protected virtual void OnDrag()
        {
            if (Drag != null) Drag(this, new DragGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), _DeltaTranslation, _TotalTranslation));
        }

        /// <summary> Occurs when a touch gesture started </summary>
        public event GestureEventHandler TouchStart;
        /// <summary> Occurs when a touch gesture started </summary>
        protected virtual void OnTouchStart()
        {
            TouchStartTime = Time.time;
            TouchStartPoints = GetPositionOfTrackingTouches();
            if (TouchStart != null)
            {
                TouchStart(this, new GestureEventArgs(FingerCount, TouchStartPoints));
            }
        }


        /// <summary> Occurs when a touch gesture ended </summary>
        public event GestureEventHandler TouchEnd;
        /// <summary> Occurs when a touch gesture ended </summary>
        protected virtual void OnTouchEnd()
        {
            TouchEndTime = Time.time;
            if (TouchEnd != null)
            {
                Vector2[] positions = GetPositionOfTrackingTouches();
                TouchEnd(this, new GestureEventArgs(FingerCount, positions));
            }
        }

        /// <summary>
        /// Create a TouchGestureDetector
        /// </summary>        
        public TouchGestureDetector()
        {
        }

        /// <summary>
        /// Begin detection
        /// </summary>
        protected override void BeginDetection()
        {
            _TotalTranslation = _DeltaTranslation = Vector2.zero;
            _PreviousLocation = TouchLocation();
            _IsTouchStarted = false;
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns>Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {
            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;

            if (!_IsTouchStarted && LockTouches)
                UnlockTrackingTouches(); // because no touch hapened yet

            if (!_IsTouchStarted)
            {
                if (LockTouches) // because we unlock them in few lines before
                    LockTrackingTouches();
                _IsTouchStarted = true;
                OnTouchStart();
            }

            if (_IsTouchStarted)
            {
                bool draged = false;
                if (IsAnyTrackingTouchesMoved())
                {
                    // find delta rotation at currect frame
                    Vector2 currentLocation = TouchLocation();
                    _DeltaTranslation = currentLocation - _PreviousLocation;

                    _TotalTranslation += _DeltaTranslation;
                    if (_DeltaTranslation.sqrMagnitude > 0)
                        draged = true;
                    _PreviousLocation = currentLocation;
                }
                else
                    _DeltaTranslation = Vector2.zero;

                if (draged)
                    OnDrag();

                if (IsAnyTrackingTouchesEnded())
                {
                    OnTouchEnd();
                    return GestureDetectionResult.Failed;
                }
                else
                    return GestureDetectionResult.Detecting;
            }
            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _IsTouchStarted = false;
            _TotalTranslation = _DeltaTranslation = Vector2.zero;
            base.Reset();
        }
    }

}