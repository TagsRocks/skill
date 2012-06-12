using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public enum ConnectorType
    {
        Input = 0,
        Output = 1
    }

    public class Connector : IXElement
    {
        public ConnectorType Type { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }


        public XElement ToXElement()
        {
            XElement input = new XElement("Input");
            input.SetAttributeValue("Index", Index);
            input.SetAttributeValue("Name", Name);
            input.SetAttributeValue("ConnectorType", (int)Type);
            return input;
        }

        public void Load(XElement e)
        {
            this.Index = e.GetAttributeValueAsInt("Index", this.Index);
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.Type = (ConnectorType)e.GetAttributeValueAsInt("ConnectorType", 0);
        }

        public Connector Clone()
        {
            return new Connector() { Index = this.Index, Name = this.Name };
        }
    }
}
