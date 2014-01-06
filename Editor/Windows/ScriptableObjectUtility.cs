using System;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    /// <summary>
    /// Some utility methods to modify ScriptableObjects
    /// </summary>
    public static class ScriptableObjectUtility
    {
        
        /// <summary>
        /// This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        /// <typeparam name="T"> Type of asset </typeparam>
        /// <param name="assetName"> name of asset</param>
        public static void CreateAsset<T>(string assetName) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = System.IO.Path.Combine(path, string.Format("New {0}.asset", assetName));
            string uniqueAssetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetPathAndName);
            AssetDatabase.CreateAsset(asset, uniqueAssetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
