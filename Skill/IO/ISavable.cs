using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace Skill.IO
{
    public interface ISavable
    {
        /// <summary>
        /// Save data to given XmlElement e
        /// </summary>
        /// <param name="e">XmlElement to save data in</param>
        /// <param name="stream">helper stream</param>        
        void Save(XmlElement e, XmlSaveStream stream);
        /// <summary>
        /// Save data to given stream
        /// </summary>
        /// <param name="stream">Stream to save data</param>
        void Save(BinarySaveStream stream);

        /// <summary>
        /// Load data from given XmlElement e
        /// </summary>
        /// <param name="e">XmlElement that containes data</param>
        /// <param name="stream">Helper stream</param>
        void Load(XmlElement e, XmlLoadStream stream);
        /// <summary>
        /// Load data from given stream
        /// </summary>
        /// <param name="stream">Stream to load data from</param>
        void Load(BinaryLoadStream stream);
    }
}
