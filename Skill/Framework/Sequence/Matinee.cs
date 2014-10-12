using UnityEngine;
using System.Collections;
using System;
using Skill.Framework.Sequence;

namespace Skill.Framework.Sequence
{


    /// <summary>
    /// Matinee is a tool for keyframing the properties of Actors in your scene over time, including their position.
    /// It can also be used to author cinematic sequences in your level.
    /// </summary>
    public class Matinee : Skill.Framework.DynamicBehaviour
    {
        #region Editor data
        [HideInInspector]
        [SerializeField]
        public float[] EditorData = null;
        #endregion

        /// <summary> End time of matinee to play</summary>
        public float EndTime = 5.0f;
        /// <summary> Is loop? </summary>
        public bool Loop = false;
        /// <summary> Playback speed </summary>
        public float Speed = 1.0f;
        /// <summary> Is game enters cutscene mode? </summary>
        public bool Cutscene = true;
        /// <summary> Use realtime? </summary>
        public bool RealTime = false;


        /// <summary> Is Matinee playing? </summary>
        public bool IsPlaying { get; private set; }
        /// <summary> Is Matinee paused? </summary>
        public bool IsPaused { get; private set; }
        /// <summary> Time since start of matinee </summary>
        public float PlaybackTime { get { return _PlayTime; } }


        private Track[] _Tracks;
        private float _PlayTime;
        private float _LastTime;


        /// <summary>
        /// Occurs when matinee play/pause or stoppped.
        /// </summary>
        public event EventHandler PlaybackChanged;
        protected virtual void OnPlaybackChanged()
        {
            if (PlaybackChanged != null) PlaybackChanged(this, EventArgs.Empty);
        }

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            enabled = false;
            _Tracks = GetComponentsInChildren<Track>();

            _PlayTime = 0;
            IsPaused = false;
            IsPlaying = false;
            _NextStop = false;
        }

        /// <summary>
        /// Seek to specific time
        /// </summary>
        /// <param name="seekTime">time to seek (0 - Length)</param>
        public void Seek(float seekTime)
        {
            seekTime = Mathf.Clamp(seekTime, 0, EndTime);
            if (_Tracks != null)
            {
                foreach (var t in _Tracks)
                    t.Seek(seekTime);
            }
            _PlayTime = seekTime;
        }


        /// <summary> Stop playing Matinee </summary>
        /// <param name="rollBack">RollBack to start frame?</param>
        public void Stop(bool rollBack = false)
        {
            if (IsPlaying)
            {
                if (rollBack)
                    foreach (var t in _Tracks) t.Rollback();
                foreach (var t in _Tracks) t.Stop();

                IsPlaying = false;
                IsPaused = false;
                if (Cutscene)
                    Skill.Framework.Global.CutSceneEnable = false;
                enabled = false;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Start playing matinee from current(seek) time </summary>
        public void Play()
        {
            if (!IsPlaying)
            {
                if (RealTime)
                    _LastTime = Time.realtimeSinceStartup;
                else
                    _LastTime = Time.time;
                Skill.Framework.Global.CutSceneEnable = Cutscene;
                IsPlaying = true;
                if (IsPaused)
                    foreach (var t in _Tracks) t.Resume();
                IsPaused = false;
                enabled = true;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Pause playback of matinee </summary>
        public void Pause()
        {
            if (IsPlaying && !IsPaused)
            {
                foreach (var t in _Tracks) t.Pause();
                IsPaused = true;
                enabled = false;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (!Skill.Framework.Global.IsGamePaused)
            {
                if (IsPlaying && !IsPaused)
                {
                    float deltaTime;
                    if (RealTime)
                        deltaTime = Time.realtimeSinceStartup - _LastTime;
                    else
                        deltaTime = Time.deltaTime;
                    StepForward(deltaTime);
                }
                base.Update();
            }
            if (RealTime)
                _LastTime = Time.realtimeSinceStartup;
            else
                _LastTime = Time.time;
        }

        private bool _NextStop;
        // step forward playback with stepTime
        private void StepForward(float stepTime)
        {
            _PlayTime += stepTime * Speed;

            if (_NextStop)
                Stop();

            if (_PlayTime >= EndTime)
            {
                if (Loop)
                {
                    _PlayTime -= EndTime;
                    _NextStop = false;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);

                }
                else
                {
                    // let last keys evaluate
                    _NextStop = true;
                }
            }
            else if (_PlayTime <= 0)
            {
                if (Loop)
                {
                    _PlayTime += EndTime;
                    _NextStop = false;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);
                }
                else
                {
                    // let last keys evaluate
                    _NextStop = true;
                }
            }

            if (IsPlaying)
            {
                foreach (var t in _Tracks) t.Evaluate(_PlayTime);
            }
        }

        public void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0;
            maxTime = 1.0f;

            if (!Application.isPlaying)
                _Tracks = GetComponentsInChildren<Track>();
            if (_Tracks.Length > 0)
            {
                minTime = float.MaxValue;
                maxTime = float.MinValue;

                foreach (var t in _Tracks)
                {
                    float tminTime, tmaxTime;
                    t.GetTimeBounds(out tminTime, out tmaxTime);

                    minTime = Mathf.Min(minTime, tminTime);
                    maxTime = Mathf.Max(maxTime, tmaxTime);
                }
            }
        }

        /// <summary>
        /// Rollback scene to track default values
        /// </summary>
        [ContextMenu("Rollback")]
        public void Rollback()
        {
            if (!Application.isPlaying)
                _Tracks = GetComponentsInChildren<Track>();
            if (_Tracks != null)
            {
                foreach (var t in _Tracks)
                {
                    t.Rollback();
                }
            }
        }

        /// <summary>
        /// Calculate end time of matinee based on tracks
        /// </summary>
        [ContextMenu("Cal EndTime")]
        public void CalEndTime()
        {
            if (!Application.isPlaying)
                _Tracks = GetComponentsInChildren<Track>();
            if (_Tracks != null)
            {
                EndTime = 0.1f;
                foreach (var t in _Tracks)
                    EndTime = Mathf.Max(EndTime, t.MaxTime);
            }
        }
    }

}