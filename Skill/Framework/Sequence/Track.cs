using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Defines types of all available tracks in Matinee
    /// </summary>
    public enum TrackType
    {
        Event,
        Bool,
        Float,
        Integer,
        Color,
        Vector2,
        Vector3,
        Vector4,
        Quaternion,
        Sound,
        Animator,
        Animation
    }

    /// <summary>
    /// Base class for all available tracks in Matinee
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Track : Skill.Framework.StaticBehaviour
    {
        #region Editor variables
        [HideInInspector]
        public Color Color = new Color(1, 1, 1, 0.1f);
        [HideInInspector]
        public bool IsEditingCurves;
        #endregion

        /// <summary> Type of Track </summary>
        public abstract TrackType Type { get; }
        
        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            SortKeys();            
        }

        /// <summary>
        /// evaluate time relative to previous evaluation
        /// </summary>
        /// <param name="time">time to evaluate</param>
        public abstract void Evaluate(float time);

        /// <summary>
        /// evaluate time relative to previous evaluation
        /// </summary>
        /// <param name="time">time to evaluate</param>
        /// <param name="evaluation"> Evaluation filter </param>
        //public abstract void Evaluate(float time, Evaluation evaluation);

        /// <summary>
        /// Seek to specific time because Evaluate is continues relative to previous evaluation
        /// </summary>
        /// <param name="time">Seek time</param>
        public abstract void Seek(float time);

        /// <summary>
        /// Sort keys
        /// </summary>
        public abstract void SortKeys();

        /// <summary>
        /// Stop evaluation
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Get maximum time of track
        /// </summary>
        public abstract float MaxTime { get; }

        /// <summary>
        /// Rollback all changes to start frame
        /// </summary>
        public virtual void Rollback() { }


        /// <summary>
        /// Find index of closest key before specified time 
        /// </summary>
        /// <param name="keys">Keys to search inside</param>
        /// <param name="time">Time to find right before it</param>
        /// <returns>index of closest key before specified time </returns>
        /// <remarks>
        /// Keys must be sorted
        /// </remarks>    
        protected static int FindMaxIndexBeforeTime(System.Array keys, float time)
        {
            if (keys == null || keys.Length < 1)
                return -1;

            int minTimeIndex = 0;
            int maxTimeIndex = keys.Length - 1;

            while (maxTimeIndex - minTimeIndex > 1)
            {
                // calculate the midpoint for roughly equal partition
                int middleIndex = (minTimeIndex + maxTimeIndex) / 2;
                // determine which subarray to search
                if (((ITrackKey)keys.GetValue(middleIndex)).FireTime <= time)
                    // change min index to search upper subarray
                    minTimeIndex = middleIndex;
                else
                    // change max index to search lower subarray
                    maxTimeIndex = middleIndex;
            }

            // it is possible that time is lower than first key
            if (minTimeIndex == 0)
            {
                if (((ITrackKey)keys.GetValue(minTimeIndex)).FireTime > time)
                    minTimeIndex--;
            }
            // it is possible that time is greater than last key
            int index = minTimeIndex + 1;
            while (index < keys.Length)
            {
                if (((ITrackKey)keys.GetValue(index)).FireTime <= time)
                    minTimeIndex = index;
                else
                    break;
                index++;
            }

            return minTimeIndex;
        }


        public abstract void GetTimeBounds(out float minTime, out float maxTime);
    }
}