using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Defines behaviors for an object to be savable by our algorithm
    /// </summary>
    /// <remarks>
    /// actually you do not need to use this interface and implent it. Skill Studio will generate this class for you.
    /// </remarks>
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
