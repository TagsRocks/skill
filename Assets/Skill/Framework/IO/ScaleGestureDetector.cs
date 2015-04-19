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
        /// <summary> Total scale relative to initial scale</summary>
        public float TotalScale { get; private set; }

        /// <summary>
        /// Create a HoldGestureEventArgs
        /// </summary>
        /// <param name="fingerCount">Number of fingers</param>
        /// <param name="positions"> Position of touches </param>        
        /// <param name="totalScale"> Total scale relative to initial scale </param>
        public ScaleGestureEventArgs(int fingerCount, Vector2[] positions, float totalScale)
            : base(fingerCount, positions)
        {
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
            if (Scale != null) Scale(this, new ScaleGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), _TotalScale));
        }

        /// <summary> Occurs when a scale gesture started </summary>
        public event GestureEventHandler ScaleStart;
        /// <summary> Occurs when a scale gesture started </summary>
        protected virtual void OnScaleStart()
        {
            if (ScaleStart != null)
            {
                Vector2[] positions = GetPositionOfTrackingTouches();
                ScaleStart(this, new GestureEventArgs(FingerCount, positions));
            }
        }

        /// <summary>
        /// Create a ScaleGestureDetector
        /// </summary>        
        public ScaleGestureDetector()
        {
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
            OnScaleStart();
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns>Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {

            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;


            bool scaleChanged = false;

            if (_IntialDistance >= 1 && IsAnyTrackingTouchesMoved())
            {
                // find delta rotation at currect frame
                float currentDistance = DistanceBetweenTrackedTouches(0, 1);
                if (currentDistance != _PreviousDistance)
                {
                    _TotalScale = currentDistance / _IntialDistance;
                    _PreviousDistance = currentDistance;
                    scaleChanged = true;
                }
            }

            if (IsAnyTrackingTouchesEnded())
            {
                if (_TotalScale != 0)
                    return GestureDetectionResult.Detected;
                else
                    return GestureDetectionResult.Failed;
            }

            if (scaleChanged)
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
            _TotalScale = 0;
            base.Reset();
        }
    }

}