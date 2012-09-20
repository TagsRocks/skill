using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public class SkillMenu : ScriptableObject
    {
        [MenuItem("Skill/Tools/Implant", false, 0)]
        public static void ShowImplant()
        {
            Tools.Implant.Instance.Show();
        }

        [MenuItem("Skill/Tools/Distance", false, 1)]
        public static void ShowDistance()
        {
            Tools.Distance.Instance.Show();
        }

        [MenuItem("Skill/Animation/Animation Assigner", false, 50)]
        public static void ShowAnimationAssigner()
        {
            AnimationAssigner.Instance.Show();
        }

        [MenuItem("Skill/AI/BTree Debugger", false, 100)]
        public static void ShowBTreeDebugger()
        {
            BehaviorTreeDebugger.Instance.Show();
        }

        [MenuItem("Skill/Export to Skill Studio", false, 1001)]
        public static void ShowExporter()
        {
            Exporter.Instance.Show();
        }

        [MenuItem("Skill/About", false, 1002)]
        public static void ShowAbout()
        {
            AboutSkill.Instance.Show();
        }



        // Create Menu

        [MenuItem("Assets/Create/Skill/ImplantAsset")]
        public static void CreateImplantAsset()
        {
            Skill.Editor.Tools.ImplantAsset asset;
            string name = "NewImplantAsset";
            int nameIdx = 0;

            while (System.IO.File.Exists(Application.dataPath + "/" + name + nameIdx + ".asset"))
                nameIdx++;


            asset = CreateInstance<Skill.Editor.Tools.ImplantAsset>();
            AssetDatabase.CreateAsset(asset, "Assets/" + name + nameIdx + ".asset");
            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();            
        }        
    }
}
