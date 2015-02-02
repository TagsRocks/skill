using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public interface IPropertyKey<V> : ITrackKey
    {
        /// <summary> Value key</summary>
        V ValueKey { get; set; }
    }

    public abstract class DiscreteTrack<V> : PropertyTrack<V>
    {
        public abstract IPropertyKey<V>[] PropertyKeys { get; set; }
        protected float CurrecntTime { get; private set; }
        private int _Index;

        /// <summary>
        /// Get maximum time of track
        /// </summary>
        public override float MaxTime
        {
            get
            {
                if (PropertyKeys != null && PropertyKeys.Length > 0)
                    return PropertyKeys[PropertyKeys.Length - 1].FireTime;
                return 0;
            }
        }

        public override void Evaluate(float time)
        {
            int evaluatedIndex = -1;
            float preTime = CurrecntTime;
            CurrecntTime = time;
            float deltaTime = CurrecntTime - preTime;
            if (deltaTime > 0)
            {
                if (PropertyKeys != null)
                {
                    if (_Index < 0) _Index = 0;
                    while (_Index < PropertyKeys.Length)
                    {
                        float t = PropertyKeys[_Index].FireTime;
                        if (t <= CurrecntTime)
                        {
                            evaluatedIndex = _Index;
                            if (t >= preTime)
                            {
                                Execute(PropertyKeys[_Index]);
                            }
                            _Index++;
                        }
                        else
                        {
                            _Index--;
                            break;
                        }
                    }
                }
            }
            else if (deltaTime < 0)
            {
                if (PropertyKeys != null)
                {
                    if (_Index >= PropertyKeys.Length) _Index = PropertyKeys.Length - 1;
                    while (_Index >= 0)
                    {
                        float t = PropertyKeys[_Index].FireTime;
                        if (t < CurrecntTime)
                            break;
                        else if (t >= CurrecntTime && t < preTime)
                        {
                            evaluatedIndex = _Index;
                            Execute(PropertyKeys[_Index]);
                        }
                        _Index--;
                    }
                }
            }

            Evaluate(evaluatedIndex);
        }
        private void Evaluate(int evaluatedIndex)
        {
            //if (_Index < 0)
            //    Rollback();
            //else
            //{
            if (_Index >= PropertyKeys.Length) _Index = PropertyKeys.Length - 1;
            if (evaluatedIndex != _Index && _Index >= 0 && _Index < PropertyKeys.Length)
            {
                Execute(PropertyKeys[_Index]);
            }
            //}
        }

        public override void Seek(float time)
        {
            CurrecntTime = time;
            if (PropertyKeys != null && PropertyKeys.Length > 0)
                _Index = FindMaxIndexBeforeTime(PropertyKeys, time);
            else
                _Index = -1;
            Evaluate(-1);
        }

        public override void SortKeys()
        {
            if (PropertyKeys != null && PropertyKeys.Length > 1)
            {
                Skill.Framework.Utility.QuickSort(PropertyKeys, new TrackKeyComparer<IPropertyKey<V>>());
            }
        }

        /// <summary>
        /// When time is paused but make sure key applied (for curve tracks)
        /// </summary>
        /// <param name="key">Key to Verify</param>
        protected virtual void Execute(IPropertyKey<V> key)
        {
            SetValue(key.ValueKey);
        }

        protected IPropertyKey<V> GetPreviousKey()
        {
            if (PropertyKeys == null) return null;
            if (_Index > 0)
                return PropertyKeys[_Index - 1];
            else
                return null;
        }

        protected IPropertyKey<V> GetNextKey()
        {
            if (PropertyKeys == null) return null;
            if (_Index < PropertyKeys.Length - 1)
                return PropertyKeys[_Index + 1];
            else
                return null;
        }


        public override void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0.0f;
            maxTime = 0.0f;

            if (PropertyKeys != null && PropertyKeys.Length > 0)
            {
                if (!Application.isPlaying)
                    SortKeys();

                var firstEvent = PropertyKeys[0];
                var lastEvent = PropertyKeys[PropertyKeys.Length - 1];

                if (firstEvent != null)
                    minTime = Mathf.Min(0, firstEvent.FireTime);
                if (lastEvent != null)
                    maxTime = lastEvent.FireTime;
            }
            if (maxTime - minTime < 0.1f)
                maxTime = minTime + 0.1f;
        }
    }

}