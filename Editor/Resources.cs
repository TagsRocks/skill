using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public static class Resources
    {
        public static string GetResourcePath(string resourceFileName)
        {
            return "Assets/Skill/Editor/Resources/" + resourceFileName;
        }
        private static void SetTextureImportType(string path, TextureImporterType type)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.GUI;
            AssetDatabase.ImportAsset(path);
        }
        private static Texture2D LoadTexture2D(string path)
        {
            Texture2D result = AssetDatabase.LoadAssetAtPath(GetResourcePath(path), typeof(Texture2D)) as Texture2D;
            if (result == null)
                Debug.LogWarning("Can not load " + path);
            return result;
        }

        public static void ApplyImportSettings()
        {
            SetTextureImportType(GetResourcePath("UI/Condition.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Action.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Decorator.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Failure.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Success.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Running.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/White.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Plus.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/PlusNext.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Minus.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/Play.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/PlayOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Pause.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/PauseOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/StepForward.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/StepForwardOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/StepBackward.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/StepBackwardOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Loop.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/LoopOn.png"), TextureImporterType.GUI);


            SetTextureImportType(GetResourcePath("UI/RecordOn.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/RecordOff.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/Add.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Remove.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Save.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Arrow_Right.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/SelectedEventBorder.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/Paste.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/TimeFormat.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/SequenceFormat.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/Keyframe.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/KeyframeSelected.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Key.png"), TextureImporterType.GUI);

            SetTextureImportType(GetResourcePath("UI/Matinee/Folder.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Sound.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Event.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Boolean.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Integer.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Float.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Quaternion.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Color.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Vector2.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Vector3.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Vector4.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Checkbox_Checked.png"), TextureImporterType.GUI);
            SetTextureImportType(GetResourcePath("UI/Matinee/Checkbox_Unchecked.png"), TextureImporterType.GUI);
        }

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

        public static class Textures
        {
            private static System.Collections.Generic.Dictionary<string, Texture2D> _Textures;
            private static Texture2D GetTexture(string name)
            {
                if (_Textures == null) _Textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
                Texture2D result = null;
                if (!_Textures.TryGetValue(name, out result))
                {
                    result = LoadTexture2D(name);
                    if (result != null)
                        _Textures.Add(name, result);
                }
                return result;
            }

            public static Texture2D Condition { get { return GetTexture("UI/Condition.png"); } }
            public static Texture2D Decorator { get { return GetTexture("UI/Decorator.png"); } }
            public static Texture2D Action { get { return GetTexture("UI/Action.png"); } }
            public static Texture2D Failure { get { return GetTexture("UI/Failure.png"); } }
            public static Texture2D Success { get { return GetTexture("UI/Success.png"); } }
            public static Texture2D Running { get { return GetTexture("UI/Running.png"); } }
            public static Texture2D Plus { get { return GetTexture("UI/Plus.png"); } }
            public static Texture2D PlusNext { get { return GetTexture("UI/PlusNext.png"); } }
            public static Texture2D Minus { get { return GetTexture("UI/Minus.png"); } }
            public static Texture2D SelectedEventBorder { get { return GetTexture("UI/SelectedEventBorder.png"); } }
            public static Texture2D Add { get { return GetTexture("UI/Add.png"); } }
            public static Texture2D Remove { get { return GetTexture("UI/Remove.png"); } }
            public static Texture2D Save { get { return GetTexture("UI/Save.png"); } }
            public static Texture2D ArrowRight { get { return GetTexture("UI/Arrow_Right.png"); } }
            public static Texture2D Paste { get { return GetTexture("UI/Paste.png"); } }

            public static Texture2D TimeFormat { get { return GetTexture("UI/TimeFormat.png"); } }
            public static Texture2D SequenceFormat { get { return GetTexture("UI/SequenceFormat.png"); } }

            public static Texture2D Keyframe { get { return GetTexture("UI/Keyframe.png"); } }
            public static Texture2D KeyframeSelected { get { return GetTexture("UI/KeyframeSelected.png"); } }

            public static Texture2D Key { get { return GetTexture("UI/Key.png"); } }

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
                        _HorizontalSplitter = Skill.Editor.Resources.CreateTexture(new Color(0.129f, 0.129f, 0.129f, 1.0f), 2);
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
                        _VerticalSplitter = Skill.Editor.Resources.CreateTexture(new Color(0.129f, 0.129f, 0.129f, 1.0f), 2);
                    }
                    return _VerticalSplitter;
                }
            }
            #endregion

            #region Media buttons

            public static Texture2D RecordOn { get { return GetTexture("UI/RecordOn.png"); } }
            public static Texture2D RecordOff { get { return GetTexture("UI/RecordOff.png"); } }            

            public static Texture2D Play { get { return GetTexture("UI/Play.png"); } }
            public static Texture2D PlayOn { get { return GetTexture("UI/PlayOn.png"); } }
            public static Texture2D Pause { get { return GetTexture("UI/Pause.png"); } }
            public static Texture2D PauseOn { get { return GetTexture("UI/PauseOn.png"); } }
            public static Texture2D StepForward { get { return GetTexture("UI/StepForward.png"); } }
            public static Texture2D StepForwardOn { get { return GetTexture("UI/StepForwardOn.png"); } }
            public static Texture2D StepBackward { get { return GetTexture("UI/StepBackward.png"); } }
            public static Texture2D StepBackwardOn { get { return GetTexture("UI/StepBackwardOn.png"); } }
            public static Texture2D Loop { get { return GetTexture("UI/Loop.png"); } }
            public static Texture2D LoopOn { get { return GetTexture("UI/LoopOn.png"); } }
            #endregion

            #region Matinee
            public static class Matinee
            {
                public static Texture2D Folder { get { return GetTexture("UI/Matinee/Folder.png"); } }
                public static Texture2D Sound { get { return GetTexture("UI/Matinee/Sound.png"); } }
                public static Texture2D Event { get { return GetTexture("UI/Matinee/Event.png"); } }
                public static Texture2D Boolean { get { return GetTexture("UI/Matinee/Boolean.png"); } }
                public static Texture2D Integer { get { return GetTexture("UI/Matinee/Integer.png"); } }
                public static Texture2D Float { get { return GetTexture("UI/Matinee/Float.png"); } }
                public static Texture2D Quaternion { get { return GetTexture("UI/Matinee/Quaternion.png"); } }
                public static Texture2D Color { get { return GetTexture("UI/Matinee/Color.png"); } }
                public static Texture2D Vector2 { get { return GetTexture("UI/Matinee/Vector2.png"); } }
                public static Texture2D Vector3 { get { return GetTexture("UI/Matinee/Vector3.png"); } }
                public static Texture2D Vector4 { get { return GetTexture("UI/Matinee/Vector4.png"); } }
                public static Texture2D Checkbox_Checked { get { return GetTexture("UI/Matinee/Checkbox_Checked.png"); } }
                public static Texture2D Checkbox_Unchecked { get { return GetTexture("UI/Matinee/Checkbox_Unchecked.png"); } }
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
                        _HSplitter.normal.background = Textures.HorizontalSplitter;
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
                        _VSplitter.normal.background = Textures.VerticalSplitter;
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
                        _Header = UnityEditor.EditorStyles.toolbar;
                        _Header.normal.textColor = Color.white;
                        _Header.alignment = TextAnchor.MiddleLeft;

                    }
                    return _Header;
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
                        _TreeViewItem = new GUIStyle();
                        _TreeViewItem.imagePosition = ImagePosition.ImageLeft;
                        _TreeViewItem.alignment = TextAnchor.MiddleLeft;
                        _TreeViewItem.padding = new RectOffset(2, 2, 2, 2);
                        _TreeViewItem.normal.textColor = Color.white;
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
                        _HiddenTreeViewItem.normal.textColor = Color.gray;
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
                        _SelectedEventBorder.normal.background = Textures.SelectedEventBorder;
                        _SelectedEventBorder.border = new RectOffset(2, 2, 2, 2);
                        _SelectedEventBorder.overflow = new RectOffset(1, 1, 1, 1);

                    }
                    return _SelectedEventBorder;
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
    }
}
