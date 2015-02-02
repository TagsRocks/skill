using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{    
    public class ConnectorData : IXmlElementSerializable
    {        
        public int Index { get; set; }
        public string Name { get; set; }


        public XmlElement ToXmlElement()
        {
            XmlElement input = new XmlElement("Input");
            input.SetAttributeValue("Index", Index);
            input.SetAttributeValue("Name", Name);            
            return input;
        }

        public void Load(XmlElement e)
        {
            this.Index = e.GetAttributeValueAsInt("Index", this.Index);
            this.Name = e.GetAttributeValueAsString("Name", this.Name);            
        }

        public ConnectorData Clone()
        {
            return new ConnectorData() { Index = this.Index, Name = this.Name };
        }
    }
}
