using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class AnimationTreeProfileData : IXmlElementSerializable
    {
        public string Name { get; set; }
        public string Format { get; set; }

        public AnimationTreeProfileData()
        {
            this.Name = "Profile";
            this.Format = "{0}";
        }

        public XmlElement ToXmlElement()
        {
            XmlElement e = new XmlElement("Profile");
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Format", Format);
            return e;
        }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "Profile");
            this.Format = e.GetAttributeValueAsString("Format", "{0}");
        }
    }
}
