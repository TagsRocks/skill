using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Handle SequenceEvent
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="args">Args that contains information about event</param>
    public delegate void SequenceEventHandler(object sender, SequenceEventArgs args);

    /// <summary>
    /// Args that contains information about event
    /// </summary>
    public class SequenceEventArgs : EventArgs
    {
        /// <summary> event </summary>
        public TimeEvent Event { get; private set; }
        /// <summary> Index of event in SequenceEvent</summary>
        public int Index { get; private set; }

        /// <summary>
        /// Create a SequenceEventArgs
        /// </summary>
        /// <param name="e">Event</param>
        /// <param name="index">Index of event</param>
        public SequenceEventArgs(TimeEvent e, int index)
        {
            this.Event = e;
            this.Index = index;
        }
    }

    /// <summary>
    /// An event that execute at specific time
    /// </summary>
    [Serializable]
    public class TimeEvent
    {
        /// <summary> Name of event </summary>
        public string Name;
        /// <summary> Local time of event </summary>
        public float Time;

        /// <summary>
        /// Create an Event
        /// </summary>
        /// <param name="name">Name of event</param>
        /// <param name="time">Local time of event</param>
        public TimeEvent(string name, float time)
        {
            this.Name = name;
            this.Time = time;
        }
    }

    internal class TimeEventComparer : IComparer<TimeEvent>
    {
        public int Compare(TimeEvent x, TimeEvent y)
        {
            return x.Time.CompareTo(y.Time);
        }
    }

    /// <summary>
    /// Represent a sequence of events. this class manage execution of these events. it is useful when you want to do some works in sequence but each in cetain time
    /// </summary>
    public class EventSequence : DynamicBehaviour
    {
        /// <summary> Events </summary>
        public TimeEvent[] TimeEvents;

        /// <summary> Is loop throw events after execution of lase event </summary>
        public bool Loop;

        /// <summary> Occurs when an event's time reached </summary>
        public event SequenceEventHandler Event;

        /// <summary> Occurs when an event's time reached </summary>
        protected virtual void OnEvent(int index)
        {
            if (Event != null) Event(this, new SequenceEventArgs(TimeEvents[index], index));
        }

        private float _ElapsedTime;
        private int _Index;

        /// <summary>
        /// Is all events executed (never be true if loop)
        /// </summary>
        public bool Finished { get; private set; }

        protected override void Start()
        {
            base.Start();
            MathHelper.QuickSort(this.TimeEvents, new TimeEventComparer());
            Reset();

        }

        /// <summary>
        /// Update sequence
        /// </summary>
        protected override void Update()
        {
            if (!Global.IsGamePaused)
            {
                if (Events != null && TimeEvents.Length > 0)
                {
                    if (Loop) Finished = false;
                    if (!Finished)
                    {
                        _ElapsedTime += Time.deltaTime;
                        for (; _Index < TimeEvents.Length; _Index++)
                        {
                            if (TimeEvents[_Index].Time <= _ElapsedTime)
                                OnEvent(_Index);
                            else
                                break;
                        }

                        if (_Index >= TimeEvents.Length)
                        {
                            if (Loop)
                                _ElapsedTime -= TimeEvents[TimeEvents.Length - 1].Time;
                            else
                                Finished = true;
                        }
                    }
                }
            }
            base.Update();
        }

        /// <summary>
        /// Reset sequence from begining if it is started
        /// </summary>
        public void Reset()
        {
            _ElapsedTime = 0;
            _Index = 0;
            Finished = false;
        }
    }
}
