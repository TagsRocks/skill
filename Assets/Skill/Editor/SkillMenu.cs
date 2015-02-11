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
            Distance.Instance.Show();
        }

        [MenuItem("Skill/Tools/Copy Component", false, 1)]
        static void ShowCopyComponentsWindow()
        {
            CopyPaseComponentsWindow.Instance.Show();
        }

        [MenuItem("Skill/Tools/Move", false, 2)]
        static void ShowMoveWindow()
        {
            MoveWindow.Instance.Show();
        }




        [MenuItem("Skill/Diagnostics/BehaviorTree State", false, 400)]
        static void ShowBTreeStateDebugger()
        {
            Diagnostics.BehaviorTreeStateDebugger.Instance.Show();
        }

        [MenuItem("Skill/Matinee Editor", false, 600)]
        static void ShowMatineeEditor()
        {
            Skill.Editor.Sequence.MatineeEditorWindow.Instance.Show();
        }

        [UnityEditor.MenuItem("Skill/Curve Editor", false, 601)]
        static void ShowCurveEditorWindow()
        {
            Skill.Editor.Curve.CurveEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Dictionary", false, 602)]
        static void ShowDictionaryEditor()
        {
            DictionaryEditorWindow.Instance.Show();
        }

        [MenuItem("Skill/Translator", false, 603)]
        static void ShowDictionaryTranslate()
        {
            DictionaryTranslateWindow.Instance.Show();
        }






        [MenuItem("Skill/About", false, 10000)]
        static void ShowAbout()
        {
            AboutSkill.Instance.ShowUtility();
        }


        // ********************************* Create Menu ********************************* 

        [UnityEditor.MenuItem("Assets/Create/Skill/BehaviorTree", false, 41)]
        static void CreateBehaviorTreeDataAsset()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<AI.BehaviorTreeAsset>("BehaviorTree");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/SharedAccessKeys", false, 42)]
        static void CreateSharedAccessKeysDataAsset()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<AI.SharedAccessKeysAsset>("SharedAccessKeys");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/AnimationTree", false, 43)]
        static void CreateAnimationTreeDataAsset()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<Animation.AnimationTreeAsset>("AnimationTree");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/SkinMesh", false, 44)]
        static void CreateSkinMeshDataAsset()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<Animation.SkinMeshAsset>("SkinMesh");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/SaveData", false, 45)]
        static void CreateSaveDataAsset()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<IO.SaveDataAsset>("SaveData");
        }

        [UnityEditor.MenuItem("Assets/Create/Skill/Dictionary", false, 46)]
        static void CreateDictionary()
        {
            Skill.Editor.ScriptableObjectUtility.CreateAsset<Skill.Framework.Dictionary>("Dictionary");
        }

        [MenuItem("Assets/Create/Skill/Spawn Asset", false, 47)]
        static void CreateSpawnData()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Framework.SpawnAsset>("SpawnData");
        }

        [MenuItem("Assets/Create/Skill/Implant Asset", false, 48)]
        static void CreateImplantAsset()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Editor.ImplantAsset>("ImplantAsset", delegate(Skill.Editor.ImplantAsset asset)
            {
                if (asset.DefaultObject == null)
                    asset.DefaultObject = new Skill.Editor.ImplantObject();
                asset.DefaultObject.Prefab = null;
                asset.DefaultObject.MinScalePercent = 0.8f;
                asset.DefaultObject.MaxScalePercent = 1.0f;
                asset.DefaultObject.Weight = 1.0f;
                asset.DefaultObject.Rotation = Skill.Editor.ImplantObjectRotation.SurfaceNormal;
            });

        }


        [MenuItem("Assets/Create/Skill/BrowserStyle", false, 49)]
        static void CreateBrowserStyle()
        {
            ScriptableObjectUtility.CreateAsset<Skill.Framework.UI.BrowserStyles>("BrowserStyle");
        }


        // ********************************* Commands ********************************* 

        //[MenuItem("Skill/Commands/Create Empty Child %m", false, 60)] // Ctrl + m
        //static void CreateEmptyChildWithName()
        //{
        //    Commands.CreateEmptyChild("GameObject");
        //}

        //[MenuItem("Skill/Commands/Create Empty Child Dialog %#m", false, 61)] // Ctrl + Shift + m
        //static void CreateEmptyChild()
        //{
        //    CreateEmptyChildObjectDialog.Instance.ShowUtility();
        //}



        [MenuItem("Skill/Camera/Align MainCamera to Selected", false, 200)]
        static void AlignMainCameraToSelectedObject()
        {
            Commands.AlignMainCameraToSelectedObject();
        }
        [MenuItem("Skill/Camera/Align MainCamera to View", false, 201)]
        static void AlignMainCameraToView()
        {
            Commands.AlignMainCameraToView();
        }

        [MenuItem("Skill/Edit/Group %g", false, 100)] // Ctrl + G
        static void GroupObjects()
        {
            Commands.GroupObjects("Group");
        }

        [MenuItem("Skill/Edit/Group Dialog %#g", false, 101)] // Ctrl + Shift + G
        static void GroupObjectsWithName()
        {
            CreateGroupDialog.Instance.ShowUtility();
        }

        [MenuItem("Skill/Edit/Copy Transform", false, 102)]
        static void CopyTransform()
        {
            Commands.CopyTransform();
        }

        [MenuItem("Skill/Edit/Paste Transform", false, 103)]
        static void PasteTransform()
        {
            Commands.PasteTransform();
        }


        [UnityEditor.MenuItem("CONTEXT/BoxCollider/CenterPivot")]
        public static void CenterPivot(UnityEditor.MenuCommand command)
        {
            BoxCollider collider = command.context as BoxCollider;
            if (collider != null)
                Commands.CenterPivot(collider);
        }
        [MenuItem("CONTEXT/Transform/Bring to Ground")]
        public static void BringToGround(UnityEditor.MenuCommand command)
        {
            Transform transform = command.context as Transform;
            if (transform != null)
                Commands.BringToGround(transform);
        }
    }
}
