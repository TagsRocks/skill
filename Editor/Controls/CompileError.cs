using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.Controls
{
    public enum ErrorType
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Message = 4,
    }

    public class CompileError
    {
        private static int _Counter;
        public static void ResetCounter() { _Counter = 0; }

        public string Description { get; set; }
        public EntityNodeViewModel Node { get; set; }
        public ErrorType Type { get; set; }
        public int Order { get; private set; }

        public CompileError()
        {
            Order = _Counter++;
        }

        public string File
        {
            get
            {
                if (Node == null) return "";
                return Node.LocalFileName;
            }
        }
    }
}
