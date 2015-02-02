using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{

    public class RootMotionStateData : IXmlElementSerializable
    {
        public const string ElementName = "RootMotionState";

        public bool PositionX { get; set; }
        public bool PositionY { get; set; }
        public bool PositionZ { get; set; }

        public bool RotationX { get; set; }
        public bool RotationY { get; set; }
        public bool RotationZ { get; set; }

        public XmlElement ToXmlElement()
        {
            XmlElement e = new XmlElement(ElementName);

            e.SetAttributeValue("PositionX", this.PositionX);
            e.SetAttributeValue("PositionY", this.PositionY);
            e.SetAttributeValue("PositionZ", this.PositionZ);

            e.SetAttributeValue("RotationX", this.RotationX);
            e.SetAttributeValue("RotationY", this.RotationY);
            e.SetAttributeValue("RotationZ", this.RotationZ);

            return e;
        }

        public void Load(XmlElement e)
        {
            this.PositionX = e.GetAttributeValueAsBoolean("PositionX", false);
            this.PositionY = e.GetAttributeValueAsBoolean("PositionY", false);
            this.PositionZ = e.GetAttributeValueAsBoolean("PositionZ", false);

            this.RotationX = e.GetAttributeValueAsBoolean("RotationX", false);
            this.RotationY = e.GetAttributeValueAsBoolean("RotationY", false);
            this.RotationZ = e.GetAttributeValueAsBoolean("RotationZ", false);
        }
    }

    public class RootMotionData : IXmlElementSerializable
    {
        public const string ElementName = "RootMotion";

        public KeyframeDataCollection XKeys { get; private set; }
        public KeyframeDataCollection YKeys { get; private set; }
        public KeyframeDataCollection ZKeys { get; private set; }


        public KeyframeDataCollection RXKeys { get; private set; }
        public KeyframeDataCollection RYKeys { get; private set; }
        public KeyframeDataCollection RZKeys { get; private set; }

        public RootMotionData()
        {
            this.XKeys = new KeyframeDataCollection();
            this.YKeys = new KeyframeDataCollection();
            this.ZKeys = new KeyframeDataCollection();

            this.RXKeys = new KeyframeDataCollection();
            this.RYKeys = new KeyframeDataCollection();
            this.RZKeys = new KeyframeDataCollection();
        }

        public XmlElement ToXmlElement()
        {
            XmlElement rootMotion = new XmlElement(ElementName);

            if (XKeys.Count > 0)
            {
                XmlElement xKeys = new XmlElement("XKeys");
                xKeys.AppendChild(XKeys.ToXmlElement());
                rootMotion.AppendChild(xKeys);
            }
            if (YKeys.Count > 0)
            {
                XmlElement yKeys = new XmlElement("YKeys");
                yKeys.AppendChild(YKeys.ToXmlElement());
                rootMotion.AppendChild(yKeys);
            }

            if (ZKeys.Count > 0)
            {
                XmlElement zKeys = new XmlElement("ZKeys");
                zKeys.AppendChild(ZKeys.ToXmlElement());
                rootMotion.AppendChild(zKeys);
            }

            if (RXKeys.Count > 0)
            {
                XmlElement rxKeys = new XmlElement("RXKeys");
                rxKeys.AppendChild(RXKeys.ToXmlElement());
                rootMotion.AppendChild(rxKeys);
            }

            if (RYKeys.Count > 0)
            {
                XmlElement ryKeys = new XmlElement("RYKeys");
                ryKeys.AppendChild(RYKeys.ToXmlElement());
                rootMotion.AppendChild(ryKeys);
            }

            if (RZKeys.Count > 0)
            {
                XmlElement rzKeys = new XmlElement("RZKeys");
                rzKeys.AppendChild(RZKeys.ToXmlElement());
                rootMotion.AppendChild(rzKeys);
            }

            return rootMotion;
        }

        public void Load(XmlElement e)
        {
            foreach (var item in e)
            {
                if (item.Name == "XKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        XKeys.Load(keys);
                }
                else if (item.Name == "YKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        YKeys.Load(keys);
                }
                else if (item.Name == "ZKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        ZKeys.Load(keys);
                }


                if (item.Name == "RXKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        RXKeys.Load(keys);
                }
                else if (item.Name == "RYKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        RYKeys.Load(keys);
                }
                else if (item.Name == "RZKeys")
                {
                    XmlElement keys = item[KeyframeDataCollection.ElementName];
                    if (keys != null)
                        RZKeys.Load(keys);
                }
            }
        }
    }
}
