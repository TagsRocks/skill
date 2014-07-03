using UnityEngine;
using System.Collections;
using System.Reflection;
using System;


namespace Skill.Framework.Sequence
{

    /// <summary>
    /// Base class for matinee events
    /// </summary>
    /// <remarks>
    /// Subclass must have it's own c# file in unity project
    /// </remarks>
    public abstract class EventKey : ScriptableObject, ITrackKey
    {
        [SerializeField]
        private float _Time;

        [SerializeField]
        private float _Duration;



        [ExposeProperty(0, "Fire time")]
        public virtual float FireTime { get { return _Time; } set { _Time = value; } }

        [ExposeProperty(1, "Duration", "Duration of event (optional)")]
        public virtual float Duration { get { return _Duration; } set { _Duration = value; } }

        /// <summary>Fire event </summary>
        public abstract void Fire();
    }

    public class EventTrack : Track
    {
        [SerializeField]
        [HideInInspector]
        public EventKey[] EventKeys;

        public override TrackType Type { get { return TrackType.Event; } }
        protected float CurrecntTime { get; private set; }

        public override float Length
        {
            get
            {
                if (EventKeys != null && EventKeys.Length > 0)
                {
                    if (!Application.isPlaying)
                        SortKeys();

                    var lastEvent = EventKeys[EventKeys.Length - 1];
                    if (lastEvent != null)
                        return lastEvent.FireTime + Mathf.Max(0, lastEvent.Duration);
                }
                return 0;
            }
        }

        private int _Index;
        public override void Evaluate(float time)
        {
            float preTime = CurrecntTime;
            CurrecntTime = time;
            float deltaTime = CurrecntTime - preTime;
            if (deltaTime > 0)
            {
                if (EventKeys != null)
                {
                    if (_Index < 0) _Index = 0;
                    for (; _Index < EventKeys.Length; _Index++)
                    {
                        float t = EventKeys[_Index].FireTime;
                        if (t <= CurrecntTime)
                        {
                            if (t >= preTime)
                            {
                                EventKeys[_Index].Fire();
                            }
                            _Index++;
                        }
                        else
                            break;
                    }
                }
            }
        }
        public override void Seek(float time)
        {
            CurrecntTime = time;
            if (Events != null && EventKeys.Length > 0)
                _Index = FindMaxIndexBeforeTime(EventKeys, time);
            else
                _Index = -1;
        }
        public override void SortKeys()
        {
            if (Events != null && EventKeys.Length > 1)
                Skill.Framework.MathHelper.QuickSort(EventKeys, new TrackKeyComparer<EventKey>());

        }
        public override void Stop() { }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEventAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public string Path { get; private set; }
        public CustomEventAttribute(string displayName, string path = "Custom")
        {
            this.DisplayName = displayName;
            this.Path = path;
        }
    }
}