using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// for mobile games avoid screen turn off when playing game
    /// </summary>
    public class AvoidScreenSleep : MonoBehaviour
    {        
        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            else
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
