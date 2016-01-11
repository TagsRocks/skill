using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.IO
{
    /// <summary>
    /// base class for joy elements
    /// </summary>
    public abstract class JoyElement : DynamicBehaviour
    {
        /// <summary> absolute render area of element </summary>
        public Rect RenderArea = new Rect(400, 400, 40, 40);

        /// <summary> default render area of element relative to screen</summary>
        public Rect DefaultRenderArea = new Rect(0.1f, 0.1f, 0.5f, 0.5f);




        [ContextMenu("ResetRenderArea")]
        public void SetDefault()
        {
            float factor = (Screen.width + Screen.height) * 0.5f;
            RenderArea.x = DefaultRenderArea.x * Screen.width;
            RenderArea.y = DefaultRenderArea.y * Screen.height;
            RenderArea.width = DefaultRenderArea.width * factor;
            RenderArea.height = DefaultRenderArea.height * factor;
        }



#if UNITY_EDITOR
        protected virtual void OnGUI()
        {
            if (!Application.isPlaying)
            {
                Rect renderArea = this.RenderArea;
                renderArea.y = Screen.height - renderArea.y - renderArea.height;
                GUI.Box(renderArea, this.name);
            }
        }
#endif
    }
}