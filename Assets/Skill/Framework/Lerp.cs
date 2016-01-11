using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Simplify using Mathf.Lerp
    /// </summary>
    public struct Lerp
    {
        private TimeWatch _TW;
        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public float From { get; private set; }

        /// <summary> to </summary>
        public float To { get; private set; }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary> result of lerp </summary>
        public float Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                        return Mathf.SmoothStep(From, To, _TW.Percent);
                    else
                        return Mathf.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }
        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }


        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float from, float to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }


    /// <summary>
    /// Simplify using Vector2.Lerp
    /// </summary>
    public struct Lerp2D
    {
        private TimeWatch _TW;
        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public Vector2 From { get; private set; }

        /// <summary> to </summary>
        public Vector2 To { get; private set; }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary> result of lerp </summary>
        public Vector2 Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                    {
                        float percent = _TW.Percent;
                        return new Vector2(Mathf.SmoothStep(From.x, To.x, percent), Mathf.SmoothStep(From.y, To.y, percent));
                    }
                    else
                        return Vector2.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }


        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector2 from, Vector2 to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector2 to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }



    /// <summary>
    /// Simplify using Vector3.Lerp
    /// </summary>
    public struct Lerp3D
    {
        private TimeWatch _TW;
        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public Vector3 From { get; private set; }

        /// <summary> to </summary>
        public Vector3 To { get; private set; }

        /// <summary> result of lerp </summary>
        public Vector3 Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                    {
                        float percent = _TW.Percent;
                        return new Vector3(Mathf.SmoothStep(From.x, To.x, percent), Mathf.SmoothStep(From.y, To.y, percent), Mathf.SmoothStep(From.z, To.z, percent));
                    }
                    else
                        return Vector3.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector3 from, Vector3 to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector3 to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }


    /// <summary>
    /// Simplify using Vector4.Lerp
    /// </summary>
    public struct Lerp4D
    {
        private TimeWatch _TW;
        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public Vector4 From { get; private set; }

        /// <summary> to </summary>
        public Vector4 To { get; private set; }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary> result of lerp </summary>
        public Vector4 Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                    {
                        float percent = _TW.Percent;
                        return new Vector4(Mathf.SmoothStep(From.x, To.x, percent), Mathf.SmoothStep(From.y, To.y, percent), Mathf.SmoothStep(From.z, To.z, percent), Mathf.SmoothStep(From.w, To.w, percent));
                    }
                    else
                        return Vector4.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }


        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector4 from, Vector4 to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Vector4 to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }



    /// <summary>
    /// Simplify using Color.Lerp
    /// </summary>
    public struct LerpColor
    {
        private TimeWatch _TW;

        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public Color From { get; private set; }

        /// <summary> to </summary>
        public Color To { get; private set; }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary> result of lerp </summary>
        public Color Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                        return Color.Lerp(From, To, Mathf.SmoothStep(0.0f, 1.0f, _TW.Percent));
                    else
                        return Color.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }


        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Color from, Color to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Color to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }


    /// <summary>
    /// Simplify using Mathf.LerpAngle
    /// </summary>
    public struct LerpAngle
    {
        private TimeWatch _TW;

        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public float From { get; private set; }

        /// <summary> to </summary>
        public float To { get; private set; }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary> result of lerp </summary>
        public float Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                        return Mathf.LerpAngle(From, To, Mathf.SmoothStep(0.0f, 1.0f, _TW.Percent));
                    else
                        return Mathf.LerpAngle(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }


        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float from, float to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(float to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }






    /// <summary>
    /// Simplify using Quaternion.Lerp
    /// </summary>
    public struct LerpRotation
    {
        private TimeWatch _TW;

        public bool SmoothStep { get; set; }

        /// <summary> from </summary>
        public Quaternion From { get; private set; }

        /// <summary> to </summary>
        public Quaternion To { get; private set; }

        /// <summary> result of lerp </summary>
        public Quaternion Value
        {
            get
            {
                if (_TW.IsEnabled)
                {
                    if (SmoothStep)
                        return Quaternion.Lerp(From, To, Mathf.SmoothStep(0.0f, 1.0f, _TW.Percent));
                    else
                        return Quaternion.Lerp(From, To, _TW.Percent);
                }
                return To;
            }
        }

        /// <summary>
        /// Is enabled (begined)
        /// </summary>
        public bool IsEnabled { get { return _TW.IsEnabled; } }
        /// <summary>
        /// Whether Lerp is disabled or enabled and current time is greater than OverTime
        /// </summary>
        public bool IsOver { get { return _TW.IsOver; } }

        /// <summary> Duration of lerp </summary>
        public float Duration { get { return _TW.Length; } }

        /// <summary>
        /// Begin lerp
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Quaternion from, Quaternion to, float duration, bool useRealTime = false)
        {
            this.From = from;
            this.To = to;
            _TW.Begin(duration, useRealTime);
        }

        /// <summary>
        /// Begin lerp from current value to new value
        /// </summary>        
        /// <param name="to">to</param>
        /// <param name="duration">duration</param>
        /// <param name="useRealTime"> if true TimeWatch use Time.realtimeSinceStartup instead of Time.time</param>
        public void Begin(Quaternion to, float duration, bool useRealTime = false)
        {
            Begin(this.Value, to, duration, useRealTime);
        }

        /// <summary>
        /// End lerp
        /// </summary>
        public void End()
        {
            _TW.End();
        }
    }
}
