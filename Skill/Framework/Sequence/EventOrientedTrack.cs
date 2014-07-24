using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Skill.Framework.Sequence
{/// <summary>
    /// Base class for OrientedKey
    /// </summary>
    /// <remarks>
    /// Subclass must have it's own c# file in unity project or compiled in a dll, otherwise not serialized and Matinee can not save this class
    /// </remarks>
    public abstract class EventOrientedKey : ScriptableObject, ITrackKey
    {
        [SerializeField]
        private float _FireTime;

        [SerializeField]
        private string _Comment;

        [ExposeProperty(0, "Fire time")]
        public virtual float FireTime { get { return _FireTime; } set { _FireTime = value; } }

        [ExposeProperty(100, "Comment", "Comment")]
        public virtual string Comment { get { return _Comment; } set { _Comment = value; } }

        /// <summary> Lenght of key over time </summary>
        public virtual float Length { get { return 0.001f; } }

        /// <summary>
        /// Update key over time
        /// </summary>        
        /// <param name="track">EventOrientedTrack</param>
        /// <param name="time">time from matinee started</param>        
        public virtual void ProcessEvent(EventOrientedTrack track, float time) { }

        /// <summary>
        /// Initialize key before execute
        /// </summary>
        /// <param name="track">EventOrientedTrack</param>
        public virtual void InitializeEvent(EventOrientedTrack track) { }

        /// <summary>
        /// execute event and submit any changes
        /// </summary>
        public abstract void ExecuteEvent(EventOrientedTrack track);

        /// <summary>
        /// Undo event if it is possible
        /// </summary>
        /// <param name="track">EventOrientedTrack</param>
        //public virtual void UndoEvent(EventOrientedTrack track) { }
    }

    public abstract class EventOrientedTrack : Track
    {
        [SerializeField]
        [HideInInspector]
        public EventOrientedKey[] Keys;
        protected float CurrecntTime { get; private set; }
        protected float PreviousTime { get; private set; }
        protected float DeltaTime { get; private set; }
        protected int Index { get { return _Index; } }

        private int _Index;
        public override void Evaluate(float time)
        {
            PreviousTime = CurrecntTime;
            CurrecntTime = time;
            DeltaTime = CurrecntTime - PreviousTime;
            if (DeltaTime > 0)
            {
                if (Keys != null)
                {
                    if (_Index < 0) _Index = 0;
                    for (; _Index < Keys.Length; _Index++)
                    {
                        var key = Keys[_Index];
                        float t = key.FireTime;
                        if (t <= CurrecntTime)
                        {
                            if (t >= PreviousTime)
                            {
                                if (key.Length >= 0.01f)
                                {
                                    key.InitializeEvent(this);
                                    AddUpdatingKey(key);
                                }
                                else
                                    key.ExecuteEvent(this);
                            }
                            _Index++;
                        }
                        else
                            break;
                    }
                }
                UpdateKeys();
            }
            //else if (DeltaTime < 0)
            //{
            //    if (Keys != null)
            //    {
            //        if (_Index >= Keys.Length) _Index = Keys.Length - 1;
            //        while (_Index >= 0)
            //        {
            //            float t = Keys[_Index].FireTime;
            //            if (t < CurrecntTime)
            //                break;
            //            else if (t >= CurrecntTime && t < PreviousTime)
            //            {
            //                Keys[_Index].UndoEvent(this);
            //            }
            //            _Index--;
            //        }
            //    }
            //}            
        }

        public override void Seek(float time)
        {
            PreviousTime = CurrecntTime;
            CurrecntTime = time;
            DeltaTime = CurrecntTime - PreviousTime;
            if (Keys != null && Keys.Length > 0)
                _Index = FindMaxIndexBeforeTime(Keys, time);
            else
                _Index = -1;
            ClearKeys();
        }

        public override void SortKeys()
        {
            if (Keys != null && Keys.Length > 1)
                Skill.Framework.MathHelper.QuickSort(Keys, new TrackKeyComparer<EventOrientedKey>());
        }
        public override void Stop() { }

        private List<EventOrientedKey> _UpdatingKeys;
        private void UpdateKeys()
        {
            if (_UpdatingKeys != null && _UpdatingKeys.Count > 0)
            {
                int index = 0;
                while (index < _UpdatingKeys.Count)
                {
                    EventOrientedKey task = _UpdatingKeys[index];
                    if (CurrecntTime < task.FireTime)
                    {
                        task.ExecuteEvent(this);
                        continue;
                    }
                    else if (CurrecntTime >= task.FireTime + task.Length)
                    {
                        task.ExecuteEvent(this);
                        _UpdatingKeys.RemoveAt(index);
                        continue;
                    }
                    else
                    {
                        task.ProcessEvent(this, CurrecntTime);
                    }
                    index++;
                }
            }
        }
        private void ClearKeys()
        {
            if (_UpdatingKeys != null && _UpdatingKeys.Count > 0)
                _UpdatingKeys.Clear();
        }

        private void AddUpdatingKey(EventOrientedKey key)
        {
            if (_UpdatingKeys == null)
                _UpdatingKeys = new List<EventOrientedKey>();
            _UpdatingKeys.Add(key);
        }

        public override void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0.0f;
            maxTime = 0.0f;

            if (Keys != null && Keys.Length > 0)
            {
                if (!Application.isPlaying)
                    SortKeys();

                var firstEvent = Keys[0];
                var lastEvent = Keys[Keys.Length - 1];

                if (firstEvent != null)
                    minTime = Mathf.Min(0, firstEvent.FireTime);
                if (lastEvent != null)
                    maxTime = lastEvent.FireTime + Mathf.Max(0.1f, lastEvent.Length);

            }
            if (maxTime - minTime < 0.1f)
                maxTime = minTime + 0.1f;
        }
    }
}
