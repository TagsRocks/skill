using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using System.Collections.Generic;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// TrackBar to edit PropertyTrack
    /// </summary>
    public abstract class PropertyTrackBar<V> : BaseTrackBar
    {
        private PropertyTrack<V> _PropertyTrack;

        /// <summary>
        /// Create a PropertyTrack
        /// </summary>
        /// <param name="track"> PropertyTrack to edit</param>
        public PropertyTrackBar(PropertyTrack<V> track)
            : base(track)
        {
            _PropertyTrack = track;
        }

        public virtual void UpdateDefaultValue(V defaultValue)
        {
            _PropertyTrack.DefaultValue = defaultValue;
        }
    }
}