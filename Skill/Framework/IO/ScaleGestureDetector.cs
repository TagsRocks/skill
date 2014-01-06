using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{

    /// <summary>
    /// ScaleGestureEventArgs
    /// </summary>
    public class ScaleGestureEventArgs : GestureEventArgs
    {
        /// <summary> Delta scale relative to previous scale</summary>
        public float DeltaScale { get; private set; }
        /// <summary> Total scale relative to initial scale</summary>
        public float TotalScale { get; private set; }

        /// <summary>
        /// Create a HoldGestureEventArgs
        /// </summary>
        /// <param name="fingerCount">Number of fingers</param>
        /// <param name="positions"> Position of touches </param>
        /// <param name="deltaScale"> Delta scale relative to previous scale </param>
        /// <param name="totalScale"> Total scale relative to initial scale </param>
        public ScaleGestureEventArgs(int fingerCount, Vector2[] positions, float deltaScale, float totalScale)
            : base(fingerCount, positions)
        {
            this.DeltaScale = deltaScale;
            this.TotalScale = totalScale;
        }
    }

    /// <summary>
    /// Handle scale gesture event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> Arguments </param>
    public delegate void ScaleGestureEventHandler(object sender, ScaleGestureEventArgs args);

    /// <summary>
    /// Detector for scale
    /// </summary>
    public class ScaleGestureDetector : LockerGestureDetector
    {
        private float _DeltaScale;
        private float _TotalScale;
        private float _IntialDistance;
        private float _PreviousDistance;

        /// <summary>
        /// Number of fingers
        /// </summary>
        public override int FingerCount
        {
            get { return 2; }
            set { }
        }

        /// <summary> Occurs when scale event detected </summary>
        public event ScaleGestureEventHandler Scale;
        /// <summary> Occurs when scale event detected </summary>
        protected virtual void OnScale()
        {
            if (Scale != null) Scale(this, new ScaleGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), _DeltaScale, _TotalScale));
        }

        /// <summary>
        /// Create a ScaleGestureDetector
        /// </summary>        
        public ScaleGestureDetector()            
        {
            _DeltaScale = 0;
            _TotalScale = 0;
            _IntialDistance = 0;
            _PreviousDistance = 0;
        }

        /// <summary>
        /// Begin Detection
        /// </summary>
        protected override void BeginDetection()
        {
            _IntialDistance = DistanceBetweenTrackedTouches(0, 1);
            _PreviousDistance = _IntialDistance;
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns>Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {

            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;


            if (IsAnyTrackingTouchesMoved())
            {
                // find delta rotation at currect frame
                float currentDistance = DistanceBetweenTrackedTouches(0, 1);
                _DeltaScale = (currentDistance - _PreviousDistance) / _IntialDistance;
                _TotalScale += _DeltaScale;
                _PreviousDistance = currentDistance;
            }
            else
                _DeltaScale = 0;

            if (IsAnyTrackingTouchesEnded())
            {
                if (_TotalScale != 0)
                    return GestureDetectionResult.Detected;
                else
                    return GestureDetectionResult.Failed;
            }

            if (_DeltaScale != 0)
                OnScale();
            if (_TotalScale != 0)
                return GestureDetectionResult.Detecting;

            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _TotalScale = _DeltaScale = 0;
            base.Reset();
        }
    }

}