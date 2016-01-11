using UnityEngine;
using System.Collections;


namespace Skill.Framework.ModernUI
{
    public class ColorKey : MonoBehaviour
    {
        public string NameInBank = "Text";


        void Start()
        {
            ColorBank.Apply(this);
        }

#if UNITY_EDITOR

        [ContextMenu("Update from Bank")]
        public void UpdateFromBank()
        {
            UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
            UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
            ToggleTextColor ttc = GetComponent<ToggleTextColor>();

            if (text != null) UnityEditor.Undo.RecordObject(text, "Change Color");
            if (button != null) UnityEditor.Undo.RecordObject(button, "Change Color");
            if (image != null) UnityEditor.Undo.RecordObject(image, "Change Color");
            if (ttc != null) UnityEditor.Undo.RecordObject(ttc, "Change Color");

            ColorBank.Apply(this);

            if (text != null) UnityEditor.EditorUtility.SetDirty(text);
            if (button != null) UnityEditor.EditorUtility.SetDirty(button);
            if (image != null) UnityEditor.EditorUtility.SetDirty(image);
            if (ttc != null) UnityEditor.EditorUtility.SetDirty(ttc);
        }

#endif
    }
}