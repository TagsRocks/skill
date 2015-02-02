using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Manage gesture detectors
    /// </summary>
    public class InputManager : Skill.Framework.DynamicBehaviour, ITouchStateProvider
    {
        /// <summary> Maximum number of touch to process </summary>
        public int MaxTouchesToProcess = 2;

        /// <summary>
        /// This is how long to wait for input before all input data is expired.
        /// This prevents the player from performing half of a move, waiting, then
        /// performing the rest of the move after they forgot about the first half.
        /// </summary>
        public float BufferTimeOut = 0.5f;


        /// <summary>
        /// This is the size of the "merge window" for combining button presses that
        /// occur at almsot the same time.
        /// If it is too small, players will find it difficult to perform moves which
        /// require pressing several buttons simultaneously.
        /// If it is too large, players will find it difficult to perform moves which
        /// require pressing several buttons in sequence.
        /// </summary>
        public float MergeInputTime = 0.1f;



        private bool _NeedToSortGestures;
        private bool _NeedToSortMoves;
        private List<GestureDetector> _GestureDetectors = new List<GestureDetector>();
        private List<Move> _Moves = new List<Move>();
        private TouchState[] _TouchStates;
        private List<InputButton> _Buttons = new List<InputButton>();
        private List<int> _ButtonsBuffer = new List<int>();
        private bool _ShouldCheckForLostTouches;
        // The last "real time" that new input was received. Slightly late button
        // presses will not update this time; they are merged with previous input.    
        private float _LastInputTime;

        /// <summary> Single instance of Gestures </summary>
        public static InputManager Instance { get; private set; }

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            if (MaxTouchesToProcess < 1) MaxTouchesToProcess = 1;
            _TouchStates = new TouchState[MaxTouchesToProcess];
            for (int i = 0; i < MaxTouchesToProcess; i++)
                _TouchStates[i] = new TouchState(i);
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            UpdateTouchStates();

            if (_GestureDetectors.Count > 0)
                UpdateGestures();

            if (_Moves.Count > 0)
                UpdateMoves();
            base.Update();
        }

        private void UpdateMoves()
        {
            if (_NeedToSortMoves)
            {
                _Moves.Sort(new MoveComparer());
                _NeedToSortMoves = false;
                _ButtonsBuffer.Capacity = _Moves[0].Sequence.Length;
            }

            // Expire old input.        
            float timeSinceLast = Time.time - _LastInputTime;
            if (timeSinceLast > BufferTimeOut)
                _ButtonsBuffer.Clear();

            // It is very hard to press two buttons on exactly the same frame.
            // If they are close enough, consider them pressed at the same time.
            bool mergeInput = (_ButtonsBuffer.Count > 0 && timeSinceLast < MergeInputTime);

            // Get all of the non-direction buttons pressed.
            int buttons = 0;
            foreach (var btn in _Buttons)
            {
                if (btn.IsPressed)
                // Use a bitwise-or to accumulate button presses.
                {
                    buttons |= btn.Value;
                    if (btn.IsDirection)
                        mergeInput = false;
                }
            }

            // If there was any new input on this update, add it to the buffer.
            if (buttons != 0)
            {
                if (mergeInput)
                {
                    // Use a bitwise-or to merge with the previous input.
                    // LastInputTime isn't updated to prevent extending the merge window.
                    _ButtonsBuffer[_ButtonsBuffer.Count - 1] = _ButtonsBuffer[_ButtonsBuffer.Count - 1] | buttons;
                }
                else
                {
                    // Append this input to the buffer, expiring old input if necessary.
                    if (_ButtonsBuffer.Count == _ButtonsBuffer.Capacity)
                        _ButtonsBuffer.RemoveAt(0);

                    _ButtonsBuffer.Add(buttons);

                    // Record this the time of this input to begin the merge window.
                    _LastInputTime = Time.time;
                }


                // Perform a linear search for a move which matches the input. This relies
                // on the moves array being in order of decreasing sequence length.
                foreach (Move move in _Moves)
                {
                    // If the move is longer than the buffer, it can't possibly match.
                    if (_ButtonsBuffer.Count < move.Sequence.Length)
                        continue;

                    bool valid = true;
                    // Loop backwards to match against the most recent input.
                    for (int i = 1; i <= move.Sequence.Length; ++i)
                    {
                        if (_ButtonsBuffer[_ButtonsBuffer.Count - i] != move.Sequence[move.Sequence.Length - i])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        // Rnless this move is a component of a larger sequence,
                        if (!move.IsSubMove)
                        {
                            // consume the used inputs.
                            _ButtonsBuffer.Clear();
                        }
                        move.RaisePerform();
                    }
                }
            }
        }

        private void UpdateTouchStates()
        {
            if (Application.isEditor)
            {
                for (int i = 0; i < _TouchStates.Length; i++)
                    _TouchStates[i].UpdateFromMouse();
            }

            // get all touches and examine them. only do our touch processing if we have some touches
            if (Input.touchCount > 0)
            {
                _ShouldCheckForLostTouches = true;
                int maxTouchIndexToExamine = Mathf.Min(Input.touches.Length, MaxTouchesToProcess);
                for (var i = 0; i < maxTouchIndexToExamine; i++)
                {
                    var touch = Input.touches[i];
                    if (touch.fingerId < _TouchStates.Length)
                        _TouchStates[touch.fingerId].Update(touch);

                }
            }
            else
            {
                if (_ShouldCheckForLostTouches)
                {

                    for (int i = 0; i < _TouchStates.Length; i++)
                    {
                        if (_TouchStates[i].Phase != TouchPhase.Ended)
                        {
                            //Debug.LogWarning("found touch Unity forgot to end with phase: " + _TouchStates[i].Phase);
                            _TouchStates[i].Phase = TouchPhase.Ended;
                        }
                    }
                    _ShouldCheckForLostTouches = false;
                }
            }
        }

        private void UpdateGestures()
        {
            if (_NeedToSortGestures)
                RearrangeGestureDetectors();
            
            // first allow detecting gestures to chance detect
            foreach (var detector in _GestureDetectors)
            {
                if (detector.IsEnabled && detector.LastReslut == GestureDetectionResult.Detecting)
                {
                    detector.LastReslut = detector.Update(this);
                    if (detector.LastReslut != GestureDetectionResult.Detecting)
                    {
                        if (detector.LastReslut == GestureDetectionResult.Detected)
                            detector.UseTouches();
                        if (detector.LastReslut != GestureDetectionResult.None)
                        {
                            detector.Reset();
                            detector.LastReslut = GestureDetectionResult.None;
                        }
                    }
                }
            }
            // then allow the rest of detectors to detect
            foreach (var detector in _GestureDetectors)
            {
                if (detector.IsEnabled && detector.LastReslut != GestureDetectionResult.Detecting)
                {
                    detector.LastReslut = detector.Update(this);
                    if (detector.LastReslut != GestureDetectionResult.Detecting)
                    {
                        if (detector.LastReslut == GestureDetectionResult.Detected)
                            detector.UseTouches();
                        if (detector.LastReslut != GestureDetectionResult.None)
                        {
                            detector.Reset();
                            detector.LastReslut = GestureDetectionResult.None;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Call this if you change priority of a GestureDetector after add to InputManager
        /// </summary>
        public void RearrangeGestureDetectors()
        {
            Instance._GestureDetectors.Sort(new GestureDetectorComparer());
            _NeedToSortGestures = false;
        }


        /// <summary>
        /// Add a GestureDetector
        /// </summary>
        /// <param name="detector">GestureDetector to add</param>
        public static void Add(GestureDetector detector)
        {
            // add, then sort and reverse so the higher pPriority items will be on top
            Instance._GestureDetectors.Add(detector);
            Instance._NeedToSortGestures = true;
        }


        /// <summary>
        /// Remove GestureDetector
        /// </summary>
        /// <param name="detector">GestureDetector to remove</param>
        /// <returns>True if removed, otherwise false</returns>
        public static bool Remove(GestureDetector detector)
        {
            bool result = Instance._GestureDetectors.Remove(detector);
            if (result)
                detector.Reset();
            return result;
        }

        /// <summary>
        /// Remove all GestureDetectors
        /// </summary>
        public static void RemoveAllGestures()
        {
            foreach (var g in Instance._GestureDetectors)
            {
                if (g.IsEnabled)
                    g.Reset();
            }
            Instance._GestureDetectors.Clear();
        }

        class GestureDetectorComparer : IComparer<GestureDetector>
        {
            public int Compare(GestureDetector x, GestureDetector y)
            {
                // compare in descending order
                return (x.Priority > y.Priority) ? -1 : 1;
            }
        }

        class MoveComparer : IComparer<Move>
        {
            public int Compare(Move x, Move y)
            {
                // compare in descending order
                return (x.Sequence.Length > y.Sequence.Length) ? -1 : 1;
            }
        }

        /// <summary>
        /// enumorate throw touches and retrieves free touches with specified phase
        /// </summary>
        /// <param name="phase">Phase of free touch</param>
        /// <param name="boundaryFrame">Boundary frame (if available)</param>
        /// <returns>Free Touches</returns>
        public IEnumerable<TouchState> GetFreeTouches(TouchPhase phase, Rect? boundaryFrame)
        {
            for (int i = 0; i < _TouchStates.Length; i++)
            {
                var touch = _TouchStates[i];
                if (!touch.IsLocked && !touch.IsUsed && touch.Phase == phase)
                {
                    if (boundaryFrame != null && boundaryFrame.HasValue)
                        if (!boundaryFrame.Value.Contains(touch.Position))
                            continue;
                    yield return touch;
                }
            }
        }



        /// <summary>
        /// Add a Move
        /// </summary>
        /// <param name="move">Move to add</param>
        public static void Add(Move move)
        {
            Instance._Moves.Add(move);
            Instance._NeedToSortMoves = true;
        }


        /// <summary>
        /// Remove Move
        /// </summary>
        /// <param name="move">Move to remove</param>
        /// <returns>True if removed, otherwise false</returns>
        public static bool Remove(Move move)
        {
            return Instance._Moves.Remove(move);
        }

        /// <summary>
        /// Remove all Moves
        /// </summary>
        public static void RemoveAllMoves()
        {
            Instance._Moves.Clear();
        }

        /// <summary>
        /// Add a Button
        /// </summary>
        /// <param name="name">name of button</param>
        /// <param name="keys">Keys</param>
        public static InputButton CreateButton(string name, params KeyCode[] keys)
        {
            InputButton btn = new InputButton(name, keys);
            Instance._Buttons.Add(btn);
            return btn;
        }
    }

}