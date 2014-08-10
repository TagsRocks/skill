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

        [ExposeProperty(999999, "Comment", "Comment")]
        public virtual string Comment { get { return _Comment; } set { _Comment = value; } }


        /// <summary>
        /// Is this do all job in single execute event or needs update for a period of time
        /// </summary>
        public abstract bool IsSingleExecution { get; }

        /// <summary> Lenght of key over time </summary>
        public virtual float Length { get { return 0.0f; } }

        /// <summary>
        /// Update event over time (if IsSingleExecution = false)
        /// </summary>                
        /// <param name="time">time from matinee started</param>        
        public virtual void UpdateEvent(float time) { }

        /// <summary>
        /// Initialize event before execute (if IsSingleExecution = false)
        /// </summary>        
        public virtual void StartEvent() { }

        /// <summary>
        /// finish event (if IsSingleExecution = false)
        /// </summary>        
        public virtual void FinishEvent() { }

        /// <summary>
        /// execute event (if IsSingleExecution = true)
        /// </summary>
        public virtual void FireEvent() { }
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

        /// <summary>
        /// Get maximum time of track
        /// </summary>
        public override float MaxTime
        {
            get
            {
                if (Keys != null && Keys.Length > 0)
                    return Keys[Keys.Length - 1].FireTime;
                return 0;
            }
        }

        protected virtual bool ExecuteInEditMode { get { return true; } }

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
                    while (_Index < Keys.Length)
                    {
                        var key = Keys[_Index];
                        float t = key.FireTime;
                        if (t <= CurrecntTime)
                        {
                            if (t >= PreviousTime)
                            {
                                if (Application.isPlaying || ExecuteInEditMode)
                                {
                                    InitializeEvent(key);
                                    if (key.IsSingleExecution)
                                    {
                                        key.FireEvent();
                                    }
                                    else
                                    {
                                        key.StartEvent();
                                        AddUpdatingKey(key);
                                    }
                                }
                            }
                            _Index++;
                        }
                        else
                        {
                            break;
                        }
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

        /// <summary>
        /// Initialize event before execution
        /// </summary>
        /// <param name="key">EventOrientedKey to initialize</param>
        protected virtual void InitializeEvent(EventOrientedKey key) { }

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
                    if (CurrecntTime >= task.FireTime + task.Length)
                    {
                        task.FinishEvent();
                        _UpdatingKeys.RemoveAt(index);
                        continue;
                    }
                    else if (CurrecntTime > task.FireTime)
                    {
                        task.UpdateEvent(CurrecntTime);
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
