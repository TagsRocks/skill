using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace Skill.IO
{
    public interface ISavableClass
    {
        XmlElement ToXmlElement(XmlSaveStream stream);
        void Save(BinarySaveStream stream);

        void Load(XmlElement e);
        void Load(BinaryLoadStream stream);
    }
}
