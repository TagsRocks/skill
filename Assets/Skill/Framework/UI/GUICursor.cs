using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Simple cursor
    /// </summary>
    public class GUICursor : MonoBehaviour
    {
        public int GUIDepth = 0;
        /// <summary>
        /// Cursor texture
        /// </summary>
        public Texture2D Normal;
        public Vector2 Offset = new Vector2(32, 32);

        protected virtual Texture2D CursorTexture { get; set; }
        void Awake()
        {

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        this.gameObject.SetActive(false);
#endif
            Cursor.visible  = false;
            CursorTexture = Normal;
        }

        void OnGUI()
        {
            Texture2D cr = CursorTexture;
            if (cr != null)
            {
                GUI.depth = GUIDepth;
                var pos = Input.mousePosition;
                float w = cr.width;
                float h = cr.height;
                GUI.DrawTexture(new Rect(pos.x + Offset.x, (Screen.height - pos.y) + Offset.y, w, h), cr);
            }
        }
    }
}
