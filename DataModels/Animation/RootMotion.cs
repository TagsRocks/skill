using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{

    public class RootMotionState : IXElement
    {
        public const string ElementName = "RootMotionState";

        public bool PositionX { get; set; }
        public bool PositionY { get; set; }
        public bool PositionZ { get; set; }

        public XElement ToXElement()
        {
            XElement e = new XElement(ElementName);

            e.SetAttributeValue("PositionX", this.PositionX);
            e.SetAttributeValue("PositionY", this.PositionY);
            e.SetAttributeValue("PositionZ", this.PositionZ);

            return e;
        }

        public void Load(XElement e)
        {
            this.PositionX = e.GetAttributeValueAsBoolean("PositionX", false);
            this.PositionY = e.GetAttributeValueAsBoolean("PositionY", false);
            this.PositionZ = e.GetAttributeValueAsBoolean("PositionZ", false);
        }
    }

    public class RootMotion : IXElement
    {
        public const string ElementName = "RootMotion";

        public KeyframeCollection XKeys { get; private set; }
        public KeyframeCollection YKeys { get; private set; }
        public KeyframeCollection ZKeys { get; private set; }

        public RootMotion()
        {
            this.XKeys = new KeyframeCollection();
            this.YKeys = new KeyframeCollection();
            this.ZKeys = new KeyframeCollection();
        }

        public XElement ToXElement()
        {
            XElement rootMotion = new XElement(ElementName);

            if (XKeys.Count > 0)
            {
                XElement xKeys = new XElement("XKeys");
                xKeys.Add(XKeys.ToXElement());
                rootMotion.Add(xKeys);
            }
            if (YKeys.Count > 0)
            {
                XElement yKeys = new XElement("YKeys");
                yKeys.Add(YKeys.ToXElement());
                rootMotion.Add(yKeys);
            }

            if (ZKeys.Count > 0)
            {
                XElement zKeys = new XElement("ZKeys");
                zKeys.Add(ZKeys.ToXElement());
                rootMotion.Add(zKeys);
            }

            return rootMotion;
        }

        public void Load(XElement e)
        {
            foreach (var item in e.Elements())
            {
                if (item.Name == "XKeys")
                {
                    XElement keys = item.FindChildByName(KeyframeCollection.ElementName);
                    if (keys != null)
                        XKeys.Load(keys);
                }
                else if (item.Name == "YKeys")
                {
                    XElement keys = item.FindChildByName(KeyframeCollection.ElementName);
                    if (keys != null)
                        YKeys.Load(keys);
                }
                else if (item.Name == "ZKeys")
                {
                    XElement keys = item.FindChildByName(KeyframeCollection.ElementName);
                    if (keys != null)
                        ZKeys.Load(keys);
                }
            }
        }
    }
}
