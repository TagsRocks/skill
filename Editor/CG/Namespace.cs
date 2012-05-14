using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.CG
{
    /// <summary>
    /// Defines a namespace in code file
    /// namespace contains list of classes
    /// </summary>
    class Namespace : IPartialCode
    {

        List<Class> _Classes; // class list

        /// <summary>Name of namespace </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a namespace
        /// </summary>
        /// <param name="name">Nname</param>
        public Namespace(string name)
        {
            this.Name = name;
            _Classes = new List<Class>(1);
        }

        /// <summary>
        /// Add a class
        /// </summary>
        /// <param name="cls">class to add</param>
        void Add(Class cls)
        {
            _Classes.Add(cls);
        }
        /// <summary>
        /// Remove specyfied class
        /// </summary>
        /// <param name="cls">class to remove</param>
        /// <returns>True if success, otherwise false</returns>
        bool Remove(Class cls)
        {
            return _Classes.Remove(cls);
        }
        

        /// <summary>
        /// write auto part of namespace
        /// </summary>
        /// <param name="writer">Stream</param>
        public void Write(System.IO.StreamWriter writer)
        {
            writer.WriteLine(string.Format("namespace {0}", Name));
            writer.WriteLine("{");

            foreach (var cls in _Classes)            
                cls.Write(writer);
            

            writer.WriteLine("}");
        }
        /// <summary>
        /// write partial part of code
        /// </summary>
        /// <param name="writer">Stream</param>
        /// <param name="oldCode">old and user code</param>
        public void WritePartial(System.IO.StreamWriter writer,string oldCode)
        {
            if (string.IsNullOrEmpty(oldCode))
            {
                writer.WriteLine(string.Format("namespace {0}", Name));
                writer.WriteLine("{");                
            }
            foreach (var cls in _Classes)
                cls.WritePartial(writer,oldCode);
            writer.WriteLine("}");
        }
    }
}
