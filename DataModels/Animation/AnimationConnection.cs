using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    public class AnimationConnection
    {

        public AnimNode Source { get; private set; }
        public AnimNode Sink { get; private set; }
        public int SinkConnectorIndex { get; private set; }

        public AnimationConnection(AnimNode source, AnimNode sink, int sinkConnectorIndex)
        {
            this.Source = source;
            this.Sink = sink;
            this.SinkConnectorIndex = sinkConnectorIndex;
        }

        public AnimationConnectionInfo Info
        {
            get
            {
                return new AnimationConnectionInfo() { SourceId = this.Source.Id, SinkId = this.Sink.Id, SinkConnectorIndex = this.SinkConnectorIndex };
            }
        }
    }


    public class AnimationConnectionInfo : IXElement
    {
        public int SourceId { get; set; }
        public int SinkId { get; set; }
        public int SinkConnectorIndex { get; set; }

        public AnimationConnectionInfo()
        {

        }

        public XElement ToXElement()
        {
            XElement e = new XElement("Connection");

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
