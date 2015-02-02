using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Skill.Editor
{
    static class Commands
    {
        /// <summary>
        /// Copy localPosition, localRotation and localScale of selected objetc to clipboard as formated string
        /// </summary>
        public static void CopyTransform()
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                Quaternion rotation = t.localRotation;
                Vector3 position = t.localPosition;
                Vector3 scale = t.localScale;

                string tData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", rotation.x, rotation.y, rotation.z, rotation.w,
                                                                                       position.x, position.y, position.z,
                                                                                       scale.x, scale.y, scale.z);
                EditorGUIUtility.systemCopyBuffer = tData;
                Debug.Log("Transform copied to clipboard.");
            }
            else
            {
                Debug.LogError("No selected transform to copy.");
            }
        }

        /// <summary>
        /// Paste localPosition, localRotation and localScale from clipboard as formated string to selected objetc
        /// </summary>
        public static void PasteTransform()
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                string tData = EditorGUIUtility.systemCopyBuffer;
                if (string.IsNullOrEmpty(tData))
                {
                    Debug.LogError("Clipboard is empty.");
                    return;
                }
                string[] values = tData.Split(new char[] { ',' });
                if (values == null || values.Length != 10)
                {
                    Debug.LogError("Invalid transform data.");
                    return;
                }

                float rx, ry, rz, rw;
                float px, py, pz;
                float sx, sy, sz;

                if (!float.TryParse(values[0], out rx)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[1], out ry)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[2], out rz)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[3], out rw)) { Debug.LogWarning("Invalid transform data."); return; }

                if (!float.TryParse(values[4], out px)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[5], out py)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[6], out pz)) { Debug.LogWarning("Invalid transform data."); return; }

                if (!float.TryParse(values[7], out sx)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[8], out sy)) { Debug.LogWarning("Invalid transform data."); return; }
                if (!float.TryParse(values[9], out sz)) { Debug.LogWarning("Invalid transform data."); return; }

                t.localRotation = new Quaternion(rx, ry, rz, rw);
                t.localPosition = new Vector3(px, py, pz);
                t.localScale = new Vector3(sx, sy, sz);

                Debug.Log("Transform pasted.");
            }
            else
            {
                Debug.LogError("No selected transform to paste.");
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

                if (transform.collider != null)
                {
                    centerY = transform.collider.bounds.center.y;
                    minY = transform.collider.bounds.min.y;
                }

                position.y += centerY - minY;
                transform.position = position;
            }
        }
    }
}
