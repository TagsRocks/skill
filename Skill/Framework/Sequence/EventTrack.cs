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
    public abstract class EventKey : EventOrientedKey
    {
        /// <summary>
        /// Is this do all job in single execute event or needs update for a period of time
        /// </summary>
        public override bool IsSingleExecution { get { return true; } }
    }

    public class EventTrack : EventOrientedTrack
    {
        public override TrackType Type { get { return TrackType.Event; } }
        protected override bool ExecuteInEditMode { get { return false; } }
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