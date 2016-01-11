using UnityEngine;
using System.Collections;
using Skill.Framework;


namespace Skill.Framework.IO
{
    [ExecuteInEditMode]
    public class JoyButton : JoyElement
    {
        /// <summary> Control when position is out of detection  </summary>
        public float DeadRadius = 200;

        private TouchState _Touch;

        /// <summary> Returns true during the frame the user releases the button </summary>
        public bool IsReleased { get; private set; }

        /// <summary> Returns true during the frame the user starts pressing down the button </summary>
        public bool IsPressed { get; private set; }

        /// <summary> Returns true while the user holds down the Button </summary>
        public bool IsDown { get; private set; }

        /// <summary> Radius of button in pixel</summary>
        public float Radius { get; private set; }

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying && InputManager.Instance == null)
                Debug.LogWarning("You must place an instance of InputManager in scene to make JoyButton works correctly");
        }

        protected override void Update()
        {
            if (Global.IsGamePaused || Global.CutSceneEnable) return;
            IsReleased = false;
            IsDown = false;
            IsPressed = false;


            if (_Touch != null)
            {
                if (_Touch.Phase == TouchPhase.Canceled || _Touch.Phase == TouchPhase.Ended)
                {
                    _Touch = null;
                    IsReleased = true;
                }
            }

            if (_Touch == null && InputManager.Instance != null)
            {
                Radius = (RenderArea.width + RenderArea.height) * 0.22f;
                _Touch = FindTouch(TouchPhase.Began);
                if (_Touch == null) _Touch = FindTouch(TouchPhase.Moved);
                if (_Touch != null)
                {
                    IsPressed = true;
                    IsDown = true;
                }
            }

            if (_Touch != null)
            {
                if (_Touch.Phase == TouchPhase.Canceled || _Touch.Phase == TouchPhase.Ended || Vector2.Distance(RenderArea.center, _Touch.Position) > DeadRadius)
                {
                    _Touch = null;
                    IsDown = false;
                    IsPressed = false;
                    IsReleased = true;
                }
                else
                {
                    IsDown = true;
                }
            }
            base.Update();
        }

        private TouchState FindTouch(TouchPhase phase)
        {
            TouchState ts = null;
            foreach (var item in InputManager.Instance.GetFreeTouches(phase, null))
            {
                Vector2 pos = item.Position;
                if (Vector2.Distance(RenderArea.center, pos) <= Radius)
                {
                    ts = item;
                    break;
                }
            }
            return ts;
        }

    }
}