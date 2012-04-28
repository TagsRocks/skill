using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.CG
{
    /// <summary>
    /// Defines information about a property
    /// </summary>
    class Property
    {
        /// <summary> Modifier of property (public, internal, private, protected) </summary>
        public Modifiers Modifiers { get; set; }
        /// <summary> whether this variable settable</summary>
        public bool HasSet { get; private set; }
        /// <summary> string that represent type of property (int, float, string, ....)</summary>
        public string Type { get; private set; }
        /// <summary> Name of property</summary>
        public string Name { get; private set; }
        /// <summary> Value of property (can be name of variable or other value)</summary>
        public string Value { get; private set; }

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
            this.Modifiers = Modifiers.Public;
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
            writer.WriteLine(string.Format("{0} {1} {2} ", Modifiers.ToString().ToLower(), Type, Name));
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
    }
}
