using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework
{
    /// <summary>
    /// Helper class for track limitation of a job
    /// </summary>
    public struct TimeWatch
    {
        /// <summary>
        /// Start time
        /// </summary>
        public float StartTime { get; private set; }
        /// <summary>
        /// End time
        /// </summary>
        public float OverTime { get; private set; }
        /// <summary>
        /// Get and set Length
        /// </summary>
        public float Length { get; set; }
        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Is enabled (begined) and current time is greater than OverTime
        /// </summary>
        public bool EnabledAndOver { get { return Enabled && IsOver; } }

        /// <summary>
        /// Whether TimeWatch is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver
        {
            get
            {
                if (!Enabled) return true;
                if (UseRealTime)
                    return UnityEngine.Time.realtimeSinceStartup >= OverTime;
                else
                    return UnityEngine.Time.time >= OverTime;
            }
        }

        /// <summary>
        /// if true TimeWatch use Time.realtimeSinceStartup instead of Time.time
        /// </summary>
        public bool UseRealTime { get; private set; }

        /// <summary>
        /// Begin timer 
        /// </summary>
        /// <param name="length">Lenght of timer</param>
        /// <param name="useRealTime">if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float length, bool useRealTime = false)
        {
            this.UseRealTime = useRealTime;
            this.Length = length;

            if (this.UseRealTime)
                StartTime = UnityEngine.Time.realtimeSinceStartup;
            else
                StartTime = UnityEngine.Time.time;
            OverTime = StartTime + Length;
            Enabled = true;
        }

        /// <summary>
        /// End
        /// </summary>
        public void End()
        {
            StartTime = 0;
            Enabled = false;
        }
    }
}
