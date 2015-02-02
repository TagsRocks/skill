using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.IO
{
    /// <summary>
    /// State of touch
    /// </summary>
    public class TouchState
    {
        /// <summary> The unique index for the touch. </summary>
        public int FingerId { get; private set; }
        /// <summary> The position of the touch in pixel coordinates. </summary>
        public Vector2 Position { get; set; }
        /// <summary> The position delta since last change. </summary>
        public Vector2 DeltaPosition { get; set; }
        /// <summary>  Amount of time that has passed since the last recorded change in Touch values. </summary>
        public float DeltaTime { get; set; }
        /// <summary> Number of taps. </summary>
        public int TapCount { get; set; }
        /// <summary> Describes the phase of the touch. </summary>
        public TouchPhase Phase { get; set; }

        /// <summary>
        /// Create a TouchState
        /// </summary>
        /// <param name="fingerId"> The unique index for the touch. </param>
        public TouchState(int fingerId)
        {
            // lock this touch to the fingerId
            this.FingerId = fingerId;
            Phase = TouchPhase.Ended;
        }

        /// <summary>
        /// Update state from a touch
        /// </summary>
        /// <param name="touch">Touch</param>
        public void Update(Touch touch)
        {
            this.Position = touch.position;
            this.DeltaPosition = touch.deltaPosition;
            this.DeltaTime = touch.deltaTime;
            this.TapCount = touch.tapCount;
            this.Phase = touch.phase;

            this.IsUsed = false;
        }

        /// <summary>
        /// update from Input.GetTouch(FingerId)
        /// </summary>
        public void Update()
        {
            if (Input.touchCount > FingerId)
                Update(Input.GetTouch(FingerId));
        }

        // used to track mouse movement and fake touches
        private static Vector2? _LastMousePosition;
        private double _LastClickTime;
        private double _MultipleClickInterval = 0.2;

        /// <summary> Update from mouse input </summary>
        /// <remarks>
        /// seperating this out into a seperate method allows us to pass in a real mousePosition or a simulated mouse position when populating the touch
        /// </remarks>
        public void UpdateFromMouse(Vector3 mousePosition)
        {
            // do we have some input to work with?
            if (Input.GetMouseButtonUp(FingerId) || Input.GetMouseButton(FingerId))
            {
                var currentMousePosition = new Vector2(mousePosition.x, mousePosition.y);

                // if we have a lastMousePosition use it to get a delta
                if (_LastMousePosition.HasValue)
                    DeltaPosition = currentMousePosition - _LastMousePosition.Value;

                if (Input.GetMouseButtonDown(FingerId))
                {
                    this.Phase = TouchPhase.Began;
                    _LastMousePosition = Input.mousePosition;

                    // check for multiple clicks
                    if (Time.time < _LastClickTime + _MultipleClickInterval)
                        TapCount++;
                    else
                        TapCount = 1;
                    _LastClickTime = Time.time;
                }
                else if (Input.GetMouseButtonUp(FingerId))
                {
                    Phase = TouchPhase.Ended;
                    _LastMousePosition = null;
                }
                else if (Input.GetMouseButton(FingerId))
                {
                    Phase = TouchPhase.Moved;
                    _LastMousePosition = mousePosition;
                }

                Position = currentMousePosition;
            }
            this.IsUsed = false;
        }

        /// <summary>
        /// Update from mouse input
        /// </summary>
        public void UpdateFromMouse()
        {
            UpdateFromMouse(Input.mousePosition);
        }

        /// <summary>
        /// Represent state as string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("FingerId: {0}, Phase: {1}, Position: {2}, IsLocked: {3}, IsUsed: {4}", this.FingerId, this.Phase, this.Position, this.IsLocked, this.IsUsed);
        }

        /// <summary> Is state locked by a detector </summary>
        public bool IsLocked { get; private set; }
        private object _Locker = null;// the object that locked this touch
        /// <summary>
        /// Lock touch and get exclusive access
        /// </summary>
        /// <param name="locker">who wants to lock touch</param>
        public void Lock(object locker)
        {
            if (IsLocked) throw new InvalidOperationException("Can not lock TouchState more than once");
            IsLocked = true;
            _Locker = locker;
        }

        /// <summary>
        /// Unlock touch by locker
        /// </summary>
        /// <param name="locker">who locked the touch</param>
        public void UnLock(object locker)
        {
            if (!IsLocked) throw new InvalidOperationException("Can not unlock TouchState more than once");
            if (locker == this._Locker)
            {
                IsLocked = false;
                locker = null;
            }
            else
                throw new ArgumentException("Invalid locker");
        }

        /// <summary> Is touch used at current frame. touch will be unused at next update </summary>
        public bool IsUsed { get; private set; }
        /// <summary> Set touch as used </summary>
        public void Use() { IsUsed = true; }

        /// <summary>
        /// Is touch locked by specified object
        /// </summary>
        /// <param name="locker">object that maybe locked this touch</param>
        /// <returns>True if locked, otherwise false</returns>
        public bool IsLockedBy(object locker)
        {
            return IsLocked && _Locker == locker;
        }

        /// <summary>
        /// Is touch valid to use by specified object
        /// </summary>
        /// <param name="locker">object that maybe locked this touch</param>
        /// <returns>True if valid, otherwise false</returns>
        public bool IsValidFor(object locker)
        {
            return !(IsUsed || (IsLocked && _Locker != locker) || Phase == TouchPhase.Canceled);
        }
    }

}