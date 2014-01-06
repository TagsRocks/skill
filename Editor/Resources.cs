using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public static class Resources
    {
        private static string GetResourcePath(string resourceFileName)
        {
            return "Assets/Skill/Editor/Resources/" + resourceFileName;
        }
        private static string GetUIResourcePath(string resourceFileName)
        {
            return "Assets/Skill/Editor/Resources/UI/" + resourceFileName;
        }
        private static void SetTextureImportType(string path, TextureImporterType type)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.GUI;
            AssetDatabase.ImportAsset(path);
        }

        public static void ApplyImportSettings()
        {
            SetTextureImportType(GetUIResourcePath("SelectedItemBg.png"), TextureImporterType.GUI);
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
            SetTextureImportType(GetUIResourcePath("EvenItem.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("OddItem.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Red.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Green.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Blue.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Black.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/White.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Yellow.png"), TextureImporterType.GUI);
            SetTextureImportType(GetUIResourcePath("Colors/Transparent.png"), TextureImporterType.GUI);            
        }

        private static Texture2D LoadTexture2D(string path)
        {
            Texture2D result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            if (result == null)
                Debug.LogWarning("Can not load " + path);
            return result;
        }


        public static class Textures
        {
            private static Texture2D _SelectedItemBackground;
            public static Texture2D SelectedItemBackground
            {
                get
                {
                    if (_SelectedItemBackground == null)
                        _SelectedItemBackground = LoadTexture2D(GetUIResourcePath("SelectedItemBg.png"));
                    return _SelectedItemBackground;
                }
            }

            private static Texture2D _Condition;
            public static Texture2D Condition
            {
                get
                {
                    if (_Condition == null)
                        _Condition = LoadTexture2D(GetUIResourcePath("Condition.png"));
                    return _Condition;
                }
            }

            private static Texture2D _Decorator;
            public static Texture2D Decorator
            {
                get
                {
                    if (_Decorator == null)
                        _Decorator = LoadTexture2D(GetUIResourcePath("Decorator.png"));
                    return _Decorator;
                }
            }

            private static Texture2D _Action;
            public static Texture2D Action
            {
                get
                {
                    if (_Action == null)
                        _Action = LoadTexture2D(GetUIResourcePath("Action.png"));
                    return _Action;
                }
            }

            private static Texture2D _Failure;
            public static Texture2D Failure
            {
                get
                {
                    if (_Failure == null)
                        _Failure = LoadTexture2D(GetUIResourcePath("Failure.png"));
                    return _Failure;
                }
            }

            private static Texture2D _Success;
            public static Texture2D Success
            {
                get
                {
                    if (_Success == null)
                        _Success = LoadTexture2D(GetUIResourcePath("Success.png"));
                    return _Success;
                }
            }

            private static Texture2D _Running;
            public static Texture2D Running
            {
                get
                {
                    if (_Running == null)
                        _Running = LoadTexture2D(GetUIResourcePath("Running.png"));
                    return _Running;
                }
            }

            private static Texture2D _WhiteTexture;
            public static Texture2D WhiteTexture
            {
                get
                {
                    if (_WhiteTexture == null)
                        _WhiteTexture = LoadTexture2D(GetUIResourcePath("White.png"));
                    return _WhiteTexture;
                }
            }

            private static Texture2D _Plus;
            public static Texture2D Plus
            {
                get
                {
                    if (_Plus == null)
                        _Plus = LoadTexture2D(GetUIResourcePath("Plus.png"));
                    return _Plus;
                }
            }

            private static Texture2D _PlusNext;
            public static Texture2D PlusNext
            {
                get
                {
                    if (_PlusNext == null)
                        _PlusNext = LoadTexture2D(GetUIResourcePath("PlusNext.png"));
                    return _PlusNext;
                }
            }

            private static Texture2D _Minus;
            public static Texture2D Minus
            {
                get
                {
                    if (_Minus == null)
                        _Minus = LoadTexture2D(GetUIResourcePath("Minus.png"));
                    return _Minus;
                }
            }

            private static Texture2D _EvenItem;
            public static Texture2D EvenItem
            {
                get
                {
                    if (_EvenItem == null)
                        _EvenItem = LoadTexture2D(GetUIResourcePath("EvenItem.png"));
                    return _EvenItem;
                }
            }

            private static Texture2D _OddItem;
            public static Texture2D OddItem
            {
                get
                {
                    if (_OddItem == null)
                        _OddItem = LoadTexture2D(GetUIResourcePath("OddItem.png"));
                    return _OddItem;
                }
            }

            private static Texture2D _Red;
            public static Texture2D Red
            {
                get
                {
                    if (_Red == null)
                        _Red = LoadTexture2D(GetUIResourcePath("Colors/Red.png"));
                    return _Red;
                }
            }
            private static Texture2D _Green;
            public static Texture2D Green
            {
                get
                {
                    if (_Green == null)
                        _Green = LoadTexture2D(GetUIResourcePath("Colors/Green.png"));
                    return _Green;
                }
            }

            private static Texture2D _Blue;
            public static Texture2D Blue
            {
                get
                {
                    if (_Blue == null)
                        _Blue = LoadTexture2D(GetUIResourcePath("Colors/Blue.png"));
                    return _Blue;
                }
            }

            private static Texture2D _Black;
            public static Texture2D Black
            {
                get
                {
                    if (_Black == null)
                        _Black = LoadTexture2D(GetUIResourcePath("Colors/Black.png"));
                    return _Black;
                }
            }

            private static Texture2D _White;
            public static Texture2D White
            {
                get
                {
                    if (_White == null)
                        _White = LoadTexture2D(GetUIResourcePath("Colors/White.png"));
                    return _White;
                }
            }

            private static Texture2D _Transparent;
            public static Texture2D Transparent
            {
                get
                {
                    if (_Transparent == null)
                        _Transparent = LoadTexture2D(GetUIResourcePath("Colors/Transparent.png"));
                    return _Transparent;
                }
            }            

            private static Texture2D _Yellow;
            public static Texture2D Yellow
            {
                get
                {
                    if (_Yellow == null)
                        _Yellow = LoadTexture2D(GetUIResourcePath("Colors/Yellow.png"));
                    return _Yellow;
                }
            }
        }
        public static class Styles
        {
            private static GUIStyle _SmallButtonStyle;
            public static GUIStyle SmallButtonStyle
            {
                get
                {
                    if (_SmallButtonStyle == null)
                    {
                        _SmallButtonStyle = new GUIStyle();
                        _SmallButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                        _SmallButtonStyle.margin = new RectOffset(1, 1, 1, 1);
                    }
                    return _SmallButtonStyle;
                }
            }
        }
    }
}
