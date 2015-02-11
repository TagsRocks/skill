using UnityEngine;
using System.Collections;
using Skill.Framework.Audio;


namespace Skill.Framework.Sequence
{
    /// <summary> set or reset audio event </summary>
    [CustomEvent("AudioTrigger", "Audio")]
    public class AudioControllerTriggerEvent : EventKey
    {

        [SerializeField]
        private AudioController _Audio;

        [SerializeField]
        private string _Trigger = "Ambient";

        [SerializeField]
        private bool _Set = true;


        [ExposeProperty(101, "Audio", "AudioController")]
        public AudioController Audio { get { return _Audio; } set { _Audio = value; } }

        [ExposeProperty(102, "Trigger", "Name of Trigger")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }

        [ExposeProperty(103, "Set", "Set or Reset?")]
        public bool Set { get { return _Set; } set { _Set = value; } }

        public override void FireEvent()
        {
            if (_Audio != null)
            {
                if (!string.IsNullOrEmpty(_Trigger))
                {
                    var trigger = _Audio[_Trigger];
                    if (trigger != null)
                    {
                        if (_Set)
                            trigger.Set();
                        else
                            trigger.Reset();
                    }
                    else
                        Debug.LogWarning("trigger not found");
                }
                else
                    Debug.LogWarning("invalid trigger name");
            }
            else
                Debug.LogWarning("Invalid AudioController");
        }
    }
}