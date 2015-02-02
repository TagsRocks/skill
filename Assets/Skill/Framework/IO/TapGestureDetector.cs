using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// TapGestureEventArgs 
    /// </summary>
    public class TapGestureEventArgs : GestureEventArgs
    {
        /// <summary> Number of tap </summary>
        public int TapCount { get; private set; }

        /// <summary>
        /// Create a TapGestureEventArgs
        /// </summary>
        /// <param name="fingerCount"> Number of fingers </param>
        /// <param name="positions">Position of touches when event happened  </param>  
        /// <param name="tapCount">Number of touches</param>
        public TapGestureEventArgs(int fingerCount, Vector2[] positions, int tapCount)
            : base(fingerCount, positions)
        {
            this.TapCount = tapCount;
        }
    }
    /// <summary>
    /// Handle Tap Gesture events
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> arguments </param>
    public delegate void TapGestureEventHandler(object sender, TapGestureEventArgs args);


    /// <summary>
    /// Detect tap gesture. use unity result touch.tapcount
    /// </summary>
    public class TapGestureDetectorUnity : GestureDetector
    {
        private int _TapCount;

        /// <summary> Number of required tap count </summary>
        public int TapCount { get { return _TapCount; } set { _TapCount = Math.Max(1, value); } }
        /// <summary> Number of required touches </summary>
        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        /// <summary>
        /// Create a TapGestureDetector
        /// </summary>        
        public TapGestureDetectorUnity()            
        {
            this._TapCount = 1;
        }

        /// <summary> Occurs when a tap detected </summary>
        public event TapGestureEventHandler Tap;
        /// <summary> Occurs when a tap detected </summary>
        protected virtual void OnDetect()
        {
            if (Tap != null) Tap(this, new TapGestureEventArgs(this.FingerCount, GetPositionOfTrackingTouches(), this.TapCount));
        }

        /// <summary>
        /// Update detector
        /// </summary>
        /// <param name="provider">Touch provider</param>
        /// <returns>Result of detection</returns>
        public override GestureDetectionResult Update(ITouchStateProvider provider)
        {
            int fc = 0;
            foreach (var t in provider.GetFreeTouches(TouchPhase.Began, BoundaryFrame))
            {
                if (t.TapCount == TapCount)
                {
                    TrackTouch(t);
                    fc++;
                }
                if (fc == FingerCount)
                {
                    OnDetect();
                    return GestureDetectionResult.Detected;
                }
            }
            if (fc > 0)
                return GestureDetectionResult.Failed;
            return GestureDetectionResult.None;
        }
    }


    /// <summary>
    /// Detect tap gesture
    /// </summary>
    public class TapGestureDetector : LockerGestureDetector
    {
        private int _TapCount;

        /// <summary> Number of required tap count </summary>
        public int TapCount { get { return _TapCount; } set { _TapCount = Math.Max(1, value); } }
        /// <summary> Number of required touches </summary>
        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        private float _MaxTapInterval;
        /// <summary> Maximum time allowd to detect a tap</summary>
        public float MaxTapInterval
        {
            get { return _MaxTapInterval; }
            set
            {
                _MaxTapInterval = value;
                if (_MaxTapInterval < 0)
                    _MaxTapInterval = 0;
            }
        }

        private float _MaxDeltaMovement;
        /// <summary> Maximum delta movement for tap consideration </summary>
        public float MaxDeltaMovement
        {
            get { return _MaxDeltaMovement; }
            set
            {
                _MaxDeltaMovement = value;
                if (_MaxDeltaMovement < 0)
                    _MaxDeltaMovement = 0;
            }
        }

        /// <summary>
        /// Create a TapGestureDetector
        /// </summary>        
        public TapGestureDetector()            
        {
            this._TapCount = 1;
            _MaxTapInterval = 0.5f;
            _MaxDeltaMovement = 0.5f;
        }

        private int _NumberOfDetectedTap;
        private Skill.Framework.TimeWatch _TapIntervalTW;
        private TouchPhase[] _Phases;
        private TouchPhase _DesiredPhase;

        /// <summary> Occurs when a tap detected </summary>
        public event TapGestureEventHandler Tap;

        /// <summary> Occurs when a tap detected </summary>
        protected virtual void OnTap()
        {
            if (Tap != null) Tap(this, new TapGestureEventArgs(this.FingerCount, GetPositionOfTrackingTouches(), this.TapCount));
        }

        /// <summary>
        /// Begin detection
        /// </summary>
        protected override void BeginDetection()
        {
            _TapIntervalTW.Begin(MaxTapInterval);
            if (_Phases == null || _Phases.Length != FingerCount)
                _Phases = new TouchPhase[FingerCount];
            for (int i = 0; i < FingerCount; i++)
                _Phases[i] = TouchPhase.Began;
            _DesiredPhase = TouchPhase.Ended;
        }

        private bool IsAllDesiredPhase()
        {
            for (int i = 0; i < FingerCount; i++)
                if (_Phases[i] != _DesiredPhase)
                    return false;
            return true;
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns> Result of detection</returns>
        protected override GestureDetectionResult Detection()
        {

            // if we are late to detect another tap ignore previous results
            if (_TapIntervalTW.IsEnabledAndOver)
                return GestureDetectionResult.Failed;

            // check if tracking touches are valid
            for (int i = 0; i < TrackingToucheCount; i++)
            {
                TouchState touch = GetTrackingToucheByIndex(i);
                if (!touch.IsValidFor(this) || (touch.Phase == UnityEngine.TouchPhase.Moved && touch.DeltaPosition.magnitude >= MaxDeltaMovement))
                    return GestureDetectionResult.Failed;
                if (_Phases[i] != _DesiredPhase)
                    _Phases[i] = touch.Phase;
            }

            if (IsAllDesiredPhase())
            {
                if (_DesiredPhase == TouchPhase.Ended)
                {
                    _NumberOfDetectedTap++;
                    if (_NumberOfDetectedTap == TapCount)
                    {
                        OnTap();
                        return GestureDetectionResult.Detected;
                    }
                    else
                        _TapIntervalTW.Begin(MaxTapInterval);
                    _DesiredPhase = TouchPhase.Began;
                }
                else if (_DesiredPhase == TouchPhase.Began)
                {
                    _DesiredPhase = TouchPhase.Ended;
                }
            }

            return GestureDetectionResult.None;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _TapIntervalTW.End();
            _NumberOfDetectedTap = 0;
            base.Reset();
        }
    }

}