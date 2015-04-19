using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Skill.Editor
{
    static class Commands
    {
        public static bool ValidateCopyTransformHierarchy()
        {
            Transform[] ts = Selection.transforms;
            return ts != null && ts.Length == 1;
        }

        /// <summary>
        /// Copy localPosition, localRotation and localScale of selected objetc to clipboard as formated string
        /// </summary>
        public static void CopyTransformHierarchy()
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                TransformData tr = new TransformData();
                tr.Copy(t);

                Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                document.AppendChild(tr.ToXmlElement());

                EditorGUIUtility.systemCopyBuffer = document.ToString();
                Debug.Log("Hierarchy Transform copied to clipboard.");
            }
            else
            {
                Debug.LogError("No selected transform to copy.");
            }
        }

        public static bool ValidatePasteTransformHierarchy()
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                string tData = EditorGUIUtility.systemCopyBuffer;
                return !string.IsNullOrEmpty(tData);
            }
            return false;
        }

        /// <summary>
        /// Paste localPosition, localRotation and localScale from clipboard as formated string to selected objetc
        /// </summary>
        public static void PasteTransformHierarchy()
        {
            Transform[] ts = Selection.transforms;
            if (ts != null && ts.Length > 0)
            {
                string tData = EditorGUIUtility.systemCopyBuffer;
                if (!string.IsNullOrEmpty(tData))
                {
                    try
                    {
                        Skill.Framework.IO.XmlDocument document = new Framework.IO.XmlDocument();
                        document.LoadXml(tData);

                        if (document.FirstChild != null)
                        {
                            TransformData td = new TransformData();
                            td.Load(document.FirstChild);

                            foreach (var t in ts)
                                td.Paste(t);
                            Debug.Log("Hierarchy Transform pasted.");
                        }
                        else
                            Debug.Log("Invalid Hierarchy Transform data.");
                    }
                    catch
                    {
                        Debug.Log("Invalid Hierarchy Transform data.");
                    }
                }
                else
                    Debug.Log("Invalid Hierarchy Transform data.");
            }
        }        


        /// <summary>
        /// Group selected objects
        /// </summary>
        public static void GroupObjects(string groupName)
        {
            Transform[] transforms = Selection.transforms;
            if (transforms != null && transforms.Length > 0)
            {
                GameObject group = new GameObject(groupName);
                Vector3 center = transforms[0].position;
                for (int i = 1; i < transforms.Length; i++)
                    center += transforms[i].position;

                Undo.RegisterCreatedObjectUndo(group, "Create : " + group.name);

                Transform parent = transforms[0].parent;
                center /= transforms.Length;
                group.transform.position = center;
                foreach (var t in transforms)
                {
                    if (parent != null)
                    {
                        if (parent != t.parent)
                            parent = null;
                    }
                    Undo.SetTransformParent(t, group.transform, string.Format("Group {0} to {1}", t.name, group.name));
                }
                if (parent != null)
                    Undo.SetTransformParent(group.transform, parent, string.Format("Group {0} to {1}", group.name, parent.name));
            }
            else
            {
                Debug.LogError("No selected objects to group.");
            }
        }


        /// <summary>
        /// Create child GameObject
        /// </summary>
        public static void CreateEmptyChild(string objectName)
        {
            GameObject obj = new GameObject(objectName);
            Transform[] transforms = Selection.transforms;
            if (transforms != null && transforms.Length == 1)
            {
                obj.transform.parent = transforms[0];
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
            }

            Undo.RegisterCreatedObjectUndo(obj, "Create empty GameObject");
        }



        /// <summary>
        /// Align camera to selected object
        /// </summary>
        public static void AlignMainCameraToSelectedObject()
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                if (Camera.main != null)
                {
                    Transform cameraT = Camera.main.transform;
                    Undo.RecordObject(cameraT, "Bring Camera To Selected Object");
                    cameraT.position = t.position;
                    cameraT.rotation = t.rotation;

                }
            }
        }

        /// <summary>
        /// Align camera to selected object
        /// </summary>
        public static void AlignMainCameraToView()
        {
            if (Camera.main != null)
            {
                Transform cameraT = Camera.main.transform;
                Selection.activeTransform = cameraT;
                Undo.RecordObject(cameraT, "Bring Camera To Selected Object");
                UnityEditor.SceneView.currentDrawingSceneView.AlignWithView();

            }

        }


        [UnityEditor.MenuItem("CONTEXT/BoxCollider/CenterPivot")]
        public static void CenterPivot(BoxCollider collider)
        {
            if (collider != null)
            {
                Vector3 center = collider.center;

                Undo.RecordObject(collider.transform, "CenterPivot");
                if (collider.transform.parent != null)
                    collider.transform.localPosition += collider.transform.localRotation * center;
                else
                    collider.transform.position += collider.transform.rotation * center;


                collider.center = Vector3.zero;
            }
        }

        public static void BringToGround(Transform transform)
        {

            RaycastHit hit;
            Ray ray = new Ray() { origin = transform.position, direction = Vector3.down };
            if (Physics.Raycast(ray, out hit, 10000))
            {
                Undo.RecordObject(transform, "Change Position");
                Vector3 position = hit.point;

                float centerY = position.y;
                float minY = position.y;

                Collider collider = transform.GetComponent<Collider>();
                if (collider != null)
                {
                    centerY = collider.bounds.center.y;
                    minY = collider.bounds.min.y;
                }

                position.y += centerY - minY;
                transform.position = position;
            }
        }
    }
}
