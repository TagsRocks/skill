using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Skill.Editor.CG
{
    /// <summary>
    /// defines type of method when method recreate in subclass
    /// </summary>
    enum SubMethod
    {
        None,
        Override,
        Virtual
    }

    /// <summary>
    /// Defines a method
    /// </summary>
    class Method
    {
        /// <summary> whether this method should be write in partial code only </summary>
        public bool IsPartial { get; set; }
        /// <summary> whether this method is override,virtual or usual </summary>
        public SubMethod SubMethod { get; set; }
        /// <summary> Modifier of method (public, internal, private, protected) </summary>
        public Modifiers Modifiers { get; set; }
        /// <summary> string that represent return type (string, int, float, ...)</summary>
        public string ReturnType { get; private set; }
        /// <summary> Name of methods </summary>
        public string Name { get; private set; }
        /// <summary> parameters of method </summary>
        public string[] Parameters { get; private set; }
        /// <summary> body of method can be anything so let subclass to take care of it </summary>
        public string Body { get; protected set; }
        /// <summary> use in constructor methods write between ) and { </summary>
        public string BaseMethod { get; set; }
        /// <summary> whether this method is static? </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Create a method
        /// </summary>        
        /// <param name="returnType">string that represent return type (string, int, float, ...)</param>
        /// <param name="name">name of method</param>
        /// <param name="body">body</param>
        /// <param name="parameters">parameters</param>
        public Method(string returnType, string name, string body, params string[] parameters)
        {
            this.Modifiers = Modifiers.Private;
            this.ReturnType = returnType;
            this.Name = name;
            this.Body = body;
            this.Parameters = parameters;
        }


        /// <summary>
        /// write code of method
        /// </summary>
        /// <param name="writer">Stream</param>
        public void Write(System.IO.StreamWriter writer)
        {
            writer.Write(string.Format("{0} {1} {2} {3} {4}(", (Modifiers != CG.Modifiers.None) ? Modifiers.ToString().ToLower() : string.Empty,
                IsStatic ? "static" : string.Empty,
                (this.SubMethod != SubMethod.None) ? this.SubMethod.ToString().ToLower() : string.Empty,
                ReturnType,
                Name));

            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    writer.Write(Parameters[i]);
                    if (i < Parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            writer.WriteLine(")");
            if (!string.IsNullOrEmpty(BaseMethod))
                writer.WriteLine(BaseMethod);
            writer.WriteLine("{");
            if (!string.IsNullOrEmpty(Body))
                writer.WriteLine(Body);
            writer.WriteLine("}");
        }

        /// <summary>
        /// Whether given string contains difinition of this method
        /// </summary>
        /// <param name="code">string that contains code</param>
        /// <returns>true if exist, otherwise false</returns>
        public bool IsExist(string code)
        {
            string reg = Modifiers.ToString().ToLower() + "\\s+" +
                ((this.SubMethod != SubMethod.None) ? this.SubMethod.ToString().ToLower() + "\\s+" : "") + ReturnType.ToString() + "\\s+" + Name;
            Regex regex = new Regex(reg);
            return regex.Match(code).Success;
        }
    }
}
