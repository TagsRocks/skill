using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewAnimationTree", menuName = "Skill/AnimationTree", order = 43)]
    public class AnimationTreeAsset : ScriptableObject
    {
        [HideInInspector]
        public string XmlData;
        [HideInInspector]
        public string BuildPath = "Scripts/Designer";


        public SkinMeshAsset SkinMesh;

        public AnimationTreeData Load()
        {
            AnimationTreeData tree = null;
            if (!string.IsNullOrEmpty(XmlData))
            {
                try
                {
                    Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                    document.LoadXml(XmlData);
                    tree = new AnimationTreeData();
                    tree.Load(document.FirstChild);
                }
                catch (System.Exception ex)
                {
                    tree = null;
                    Debug.LogException(ex, this);
                }
            }
            else
            {
                tree = new AnimationTreeData();
                tree.Zoom = 1;
                tree.PanX = 0;
                tree.PanY = 0;
                Save(tree);
            }
            tree.Name = this.name;
            tree.SkinMesh = (this.SkinMesh != null) ? this.SkinMesh.name : string.Empty;
            return tree;
        }
        public void Save(AnimationTreeData tree)
        {
            if (!this) return; // if deleted
            if (tree == null) return;
            tree.Name = this.name;
            Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
            document.AppendChild(tree.ToXmlElement());
            XmlData = document.OuterXml;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

}
