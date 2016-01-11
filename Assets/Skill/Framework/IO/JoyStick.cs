using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.IO
{
    [ExecuteInEditMode]
    public class JoyStick : JoyElement
    {

        /// <summary> is joystick free, or is in certain rect</summary>
        public bool Free;
        /// <summary> detection area of joystick to screen </summary>
        public float DetectionScale = 1.5f;

        public float HorizontalRoundThreshold = 0.2f;
        public float VerticalRoundThreshold = 0.5f;

        private Rect _Boundary;
        private TouchState _Touch;
        private float _MaxOffset;

        private Vector2 _Delta;

        /// <summary> Delta position relative to center  </summary>
        public Vector2 Delta { get { return _Delta; } }

        /// <summary> Position of touch when down for first time  </summary>
        public Vector2 StartPosition { get; private set; }

        /// <summary> Normalized horizontal axis  </summary>
        public float HorizontalAxis { get; private set; }

        /// <summary> Normalized vertical axis  </summary>
        public float VerticalAxis { get; private set; }

        /// <summary> Rounded horizontal axis (-1,0,1) </summary>
        public float RoundedHorizontalAxis { get; private set; }

        /// <summary> Rounded vertical axis (-1,0,1) </summary>
        public float RoundedVerticalAxis { get; private set; }

        /// <summary> detection area of joystick in screen space </summary>
        public Rect Boundary { get { return _Boundary; } }

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying && InputManager.Instance == null)
                Debug.LogWarning("You must place an instance of InputManager in scene to make joystick works correctly");
        }

        protected override void Update()
        {
            if (Global.IsGamePaused || Global.CutSceneEnable) return;
            UpdateBoundary();

            _MaxOffset = (RenderArea.width + RenderArea.height) * 0.2f;

            if (_Touch != null)
            {
                if (_Touch.Phase == TouchPhase.Canceled || _Touch.Phase == TouchPhase.Ended)
                {
                    if (_Touch.IsLockedBy(this))
                        _Touch.UnLock(this);
                    _Touch = null;
                    _Delta = Vector3.zero;
                    HorizontalAxis = 0;
                    VerticalAxis = 0;
                    RoundedHorizontalAxis = 0;
                    RoundedVerticalAxis = 0;
                }
            }

            if (_Touch == null && InputManager.Instance != null)
            {
                foreach (var item in InputManager.Instance.GetFreeTouches(TouchPhase.Began, _Boundary))
                {
                    _Touch = item;
                    _Touch.Lock(this);
                    _Delta = Vector3.zero;
                    if (Free)
                        StartPosition = _Touch.Position;
                    else
                        StartPosition = RenderArea.center;
                    break;
                }
            }

            if (_Touch != null)
            {
                if (Free)
                {
                    RenderArea.x = StartPosition.x - RenderArea.width * 0.5f;
                    RenderArea.y = StartPosition.y - RenderArea.height * 0.5f;
                }


                if (Free)
                    _Delta += _Touch.DeltaPosition;
                else
                    _Delta = _Touch.Position - RenderArea.center;

                _Delta.x = Mathf.Clamp(_Delta.x, -_MaxOffset, _MaxOffset);
                _Delta.y = Mathf.Clamp(_Delta.y, -_MaxOffset, _MaxOffset);


                HorizontalAxis = _Delta.x / _MaxOffset;
                VerticalAxis = _Delta.y / _MaxOffset;
                RoundedHorizontalAxis = Round(HorizontalAxis, HorizontalRoundThreshold);
                RoundedVerticalAxis = Round(VerticalAxis, VerticalRoundThreshold);

            }
            else
            {
                HorizontalAxis = 0;
                VerticalAxis = 0;
                RoundedHorizontalAxis = 0;
                RoundedVerticalAxis = 0;
            }

            base.Update();
        }

        private void UpdateBoundary()
        {
            DetectionScale = Mathf.Max(1, DetectionScale);
            Vector2 center = RenderArea.center;
            _Boundary.width = RenderArea.width * DetectionScale;
            _Boundary.height = RenderArea.height * DetectionScale;
            _Boundary.x = center.x - _Boundary.width * 0.5f;
            _Boundary.y = center.y - _Boundary.height * 0.5f;
        }


        private float Round(float position, float threshold)
        {
            if (position > 0 && position > threshold)
                position = 1;
            else if (position < 0 && position < -threshold)
                position = -1;
            else
                position = 0;

            return position;
        }

#if UNITY_EDITOR
        protected override void OnGUI()
        {
            if (!Application.isPlaying)
            {
                UpdateBoundary();
                Rect renderArea = this.Boundary;
                renderArea.y = Screen.height - renderArea.y - renderArea.height;
                GUI.Box(renderArea, "Detection Area");
            }
            base.OnGUI();
            
        }
#endif
    }
}