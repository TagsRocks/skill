using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public static class Resources
    {
        public static string GetUIResourcePath(string resourceFileName)
        {
            if (UnityEditor.EditorGUIUtility.isProSkin)
                return "Assets/Skill/Editor/Resources/ProSkin/" + resourceFileName;
            else
                return "Assets/Skill/Editor/Resources/LightSkin/" + resourceFileName;
        }
        private static void SetTextureImportType(string path, TextureImporterType type)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.GUI;
            AssetDatabase.ImportAsset(path);
        }
        public static void ApplyImportSettings()
        {
            SetTextureImportType(GetUIResourcePath("Condition.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Action.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Decorator.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Failure.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Success.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Running.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("White.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Plus.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("PlusNext.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Minus.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("Play.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("PlayOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Pause.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("PauseOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("StepForward.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("StepForwardOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("StepBackward.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("StepBackwardOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Loop.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("LoopOn.png"), TextureImporterType.GUI);


            SetTextureImportType(GetUIResourcePath("RecordOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("RecordOff.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("Add.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Remove.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Save.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Arrow_Right.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("SelectedEventBorder.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("Paste.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("TimeFormat.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("SequenceFormat.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("Keyframe.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("KeyframeSelected.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("KeyframeEmpty.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Event.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Key.png"), TextureImporterType.GUI);

            SetTextureImportType(GetUIResourcePath("Matinee/Folder.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Sound.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Event.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Boolean.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Integer.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Float.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Quaternion.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Color.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Vector2.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Vector3.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Vector4.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Animator.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Matinee/Animation.png"), TextureImporterType.GUI);
        }

        public static class UITextures
        {
            private static System.Collections.Generic.Dictionary<string, Texture2D> _Textures;
            private static Texture2D GetTexture(string name)
            {
                if (_Textures == null) _Textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
                Texture2D result = null;
                if (!_Textures.TryGetValue(name, out result))
                {
                    result = AssetDatabase.LoadAssetAtPath(GetUIResourcePath(name), typeof(Texture2D)) as Texture2D;
                    if (result == null)
                        Debug.LogWarning("Can not load " + name);

                    if (result != null)
                        _Textures.Add(name, result);
                }
                return result;
            }

            public static Texture2D Condition { get { return GetTexture("Condition.png"); } }
            public static Texture2D Decorator { get { return GetTexture("Decorator.png"); } }
            public static Texture2D Action { get { return GetTexture("Action.png"); } }
            public static Texture2D Failure { get { return GetTexture("Failure.png"); } }
            public static Texture2D Success { get { return GetTexture("Success.png"); } }
            public static Texture2D Running { get { return GetTexture("Running.png"); } }
            public static Texture2D Plus { get { return GetTexture("Plus.png"); } }
            public static Texture2D PlusNext { get { return GetTexture("PlusNext.png"); } }
            public static Texture2D Minus { get { return GetTexture("Minus.png"); } }
            public static Texture2D SelectedEventBorder { get { return GetTexture("SelectedEventBorder.png"); } }
            public static Texture2D Add { get { return GetTexture("Add.png"); } }
            public static Texture2D Remove { get { return GetTexture("Remove.png"); } }
            public static Texture2D Save { get { return GetTexture("Save.png"); } }
            public static Texture2D ArrowRight { get { return GetTexture("Arrow_Right.png"); } }
            public static Texture2D Paste { get { return GetTexture("Paste.png"); } }

            public static Texture2D TimeFormat { get { return GetTexture("TimeFormat.png"); } }
            public static Texture2D SequenceFormat { get { return GetTexture("SequenceFormat.png"); } }

            public static Texture2D Keyframe { get { return GetTexture("Keyframe.png"); } }
            public static Texture2D KeyframeSelected { get { return GetTexture("KeyframeSelected.png"); } }
            public static Texture2D KeyframeEmpty { get { return GetTexture("KeyframeEmpty.png"); } }
            public static Texture2D Event { get { return GetTexture("Event.png"); } }

            public static Texture2D Key { get { return GetTexture("Key.png"); } }

            #region Color Textures
            private static Texture2D _Red;
            public static Texture2D Red
            {
                get
                {
                    if (_Red == null)
                        _Red = CreateTexture(Color.red, 2);
                    return _Red;
                }
            }
            private static Texture2D _Green;
            public static Texture2D Green
            {
                get
                {
                    if (_Green == null)
                        _Green = CreateTexture(Color.green, 2);
                    return _Green;
                }
            }

            private static Texture2D _Blue;
            public static Texture2D Blue
            {
                get
                {
                    if (_Blue == null)
                        _Blue = CreateTexture(Color.blue, 2);
                    return _Blue;
                }
            }

            private static Texture2D _Black;
            public static Texture2D Black
            {
                get
                {
                    if (_Black == null)
                        _Black = CreateTexture(Color.black, 2);
                    return _Black;
                }
            }

            private static Texture2D _White;
            public static Texture2D White
            {
                get
                {
                    if (_White == null)
                        _White = CreateTexture(Color.white, 2);
                    return _White;
                }
            }

            private static Texture2D _Transparent;
            public static Texture2D Transparent
            {
                get
                {
                    if (_Transparent == null)
                        _Transparent = CreateTexture(Color.clear, 2);
                    return _Transparent;
                }
            }

            private static Texture2D _Yellow;
            public static Texture2D Yellow
            {
                get
                {
                    if (_Yellow == null)
                        _Yellow = CreateTexture(Color.yellow, 2);
                    return _Yellow;
                }
            }
            #endregion

            #region Splitter
            private static Texture2D _HorizontalSplitter;
            public static Texture2D HorizontalSplitter
            {
                get
                {
                    if (_HorizontalSplitter == null)
                    {
                        float f;
                        if (UnityEditor.EditorGUIUtility.isProSkin)
                            f = 0.129f;
                        else
                            f = 0.629f;
                        _HorizontalSplitter = Skill.Editor.Resources.CreateTexture(new Color(f, f, f, 1.0f), 2);
                    }
                    return _HorizontalSplitter;
                }
            }

            private static Texture2D _VerticalSplitter;
            public static Texture2D VerticalSplitter
            {
                get
                {
                    if (_VerticalSplitter == null)
                    {
                        float f;
                        if (UnityEditor.EditorGUIUtility.isProSkin)
                            f = 0.129f;
                        else
                            f = 0.629f;
                        _VerticalSplitter = Skill.Editor.Resources.CreateTexture(new Color(f, f, f, 1.0f), 2);
                    }
                    return _VerticalSplitter;
                }
            }
            #endregion

            #region Media buttons

            public static Texture2D RecordOn { get { return GetTexture("RecordOn.png"); } }
            public static Texture2D RecordOff { get { return GetTexture("RecordOff.png"); } }

            public static Texture2D Play { get { return GetTexture("Play.png"); } }
            public static Texture2D PlayOn { get { return GetTexture("PlayOn.png"); } }
            public static Texture2D Pause { get { return GetTexture("Pause.png"); } }
            public static Texture2D PauseOn { get { return GetTexture("PauseOn.png"); } }
            public static Texture2D StepForward { get { return GetTexture("StepForward.png"); } }
            public static Texture2D StepForwardOn { get { return GetTexture("StepForwardOn.png"); } }
            public static Texture2D StepBackward { get { return GetTexture("StepBackward.png"); } }
            public static Texture2D StepBackwardOn { get { return GetTexture("StepBackwardOn.png"); } }
            public static Texture2D Loop { get { return GetTexture("Loop.png"); } }
            public static Texture2D LoopOn { get { return GetTexture("LoopOn.png"); } }
            #endregion

            #region Matinee
            public static class Matinee
            {
                public static Texture2D Folder { get { return GetTexture("Matinee/Folder.png"); } }
                public static Texture2D Sound { get { return GetTexture("Matinee/Sound.png"); } }
                public static Texture2D Event { get { return GetTexture("Matinee/Event.png"); } }
                public static Texture2D Boolean { get { return GetTexture("Matinee/Boolean.png"); } }
                public static Texture2D Integer { get { return GetTexture("Matinee/Integer.png"); } }
                public static Texture2D Float { get { return GetTexture("Matinee/Float.png"); } }
                public static Texture2D Quaternion { get { return GetTexture("Matinee/Quaternion.png"); } }
                public static Texture2D Color { get { return GetTexture("Matinee/Color.png"); } }
                public static Texture2D Vector2 { get { return GetTexture("Matinee/Vector2.png"); } }
                public static Texture2D Vector3 { get { return GetTexture("Matinee/Vector3.png"); } }
                public static Texture2D Vector4 { get { return GetTexture("Matinee/Vector4.png"); } }

                public static Texture2D Animator { get { return GetTexture("Matinee/Animator.png"); } }
                public static Texture2D Animation { get { return GetTexture("Matinee/Animation.png"); } }
            }
            #endregion


        }
        public static class Styles
        {
            private static GUIStyle _SmallButton;
            public static GUIStyle SmallButton
            {
                get
                {
                    if (_SmallButton == null)
                    {
                        _SmallButton = new GUIStyle();
                        _SmallButton.padding = new RectOffset(0, 0, 0, 0);
                        _SmallButton.margin = new RectOffset(1, 1, 1, 1);
                    }
                    return _SmallButton;
                }
            }


            private static GUIStyle _SelectedItem;
            public static GUIStyle SelectedItem
            {
                get
                {
                    if (_SelectedItem == null)
                        _SelectedItem = (GUIStyle)"SelectionRect";
                    return _SelectedItem;
                }
            }

            private static GUIStyle _HSplitter;
            public static GUIStyle HorizontalSplitter
            {
                get
                {
                    if (_HSplitter == null)
                    {
                        _HSplitter = new GUIStyle();
                        _HSplitter.normal.background = UITextures.HorizontalSplitter;
                        _HSplitter.border = new RectOffset(0, 0, 1, 1);
                    }
                    return _HSplitter;
                }
            }

            private static GUIStyle _VSplitter;
            public static GUIStyle VerticalSplitter
            {
                get
                {
                    if (_VSplitter == null)
                    {
                        _VSplitter = new GUIStyle();
                        _VSplitter.normal.background = UITextures.VerticalSplitter;
                        _VSplitter.border = new RectOffset(1, 1, 0, 0);
                    }
                    return _VSplitter;
                }
            }

            private static GUIStyle _Header;
            public static GUIStyle Header
            {
                get
                {
                    if (_Header == null)
                    {
                        _Header = new GUIStyle(UnityEditor.EditorStyles.toolbar);
                        _Header.normal.textColor = UnityEditor.EditorStyles.label.normal.textColor;
                        _Header.fixedHeight = 0;
                        _Header.alignment = TextAnchor.MiddleLeft;

                    }
                    return _Header;
                }
            }


            private static GUIStyle _Toolbar;
            public static GUIStyle Toolbar
            {
                get
                {
                    if (_Toolbar == null)
                    {
                        _Toolbar = new GUIStyle(UnityEditor.EditorStyles.toolbar);
                        _Toolbar.fixedHeight = 0;
                    }
                    return _Toolbar;
                }
            }

            private static GUIStyle _ToolbarButton;
            public static GUIStyle ToolbarButton
            {
                get
                {
                    if (_ToolbarButton == null)
                    {
                        _ToolbarButton = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
                        _ToolbarButton.fixedHeight = 0;
                    }
                    return _ToolbarButton;
                }
            }


            private static GUIStyle _ToolbarPopup;
            public static GUIStyle ToolbarPopup
            {
                get
                {
                    if (_ToolbarPopup == null)
                    {
                        _ToolbarPopup = new GUIStyle(UnityEditor.EditorStyles.toolbarPopup);
                        _ToolbarPopup.fixedHeight = 0;
                    }
                    return _ToolbarPopup;
                }
            }

            private static GUIStyle _BackgroundShadow;
            public static GUIStyle BackgroundShadow
            {
                get
                {
                    if (_BackgroundShadow == null)
                    {
                        _BackgroundShadow = (GUIStyle)"InnerShadowBg";
                    }
                    return _BackgroundShadow;
                }
            }


            private static GUIStyle _LeftButton;
            public static GUIStyle LeftButton
            {
                get
                {
                    if (_LeftButton == null)
                    {
                        _LeftButton = (GUIStyle)"ButtonLeft";
                    }
                    return _LeftButton;
                }
            }

            private static GUIStyle _RightButton;
            public static GUIStyle RightButton
            {
                get
                {
                    if (_RightButton == null)
                    {
                        _RightButton = (GUIStyle)"ButtonRight";
                    }
                    return _RightButton;
                }
            }

            private static GUIStyle _MiddleButton;
            public static GUIStyle MiddleButton
            {
                get
                {
                    if (_MiddleButton == null)
                    {
                        _MiddleButton = (GUIStyle)"ButtonMid";
                    }
                    return _MiddleButton;
                }
            }



            private static GUIStyle _LargeLeftButton;
            public static GUIStyle LargeLeftButton
            {
                get
                {
                    if (_LargeLeftButton == null)
                    {
                        _LargeLeftButton = (GUIStyle)"LargeButtonLeft";
                    }
                    return _LargeLeftButton;
                }
            }

            private static GUIStyle _LargeRightButton;
            public static GUIStyle LargeRightButton
            {
                get
                {
                    if (_LargeRightButton == null)
                    {
                        _LargeRightButton = (GUIStyle)"LargeButtonRight";
                    }
                    return _LargeRightButton;
                }
            }

            private static GUIStyle _LargeMiddleButton;
            public static GUIStyle LargeMiddleButton
            {
                get
                {
                    if (_LargeMiddleButton == null)
                    {
                        _LargeMiddleButton = (GUIStyle)"LargeButtonMid";
                    }
                    return _LargeMiddleButton;
                }
            }

            private static GUIStyle _PreviewBackground;
            public static GUIStyle PreviewBackground
            {
                get
                {
                    if (_PreviewBackground == null)
                        _PreviewBackground = (GUIStyle)"PreBackground";
                    return _PreviewBackground;
                }
            }


            private static GUIStyle _TreeViewItem;
            public static GUIStyle TreeViewItem
            {
                get
                {
                    if (_TreeViewItem == null)
                    {
                        _TreeViewItem = new GUIStyle(EditorStyles.label);
                        _TreeViewItem.imagePosition = ImagePosition.ImageLeft;
                        _TreeViewItem.alignment = TextAnchor.MiddleLeft;
                        _TreeViewItem.padding = new RectOffset(2, 2, 2, 2);
                    }
                    return _TreeViewItem;
                }
            }

            private static GUIStyle _HiddenTreeViewItem;
            public static GUIStyle HiddenTreeViewItem
            {
                get
                {
                    if (_HiddenTreeViewItem == null)
                    {
                        _HiddenTreeViewItem = new GUIStyle(TreeViewItem);
                        Color color = _HiddenTreeViewItem.normal.textColor;
                        color.a -= 0.2f;
                        _HiddenTreeViewItem.normal.textColor = color;
                    }
                    return _HiddenTreeViewItem;
                }
            }


            private static GUIStyle _EventLabel;
            public static GUIStyle EventLabel
            {
                get
                {
                    if (_EventLabel == null)
                    {
                        _EventLabel = new GUIStyle();
                        _EventLabel.clipping = TextClipping.Clip;
                        _EventLabel.alignment = TextAnchor.UpperLeft;
                        _EventLabel.padding = new RectOffset(2, 2, 2, 2);
                        _EventLabel.normal.textColor = Color.white;
                    }
                    return _EventLabel;
                }
            }

            private static GUIStyle _SelectedEventBorder;
            public static GUIStyle SelectedEventBorder
            {
                get
                {
                    if (_SelectedEventBorder == null)
                    {
                        _SelectedEventBorder = new GUIStyle();
                        _SelectedEventBorder.normal.background = UITextures.SelectedEventBorder;
                        _SelectedEventBorder.border = new RectOffset(2, 2, 2, 2);
                        _SelectedEventBorder.overflow = new RectOffset(1, 1, 1, 1);

                    }
                    return _SelectedEventBorder;
                }
            }

            private static GUIStyle _MiniToolbarButton;
            public static GUIStyle MiniToolbarButton
            {
                get
                {
                    if (_MiniToolbarButton == null)
                        _MiniToolbarButton = (GUIStyle)"MiniToolbarButton";
                    return _MiniToolbarButton;
                }
            }

            private static GUIStyle _MiniToolbarButtonLeft;
            public static GUIStyle MiniToolbarButtonLeft
            {
                get
                {
                    if (_MiniToolbarButtonLeft == null)
                        _MiniToolbarButtonLeft = (GUIStyle)"MiniToolbarButtonLeft";
                    return _MiniToolbarButtonLeft;
                }
            }

            private static GUIStyle _MiniToolbarPopup;
            public static GUIStyle MiniToolbarPopup
            {
                get
                {
                    if (_MiniToolbarPopup == null)
                        _MiniToolbarPopup = (GUIStyle)"MiniToolbarPopup";
                    return _MiniToolbarPopup;
                }
            }
        }

        public static class Colors
        {
            public static Color ThumbColor
            {
                get
                {
                    if (UnityEditor.EditorGUIUtility.isProSkin) 
                        return new Color(1.0f, 0.0f, 1.0f, 1.0f);
                    else
                        return new Color(0.5f, 0.0f, 0.5f, 1.0f);
                }
            }
        }

        #region Helper methods
        /// <summary>
        /// Create a texture and fill it with specified color
        /// </summary>
        /// <param name="color">Color of texture</param>
        /// <returns>Texture</returns>
        /// <param name="size">size of texture</param>
        /// <returns>Texture</returns>
        public static Texture2D CreateTexture(Color color, int size)
        {
            size = Mathf.Max(size, 1);
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, true);
            FillImage(texture, 0, 0, size, size, color);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Create a texture and fill it with specified color
        /// </summary>
        /// <param name="color">Color of texture</param>
        /// <returns>Texture</returns>
        /// <param name="size">size of texture</param>
        /// <param name="border">Border</param>
        /// <param name="borderColor">Border color</param>
        /// <returns>Texture</returns>        
        public static Texture2D CreateTexture(Color color, int size, Skill.Framework.UI.Thickness border, Color borderColor)
        {
            size = Mathf.Max(size, 1);
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, true);
            FillImage(texture, 0, 0, size, size, color);

            if (border.Left >= 1) FillImage(texture, 0, 0, size, (int)border.Left, borderColor);
            if (border.Right >= 1) FillImage(texture, 0, size - (int)border.Right - 1, size, (int)border.Left, borderColor);
            if (border.Top >= 1) FillImage(texture, 0, 0, (int)border.Top, size, borderColor);
            if (border.Bottom >= 1) FillImage(texture, size - (int)border.Bottom - 1, 0, (int)border.Bottom, size, borderColor);

            texture.Apply();
            return texture;
        }
        private static void FillImage(Texture2D texture, int rowIndex, int columnIndex, int rowCount, int columnCount, Color color)
        {
            rowCount += rowIndex;
            columnCount += columnIndex;
            for (int i = rowIndex; i < rowCount; i++)
            {
                for (int j = columnIndex; j < columnCount; j++)
                {
                    texture.SetPixel(i, j, color);
                }
            }
        }
        public static Texture2D FlipHorizontalFrom(Texture2D texture)
        {
            Texture2D result = new Texture2D(texture.width, texture.height, texture.format, false);
            Color[] colors = texture.GetPixels(0, 0, texture.width, texture.height, 0);

            for (int i = 0; i < result.height; i++)
            {
                for (int j = 0; j < result.width / 2; j++)
                {
                    int j2 = result.width - j;
                    Color temp = colors[j];
                    colors[j] = colors[j2];
                    colors[j2] = temp;
                }
            }
            result.SetPixels(colors);
            result.Apply();
            return result;
        }
        public static void SaveTexture(Texture2D texture, string address)
        {
            byte[] bytes = texture.EncodeToPNG();

            System.IO.FileStream fs = new System.IO.FileStream(address, System.IO.FileMode.CreateNew);
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(fs);
            w.Write(bytes);
            w.Close();
            fs.Close();
        }
        #endregion
    }
}
