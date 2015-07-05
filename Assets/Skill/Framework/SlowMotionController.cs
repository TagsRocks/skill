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
        /// <summary> duration of freez time at begining of slow motion.( Freez time is calculated as part of SlowMotion time ) </summary>
        public float Freez = 0.0f;
        /// <summary> duration of slow motion </summary>
        public float Duration = 2.0f;
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
            CopyFrom(other);
        }

        /// <summary>
        /// Create a copy of SlowMotionInfo
        /// </summary>
        /// <param name="other">Oter SlowMotionInfo to copy</param>
        public void CopyFrom(SlowMotionInfo other)
        {
            this.Freez = other.Freez;
            this.Duration = other.Duration;
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

            TimeScale = 1.0f;
        }

        /// <summary>
        /// Whether game is in slow motion mode or not
        /// </summary>
        public bool IsSlowingMotion { get { return _MotionTW.IsEnabled; } }

        public float TimeScale { get; private set; }

        /// <summary> Time left to end of slow motion </summary>
        public float TimeLeft
        {
            get
            {
                if (_MotionTW.IsEnabled)
                    return _MotionTW.TimeLeft;
                else
                    return 0;
            }
        }

        /// <summary> percen of slow motion if enabled </summary>
        public float Percent
        {
            get
            {
                if (_MotionTW.IsEnabled)
                    return _MotionTW.Percent;
                else
                    return 1.0f;
            }
        }

        private SlowMotionInfo _Info;
        private TimeWatch _MotionTW;
        private TimeWatch _FreezeTW;
        private bool _Paused;
        private float _TimeLeftWhenPaused;
        private float _FreezTimeLeftWhenPaused;
        private float _PreFixedDeltaTime;
        private float _PreTimeScale;


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
            if (_MotionTW.IsEnabled) return;
            if (args == null || args.Motion == null) return;

            if (_Info == null) _Info = new SlowMotionInfo();
            _Info.CopyFrom(args.Motion);
            _MotionTW.Begin(_Info.Duration, true);

            if (_Info.Freez > _Info.Duration)
                _Info.Freez = _Info.Duration;
            if (_Info.Freez > 0)
                _FreezeTW.Begin(_Info.Freez, true);
            else
                _FreezeTW.End();
            enabled = true;
            _PreFixedDeltaTime = Time.fixedDeltaTime;
            _PreTimeScale = Time.timeScale;
            TimeScale = _Info.TimeScale;
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
            if (Global.IsGamePaused)
            {
                if (!_Paused)
                {
                    _TimeLeftWhenPaused = _MotionTW.OverTime - Time.realtimeSinceStartup;
                    _FreezTimeLeftWhenPaused = _FreezeTW.OverTime - Time.realtimeSinceStartup;
                }
                _Paused = true;
            }
            else
            {
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

                if (_MotionTW.IsEnabled)
                {
                    if (_MotionTW.IsOver)
                    {
                        if (Skill.Framework.Audio.PitchController.Instance != null)
                            Skill.Framework.Audio.PitchController.Instance.Pitch = 1.0f;
                        TimeScale = Time.timeScale = _PreTimeScale;
                        Time.fixedDeltaTime = _PreFixedDeltaTime;
                        enabled = false;
                        _MotionTW.End();
                        _Info = null;
                        _TimeLeftWhenPaused = 0;
                        _FreezTimeLeftWhenPaused = 0;
                        OnEndSlowMotion();
                    }
                    else
                    {
                        if (_FreezeTW.IsEnabledAndOver)
                            _FreezeTW.End();

                        if (Skill.Framework.Audio.PitchController.Instance != null)
                            Skill.Framework.Audio.PitchController.Instance.Pitch = _Info.Pitch;
                        if (!_FreezeTW.IsEnabled)
                        {
                            TimeScale = Time.timeScale = _Info.TimeScale;
                        }
                        else
                        {
                            TimeScale = Time.timeScale = 0;
                        }
                    }
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }
            }
            base.Update();
        }
    }
}
