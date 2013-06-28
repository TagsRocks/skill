using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    internal static class Resources
    {
        private static string GetResourcePath(string resourceFileName)
        {
            return "Assets/Editor/Skill/Resources/" + resourceFileName;
        }
        private static string GetUIResourcePath(string resourceFileName)
        {
            return "Assets/Editor/Skill/Resources/UI/" + resourceFileName;
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
        }

        private static Texture2D LoadTexture2D(string path)
        {
            Texture2D result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            if (result == null)
                Debug.LogWarning("Can not load " + path);
            return result;            
        }

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
    }
}
