using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
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
        /// <summary>
        /// Find child element by name
        /// </summary>
        /// <param name="e">XElement to search in children</param>
        /// <param name="name">name of child element</param>
        /// <returns>found element</returns>
        public static XElement FindChildByName(this XElement e, string name)
        {
            foreach (var item in e.Elements().Where(p => p.Name == name))
                return item;
            return null;
        }

        /// <summary>
        /// Try to retrieve Attribute of element in float format
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attName">Name of Attribute</param>
        /// <param name="defaltValue">Default value to return if process of conversion failed</param>
        /// <returns>Value</returns>
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

        /// <summary>
        /// Try to retrieve Attribute of element in double format
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attName">Name of Attribute</param>
        /// <param name="defaltValue">Default value to return if process of conversion failed</param>
        /// <returns>Value</returns>
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

        /// <summary>
        /// Try to retrieve Attribute of element in int format
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attName">Name of Attribute</param>
        /// <param name="defaltValue">Default value to return if process of conversion failed</param>
        /// <returns>Value</returns>
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

        /// <summary>
        /// Try to retrieve Attribute of element in boolean format
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attName">Name of Attribute</param>
        /// <param name="defaltValue">Default value to return if process of conversion failed</param>
        /// <returns>Value</returns>
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

        /// <summary>
        /// Try to retrieve Attribute of element in string format
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attName">Name of Attribute</param>
        /// <param name="defaltValue">Default value to return if process of conversion failed</param>
        /// <returns>Value</returns>
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
