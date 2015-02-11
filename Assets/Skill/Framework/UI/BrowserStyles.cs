using UnityEngine;
using System.Collections;

namespace Skill.Framework.UI
{
    public class BrowserStyles : ScriptableObject
    {
        public float ItemHeight = 40;
        public float UpWidth = 30;
        public float UpHeigth = 30;
        public float ButtonHeigth = 30;

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
