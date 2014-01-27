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
        [MenuItem("Skill/Tools/Distance", false, 0)]
        static void ShowDistance()
        {
            Tools.Distance.Instance.Show();
        }

        [MenuItem("Skill/Tools/Animation Copy", false, 1)]
        static void ShowAnimationAssigner()
        {
            Tools.AnimationCopy.Instance.Show();
        }

        [MenuItem("Skill/Diagnostics/BehaviorTree State", false, 50)]
        static void ShowBTreeStateDebugger()
        {
            Diagnostics.BehaviorTreeStateDebugger.Instance.Show();
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
            ScriptableObjectUtility.CreateAsset<Skill.Editor.Tools.ImplantAsset>("ImplantAsset", delegate(Skill.Editor.Tools.ImplantAsset asset)
            {
                if (asset.DefaultObject == null)
                    asset.DefaultObject = new Skill.Editor.Tools.ImplantObject();
                asset.DefaultObject.Prefab = null;
                asset.DefaultObject.MinScalePercent = 0.8f;
                asset.DefaultObject.MaxScalePercent = 1.0f;
                asset.DefaultObject.Weight = 1.0f;
                asset.DefaultObject.Rotation = Skill.Editor.Tools.ImplantObjectRotation.SurfaceNormal;
            });

        }

        [MenuItem("Assets/Create/Skill/Spawn Asset")]
        static void CreateSpawnData()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Framework.SpawnAsset>("SpawnData");
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
