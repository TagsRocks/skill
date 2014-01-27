using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Simple cursor
    /// </summary>
    public class Cursor : MonoBehaviour
    {
        /// <summary>
        /// Cursor texture
        /// </summary>
        public Texture2D Normal;

        protected virtual Texture2D CursorTexture { get; set; }
        void Awake()
        {

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        this.gameObject.SetActive(false);
#endif
            Screen.showCursor = false;
        }

        void OnGUI()
        {
            Texture2D cr = CursorTexture;
            if (cr != null)
            {
                var pos = Input.mousePosition;
                float w = cr.width;
                float h = cr.height;
                GUI.DrawTexture(new Rect(pos.x - (w * 0.5f), (Screen.height - pos.y) - (h * 0.5f), w, h), cr);
            }
        }
    }
}
