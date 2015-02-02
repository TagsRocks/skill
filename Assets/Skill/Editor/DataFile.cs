using Skill.Framework.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor
{
    /// <summary>
    /// Simple helper class contains shared code used for load from xml file
    /// </summary>
    public class DataFile
    {
        /// <summary>
        /// Create empty DataFile
        /// </summary>
        public DataFile()
        {
            Document = new XmlDocument();
        }

        /// <summary>
        /// Create a DataFile and load from given filename
        /// </summary>
        /// <param name="fileName">filname to load</param>
        public DataFile(string fileName)
        {
            System.IO.FileStream file = System.IO.File.OpenRead(fileName);
            System.IO.StreamReader reader = new System.IO.StreamReader(file);
            Document.LoadXml(reader.ReadToEnd());
            reader.Close();
            file.Close();
        }

        /// <summary> Retrieve first node in document </summary>
        public XmlElement Root { get { return Document.FirstChild; } }

        /// <summary> Retrieves Document to load from or add data to it</summary>
        public XmlDocument Document { get; private set; }

        /// <summary>
        /// Save document in specified file
        /// </summary>
        /// <param name="fileName">filename to save</param>
        public void Save(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            string name = System.IO.Path.GetFileName(fileName);

            int tempPostfix = 0;
            string tempFile = System.IO.Path.Combine(dir, name + "_Tmp" + tempPostfix);

            while (System.IO.File.Exists(tempFile))
            {
                tempPostfix++;
                tempFile = System.IO.Path.Combine(dir, name + "_Tmp" + tempPostfix);
            }

            System.IO.FileStream file = new System.IO.FileStream(tempFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
            writer.Write(Document.OuterXml);
            writer.Close();
            file.Close();

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);
        }
    }
}
