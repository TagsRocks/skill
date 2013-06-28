using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Diagnostics
{
    public class UnityLogger : Skill.Net.ILogger
    {
        public void LogError(Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }

        public void LogError(string errorMsg)
        {
            UnityEngine.Debug.LogError(errorMsg);
        }

        public void LogWarning(string warningMsg)
        {
            UnityEngine.Debug.LogWarning(warningMsg);
        }

        public void LogMessage(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
    }
}
