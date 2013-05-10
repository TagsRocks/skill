using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    public interface ILogger
    {
        void LogError(Exception ex);
        void LogError(string errorMsg);
        void LogWarning(string warningMsg);
        void LogMessage(string msg);
    }

    public static class Logger
    {
        public static ILogger LoggerInstance { get; set; }

        public static void LogError(Exception ex)
        {
            if (LoggerInstance != null)
                LoggerInstance.LogError(ex);
            else
                System.Diagnostics.Debugger.Log(0, "Errors", ex.Message);
        }

        public static void LogError(string errorMsg)
        {
            if (LoggerInstance != null)
                LoggerInstance.LogError(errorMsg);
            else
                System.Diagnostics.Debugger.Log(0, "Errors", errorMsg);
        }

        public static void LogWarning(string warningMsg)
        {
            if (LoggerInstance != null)
                LoggerInstance.LogWarning(warningMsg);
            else
                System.Diagnostics.Debugger.Log(0, "Warnings", warningMsg);
        }

        public static void LogMessage(string msg)
        {
            if (LoggerInstance != null)
                LoggerInstance.LogMessage(msg);
            else
                System.Diagnostics.Debugger.Log(0, "Messages", msg);
        }
    }
}
