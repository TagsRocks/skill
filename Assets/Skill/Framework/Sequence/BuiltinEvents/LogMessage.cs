using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you would like to write a debug message to the Unity Console.
    /// </summary>
    [CustomEvent("Log", "Debug")]
    public class LogMessage : EventKey
    {
        [SerializeField]
        private string _Message;

        [ExposeProperty(101, "Message", "Log message")]
        public string Message { get { return _Message; } set { _Message = value; } }

        public override void FireEvent()
        {
            if (!string.IsNullOrEmpty(_Message))
                Debug.Log(_Message);
        }


    }
}