using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework
{
    /// <summary>
    /// Helper class for track limitation of a job
    /// </summary>
    public struct TimeWatch
    {
        private float _StartTime;
        private float _OverTime;
        private float _Length;
        private bool _IsEnabled;
        private bool _UseRealTime;

        /// <summary>
        /// Start time
        /// </summary>
        public float StartTime { get { return _StartTime; } }
        /// <summary>
        /// End time
        /// </summary>
        public float OverTime { get { return _OverTime; } }
        /// <summary>
        /// Get and set Length
        /// </summary>
        public float Length { get { return _Length; } set { _Length = value; } }
        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _IsEnabled; } }

        /// <summary>
        /// If TimeWatch is enabled returns Percent of time(0.0f - 1.0f), otherwise zero(0.0f).
        /// </summary>
        public float Percent
        {
            get
            {
                if (_IsEnabled)
                {
                    if (_UseRealTime)
                        return UnityEngine.Mathf.Clamp01((UnityEngine.Time.realtimeSinceStartup - _StartTime) / _Length);
                    else
                        return UnityEngine.Mathf.Clamp01((UnityEngine.Time.time - _StartTime) / _Length);
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Is enabled (begined) and current time is greater than OverTime
        /// </summary>
        public bool IsEnabledAndOver
        {
            get
            {
                if (!_IsEnabled) return false;
                if (_UseRealTime)
                    return UnityEngine.Time.realtimeSinceStartup >= _OverTime;
                else
                    return UnityEngine.Time.time >= _OverTime;
            }
        }

        /// <summary>
        /// Is enabled (begined) and current time is lower than OverTime
        /// </summary>
        public bool IsEnabledButNotOver
        {
            get
            {
                if (!_IsEnabled) return false;
                if (_UseRealTime)
                    return UnityEngine.Time.realtimeSinceStartup < _OverTime;
                else
                    return UnityEngine.Time.time < _OverTime;
            }
        }

        /// <summary>
        /// Whether TimeWatch is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver
        {
            get
            {
                if (!_IsEnabled) return true;
                if (_UseRealTime)
                    return UnityEngine.Time.realtimeSinceStartup >= _OverTime;
                else
                    return UnityEngine.Time.time >= _OverTime;
            }
        }

        /// <summary>
        /// Retrieves elapsed time since begin
        /// </summary>
        public float ElapsedTime
        {
            get
            {
                if (_IsEnabled)
                {
                    if (_UseRealTime)
                        return UnityEngine.Time.realtimeSinceStartup - _StartTime;
                    else
                        return UnityEngine.Time.time - StartTime;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// if true TimeWatch use Time.realtimeSinceStartup instead of Time.time
        /// </summary>
        public bool UseRealTime { get { return _UseRealTime; } }

        /// <summary>
        /// Begin timer 
        /// </summary>
        /// <param name="length">Lenght of timer</param>
        /// <param name="useRealTime">if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float length, bool useRealTime = false)
        {
            this._UseRealTime = useRealTime;
            this._Length = length;

            if (this.UseRealTime)
                _StartTime = UnityEngine.Time.realtimeSinceStartup;
            else
                _StartTime = UnityEngine.Time.time;
            _OverTime = _StartTime + _Length;
            _IsEnabled = true;
        }

        /// <summary>
        /// End
        /// </summary>
        public void End()
        {
            _StartTime = 0;
            _IsEnabled = false;
            _UseRealTime = false;
        }
    }
}
