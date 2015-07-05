using UnityEngine;
using System.Collections;

namespace Skill.Editor.IO
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewSaveData", menuName = "Skill/SaveData", order = 45)]
    public class SaveDataAsset : ScriptableObject
    {
        public string XmlData;
        public string BuildPath = "Scripts/Designer";

        public SaveData Load()
        {
            SaveData saveData = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    saveData = new SaveData();
                    saveData.Load(document.FirstChild);
                }
                catch (System.Exception ex)
                {
                    saveData = null;
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                saveData = new SaveData();                
                Save(saveData);
            }
            saveData.Name = this.name;
            return saveData;
        }

        public void Save(SaveData saveData)
        {
            if (!this) return; // if deleted
            if (saveData == null) return;
            saveData.Name = this.name;
            Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
            document.AppendChild(saveData.ToXmlElement());
            XmlData = document.OuterXml;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

}
