using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewSharedAccessKeys", menuName = "Skill/SharedAccessKeys", order = 42)]
    public class SharedAccessKeysAsset : ScriptableObject
    {
        public string XmlData;
        public string BuildPath = "Scripts/AI";

        public SharedAccessKeysData Load()
        {
            SharedAccessKeysData data = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    data = new SharedAccessKeysData();
                    data.Load(document.FirstChild);
                }
                catch (System.Exception ex)
                {
                    data = null;
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                data = new SharedAccessKeysData();
                data.Keys = new AccessKeyData[0];
                Save(data);
            }
            data.Name = this.name;
            return data;
        }
        public void Save(SharedAccessKeysData data)
        {
            if (!this) return; // if deleted
            if (data == null) return;
            data.Name = this.name;
            Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
            document.AppendChild(data.ToXmlElement());
            XmlData = document.OuterXml;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

}
