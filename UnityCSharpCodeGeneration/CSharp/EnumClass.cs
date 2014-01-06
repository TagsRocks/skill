using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.CodeGeneration.CSharp
{
    class EnumClassMember
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }

    class EnumClass
    {
        private List<EnumClassMember> _Members;
        private int _MaxValue;

        public string Name { get; private set; }
        /// <summary> Modifier of method (public, internal, private, protected) </summary>
        public Modifiers Modifier { get; set; }

        public int Count { get { return _Members.Count; } }

        public EnumClass(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name of enum class");
            this.Name = name;
            _Members = new List<EnumClassMember>();
            _MaxValue = -1;
        }

        public void Add(string name, int value = -1)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name of enum class member");
            if (value < 0)
                value = ++_MaxValue;
            else
                _MaxValue = Math.Max(_MaxValue, value);

            _Members.Add(new EnumClassMember() { Name = name, Value = value });
        }

        public bool Remove(string name)
        {
            int index = -1;
            for (int i = 0; i < _Members.Count; i++)
            {
                if (_Members[i].Name == name)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                _Members.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// write code of enum
        /// </summary>
        /// <param name="writer">Stream</param>
        public void Write(System.IO.StreamWriter writer)
        {
            writer.WriteLine(string.Format("{0} enum {1}", Modifier.ToString().ToLower(), Name));
            writer.WriteLine("{");
            foreach (var item in _Members)
            {
                writer.WriteLine(string.Format("{0} = {1},", item.Name, item.Value));
            }
            writer.WriteLine("}");
        }
    }
}
