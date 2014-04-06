using System;
using System.Collections.Generic;
using UnityEditor;

namespace Skill.Editor.UI
{
    public class GridSplitter : Skill.Framework.UI.GridSplitter
    {
        protected override void BeginRender()
        {
            base.BeginRender();
            EditorGUIUtility.AddCursorRect(_RenderAreaWidthOverFlow, Orientation == Skill.Framework.UI.Orientation.Horizontal ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal);
        }
    }
}
