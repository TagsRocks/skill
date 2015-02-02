using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    public class BehaviorTreeAsset : ScriptableObject
    {
        [HideInInspector]
        public string XmlData;
        [HideInInspector]
        public string BuildPath = "Scripts/AI";

        public SharedAccessKeysAsset[] SharedKeys;

        /// <summary> Used by code generation to decide how implement methods</summary>
        public bool ExpandMethods = true;

        public BehaviorTreeData Load()
        {
            BehaviorTreeData data = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    data = new BehaviorTreeData();
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
                data = new BehaviorTreeData();
                Save(data);

            }
            data.Name = this.name;
            data.ExpandMethods = this.ExpandMethods;
            return data;
        }
        public void Save(BehaviorTreeData data)
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
