﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    class SkillMenu : ScriptableObject
    {
        // ********************************* Editor Windows ********************************* 
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

        [MenuItem("Skill/Matinee Editor", false, 20)]
        static void ShowMatineeEditor()
        {
            Skill.Editor.Sequence.MatineeEditorWindow.Instance.Show();
        }
        [MenuItem("Skill/Subtitle", false, 21)]
        static void ShowSubtitleEditor()
        {
            Skill.Editor.Tools.AudioSubtitleEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Dictionary", false, 22)]
        static void ShowDictionaryEditor()
        {
            Skill.Editor.Tools.DictionaryDataEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Translator", false, 23)]
        static void ShowDictionaryTranslate()
        {
            Skill.Editor.Tools.DictionaryDataTranslateWindow.Instance.Show();
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
            AboutSkill.Instance.ShowUtility();
        }




        // ********************************* Create Menu ********************************* 
        [MenuItem("Assets/Create/Skill/Implant Asset", false, 41)]
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

        [MenuItem("Assets/Create/Skill/Spawn Asset", false, 42)]
        static void CreateSpawnData()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Framework.SpawnAsset>("SpawnData");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/Dictionary Data", false, 43)]
        static void CreateDictionary()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<Skill.Framework.DictionaryData>("NewDictionaryData");
        }


        // ********************************* Commands ********************************* 

        [MenuItem("Skill/Commands/Create Empty Child %m", false, 60)] // Ctrl + m
        static void CreateEmptyChildWithName()
        {
            Skill.Editor.Tools.Commands.CreateEmptyChild("GameObject");
        }

        [MenuItem("Skill/Commands/Create Empty Child Dialog %#m", false, 61)] // Ctrl + Shift + m
        static void CreateEmptyChild()
        {
            Skill.Editor.Tools.CreateEmptyChildObjectDialog.Instance.ShowUtility();
        }

        [MenuItem("Skill/Commands/Group %g", false, 70)] // Ctrl + G
        static void GroupObjects()
        {
            Skill.Editor.Tools.Commands.GroupObjects("Group");
        }

        [MenuItem("Skill/Commands/Group Dialog %#g", false, 70)] // Ctrl + Shift + G
        static void GroupObjectsWithName()
        {
            Skill.Editor.Tools.CreateGroupDialog.Instance.ShowUtility();
        }

        [MenuItem("Skill/Commands/Copy Transform", false, 80)]
        static void CopyTransform()
        {
            Skill.Editor.Tools.Commands.CopyTransform();
        }

        [MenuItem("Skill/Commands/Paste Transform", false, 81)]
        static void PasteTransform()
        {
            Skill.Editor.Tools.Commands.PasteTransform();
        }


    }
}
