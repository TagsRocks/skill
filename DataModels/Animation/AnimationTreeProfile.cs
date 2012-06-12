using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimationTreeProfile : IXElement
    {
        public string Name { get; set; }
        public string Format { get; set; }

        public AnimationTreeProfile()
        {
            this.Name = "Profile";
            this.Format = "{0}";
        }

        public XElement ToXElement()
        {
            XElement e = new XElement("Profile");
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Format", Format);
            return e;
        }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", "Profile");
            this.Format = e.GetAttributeValueAsString("Format", "{0}");
        }
    }
}
