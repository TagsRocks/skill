using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Base class for a key to use in Matinee
    /// </summary>
    public interface ITrackKey
    {
        /// <summary> Execution time </summary>
        float FireTime { get; set; }
    }


    /// <summary>
    /// Comparer for ITrackKey to sort them based on ExecutionTime
    /// </summary>
    public class TrackKeyComparer<K> : IComparer<K> where K : ITrackKey
    {
        public TrackKeyComparer() { }

        /// <summary>
        /// Compare two ITrackKey based on ExecutionTime
        /// </summary>
        /// <param name="x">ITrackKey</param>
        /// <param name="y">ITrackKey</param>
        /// <returns>x relative to y</returns>
        public int Compare(K x, K y)
        {
            return x.FireTime.CompareTo(y.FireTime);
        }
    }
}