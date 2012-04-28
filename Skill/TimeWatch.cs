using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
{
    public struct TimeWatch
    {
        public float StartTime { get; private set; }
        public float OverTime { get; private set; }
        public float Length { get; private set; }
        public bool Enabled { get; private set; }

        public bool IsOver
        {
            get
            {
                return UnityEngine.Time.time >= OverTime;
            }
        }
        public void Begin(float length)
        {
            this.Length = length;
            StartTime = UnityEngine.Time.time;
            OverTime = StartTime + Length;
            Enabled = true;
        }
        public void End()
        {
            StartTime = 0;
            Enabled = false;
        }
    }
}
