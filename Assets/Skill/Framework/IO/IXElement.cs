using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Defines methods for save in Xml file
    /// </summary>
    public interface IXmlElementSerializable
    {                
        /// <summary> Fill a XmlElement with internal data </summary>
        /// <returns>XmlElement</returns>
        XmlElement ToXmlElement();
        /// <summary> Load data from XmlElement loaded from file </summary>
        /// <param name="e">XmlElement containing data</param>
        void Load(XmlElement e);
    }    
}
