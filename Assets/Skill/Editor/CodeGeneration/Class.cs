using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.CodeGeneration
{
    public enum ClassModifiers
    {
        None,
        Static,
        Abstract,
        Sealed
    }

    /// <summary>
    /// Defines a class
    /// </summary>
    abstract class Class : IPartialCode
    {
        List<Method> _Methods; // method list
        List<Property> _Properties;// property list
        List<Variable> _Variables;// variable list
        List<EnumClass> _Enums;// variable list
        List<Class> _Classes;// class list
        List<string> _InheritClasses;// inherit classes
        List<string> _Attributes;// inherit classes

        /// <summary> Name of class </summary>
        public string Name { get; private set; }
        /// <summary> Whether class is public </summary>
        public Modifiers Modifier { get; set; }
        /// <summary> Whether class is public </summary>
        public ClassModifiers ClassModifier { get; set; }
        /// <summary> Whether class is partial </summary>
        public bool IsPartial { get; set; }
        /// <summary> Whether class is partial </summary>
        public bool IsStruct { get; set; }

        /// <summary> Comment of class </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Create a class
        /// </summary>
        /// <param name="name">Name of class</param>
        public Class(string name)
        {
            this.Name = name;
            this._Classes = new List<Class>();
            this._Methods = new List<Method>();
            this._Properties = new List<Property>();
            this._Variables = new List<Variable>();
            this._Attributes = new List<string>();
            _Enums = new List<EnumClass>();
            this._InheritClasses = new List<string>();
            this.Modifier = Modifiers.Public;
            this.ClassModifier = ClassModifiers.None;
            this.IsPartial = true;
        }

        /// <summary>
        /// Add name of class that this class inherit from
        /// </summary>
        /// <param name="inheritClassName">inherit class</param>
        public void AddInherit(string inheritClassName)
        {
            if (!_InheritClasses.Contains(inheritClassName))
                _InheritClasses.Add(inheritClassName);
        }
        /// <summary>
        /// Remove inherit class
        /// </summary>
        /// <param name="inheritClassName">name of inherit class</param>
        /// <returns>True if success, othrwise false</returns>
        public bool RemoveInherit(string inheritClassName)
        {
            return _InheritClasses.Remove(inheritClassName);
        }

        /// <summary>
        /// Add a class
        /// </summary>
        /// <param name="cl">Class to add</param>
        public void Add(Class cl)
        {
            if (!_Classes.Contains(cl))
                _Classes.Add(cl);
        }        

        /// <summary>
        /// Remove specyfied Class
        /// </summary>
        /// <param name="cl">Class to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool Remove(Class cl)
        {
            return _Classes.Remove(cl);
        }

        /// <summary>
        /// Add an attribute
        /// </summary>
        /// <param name="att">Attribute to add</param>
        public void AddAttribute(string att)
        {
            if (!_Attributes.Contains(att))
                _Attributes.Add(att);
        }
        /// <summary>
        /// Remove specyfied attribute
        /// </summary>
        /// <param name="cl">Attribute to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool RemoveAttribute(string att)
        {
            return _Attributes.Remove(att);
        }

        /// <summary>
        /// Add a Enum
        /// </summary>
        /// <param name="e">enum to add</param>
        public void Add(EnumClass e)
        {
            if (!_Enums.Contains(e))
                _Enums.Add(e);
        }
        /// <summary>
        /// Remove specyfied Enum
        /// </summary>
        /// <param name="e">Enum to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool Remove(EnumClass e)
        {
            return _Enums.Remove(e);
        }

        /// <summary>
        /// Add a method
        /// </summary>
        /// <param name="method">method to add</param>
        public void Add(Method method)
        {
            if (!_Methods.Contains(method))
                _Methods.Add(method);
        }
        /// <summary>
        /// Remove specyfied method
        /// </summary>
        /// <param name="method">method to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool Remove(Method method)
        {
            return _Methods.Remove(method);
        }

        /// <summary>
        /// Add a property
        /// </summary>
        /// <param name="method">property to add</param>        
        public void Add(Property p)
        {
            if (!_Properties.Contains(p))
                _Properties.Add(p);
        }
        /// <summary>
        /// Remove specyfied property
        /// </summary>
        /// <param name="method">property to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool Remove(Property p)
        {
            return _Properties.Remove(p);
        }

        /// <summary>
        /// Add a variable
        /// </summary>
        /// <param name="method">variable to add</param>
        public void Add(Variable v)
        {
            if (!_Variables.Contains(v))
                _Variables.Add(v);
        }
        /// <summary>
        /// Remove specyfied variable
        /// </summary>
        /// <param name="method">variable to remove</param>
        /// <returns>True if success, othrwise false</returns>
        public bool Remove(Variable v)
        {
            return _Variables.Remove(v);
        }

        /// <summary>
        /// write auto part of code
        /// </summary>
        /// <param name="writer">stream</param>
        public void Write(System.IO.StreamWriter writer)
        {
            CommentWriter.Write(writer, Comment);
            for (int i = 0; i < _Attributes.Count; i++) // write attributes            
                writer.WriteLine(string.Format("[{0}]", _Attributes[i]));

            writer.Write(Modifier.ToString().ToLower());
            writer.Write(" ");
            if (ClassModifier != ClassModifiers.None)
            {
                writer.Write(ClassModifier.ToString().ToLower());
                writer.Write(" ");
            }
            if (IsPartial) writer.Write("partial ");
            writer.Write(string.Format("{0} {1}", IsStruct ? "struct" : "class", Name));
            if (_InheritClasses.Count > 0)
            {
                writer.Write(" : ");
                for (int i = 0; i < _InheritClasses.Count; i++)
                {
                    writer.Write(_InheritClasses[i]);
                    if (i < _InheritClasses.Count - 1)
                        writer.Write(", ");
                }
            }
            writer.WriteLine();
            writer.WriteLine("{");

            if (_Enums.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("// Internal Enumurators");
                foreach (var e in _Enums)
                    e.Write(writer);
            }

            if (_Classes.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("// Internal class");
                foreach (var cl in _Classes)
                    cl.Write(writer);
            }

            writer.WriteLine();
            writer.WriteLine("// Variables");
            foreach (var v in _Variables)
                v.Write(writer);

            writer.WriteLine();
            writer.WriteLine("// Properties");
            foreach (var p in _Properties)
                p.Write(writer);

            writer.WriteLine();
            writer.WriteLine("// Methods");
            foreach (var m in _Methods)
            {
                if (!m.IsPartial)
                    m.Write(writer);
            }

            writer.WriteLine();
            writer.WriteLine("}");
        }

        /// <summary>
        /// write partial part of code
        /// </summary>
        /// <param name="writer">stream</param>
        /// <param name="oldCode">old user code</param>
        public void WritePartial(System.IO.StreamWriter writer, string oldCode)
        {
            if (string.IsNullOrEmpty(oldCode))
            {
                writer.Write(Modifier.ToString().ToLower());
                writer.Write(" ");
                if (IsPartial) writer.Write("partial ");
                writer.Write(string.Format("{0} {1}", IsStruct ? "struct" : "class", Name));
                if (_InheritClasses.Count > 0)
                {
                    writer.Write(" : ");
                    for (int i = 0; i < _InheritClasses.Count; i++)
                    {
                        writer.Write(_InheritClasses[i]);
                        if (i < _InheritClasses.Count - 1)
                            writer.Write(", ");
                    }
                }
                writer.WriteLine();
                writer.WriteLine("{");

                writer.WriteLine();
                writer.WriteLine("// Methods");
                foreach (var m in _Methods)
                {
                    if (m.IsPartial)
                        m.Write(writer);
                }
                writer.WriteLine();
            }
            else // for now we have only one class per document so
            {
                // find one } for end of class                
                int index = oldCode.Length - 1;
                for (; index >= 0; index--)
                {
                    if (oldCode[index] == '}')
                        break;
                }
                if (index > 0)
                {
                    string codeUntilBeforeEndOfClass = oldCode.Substring(0, index);
                    writer.Write(codeUntilBeforeEndOfClass);
                }

                foreach (var m in _Methods)
                {
                    if (m.IsPartial && !m.IsExist(oldCode))
                        m.Write(writer);
                }
            }

            writer.WriteLine("}");
        }


        protected virtual string SetProperty(string variable, string property, object value)
        {
            return string.Format("this.{0}.{1} = {2};", Variable.GetName(variable), property, value);
        }

        protected string CreateStringArray(string[] array)
        {
            StringBuilder result = new StringBuilder();

            result.Append("new string[]{ ");

            foreach (var str in array)
            {
                result.Append(string.Format("\"{0}\", ", str));
            }

            result.Append(" }");

            return result.ToString();
        }
    }
}
