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


        // ********************************* Commands ********************************* 


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

        [MenuItem("Skill/Edit/Copy Transform Hierarchy", false, 102)]
        static void CopyHierarchyTransform()
        {
            Skill.Editor.Commands.CopyTransformHierarchy();
        }

        [MenuItem("Skill/Edit/Copy Transform Hierarchy", true)]
        static bool ValidateCopyHierarchyTransform()
        {
            return Skill.Editor.Commands.ValidateCopyTransformHierarchy();
        }

        [MenuItem("Skill/Edit/Paste Transform Hierarchy", false, 103)]
        static void PasteHierarchyTransform()
        {
            Skill.Editor.Commands.PasteTransformHierarchy();
        }

        [MenuItem("Skill/Edit/Paste Transform Hierarchy", true)]
        static bool ValidatePasteTransformHierarchy()
        {
            return Skill.Editor.Commands.ValidatePasteTransformHierarchy();
        }


        [MenuItem("Skill/Edit/Export to Wavefront .obj", false, 104)]
        static void ExportToWavefrontObject()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            string fileName = EditorUtility.SaveFilePanel("Export to Wavefront .obj", "", "Object", "obj");
            if (!string.IsNullOrEmpty(fileName))
            {
                List<MeshFilter> meshes = new List<MeshFilter>();
                foreach (var g in Selection.gameObjects)
                    meshes.AddRange(g.GetComponentsInChildren<MeshFilter>());
                MeshFilter[] ms = meshes.ToArray();
                if (ms.Length > 0)
                {
                    MeshExporter exporter = new MeshExporter();

                    if (Selection.gameObjects.Length == 1)
                    {
                        Transform t = Selection.gameObjects[0].transform;
                        Vector3 originalPosition = t.position;
                        t.position = Vector3.zero;
                        exporter.ExportObject(fileName, ms);
                        t.position = originalPosition;
                    }
                    else
                    {
                        exporter.ExportObject(fileName, ms);
                    }
                }

            }
        }

        [MenuItem("Skill/Edit/Export to Wavefront .obj", true)]
        static bool ValidateExportToWavefrontObject()
        {
            if (Selection.gameObjects.Length == 0)
                return false;

            foreach (var g in Selection.gameObjects)
            {
                if (g.GetComponentsInChildren<MeshFilter>() != null)
                    return true;
            }

            return false;
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


        [MenuItem("CONTEXT/Text/ConvertToPersian")]
        public static void ConvertToPersian(UnityEditor.MenuCommand command)
        {
            UnityEngine.UI.Text uit = command.context as UnityEngine.UI.Text;
            if (uit != null)
                uit.text = Commands.ConvertToPersian(uit.text);
        }
    }
}
