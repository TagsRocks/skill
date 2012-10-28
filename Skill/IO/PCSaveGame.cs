using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Skill.IO
{
    /// <summary>
    /// Implements pc(windows) version helper methods to save and load ISavable interface
    /// </summary>
    public static class PCSaveGame
    {
        /// <summary>
        /// Name of root XmlElement in xml data
        /// </summary>
        public const string RootXmlElementName = "Root";

        /// <summary>
        /// Create a full path filename in 'My Games' directory
        /// </summary>
        /// <param name="companyName"> Name of the company</param>
        /// <param name="gameName">Name of the game</param>
        /// <param name="fileName"> name of file with extension </param>
        /// <returns> full path filename in 'My Games' directory </returns>
        public static string CreateFileName(string companyName, string gameName, string fileName)
        {
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string myGamesPath = System.IO.Path.Combine(myDocumentsPath, "My Games");
            string companyPath = System.IO.Path.Combine(myGamesPath, companyName);
            string gamesPath = System.IO.Path.Combine(companyPath, gameName);
            return System.IO.Path.Combine(gamesPath, fileName);
        }

        #region Load methods
        /// <summary>
        /// Load specified ISavable object from binary file
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="fileName"> full path of file</param>
        public static void LoadFromBinaryFile(ISavable savable, string fileName)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Invalie fileName.");
            PCBinaryLoadStream stream = new PCBinaryLoadStream(fileName);
            LoadFromStream(savable, stream);
            stream.Close();
        }

        /// <summary>
        /// Load specified ISavable object from xml file
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="fileName"> full path of file</param>
        public static void LoadFromXmlFile(ISavable savable, string fileName)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Invalie fileName.");

            string xmlContent = File.ReadAllText(fileName);
            LoadFromXmlContent(savable, xmlContent);

        }

        /// <summary>
        /// Load specified ISavable object from a xml string already saved in PlayerPrefs
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="keyString">key of xmldata in PlayerPrefs</param>
        public static void LoadXmlFromPlayerPrefs(ISavable savable, string keyString)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (string.IsNullOrEmpty(keyString)) throw new ArgumentException("Invalid key.");

            string xmlContent = UnityEngine.PlayerPrefs.GetString(keyString);
            LoadFromXmlContent(savable, xmlContent);
        }

        /// <summary>
        /// Load specified ISavable object from a binarydata as string already saved in PlayerPrefs
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="keyString">key of binarydata in PlayerPrefs</param>
        public static void LoadBinatyFromPlayerPrefs(ISavable savable, string keyString)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (string.IsNullOrEmpty(keyString)) throw new ArgumentException("Invalid key.");

            string binaryAsStringData = UnityEngine.PlayerPrefs.GetString(keyString);
            byte[] binaryData = Encoding.Unicode.GetBytes(binaryAsStringData);

            MemoryStream stream = new MemoryStream(binaryData, false);
            PCBinaryLoadStream loadStream = new PCBinaryLoadStream(stream);
            LoadFromStream(savable, loadStream);

            loadStream.Close();
        }

        /// <summary>
        /// Load specified ISavable object from a stream
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="stream">Stream to load</param>
        public static void LoadFromStream(ISavable savable, PCBinaryLoadStream stream)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            savable.Load(stream);
        }

        /// <summary>
        /// Load specified ISavable object from a xml data
        /// </summary>
        /// <param name="savable">ISavable to load</param>
        /// <param name="xmlContent">String contains xml data</param>
        public static void LoadFromXmlContent(ISavable savable, string xmlContent)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (string.IsNullOrEmpty(xmlContent)) throw new InvalidDataException("Invalid Xml content");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlContent);
            XmlElement root = document[RootXmlElementName];

            if (root != null)
            {
                XmlLoadStream stream = new XmlLoadStream();                
                savable.Load(root, stream);
            }
            else
                throw new InvalidDataException("Invalid Xml content");
        }
        #endregion

        #region Save methods

        /// <summary>
        /// Load specified ISavable object to binary file
        /// </summary>
        /// <param name="savable">ISavable to save</param>
        /// <param name="fileName"> full path of destination file</param>
        public static void SaveToBinaryFile(ISavable savable, string fileName)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");

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
            PCBinarySaveStream stream = new PCBinarySaveStream(fileName);
            SaveToStream(savable, stream);
            stream.Close();

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);

        }

        /// <summary>
        /// Load specified ISavable object to xml file
        /// </summary>
        /// <param name="savable">ISavable to save</param>        
        /// <param name="fileName"> full path of destination file</param>
        public static void SaveToXmlFile(ISavable savable, string fileName)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");

            string dir = System.IO.Path.GetDirectoryName(fileName);
            string name = System.IO.Path.GetFileName(fileName);

            int tempPostfix = 0;
            string tempFile = System.IO.Path.Combine(dir, name + "_Tmp" + tempPostfix);

            while (System.IO.File.Exists(tempFile))
            {
                tempPostfix++;
                tempFile = System.IO.Path.Combine(dir, name + "_Tmp" + tempPostfix);
            }

            string xmlContent = SaveToXmlContent(savable);
            File.WriteAllText(tempFile, xmlContent);

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
            System.IO.File.Move(tempFile, fileName);
        }


        /// <summary>
        /// Save specified ISavable object to a xml string in PlayerPrefs
        /// </summary>
        /// <param name="savable">ISavable to save</param> 
        /// <param name="keyString">key of xmldata in PlayerPrefs</param>
        public static void SaveXmlToPlayerPrefs(ISavable savable, string keyString)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (string.IsNullOrEmpty(keyString)) throw new ArgumentException("Invalid key.");
            string xmlContent = SaveToXmlContent(savable);
            UnityEngine.PlayerPrefs.SetString(keyString, xmlContent);
        }

        /// <summary>
        /// Save specified ISavable object to binary data as string in PlayerPrefs
        /// </summary>
        /// <param name="savable">ISavable to save</param> 
        /// <param name="keyString">key of binarydata in PlayerPrefs</param>
        public static void SaveBinatyToPlayerPrefs(ISavable savable, string keyString)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            if (string.IsNullOrEmpty(keyString)) throw new ArgumentException("Invalid key.");

            string binaryAsStringData = UnityEngine.PlayerPrefs.GetString(keyString);
            byte[] binaryData = Encoding.Unicode.GetBytes(binaryAsStringData);

            MemoryStream stream = new MemoryStream();
            PCBinarySaveStream saveStream = new PCBinarySaveStream(stream);
            SaveToStream(savable, saveStream);

            string saveData = Encoding.Unicode.GetString(stream.GetBuffer());
            UnityEngine.PlayerPrefs.SetString(keyString, saveData);

            saveStream.Close();
        }

        /// <summary>
        /// Save specified ISavable object to stream
        /// </summary>
        /// <param name="savable">ISavable to save</param> 
        /// <param name="stream">Stream to save</param>
        public static void SaveToStream(ISavable savable, PCBinarySaveStream stream)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");
            savable.Save(stream);
        }

        /// <summary>
        /// Save specified ISavable object to xml content
        /// </summary>
        /// <param name="savable">ISavable to save</param> 
        /// <returns>Saved data as string in xml format</returns>
        public static string SaveToXmlContent(ISavable savable)
        {
            if (savable == null) throw new ArgumentNullException("Invalid ISavable object.");

            XmlSaveStream SaveStream = new XmlSaveStream();
            XmlElement root = SaveStream.Document.CreateElement(RootXmlElementName);
            savable.Save(root, SaveStream);
            SaveStream.Document.AppendChild(root);

            return SaveStream.Document.OuterXml;
        }

        #endregion
    }
}
