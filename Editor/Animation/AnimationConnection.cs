using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.Editor.Animation
{
    public class AnimationConnection : IXElement
    {
        public static string ElementName = "AnimConnection";

        public int SourceId { get; private set; }        
        public int SinkId { get; private set; }
        public int SinkConnectorIndex { get; private set; }

        public AnimationConnection()
        {

        }
        public AnimationConnection(int sourceId, int sinkId, int sinkConnectorIndex)
        {
            this.SourceId = sourceId;            
            this.SinkId = sinkId;
            this.SinkConnectorIndex = sinkConnectorIndex;
        }

        public XElement ToXElement()
        {
            XElement e = new XElement(ElementName);

            e.SetAttributeValue("SourceId", SourceId);            
            e.SetAttributeValue("SinkId", SinkId);
            e.SetAttributeValue("SinkConnectorIndex", SinkConnectorIndex);

            return e;
        }

        public void Load(System.Xml.Linq.XElement e)
        {
            SourceId = e.GetAttributeValueAsInt("SourceId", -1);            
            SinkId = e.GetAttributeValueAsInt("SinkId", -1);
            SinkConnectorIndex = e.GetAttributeValueAsInt("SinkConnectorIndex", -1);
        }
    }
}
