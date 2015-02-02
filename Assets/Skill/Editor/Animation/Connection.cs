using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public class ConnectionData : IXmlElementSerializable
    {

        public AnimNodeData Start { get; private set; }
        public AnimNodeData End { get; private set; }
        public int EndConnectorIndex { get; private set; }

        public ConnectionData(AnimNodeData start, AnimNodeData end, int endConnectorIndex)
        {
            this.Start = start;
            this.End = end;
            this.EndConnectorIndex = endConnectorIndex;
        }

        private AnimationTreeData _AnimationTree;
        internal ConnectionData(AnimationTreeData animationTree)
        {
            this._AnimationTree = animationTree;
        }

        public XmlElement ToXmlElement()
        {
            XmlElement e = new XmlElement("Connection");
            e.SetAttributeValue("Start", Start.Id);
            e.SetAttributeValue("End", End.Id);
            e.SetAttributeValue("EndConnectorIndex", EndConnectorIndex);

            return e;
        }

        public void Load(XmlElement e)
        {
            int startId = e.GetAttributeValueAsInt("Start", -1);
            int endId = e.GetAttributeValueAsInt("End", -1);
            EndConnectorIndex = e.GetAttributeValueAsInt("EndConnectorIndex", -1);

            if (_AnimationTree != null)
            {
                if (startId >= 0) Start = _AnimationTree.Find(startId);
                if (endId >= 0) End = _AnimationTree.Find(endId);
            }
        }
    }
}
