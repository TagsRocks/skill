using System;
using System.Collections.Generic;
using UnityEngine;
namespace Skill.Framework.IO
{
    /// <summary>
    /// DragGestureEventArgs 
    /// </summary>
    public class DragGestureEventArgs : GestureEventArgs
    {
        /// <summary> Delta translation since last event </summary>
        public Vector2 DeltaTranslation { get; private set; }
        /// <summary> total translation relative to start position </summary>
        public Vector2 TotalTranslation { get; private set; }

        /// <summary>
        /// Create a DragGestureEventArgs
        /// </summary>    
        /// <param name="fingerCount"> Number of fingers </param>
        /// <param name="positions">Position of touches when event happened  </param>  
        /// <param name="deltaTranslation">Delta translation since last event</param>
        /// <param name="totalTranslation"> total translation relative to start position  </param>
        public DragGestureEventArgs(int fingerCount, Vector2[] positions, Vector2 deltaTranslation, Vector2 totalTranslation)
            : base(fingerCount, positions)
        {
            this.DeltaTranslation = deltaTranslation;
            this.TotalTranslation = totalTranslation;
        }
    }

    /// <summary>
    /// Handle Drag Gesture events
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> arguments </param>
    public delegate void DragGestureEventHandler(object sender, DragGestureEventArgs args);

    /// <summary>
    /// Drag gesture detector 
    /// </summary>
    public class DragGestureDetector : LockerGestureDetector
    {
        private Vector2 _PreviousLocation;
        private Vector2 _DeltaTranslation;
        private Vector2 _TotalTranslation;
        private bool _IsDragStarted;

        /// <summary> position of pointers when drag started</summary>
        public Vector2[] DragStartPoints { get; private set; }

        /// <summary> the time when drag started</summary>
        public float DragStartTime { get; private set; }

        /// <summary> the time when drag ended</summary>
        public float DragEndTime { get; private set; }


        /// <summary> Minimum drag to start detecting </summary>
        public float MinimumDrag { get; set; }

        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        /// <summary> Occurs when a drag gesture event detected </summary>
        public event DragGestureEventHandler Drag;
        /// <summary> Occurs when a drag gesture event detected </summary>
        protected virtual void OnDrag()
        {
            if (Drag != null) Drag(this, new DragGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), _DeltaTranslation, _TotalTranslation));
        }

        /// <summary> Occurs when a drag gesture started </summary>
        public event GestureEventHandler DragStart;
        /// <summary> Occurs when a drag gesture started </summary>
        protected virtual void OnDragStart()
        {
            DragStartTime = Time.time;
            DragStartPoints = GetPositionOfTrackingTouches();
            if (DragStart != null)
            {
                for (int i = 0; i < DragStartPoints.Length; i++)// find start positions before drag 
                    DragStartPoints[i] -= _DeltaTranslation;
                DragStart(this, new GestureEventArgs(FingerCount, DragStartPoints));
            }
        }


        /// <summary> Occurs when a drag gesture ended </summary>
        public event GestureEventHandler DragEnd;
        /// <summary> Occurs when a drag gesture ended </summary>
        protected virtual void OnDragEnd()
        {
            DragEndTime = Time.time;
            if (DragEnd != null)
            {
                Vector2[] positions = GetPositionOfTrackingTouches();
                DragEnd(this, new GestureEventArgs(FingerCount, positions));
            }
        }

        /// <summary>
        /// Create a DragGestureDetector
        /// </summary>        
        public DragGestureDetector()
        {
        }

        /// <summary>
        /// Begin detection
        /// </summary>
        protected override void BeginDetection()
        {
            _TotalTranslation = _DeltaTranslation = Vector2.zero;
            _PreviousLocation = TouchLocation();
            _IsDragStarted = false;
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns>Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {
            if (!IsTrackingTouchesValid())
                return GestureDetectionResult.Failed;

            if (!_IsDragStarted && LockTouches)
                UnlockTrackingTouches(); // because no drag hapened yet

            bool draged = false;
            if (IsAnyTrackingTouchesMoved())
            {
                // find delta rotation at currect frame
                Vector2 currentLocation = TouchLocation();
                _DeltaTranslation = currentLocation - _PreviousLocation;

                if (!_IsDragStarted)
                {
                    if (_DeltaTranslation.magnitude >= MinimumDrag)
                    {
                        if (LockTouches) // because we unlock them in few lines before
                            LockTrackingTouches();
                        _IsDragStarted = true;
                        OnDragStart();
                    }
                }
                if (_IsDragStarted)
                {
                    _TotalTranslation += _DeltaTranslation;
                    if (_DeltaTranslation.sqrMagnitude > 0)
                        draged = true;
                    _PreviousLocation = currentLocation;

                }
            }
            else
                _DeltaTranslation = Vector2.zero;


            if (IsAnyTrackingTouchesEnded())
            {
                if (_TotalTranslation != Vector2.zero)
                {
                    OnDragEnd();
                    return GestureDetectionResult.Detected;
                }
                else
                    return GestureDetectionResult.Failed;
            }

            if (draged)
                OnDrag();
            if (_TotalTranslation != Vector2.zero)
                return GestureDetectionResult.Detecting;

            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _IsDragStarted = false;
            _TotalTranslation = _DeltaTranslation = Vector2.zero;
            base.Reset();
        }
    }

}