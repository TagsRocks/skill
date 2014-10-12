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
        // ********************************* Editor Windows ********************************* 
        [MenuItem("Skill/Tools/Distance", false, 0)]
        static void ShowDistance()
        {
            Tools.Distance.Instance.Show();
        }

        [MenuItem("Skill/Tools/Copy Component", false, 1)]
        static void ShowCopyComponentsWindow()
        {
            Tools.CopyPaseComponentsWindow.Instance.Show();
        }

        [MenuItem("Skill/Matinee Editor", false, 20)]
        static void ShowMatineeEditor()
        {
            Skill.Editor.Sequence.MatineeEditorWindow.Instance.Show();
        }

        [UnityEditor.MenuItem("Skill/Curve Editor", false, 21)]
        static void ShowCurveEditorWindow()
        {
            Skill.Editor.Curve.CurveEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Dictionary", false, 22)]
        static void ShowDictionaryEditor()
        {
            Skill.Editor.Tools.DictionaryEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Translator", false, 23)]
        static void ShowDictionaryTranslate()
        {
            Skill.Editor.Tools.DictionaryTranslateWindow.Instance.Show();
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

        [UnityEditor.MenuItem("Assets/Create/Skill/Matinee", false, 41)]
        static void CreateMatinee()
        {
            Selection.activeObject = new GameObject("New Matinee", typeof(Skill.Framework.Sequence.Matinee));
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/Dictionary", false, 42)]
        static void CreateDictionary()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<Skill.Framework.Dictionary>("Dictionary");
        }

        [MenuItem("Assets/Create/Skill/Spawn Asset", false, 43)]
        static void CreateSpawnData()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Framework.SpawnAsset>("SpawnData");
        }

        [MenuItem("Assets/Create/Skill/Implant Asset", false, 44)]
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








        // ********************************* Commands ********************************* 

        //[MenuItem("Skill/Commands/Create Empty Child %m", false, 60)] // Ctrl + m
        //static void CreateEmptyChildWithName()
        //{
        //    Skill.Editor.Tools.Commands.CreateEmptyChild("GameObject");
        //}

        //[MenuItem("Skill/Commands/Create Empty Child Dialog %#m", false, 61)] // Ctrl + Shift + m
        //static void CreateEmptyChild()
        //{
        //    Skill.Editor.Tools.CreateEmptyChildObjectDialog.Instance.ShowUtility();
        //}



        [MenuItem("Skill/Camera/Align MainCamera to Selected", false, 80)]
        static void AlignMainCameraToSelectedObject()
        {
            Skill.Editor.Tools.Commands.AlignMainCameraToSelectedObject();
        }
        [MenuItem("Skill/Camera/Align MainCamera to View", false, 81)]
        static void AlignMainCameraToView()
        {
            Skill.Editor.Tools.Commands.AlignMainCameraToView();
        }

        [MenuItem("Skill/Edit/Group %g", false, 70)] // Ctrl + G
        static void GroupObjects()
        {
            Skill.Editor.Tools.Commands.GroupObjects("Group");
        }

        [MenuItem("Skill/Edit/Group Dialog %#g", false, 71)] // Ctrl + Shift + G
        static void GroupObjectsWithName()
        {
            Skill.Editor.Tools.CreateGroupDialog.Instance.ShowUtility();
        }

        [MenuItem("Skill/Edit/Copy Transform", false, 72)]
        static void CopyTransform()
        {
            Skill.Editor.Tools.Commands.CopyTransform();
        }

        [MenuItem("Skill/Edit/Paste Transform", false, 73)]
        static void PasteTransform()
        {
            Skill.Editor.Tools.Commands.PasteTransform();
        }


    }
}
