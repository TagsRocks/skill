using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.CG
{
    /// <summary>
    /// Contains information about variables
    /// </summary>
    class Variable
    {
        /// <summary> Modifier of variable (public, internal, private, protected) </summary>
        public Modifiers Modifiers { get; private set; }
        /// <summary> string that represent type of variable (int, float, string, ....)</summary>
        public string Type { get; private set; }
        /// <summary> Name of variable without underline </summary>
        public string Name { get; private set; }
        /// <summary> string that represent iitial value of variable </summary>
        public string InitialValue { get; private set; }
        /// <summary> whether this variable is static? </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Create a variable
        /// </summary>
        /// <param name="type">type of variable (string, int, float, ...)</param>
        /// <param name="name">name of variable</param>
        /// <param name="initialValue">initial value (default is null and represent nothing)</param>
        public Variable(string type, string name, string initialValue = null)
        {
            this.Type = type;
            this.Name = name;
            this.Modifiers = Modifiers.Private;
            this.InitialValue = initialValue;
        }

        /// <summary>
        /// Write a line of code that defines variable
        /// </summary>
        /// <param name="writer"></param>
        public void Write(System.IO.StreamWriter writer)
        {
            writer.WriteLine(string.Format("{0} {1} {2} {3}{4};",
                (Modifiers != CG.Modifiers.None) ? Modifiers.ToString().ToLower() : string.Empty,
                IsStatic ? "static" : string.Empty,
                Type, GetName(Name),
                (InitialValue != null) ? " = " + InitialValue : ""));
        }

        /// <summary>
        /// all variables use same method of naming
        /// /// </summary>
        /// <param name="name">Name of variable without underline</param>
        /// <returns>target name of variable that used in generated code</returns>
        public static string GetName(string name)
        {
            return "_" + name; // just add an underline at begin of variables name
        }
    }
}
