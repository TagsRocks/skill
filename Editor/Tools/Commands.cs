using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Skill.Editor.Tools
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
    }
}
