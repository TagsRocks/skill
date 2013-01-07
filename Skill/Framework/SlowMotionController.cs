using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Defines basic information about slow motion
    /// </summary>
    [Serializable]
    public class SlowMotionInfo
    {
        /// <summary> Lenght of freez time at begining of slow motion.( Freez time is calculated as part of SlowMotion time ) </summary>
        public float Freez = 0.0f;
        /// <summary> Lenght of slow motion </summary>
        public float SlowMotion = 2.0f;
        /// <summary> Target TimeScale when slow motion </summary>
        public float TimeScale = 0.2f;
        /// <summary> Target sound pitch when slow motion </summary>
        public float Pitch = 0.5f;

        /// <summary> Default constructor </summary>
        public SlowMotionInfo() { }

        /// <summary>
        /// Create a copy of SlowMotionInfo
        /// </summary>
        /// <param name="other">Oter SlowMotionInfo to copy</param>
        public SlowMotionInfo(SlowMotionInfo other)
        {
            this.Freez = other.Freez;
            this.SlowMotion = other.SlowMotion;
            this.TimeScale = other.TimeScale;
            this.Pitch = other.Pitch;
        }
    }

    /// <summary>
    /// containing SlowMotion event data.
    /// </summary>
    public class SlowMotionEventArgs : EventArgs
    {
        /// <summary> SlowMotion information </summary>
        public SlowMotionInfo Motion { get; private set; }

        /// <summary>
        /// Create SlowMotionEventArgs
        /// </summary>
        /// <param name="motion"> SlowMotion information </param>
        public SlowMotionEventArgs(SlowMotionInfo motion)
        {
            this.Motion = motion;
        }
    }

    /// <summary>
    /// Handle SlowMotion
    /// </summary>    
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> A SlowMotionEventArgs containing SlowMotion event data </param>
    public delegate void SlowMotionEventHandler(object sender, SlowMotionEventArgs args);

    /// <summary>
    /// Modify TimeScale to simulate slowmotion
    /// </summary>
    [AddComponentMenu("Skill/Base/SlowMotionController")]
    public class SlowMotionController : DynamicBehaviour
    {

        /// <summary>
        /// The only instance of Global object in scene
        /// </summary>
        public static SlowMotionController Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (Instance != null)
                Debug.LogWarning("More thn on instance of Skill.SlowMotionController object found");
            Instance = this;
        }

        /// <summary>
        /// Whether game is in slow motion mode or not
        /// </summary>
        public bool IsSlowingMotion { get { return _FreezeTW.Enabled || _MotionTW.Enabled; } }

        private SlowMotionInfo _Info;
        private TimeWatch _MotionTW;
        private TimeWatch _FreezeTW;
        private bool _Paused;
        private float _TimeLeftWhenPaused;
        private float _FreezTimeLeftWhenPaused;

        /// <summary>
        /// Hook required events if needed
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            Global.SlowMotion += Global_SlowMotion;
        }

        /// <summary>
        /// Unhook hooked events 
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            Global.SlowMotion -= Global_SlowMotion;
        }

        private void Global_SlowMotion(object sender, SlowMotionEventArgs args)
        {
            if (_MotionTW.Enabled) return;
            if (args == null || args.Motion == null) return;

            _Info = args.Motion;
            _MotionTW.Begin(_Info.SlowMotion, true);
            if (_Info.Freez > 0)
                _FreezeTW.Begin(_Info.Freez, true);
            else
                _FreezeTW.End();
            enabled = true;
            OnStartSlowMotion();
        }

        /// <summary>
        /// Occurs when a SlowMotion started
        /// </summary>
        public event EventHandler StartSlowMotion;

        /// <summary>
        /// Occurs when a SlowMotion begined
        /// </summary>        
        protected virtual void OnStartSlowMotion()
        {
            if (StartSlowMotion != null)
                StartSlowMotion(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a SlowMotion ended
        /// </summary>
        public event EventHandler EndSlowMotion;

        /// <summary>
        /// Occurs when a SlowMotion ended
        /// </summary>        
        protected virtual void OnEndSlowMotion()
        {
            if (EndSlowMotion != null)
                EndSlowMotion(this, EventArgs.Empty);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            // it is possible to player pasue game when slowmotion
            // as we control slowmotion by realtime of game, we need to save how many times left from start of slowmotion to continue after resume game
            if (Time.timeScale == 0 && Global.IsGamePaused)
            {
                if (!_Paused)
                {
                    _TimeLeftWhenPaused = _MotionTW.OverTime - Time.realtimeSinceStartup;
                    _FreezTimeLeftWhenPaused = _FreezeTW.OverTime - Time.realtimeSinceStartup;
                }
                _Paused = true;
                return;
            }

            if (_Paused)
            {
                if (_TimeLeftWhenPaused > 0)
                    _MotionTW.Begin(_TimeLeftWhenPaused, true);
                if (_FreezTimeLeftWhenPaused > 0 && _Info.Freez > 0)
                    _FreezeTW.Begin(_FreezTimeLeftWhenPaused, true);
                _Paused = false;
                _TimeLeftWhenPaused = 0;
                _FreezTimeLeftWhenPaused = 0;
            }

            if (_MotionTW.Enabled)
            {
                if (_MotionTW.IsOver)
                {
                    if (Skill.Framework.Sounds.PitchController.Instance != null)
                        Skill.Framework.Sounds.PitchController.Instance.Pitch = 1.0f;
                    Time.timeScale = 1;
                    enabled = false;
                    _MotionTW.End();
                    _Info = null;
                    _TimeLeftWhenPaused = 0;
                    _FreezTimeLeftWhenPaused = 0;
                    OnEndSlowMotion();
                }
                else
                {
                    if (_FreezeTW.EnabledAndOver)
                        _FreezeTW.End();

                    if (Skill.Framework.Sounds.PitchController.Instance != null)
                        Skill.Framework.Sounds.PitchController.Instance.Pitch = _Info.Pitch;
                    if (!_FreezeTW.Enabled)
                    {
                        Time.timeScale = _Info.TimeScale;
                    }
                    else
                    {
                        Time.timeScale = 0;
                    }
                }
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }

            base.Update();
        }
    }
}
