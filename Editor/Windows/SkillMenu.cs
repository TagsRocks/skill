using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    class SkillMenu : ScriptableObject
    {
        [MenuItem("Skill/Tools/Implant", false, 0)]
        static void ShowImplant()
        {
            Tools.Implant.Instance.Show();
        }

        [MenuItem("Skill/Tools/Distance", false, 1)]
        static void ShowDistance()
        {
            Tools.Distance.Instance.Show();
        }

        [MenuItem("Skill/Tools/Animation Assigner", false, 2)]
        static void ShowAnimationAssigner()
        {
            AnimationAssigner.Instance.Show();
        }

        [MenuItem("Skill/Debug/BehaviorTree State", false, 50)]
        static void ShowBTreeStateDebugger()
        {
            BehaviorTreeStateDebugger.Instance.Show();
        }

        [MenuItem("Skill/Export to Skill Studio", false, 1001)]
        static void ShowExporter()
        {
            Exporter.Instance.Show();
        }

        [MenuItem("Skill/Apply Import Settings", false, 1002)]
        static void ApplyImportSettings()
        {
            Resources.ApplyImportSettings();
        }

        [MenuItem("Skill/About", false, 1002)]
        static void ShowAbout()
        {
            AboutSkill.Instance.Show();
        }



        // Create Menu
        [MenuItem("Assets/Create/Skill/Implant Asset")]
        static void CreateImplantAsset()
        {
            Skill.Editor.Tools.ImplantAsset asset;
            string name = "ImplantAsset";
            int nameIdx = 0;

            while (System.IO.File.Exists(Application.dataPath + "/" + name + nameIdx + ".asset"))
                nameIdx++;


            asset = CreateInstance<Skill.Editor.Tools.ImplantAsset>();
            AssetDatabase.CreateAsset(asset, "Assets/" + name + nameIdx + ".asset");
            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("Assets/Create/Skill/Spawn Asset")]
        static void CreateSpawnData()
        {
            Skill.SpawnAsset data;
            string name = "SpawnAsset";
            int nameIdx = 0;

            while (System.IO.File.Exists(Application.dataPath + "/" + name + nameIdx + ".asset"))
                nameIdx++;


            data = CreateInstance<SpawnAsset>();
            AssetDatabase.CreateAsset(data, "Assets/" + name + nameIdx + ".asset");
            Selection.activeObject = data;
            EditorUtility.FocusProjectWindow();
        }


        // commands
        [MenuItem("Skill/Commands/Copy Transform")]
        static void CopyTransform()
        {
            Skill.Editor.Tools.Commands.CopyTransform();
        }

        [MenuItem("Skill/Commands/Paste Transform")]
        static void PasteTransform()
        {
            Skill.Editor.Tools.Commands.PasteTransform();
        }
    }
}
