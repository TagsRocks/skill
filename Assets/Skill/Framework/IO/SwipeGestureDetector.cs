using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Framework.IO
{
    /// <summary>
    /// Swie direction
    /// </summary>
    public enum SwipeDirection
    {
        /// <summary> None </summary>
        None = 0,
        /// <summary> Left </summary>
        Left = (1 << 0),
        /// <summary> Right </summary>
        Right = (1 << 1),
        /// <summary> Up </summary>
        Up = (1 << 2),
        /// <summary> Down </summary>
        Down = (1 << 4),
        /// <summary> Left or Right </summary>
        Horizontal = (Left | Right),
        /// <summary> Up or Down </summary>
        Vertical = (Up | Down),
        /// <summary> Left or Right or Up or Down </summary>
        AnyDirection = (Horizontal | Vertical)
    }

    /// <summary>
    /// SwipeGestureEventArgs 
    /// </summary>
    public class SwipeGestureEventArgs : GestureEventArgs
    {
        /// <summary> Velocity of swipe </summary>
        public float Velocity { get; private set; }
        /// <summary> Swipe direction  </summary>
        public SwipeDirection Direction { get; private set; }

        /// <summary>
        /// Create a SwipeGestureEventArgs
        /// </summary>    
        /// <param name="fingerCount"> Number of fingers </param>
        /// <param name="positions">Position of touches when event happened  </param>          
        /// <param name="direction"> Swipe direction </param>
        /// <param name="swipeVelocity"> Velocity of swipe </param>
        public SwipeGestureEventArgs(int fingerCount, Vector2[] positions, SwipeDirection direction, float swipeVelocity)
            : base(fingerCount, positions)
        {
            this.Direction = direction;
            this.Velocity = swipeVelocity;
        }
    }

    /// <summary>
    /// Handle Swipe Gesture events
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args"> arguments </param>
    public delegate void SwipeGestureEventHandler(object sender, SwipeGestureEventArgs args);

    /// <summary>
    /// Detect swipe gesture
    /// </summary>
    public class SwipeGestureDetector : LockerGestureDetector
    {

        private float _TimeToSwipe;
        /// <summary> Maximum allowed time to swipe (default :  0.5f) </summary>
        public float TimeToSwipe { get { return _TimeToSwipe; } set { _TimeToSwipe = Mathf.Max(0, value); } }


        private float _AllowedVariance;
        /// <summary> Maximum allowed variance in perpendicular direction (default : 35.0f) </summary>
        public float AllowedVariance { get { return _AllowedVariance; } set { _AllowedVariance = Mathf.Max(0, value); } }

        private float _MinimumDistance;
        /// <summary> Minimum distance to swipe </summary>
        public float MinimumDistance { get { return _MinimumDistance; } set { _MinimumDistance = Mathf.Max(0, value); } }
        /// <summary>
        /// Number of finger
        /// </summary>
        public override int FingerCount { get { return base.FingerCount; } set { base.FingerCount = Math.Max(1, value); } }

        /// <summary> Detected swipe drection </summary>
        public SwipeDirection DetectedSwipeDirection { get; private set; }

        private SwipeDirection _SwipesToDetect;
        /// <summary> What swipe direction to detect </summary>
        public SwipeDirection SwipesToDetect
        {
            get { return _SwipesToDetect; }
            set
            {
                if (value == SwipeDirection.None)
                    throw new ArgumentException("Invalid SwipeDirection to detect");
                _SwipesToDetect = value;
            }
        }

        private float _StartTime;
        private SwipeDirection _SwipeDetectionState; // the current swipes that are still possibly valid    
        private Vector2 _StartPoint;

        /// <summary> Occurs when a swipe gesture detected </summary>
        public event SwipeGestureEventHandler Swipe;
        /// <summary> Occurs when a swipe gesture detected </summary>
        protected virtual void OnSwipe(SwipeDirection direction, float swipeVelocity)
        {
            if (Swipe != null) Swipe(this, new SwipeGestureEventArgs(FingerCount, GetPositionOfTrackingTouches(), direction, swipeVelocity));
        }

        /// <summary>
        /// Create a SwipeGestureDetector
        /// </summary>        
        public SwipeGestureDetector()            
        {
            _TimeToSwipe = 0.5f;
            AllowedVariance = 35.0f;
            MinimumDistance = 40.0f;
            _SwipesToDetect = SwipeDirection.AnyDirection;
        }

        private bool CheckForEvent(SwipeDirection dirToCheck, float deltaPosition, float variance)
        {
            if ((_SwipeDetectionState & dirToCheck) != 0)
            {
                if (deltaPosition > _MinimumDistance)
                {
                    if (variance <= _AllowedVariance)
                    {
                        DetectedSwipeDirection = dirToCheck;
                        OnSwipe(dirToCheck, deltaPosition / (Time.time - _StartTime));
                        return true;
                    }

                    // We exceeded our variance so this swipe is no longer allowed
                    _SwipeDetectionState &= ~dirToCheck;
                }
            }
            return false;
        }

        /// <summary> Begin detection </summary>
        protected override void BeginDetection()
        {
            _SwipeDetectionState = _SwipesToDetect;
            _StartPoint = TouchLocation();
            _StartTime = Time.time;
        }

        /// <summary>
        /// Detection
        /// </summary>
        /// <returns>Result of detection </returns>
        protected override GestureDetectionResult Detection()
        {
            // if we have a time stipulation and we exceeded it stop listening for swipes
            if ((Time.time - _StartTime) > _TimeToSwipe)
                return GestureDetectionResult.Failed;

            for (int i = 0; i < FingerCount; i++)
            {
                TouchState ts = GetTrackingToucheByIndex(i);

                if (!ts.IsValidFor(this))
                    return GestureDetectionResult.Failed;
                // check the delta move positions.  We can rule out at least 2 directions
                if (ts.DeltaPosition.x > 0.0f)
                    _SwipeDetectionState &= ~SwipeDirection.Left;
                if (ts.DeltaPosition.x < 0.0f)
                    _SwipeDetectionState &= ~SwipeDirection.Right;

                if (ts.DeltaPosition.y < 0.0f)
                    _SwipeDetectionState &= ~SwipeDirection.Up;
                if (ts.DeltaPosition.y > 0.0f)
                    _SwipeDetectionState &= ~SwipeDirection.Down;
            }

            // Grab the total distance moved in both directions
            Vector2 deltaLocation = _StartPoint - TouchLocation();
            deltaLocation.x = Mathf.Abs(deltaLocation.x);
            deltaLocation.y = Mathf.Abs(deltaLocation.y);


            if (CheckForEvent(SwipeDirection.Left, deltaLocation.x, deltaLocation.y)) // left check                            
                return GestureDetectionResult.Detected;
            else if (CheckForEvent(SwipeDirection.Right, deltaLocation.x, deltaLocation.y)) // right check
                return GestureDetectionResult.Detected;
            else if (CheckForEvent(SwipeDirection.Up, deltaLocation.y, deltaLocation.x)) // up check
                return GestureDetectionResult.Detected;
            else if (CheckForEvent(SwipeDirection.Down, deltaLocation.y, deltaLocation.x)) // down check
                return GestureDetectionResult.Detected;

            return GestureDetectionResult.None;
        }
    }

}