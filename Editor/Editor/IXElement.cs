using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.Studio
{
    
    /// <summary>
    /// Defines methods for save in Xml file
    /// </summary>
    public interface IXElement
    {
        /// <summary> Fill a XElement with internal data </summary>
        /// <returns>XElement</returns>
        XElement ToXElement();
        /// <summary> Load data from XElement loaded from file </summary>
        /// <param name="e">XElement containing data</param>
        void Load(XElement e);
    }

    public static class XElementHelper
    {
        public static XElement FindChildByName(this XElement e, string name)
        {
            foreach (var item in e.Elements().Where(p => p.Name == name))
                return item;
            return null;
        }

        public static float GetAttributeValueAsFloat(this XElement e, string attName, float defaltValue)
        {
            var p = e.Attribute(attName);
            if (p != null)
            {
                float f;
                if (float.TryParse(p.Value, out f))
                    return f;
            }
            return defaltValue;
        }

        public static double GetAttributeValueAsDouble(this XElement e, string attName, double defaltValue)
        {
            var p = e.Attribute(attName);
            if (p != null)
            {
                double d;
                if (double.TryParse(p.Value, out d))
                    return d;
            }
            return defaltValue;
        }

        public static int GetAttributeValueAsInt(this XElement e, string attName, int defaltValue)
        {
            var p = e.Attribute(attName);
            if (p != null)
            {
                int i;
                if (int.TryParse(p.Value, out i))
                    return i;
            }
            return defaltValue;
        }        

        public static bool GetAttributeValueAsBoolean(this XElement e, string attName, bool defaltValue)
        {
            var p = e.Attribute(attName);
            if (p != null)
            {
                bool b;
                if (Boolean.TryParse(p.Value, out b))
                    return b;
            }
            return defaltValue;
        }

        public static string GetAttributeValueAsString(this XElement e, string attName, string defaltValue)
        {
            var p = e.Attribute(attName);
            if (p != null)
            {
                return p.Value;
            }
            return defaltValue;
        }
    }    
}
