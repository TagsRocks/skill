using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Result of GestureDetector
    /// </summary>
    public enum GestureDetectionResult
    {
        /// <summary> nothing </summary>
        None,
        /// <summary> detector was detecting but failed to complete detection </summary>
        Failed,
        /// <summary> detector is successfully detecting </summary>
        Detecting,
        /// <summary> detector successfully detecting  </summary>
        Detected
    }

    /// <summary>
    /// Event arguments of GestureDetector 
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary> Position of touches when event happened  </summary>
        public Vector2[] Positions { get; private set; }

        /// <summary> Number of fingers </summary>
        public int FingerCount { get; private set; }

        /// <summary>
        /// Create a GestureDetectorEventArgs
        /// </summary>    
        /// <param name="fingerCount"> Number of fingers </param>
        /// <param name="positions">Position of touches when event happened  </param>    
        public GestureEventArgs(int fingerCount, Vector2[] positions)
        {
            this.FingerCount = fingerCount;
            this.Positions = positions;
        }
    }

    /// <summary>
    /// Handle Gesture events
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> arguments </param>
    public delegate void GestureEventHandler(object sender, GestureEventArgs args);

    /// <summary>
    /// Base definition of gesture detectors
    /// </summary>
    public abstract class GestureDetector
    {
        private bool _IsEnabled;
        /// <summary> Is Detector enabled </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    if (!_IsEnabled)
                        Reset();
                }
            }
        }

        /// <summary> Frame that the touch must be within to be recognized. null means full screen. </summary>
        public Rect? BoundaryFrame { get; set; }

        /// <summary> Priority of detector</summary>
        public uint Priority { get; set; }

        /// <summary> Number of fingers required to detect gesture </summary>
        public virtual int FingerCount { get; set; }


        private List<TouchState> _TrackingTouches;

        /// <summary>
        /// Create a GestureDetector
        /// </summary>
        /// <param name="priority"> Priority of detector </param>
        protected GestureDetector()
        {
            _TrackingTouches = new List<TouchState>();
            this._IsEnabled = true;
            this.Priority = 0;
            this.FingerCount = 1;
            Reset();
        }

        /// <summary>
        /// Update detector
        /// </summary>
        /// <param name="provider">touch provider</param>
        /// <returns>result of detection</returns>
        public abstract GestureDetectionResult Update(ITouchStateProvider provider);

        /// <summary> Reset detector and unlock all locked touches </summary>
        public virtual void Reset()
        {
            UnlockTrackingTouches();
            _TrackingTouches.Clear();
        }

        /// <summary> Last result of detector </summary>
        public GestureDetectionResult LastReslut { get; internal set; }

        /// <summary>
        /// returns the location of the touches. If there are multiple touches this will return the centroid of the location.
        /// </summary>
        public Vector2 TouchLocation()
        {
            Vector2 location = Vector3.zero;
            if (_TrackingTouches.Count > 0)
            {
                for (int i = 0; i < _TrackingTouches.Count; i++)
                    location += _TrackingTouches[i].Position;
                return location / _TrackingTouches.Count;
            }
            return location;
        }

        internal void UseTouches()
        {
            foreach (var ts in _TrackingTouches)
                ts.Use();
        }

        /// <summary>
        /// Remove the tracking touches that is no more valid for this detector
        /// </summary>
        protected void RemoveInvalidTouches()
        {
            int index = 0;
            while (index < _TrackingTouches.Count)// check if any of previous touches became invalid
            {
                var ts = _TrackingTouches[index];
                if (!ts.IsValidFor(this))
                    _TrackingTouches.Remove(ts);
                else
                    index++;
            }
        }

        /// <summary> Lock tracking touches </summary>
        protected void LockTrackingTouches()
        {
            foreach (var touch in _TrackingTouches)
                touch.Lock(this);
        }

        /// <summary> unLock locked tracking touches by this detector </summary>
        protected void UnlockTrackingTouches()
        {
            foreach (var touch in _TrackingTouches)
            {
                if (touch.IsLockedBy(this))
                    touch.UnLock(this);
            }
        }

        /// <summary>
        /// Is phase of any tracking touches Phase.Moved
        /// </summary>
        /// <returns>True if moved, otherwise false</returns>
        protected bool IsAnyTrackingTouchesMoved()
        {
            foreach (var ts in _TrackingTouches)
            {
                if (ts.Phase == TouchPhase.Moved)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Is phase of any tracking touches Phase.Ended
        /// </summary>
        /// <returns>True if ended, otherwise false</returns>
        protected bool IsAnyTrackingTouchesEnded()
        {
            foreach (var ts in _TrackingTouches)
            {
                if (ts.Phase == TouchPhase.Ended)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add given TouchState to list of tracking touches and start track this touch
        /// </summary>
        /// <param name="ts">TouchState to track</param>
        protected void TrackTouch(TouchState ts)
        {
            _TrackingTouches.Add(ts);
        }

        /// <summary>
        /// Retrieves positions of tracking touches
        /// </summary>
        /// <returns>positions of tracking touches</returns>
        protected Vector2[] GetPositionOfTrackingTouches()
        {
            Vector2[] positions = new Vector2[_TrackingTouches.Count];
            for (int i = 0; i < _TrackingTouches.Count; i++)
                positions[i] = _TrackingTouches[i].Position;
            return positions;
        }

        /// <summary>
        /// Retrieve tracking touch by index
        /// </summary>
        /// <param name="index">Index of TrackingTouche</param>
        /// <returns>tracking touch</returns>
        protected TouchState GetTrackingToucheByIndex(int index)
        {
            return _TrackingTouches[index];
        }

        /// <summary> Number of tracking touches </summary>
        protected int TrackingToucheCount { get { return _TrackingTouches.Count; } }

        /// <summary>
        /// Calc distance between two tracking touch
        /// </summary>
        /// <param name="touch1">touch 1</param>
        /// <param name="touch2">touch 2</param>
        /// <returns> distance between touches </returns>
        protected float DistanceBetweenTrackedTouches(int touch1, int touch2)
        {
            return Vector2.Distance(_TrackingTouches[touch1].Position, _TrackingTouches[touch2].Position);
        }

        /// <summary>
        /// Is all tracking touches valid for this detector
        /// </summary>
        /// <returns>True if valid, otherwise false</returns>
        protected bool IsTrackingTouchesValid()
        {
            for (int i = 0; i < _TrackingTouches.Count; i++)
            {
                if (!_TrackingTouches[i].IsValidFor(this))
                    return false;
            }
            return true;
        }
    }


    /// <summary>
    /// Base class for gesture detector that need to lock touches while detecting
    /// </summary>
    public abstract class LockerGestureDetector : GestureDetector
    {
        /// <summary> found number of required touch </summary>
        protected abstract void BeginDetection();
        /// <summary>
        /// try to detect gestures every frame after begin called
        /// </summary>
        /// <returns>Result of detection</returns>
        protected abstract GestureDetectionResult Detection();

        /// <summary> Lock touches while detecting? </summary>
        public bool LockTouches { get; set; }

        /// <summary>
        /// Create a LockerGestureDetector
        /// </summary>        
        public LockerGestureDetector()            
        {
        }

        /// <summary>
        /// Update detector
        /// </summary>
        /// <param name="provider">touch provider</param>
        /// <returns>result of detection</returns>
        public override GestureDetectionResult Update(ITouchStateProvider provider)
        {
            if (TrackingToucheCount < FingerCount) // if do not get access to required touch try to get them
            {
                // remove pre touches that aquired but locked or used by another detector
                RemoveInvalidTouches();
                // lock tracking touches to get more free touches
                LockTrackingTouches();
                foreach (var t in provider.GetFreeTouches(TouchPhase.Began, BoundaryFrame))
                {
                    TrackTouch(t);
                    t.Lock(this);
                    if (TrackingToucheCount == FingerCount)
                        break;
                }
                if (TrackingToucheCount == FingerCount)// first time we get access to touches
                {
                    // we get access to required touch so begin detection
                    BeginDetection();
                }
                else
                {
                    // we do't have required touch to begin detection so unlock touches and let another detector to chance detection
                    UnlockTrackingTouches();
                    return GestureDetectionResult.None;
                }
            }

            GestureDetectionResult result = GestureDetectionResult.None;
            if (TrackingToucheCount == FingerCount)
                result = Detection();
            if (!LockTouches) // unlock touches if we do'nt need them
                UnlockTrackingTouches();
            return result;
        }
    }
}