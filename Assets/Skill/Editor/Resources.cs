using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public static class Resources
    {
        public const string LightSkinPath = "Assets/Skill/Editor/Res/LightSkin/";
        public const string ProSkinPath = "Assets/Skill/Editor/Res/ProSkin/";
        public const string ShaderPath = "Hidden/Skill/";

        public static string GetUILightSkinResourcePath(string resourceFileName)
        {
            return LightSkinPath + resourceFileName;
        }
        public static string GetUIProSkinResourcePath(string resourceFileName)
        {
            return ProSkinPath + resourceFileName;
        }
        private static void SetTextureImportType(string assetName, TextureImporterType type)
        {
            TextureImporter textureImporter = null;
            string path = GetUILightSkinResourcePath(assetName);
            textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.textureType = type;
                AssetDatabase.ImportAsset(path);
            }

            textureImporter = null;
            path = GetUIProSkinResourcePath(assetName);
            textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.textureType = type;
                AssetDatabase.ImportAsset(path);
            }
        }


        public static Shader GetShader(string shaderName)
        {
            Shader shader = Shader.Find(ShaderPath + shaderName) as Shader;
            if (shader == null)
                Debug.Log("Can not load shader " + shaderName);
            return shader;
        }


        public static class UITextures
        {
            private static System.Collections.Generic.Dictionary<string, Texture2D> _Textures;
            private static Texture2D GetTexture(string textureName)
            {
                if (_Textures == null) _Textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
                Texture2D result = null;
                if (!_Textures.TryGetValue(textureName, out result))
                {
                    if (UnityEditor.EditorGUIUtility.isProSkin)
                        result = AssetDatabase.LoadAssetAtPath(GetUIProSkinResourcePath(textureName), typeof(Texture2D)) as Texture2D;
                    if (result == null)
                        result = AssetDatabase.LoadAssetAtPath(GetUILightSkinResourcePath(textureName), typeof(Texture2D)) as Texture2D;
                    if (result == null)
                        Debug.LogWarning("Can not load " + textureName);

                    if (result != null)
                        _Textures.Add(textureName, result);
                }
                return result;
            }
            public static Texture2D Plus { get { return GetTexture("Plus.png"); } }
            public static Texture2D PlusNext { get { return GetTexture("PlusNext.png"); } }
            public static Texture2D Minus { get { return GetTexture("Minus.png"); } }
            public static Texture2D SelectedEventBorder { get { return GetTexture("SelectedEventBorder.png"); } }
            public static Texture2D Add { get { return GetTexture("Add.png"); } }
            public static Texture2D Remove { get { return GetTexture("Remove.png"); } }
            public static Texture2D Delete { get { return GetTexture("Delete.png"); } }
            public static Texture2D Save { get { return GetTexture("Save.png"); } }
            public static Texture2D Paste { get { return GetTexture("Paste.png"); } }
            public static Texture2D TimeFormat { get { return GetTexture("TimeFormat.png"); } }
            public static Texture2D SequenceFormat { get { return GetTexture("SequenceFormat.png"); } }
            public static Texture2D Keyframe { get { return GetTexture("Keyframe.png"); } }
            public static Texture2D KeyframeSelected { get { return GetTexture("KeyframeSelected.png"); } }
            public static Texture2D KeyframeEmpty { get { return GetTexture("KeyframeEmpty.png"); } }
            public static Texture2D Event { get { return GetTexture("Event.png"); } }
            public static Texture2D Key { get { return GetTexture("Key.png"); } }
            public static Texture2D Default { get { return GetTexture("Default.png"); } }

            public static Texture2D ArrowLeft { get { return GetTexture("ArrowLeft.png"); } }
            public static Texture2D ArrowRight { get { return GetTexture("ArrowRight.png"); } }
            public static Texture2D ArrowUp { get { return GetTexture("ArrowUp.png"); } }
            public static Texture2D ArrowDown { get { return GetTexture("ArrowDown.png"); } }
            public static Texture2D ArrowHead { get { return GetTexture("ArrowHead.png"); } }


            public static Texture2D Convert { get { return GetTexture("Convert.png"); } }

            public static Texture2D AlignLeft { get { return GetTexture("AlignLeft.png"); } }
            public static Texture2D AlignRight { get { return GetTexture("AlignRight.png"); } }
            public static Texture2D AlignTop { get { return GetTexture("AlignTop.png"); } }
            public static Texture2D AlignBottom { get { return GetTexture("AlignBottom.png"); } }
            public static Texture2D AlignCenteredHorizontal { get { return GetTexture("AlignCenteredHorizontal.png"); } }
            public static Texture2D AlignCenteredVertical { get { return GetTexture("AlignCenteredVertical.png"); } }



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

            #region Colors
            public static class Colors
            {
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
            }
            #endregion

            #region BTree
            public static class BTree
            {
                public static Texture2D Condition { get { return GetTexture("BTree/Condition.png"); } }
                public static Texture2D Decorator { get { return GetTexture("BTree/Decorator.png"); } }
                public static Texture2D Action { get { return GetTexture("BTree/Action.png"); } }
                public static Texture2D AccessLimitDecorator { get { return GetTexture("BTree/AccessLimitDecorator.png"); } }
                public static Texture2D Composite { get { return GetTexture("BTree/Composite.png"); } }
                public static Texture2D ChangeState { get { return GetTexture("BTree/ChangeState.png"); } }
                public static Texture2D Concurrent { get { return GetTexture("BTree/Concurrent.png"); } }
                public static Texture2D Loop { get { return GetTexture("BTree/Loop.png"); } }
                public static Texture2D Priority { get { return GetTexture("BTree/Priority.png"); } }
                public static Texture2D Sequence { get { return GetTexture("BTree/Sequence.png"); } }
                public static Texture2D Random { get { return GetTexture("BTree/Random.png"); } }
                public static Texture2D Failure { get { return GetTexture("BTree/Failure.png"); } }
                public static Texture2D Success { get { return GetTexture("BTree/Success.png"); } }
                public static Texture2D Running { get { return GetTexture("BTree/Running.png"); } }
                public static Texture2D BehaviorTree { get { return GetTexture("BTree/BehaviorTree.png"); } }
            }
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

            #region ATree
            public static class ATree
            {
                public static Texture2D TwoD { get { return GetTexture("ATree/4Dir.png"); } }
                public static Texture2D Additive { get { return GetTexture("ATree/Additive.png"); } }
                public static Texture2D Sequence { get { return GetTexture("ATree/AnimSequence.png"); } }
                public static Texture2D Blank { get { return GetTexture("ATree/Blank.png"); } }
                public static Texture2D ByIndex { get { return GetTexture("ATree/ByIndex.png"); } }
                public static Texture2D Override { get { return GetTexture("ATree/Override.png"); } }
                public static Texture2D Root { get { return GetTexture("ATree/Root.png"); } }
                public static Texture2D SubTree { get { return GetTexture("ATree/SubTree.png"); } }
                public static Texture2D MultiLayer { get { return GetTexture("ATree/MultiLayer.png"); } }
                public static Texture2D OneD { get { return GetTexture("ATree/Speed.png"); } }
                public static Texture2D AnimationTree { get { return GetTexture("ATree/AnimationTree.png"); } }
            }
            #endregion

            #region Icons
            public static class Icons
            {
                public static Texture2D Class { get { return GetTexture("ClassIcon.png"); } }
                public static Texture2D Primitive { get { return GetTexture("PrimitiveIcon.png"); } }
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



            private static GUIStyle _Connector;
            public static GUIStyle Connector
            {
                get
                {
                    if (_Connector == null)
                    {
                        _Connector = new GUIStyle();
                        _Connector.border = new RectOffset(2, 2, 2, 2);
                        _Connector.normal.background = CreateTexture(new Color(0.6f, 0.6f, 0.6f, 1.0f), 16, new Framework.UI.Thickness(1), new Color(0.4f, 0.4f, 0.4f, 1.0f));
                        _Connector.hover.background = CreateTexture(new Color(0.8f, 0.8f, 0.8f, 1.0f), 16);

                    }
                    return _Connector;
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
