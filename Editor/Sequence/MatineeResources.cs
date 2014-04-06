using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    public static class MatineeResources
    {
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

        public static class Textures
        {

            private static System.Collections.Generic.Dictionary<string, Texture2D> _Textures;
            private static Texture2D LoadTexture(string fileName)
            {
                string path = "Assets/Skill/Editor/Resources/UI/Matinee/" + fileName;
                Texture2D result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                if (result == null)
                    Debug.LogWarning("Can not load " + path);
                return result;
            }
            private static Texture2D GetTexture(string name)
            {
                if (_Textures == null) _Textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
                Texture2D result = null;
                if (!_Textures.TryGetValue(name, out result))
                {
                    result = LoadTexture(name);
                    if (result != null)
                        _Textures.Add(name, result);
                }
                return result;
            }

            public static Texture2D Folder { get { return GetTexture("Folder.png"); } }
            public static Texture2D Sound { get { return GetTexture("Sound.png"); } }
            public static Texture2D Event { get { return GetTexture("Event.png"); } }
            public static Texture2D Boolean { get { return GetTexture("Boolean.png"); } }
            public static Texture2D Integer { get { return GetTexture("Integer.png"); } }
            public static Texture2D Float { get { return GetTexture("Float.png"); } }
            public static Texture2D Quaternion { get { return GetTexture("Quaternion.png"); } }
            public static Texture2D Color { get { return GetTexture("Color.png"); } }
            public static Texture2D Vector2 { get { return GetTexture("Vector2.png"); } }
            public static Texture2D Vector3 { get { return GetTexture("Vector3.png"); } }
            public static Texture2D Vector4 { get { return GetTexture("Vector4.png"); } }

            public static Texture2D SelectedEventBorder { get { return GetTexture("SelectedEventBorder.png"); } }

            public static Texture2D Checkbox_Checked { get { return GetTexture("Checkbox_Checked.png"); } }
            public static Texture2D Checkbox_Unchecked { get { return GetTexture("Checkbox_Unchecked.png"); } }
        }

        public static class Styles
        {
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

                        //_SelectedEventBorder = new GUIStyle((GUIStyle)"LightmapEditorSelectedHighlight");

                        _SelectedEventBorder = new GUIStyle();
                        _SelectedEventBorder.normal.background = Textures.SelectedEventBorder;
                        _SelectedEventBorder.border = new RectOffset(2, 2, 2, 2);
                        _SelectedEventBorder.overflow = new RectOffset(1, 1, 1, 1);

                    }
                    return _SelectedEventBorder;
                }
            }
        }

    }
}