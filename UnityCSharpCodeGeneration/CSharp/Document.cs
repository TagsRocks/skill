using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// a document is a c# file
    /// </summary>
    class Document : IPartialCode
    {
        List<string> _Usings; // list of using to write at top of document
        List<Namespace> _Namespaces; // list of namespaces
        List<Class> _Classes;// list of classes that does not inside of a namespace

        /// <summary>
        /// Create a document
        /// </summary>
        public Document()
        {
            _Usings = new List<string>(4);
            _Namespaces = new List<Namespace>(1);
            _Classes = new List<Class>();
        }

        /// <summary>
        /// Add new using
        /// </summary>
        /// <param name="us">name of namespace (like System or System.Collections)</param>
        public void AddUsing(string us)
        {
            if (!_Usings.Contains(us))
                _Usings.Add(us);
        }
        /// <summary>
        /// Remove using
        /// </summary>
        /// <param name="us">name of namespace</param>
        /// <returns>True if success, otherwise false</returns>
        public bool RemoveUsing(string us)
        {
            return _Usings.Remove(us);
        }

        /// <summary>
        /// Create a usig line
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <returns>full line if code</returns>
        string Using(string ns)
        {
            return string.Format("using {0};", ns);
        }

        /// <summary> Add using Skill </summary>
        public void AddUsingSkill()
        {
            AddUsing("Skill");
        }
        /// <summary> Add using Skill.Framework.AI </summary>
        public void AddUsingSkillAI()
        {
            AddUsing("Skill.Framework.AI");
        }
        /// <summary> Add using Skill.Framework.Animation </summary>
        public void AddUsingSkillAnimation()
        {
            AddUsing("Skill.Framework.Animation");
        }
        /// <summary> Add using Skill.Framework.IO </summary>
        public void AddUsingSkillIO()
        {
            AddUsing("Skill.Framework.IO");
        }
        /// <summary> Add default usings </summary>
        public void AddDefaultUsings()
        {
            AddUsing("System");
            AddUsing("System.Collections.Generic");
            AddUsing("System.Linq");
            AddUsing("System.Text");
            AddUsing("UnityEngine");
        }

        /// <summary>
        /// Add namespace
        /// </summary>
        /// <param name="ns">namespace to add</param>
        public void Add(Namespace ns)
        {
            _Namespaces.Add(ns);
        }
        /// <summary>
        /// Remove namespace
        /// </summary>
        /// <param name="ns">namespace to remove</param>
        /// <returns>true if success, otherwise false</returns>
        public bool Remove(Namespace ns)
        {
            return _Namespaces.Remove(ns);
        }

        /// <summary>
        /// Add a class
        /// </summary>
        /// <param name="cls">class to add</param>
        public void Add(Class cls)
        {
            _Classes.Add(cls);
        }
        /// <summary>
        /// Remove class
        /// </summary>
        /// <param name="cls">class to remove</param>
        /// <returns>True if success, otherwise false</returns>
        public bool Remove(Class cls)
        {
            return _Classes.Remove(cls);
        }

        /// <summary>
        /// Write auto part of code
        /// </summary>
        /// <param name="writer"></param>
        public void Write(System.IO.StreamWriter writer)
        {
            foreach (var us in _Usings)
                writer.WriteLine(Using(us));

            writer.WriteLine();
            foreach (var cls in _Classes)
            {
                cls.Write(writer);
            }

            writer.WriteLine();
            foreach (var ns in _Namespaces)
            {
                ns.Write(writer);
            }
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
                foreach (var us in _Usings)
                    writer.WriteLine(Using(us));
                writer.WriteLine();
            }

            foreach (var cls in _Classes)
            {
                cls.WritePartial(writer, oldCode);
                writer.WriteLine();
            }

            foreach (var ns in _Namespaces)
            {
                ns.WritePartial(writer, oldCode);
            }
        }

    }
}
