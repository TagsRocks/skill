using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
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
        /// Length
        /// </summary>
        public float Length { get; private set; }
        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Is enabled (begined) and current time is greater than OverTime
        /// </summary>
        public bool EnabledAndOver { get { return Enabled && IsOver; } }

        /// <summary>
        /// Whether current time is greater than OverTime
        /// </summary>
        public bool IsOver
        {
            get
            {
                return UnityEngine.Time.time >= OverTime;
            }
        }

        /// <summary>
        /// Begin timer 
        /// </summary>
        /// <param name="length">Lenght of timer</param>
        public void Begin(float length)
        {
            this.Length = length;
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
