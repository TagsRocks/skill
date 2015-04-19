using UnityEngine;
using System.Collections;

namespace Skill.Framework.UI
{
    public class BrowserStyles : ScriptableObject
    {        
        public float PathFontSize = 0.1f;
        public float ItemFontSize = 0.1f;        

        public Texture2D DirectoryImage;
        public Texture2D FileImage;
        public Texture2D SelectIcon;
        public Texture2D CancelIcon;
        public Texture2D UpIcon;
        public Texture2D[] FileTypeIcons;

        public GUIStyle BackgroundStyle;
        public GUIStyle ListBackgroundStyle;
        public GUIStyle SelectedItemStyle;
        public GUIStyle DirectoryStyle;
        public GUIStyle FileStyle;
        public GUIStyle PathButtonStyle;        
        public GUIStyle DividerStyle;
        public GUIStyle PathDividerStyle;
    }
}
