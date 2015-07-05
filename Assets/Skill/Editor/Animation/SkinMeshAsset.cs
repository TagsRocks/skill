using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewSkinMesh", menuName = "Skill/SkinMesh", order = 44)]
    public class SkinMeshAsset : ScriptableObject
    {
        public string XmlData;
        public string BuildPath = "Scripts/Designer";

        public SkinMeshData Load()
        {
            SkinMeshData skin = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    skin = new SkinMeshData();
                    skin.Load(document.FirstChild);
                }
                catch (System.Exception ex)
                {
                    skin = null;
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                skin = new SkinMeshData();
                Save(skin);
            }
            skin.Name = this.name;
            return skin;
        }
        public void Save(SkinMeshData skin)
        {
            if (!this) return; // if deleted
            if (skin == null) return;
            skin.Name = this.name;
            Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
            document.AppendChild(skin.ToXmlElement());
            XmlData = document.OuterXml;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

}
