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
        private static ILogger _LoggerInstance;

        public static void ReplaceInstance(ILogger logger)
        {
            _LoggerInstance = logger;
        }

        public static void LogError(Exception ex)
        {
            if (_LoggerInstance != null)
                _LoggerInstance.LogError(ex);
            else
                System.Diagnostics.Debugger.Log(0, "Errors", ex.Message);
        }

        public static void LogError(string errorMsg)
        {
            if (_LoggerInstance != null)
                _LoggerInstance.LogError(errorMsg);
            else
                System.Diagnostics.Debugger.Log(0, "Errors", errorMsg);
        }

        public static void LogWarning(string warningMsg)
        {
            if (_LoggerInstance != null)
                _LoggerInstance.LogWarning(warningMsg);
            else
                System.Diagnostics.Debugger.Log(0, "Warnings", warningMsg);
        }

        public static void LogMessage(string msg)
        {
            if (_LoggerInstance != null)
                _LoggerInstance.LogMessage(msg);
            else
                System.Diagnostics.Debugger.Log(0, "Messages", msg);
        }
    }
}
