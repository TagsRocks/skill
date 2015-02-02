using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// HoldGestureEventArgs 
    /// </summary>
    public class HoldGestureEventArgs : GestureEventArgs
    {
        /// <summary> Duration of hold </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Create a HoldGestureEventArgs
        /// </summary>
        /// <param name="fingerCount">Number of fingers</param>
        /// <param name="positions"> Position of touches </param>
        /// <param name="duration"> Duration of hold </param>
        public HoldGestureEventArgs(int fingerCount, Vector2[] positions, float duration)
            : base(fingerCount, positions)
        {
            this.Duration = duration;
        }
    }

    /// <summary>
    /// Handle Hold Gesture Event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> arguments </param>
    public delegate void HoldGestureEventHandler(object sender, HoldGestureEventArgs args);

    /// <summary>
    /// Detector for holding touch for specific time
    /// </summary>
    public class HoldGestureDetector : LockerGestureDetector
    {
        /// <summary> Duration of hold to detect (default = 0.5f) </summary>
        public float Duration { get; set; }
        /// <summary> Movement allowed when holding touch</summary>
        public float AllowableMovement { get; set; }

        /// <summary> Number of fingers</summary>
        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        private Vector2 _BeginLocation;
        private bool _IsEventFired;
        private float _StartTime;

        /// <summary> Occuers when a hold detected</summary>
        public event HoldGestureEventHandler Hold;

        /// <summary> Occuers when a hold detected</summary>
        protected virtual void OnHold()
        {
            if (Hold != null)
                Hold(this, new HoldGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), Duration));
        }

        /// <summary>
        /// Create aHoldGestureDetector
        /// </summary>        
        public HoldGestureDetector()            
        {
            Duration = 0.5f;
            AllowableMovement = 10f;
            _IsEventFired = false;
        }

        /// <summary>
        /// Begin detection
        /// </summary>
        protected override void BeginDetection()
        {
            _IsEventFired = false;
            _BeginLocation = TouchLocation();
            _StartTime = Time.time;
        }

        /// <summary> detecting gestures </summary>
        /// <returns>Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {
            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;

            if (IsAnyTrackingTouchesMoved())
            {
                // did we move too far?
                var moveDistance = Vector2.Distance(TouchLocation(), _BeginLocation);
                if (moveDistance > AllowableMovement)
                    return GestureDetectionResult.Failed;
            }

            if (IsAnyTrackingTouchesEnded())
            {
                if (_IsEventFired) return GestureDetectionResult.Detected;
                else return GestureDetectionResult.Failed;
            }

            if ((Time.time - _StartTime) > Duration)
            {
                if (!_IsEventFired)
                    OnHold();
                _IsEventFired = true;
                return GestureDetectionResult.Detecting;
            }
            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset detector
        /// </summary>
        public override void Reset()
        {
            this._IsEventFired = false;
            base.Reset();
        }
    }

}