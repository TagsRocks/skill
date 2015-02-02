using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.IO
{
    internal class XmlAttribute
    {
        public string Name { get; private set; }
        public string Value { get; set; }

        public XmlAttribute(string name)
            : this(name, null)
        {
        }
        public XmlAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}=\"{1}\"", Name, Value);
        }
        internal void ToString(StringBuilder buffer)
        {
            buffer.Append(string.Format("{0}=\"{1}\"", Name, Value));
        }
    }
}
