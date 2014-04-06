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
        /// <summary> Start time of visible area </summary>
        [HideInInspector]
        [SerializeField]
        public float StartVisible = 0;

        /// <summary> End time of visible area </summary>
        [HideInInspector]
        [SerializeField]
        public float EndVisible = 10;

        /// <summary> Maximum available time to scroll</summary>    
        [HideInInspector]
        [SerializeField]
        public float StartSelection = 0;

        /// <summary> End time of selected time </summary>
        [HideInInspector]
        [SerializeField]
        public float EndSelection = 0;

        /// <summary> position of time </summary>
        [HideInInspector]
        [SerializeField]
        public float TimePosition = 0;
        #endregion

        /// <summary> Length of matinee over time</summary>
        public float Length = 10;
        /// <summary> Is loop? </summary>
        public bool Loop = false;
        /// <summary> Playback speed </summary>
        public float Speed = 1.0f;
        /// <summary> Is game enters cutscene mode? </summary>
        public bool Cutscene = true;


        /// <summary> Is Matinee playing? </summary>
        public bool IsPlaying { get; private set; }
        /// <summary> Is Matinee paused? </summary>
        public bool IsPaused { get; private set; }

        private Track[] _Tracks;
        private float _PlayTime;


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
        }

        /// <summary>
        /// Seek to specific time
        /// </summary>
        /// <param name="seekTime">time to seek (0 - Length)</param>
        public void Seek(float seekTime)
        {
            seekTime = Mathf.Clamp(seekTime, 0, Length);
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

                IsPlaying = false;
                IsPaused = false;
                if (Cutscene)
                    Skill.Framework.Global.CutSceneEnable = false;
                enabled = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Start playing matinee from curret(seek) time </summary>
        public void Play()
        {
            if (!IsPlaying)
            {
                Skill.Framework.Global.CutSceneEnable = Cutscene;
                IsPlaying = true;
                IsPaused = false;
                enabled = true;
                OnPlaybackChanged();
            }
        }

        /// <summary> Pause playback of matinee </summary>
        public void Pause()
        {
            if (IsPlaying && !IsPaused)
            {
                IsPaused = true;
                enabled = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            if (IsPlaying && !IsPaused)
            {
                StepForward(Time.deltaTime);
            }
            base.Update();
        }

        // step forward playback with stepTime
        private void StepForward(float stepTime)
        {
            _PlayTime += stepTime * Speed;

            if (_PlayTime >= Length)
            {
                if (Loop)
                {
                    _PlayTime -= Length;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);
                }
                else
                {
                    Stop();
                }
            }
            else if (_PlayTime <= 0)
            {
                if (Loop)
                {
                    _PlayTime += Length;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);
                }
                else
                {
                    Stop();
                }
            }

            if (IsPlaying)
            {
                foreach (var t in _Tracks) t.Evaluate(_PlayTime);
            }
        }
    }

}