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
        }

        private static Texture2D LoadTexture2D(string path)
        {
            Texture2D result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            if (result == null)
                Debug.LogWarning("Can not load " + path);
            return result;
        }

        /// <summary>
        /// Create a texture and fill it with specified color
        /// </summary>
        /// <param name="color">Color of texture</param>
        /// <returns>Texture</returns>
        public static Texture2D CreateColorTexture(Color color, int size = 1)
        {
            size = Mathf.Max(size, 1);
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, true);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    texture.SetPixel(i, j, color);
                }
            }
            texture.Apply();
            return texture;
        }


        public static class Textures
        {            
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



            private static Texture2D _Red;
            public static Texture2D Red
            {
                get
                {
                    if (_Red == null)
                        _Red = CreateColorTexture(Color.red, 2);
                    return _Red;
                }
            }
            private static Texture2D _Green;
            public static Texture2D Green
            {
                get
                {
                    if (_Green == null)
                        _Green = CreateColorTexture(Color.green, 2);
                    return _Green;
                }
            }

            private static Texture2D _Blue;
            public static Texture2D Blue
            {
                get
                {
                    if (_Blue == null)
                        _Blue = CreateColorTexture(Color.blue, 2);
                    return _Blue;
                }
            }

            private static Texture2D _Black;
            public static Texture2D Black
            {
                get
                {
                    if (_Black == null)
                        _Black = CreateColorTexture(Color.black, 2);
                    return _Black;
                }
            }

            private static Texture2D _White;
            public static Texture2D White
            {
                get
                {
                    if (_White == null)
                        _White = CreateColorTexture(Color.white, 2);
                    return _White;
                }
            }

            private static Texture2D _Transparent;
            public static Texture2D Transparent
            {
                get
                {
                    if (_Transparent == null)
                        _Transparent = CreateColorTexture(Color.clear, 2);
                    return _Transparent;
                }
            }

            private static Texture2D _Yellow;
            public static Texture2D Yellow
            {
                get
                {
                    if (_Yellow == null)
                        _Yellow = CreateColorTexture(Color.yellow, 2);
                    return _Yellow;
                }
            }
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
        }
    }
}
