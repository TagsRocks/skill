using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Defines information about a property
    /// </summary>
    class Property
    {
        /// <summary> whether this method is override,virtual or usual </summary>
        public SubMethod SubMethod { get; set; }
        /// <summary> Modifier of property (public, internal, private, protected) </summary>
        public Modifiers Modifier { get; set; }
        /// <summary> whether this variable settable</summary>
        public bool HasSet { get; private set; }
        /// <summary> string that represent type of property (int, float, string, ....)</summary>
        public string Type { get; private set; }
        /// <summary> Name of property</summary>
        public string Name { get; private set; }
        /// <summary> Value of property (can be name of variable or other value)</summary>
        public string Value { get; private set; }
        /// <summary> Comment of property </summary>
        public string Comment { get; set; }
        /// <summary> whether result of property seperated in multilines or single line</summary>
        public bool Multiline { get; protected set; }
        /// <summary> whether this property is static? </summary>
        public bool IsStatic { get; set; }

        protected string _Get; // allow subclass to change get body code
        protected string _Set;// allow subclass to change set body code

        /// <summary>
        /// Create a property
        /// </summary>
        /// <param name="type">type of proerty (string, int, float, ...)</param>
        /// <param name="name">name of property</param>
        /// <param name="value">Value of property (can be name of variable or other value)</param>
        /// <param name="hasSet">whether this variable settable</param>
        public Property(string type, string name, string value, bool hasSet = true)
        {
            this.Modifier = Modifiers.Public;
            this.Type = type;
            this.Name = name;
            this.Value = value;
            this.HasSet = hasSet;

            _Get = string.Format("return {0};", this.Value); // default get body code
            _Set = string.Format("{0} = value;", this.Value); // default set body code
        }


        /// <summary>
        /// Write property
        /// </summary>
        /// <param name="writer">Stream to write</param>
        public void Write(System.IO.StreamWriter writer)
        {
            CommentWriter.Write(writer, Comment);
            if (Multiline)
            {
                writer.WriteLine(string.Format("{0} {1} {2} {3} {4}", Modifier.ToString().ToLower(),
                    IsStatic ? "static" : string.Empty,
                    (SubMethod != CSharp.SubMethod.None) ? SubMethod.ToString().ToLower() : string.Empty,
                    Type, GetName(Name)));
                writer.WriteLine("{");

                writer.WriteLine("get");
                writer.WriteLine("{");
                writer.WriteLine(_Get);
                writer.WriteLine("}");

                if (HasSet)
                {
                    writer.WriteLine("set");
                    writer.WriteLine("{");
                    writer.WriteLine(_Set);
                    writer.WriteLine("}");
                }

                writer.WriteLine("}");
            }
            else
            {
                if (HasSet)
                {
                    writer.WriteLine(string.Format("{0} {1} {2} {3} {4} {{ get {{ {5} }} set {{ {6} }} }}", Modifier.ToString().ToLower(),
                        IsStatic ? "static" : string.Empty,
                        (SubMethod != CSharp.SubMethod.None) ? SubMethod.ToString().ToLower() : string.Empty,
                        Type, GetName(Name), _Get, _Set));
                }
                else
                {
                    writer.WriteLine(string.Format("{0} {1} {2} {3} {4} {{ get {{ {5} }} }}", Modifier.ToString().ToLower(),
                        IsStatic ? "static" : string.Empty,
                        (SubMethod != CSharp.SubMethod.None) ? SubMethod.ToString().ToLower() : string.Empty,
                        Type, GetName(Name), _Get));
                }
            }
        }

        /// <summary>
        /// all variables use same method of naming
        /// /// </summary>
        /// <param name="name">Name of variable without underline</param>
        /// <returns>target name of property that used in generated code</returns>
        public static string GetName(string name)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (i == 0 && char.IsDigit(c))
                    builder.Append("Num");
                if (!char.IsLetterOrDigit(c))
                    builder.Append('_');
                else
                    builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
