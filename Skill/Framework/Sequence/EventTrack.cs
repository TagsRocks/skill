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
        public override void ExecuteEvent(EventOrientedTrack track) { Fire(); }
        public abstract void Fire();
    }

    public class EventTrack : EventOrientedTrack
    {
        public override TrackType Type { get { return TrackType.Event; } }
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