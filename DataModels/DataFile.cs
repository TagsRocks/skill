using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels
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
            Document = new XDocument( new XDeclaration("1.0","utf-8","yes"));            
        }

        /// <summary>
        /// Create a DataFile and load from given filename
        /// </summary>
        /// <param name="fileName">filname to load</param>
        public DataFile(string fileName)
        {
            System.IO.FileStream file = System.IO.File.OpenRead(fileName);
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(file);
            Document = XDocument.Load(reader);
            reader.Close();
            file.Close();
        }

        /// <summary> Retrieves first node in document </summary>
        public XElement Root { get { return Document.Elements().First(); } }

        /// <summary> Retrieves Document to load from or add data to it</summary>
        public XDocument Document { get; private set; }        

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
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(file);
            Document.Save(writer);
            writer.Close();
            file.Close();

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);
        }
    }
}
