using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Skill.Framework.UI;
using Skill.Editor.UI;
using System;
using Skill.Framework;
using Skill.Framework.Modules;

namespace Skill.Editor
{
    [CustomEditor(typeof(PaintColor))]
    public class PaintColorEditor : UnityEditor.Editor
    {

        #region UI
        private StackPanel _Panel;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Editor.UI.ObjectField<Texture2D> _TextureField;        

        private Skill.Editor.UI.ColorField _ColorField;

        private Skill.Editor.UI.Slider _SliRadius;
        private Skill.Editor.UI.Slider _SliStrength;
        private Skill.Editor.UI.Slider _SliFalloff;

        private Editor.UI.Extended.TabHeader _TbChannels;

        private Editor.UI.Extended.TabHeader _TbModes;

        private Skill.Editor.UI.Popup _PUV;
        private HelpBox _HelpBox;

        private Grid _PnlFavoriteColors;
        private Color[] _FavoriteColors;

        private Skill.Framework.UI.Button _BtnSaveTexture;

        private void CreateUI()
        {
            _Panel = new StackPanel() { Padding = new Skill.Framework.UI.Thickness(2, 4, 2, 0) };
            _TextureField = new Skill.Editor.UI.ObjectField<Texture2D>() { Object = _PaintColor.Texture, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Center, Width = 80, Height = 80, Margin = new Skill.Framework.UI.Thickness(2) };            

            #region _TbChannels
            _TbChannels = new UI.Extended.TabHeader(4, true) { Margin = new Thickness(0, 2, 0, 10), HorizontalAlignment = HorizontalAlignment.Center, Width = 200, Height = 20 };
            _TbChannels[0].text = "R";
            _TbChannels[1].text = "G";
            _TbChannels[2].text = "B";
            _TbChannels[3].text = "A";

            _TbChannels.SetTabSelected(0, _PaintColor.ChannelR);
            _TbChannels.SetTabSelected(1, _PaintColor.ChannelG);
            _TbChannels.SetTabSelected(2, _PaintColor.ChannelB);
            _TbChannels.SetTabSelected(3, _PaintColor.ChannelA);
            #endregion

            #region _TbModes

            _TbModes = new UI.Extended.TabHeader(2, false) { Margin = new Thickness(0, 2, 0, 10), HorizontalAlignment = HorizontalAlignment.Center, Width = 200 };

            _TbModes[0].text = "Paint";
            _TbModes[1].text = "Erase";
            _TbModes.SelectedTab = _PaintColor.EraseEnable ? 1 : 0;

            _TbModes.ColumnDefinitions.Add(4, GridUnitType.Pixel);
            _TbModes.ColumnDefinitions.Add(1, GridUnitType.Star);

            _PUV = new Popup() { Row = 0, Column = 3 };
            PopupOption uv1 = new PopupOption(0) { Name = "UV1" }; uv1.Content.text = "UV 1";
            PopupOption uv2 = new PopupOption(1) { Name = "UV2" }; uv2.Content.text = "UV 2";
            _PUV.Options.Add(uv1);
            _PUV.Options.Add(uv2);
            _PUV.SelectedIndex = _PaintColor.EraseEnable ? 1 : 0;
            _TbModes.Controls.Add(_PUV);
            #endregion

            _PnlFavoriteColors = new Grid() { Height = 22, Margin = new Thickness(0, 2) };
            _FavoriteColors = new Color[] { new Color(1.0f,0.0f,0.0f,0.0f),   // red
                                            new Color(0.0f,1.0f,0.0f,0.0f),   // green
                                            new Color(0.0f,0.0f,1.0f,0.0f),   // blue
                                            new Color(0.0f,0.0f,0.0f,1.0f),   // Alpha
                                            new Color(1.0f,1.0f,1.0f,1.0f),   // white
                                            new Color(0.0f,0.0f,0.0f,0.0f) }; // black
            _PnlFavoriteColors.Controls.Add(new Box() { Row = 0, Column = 0, ColumnSpan = _FavoriteColors.Length });
            for (int i = 0; i < _FavoriteColors.Length; i++)
            {
                _PnlFavoriteColors.ColumnDefinitions.Add(1, GridUnitType.Star);
                Skill.Framework.UI.Button btn = new Skill.Framework.UI.Button() { Tag = i.ToString(), Margin = new Thickness(4), Style = new GUIStyle(), Column = i };
                string toolTip = string.Empty;
                Texture2D background = null;
                if (i == 0) { background = Skill.Editor.Resources.UITextures.Colors.Red; toolTip = "Red"; }
                else if (i == 1) { background = Skill.Editor.Resources.UITextures.Colors.Green; toolTip = "Green"; }
                else if (i == 2) { background = Skill.Editor.Resources.UITextures.Colors.Blue; toolTip = "Blue"; }
                else if (i == 3) { background = Skill.Editor.Resources.UITextures.Colors.Transparent; toolTip = "Alpha"; }
                else if (i == 4) { background = Skill.Editor.Resources.UITextures.Colors.White; toolTip = "White"; }
                else if (i == 5) { background = Skill.Editor.Resources.UITextures.Colors.Black; toolTip = "Black"; }
                btn.Style.normal.background = btn.Style.focused.background = btn.Style.hover.background = btn.Style.active.background = background;
                btn.Content.tooltip = toolTip;
                btn.Click += btn_Click;
                _PnlFavoriteColors.Controls.Add(btn);
            }


            _ColorField = new ColorField() { Color = _PaintColor.EraseEnable ? _PaintColor.Erase : _PaintColor.Paint, Margin = new Thickness(2) };
            _SliRadius = new Skill.Editor.UI.Slider() { Value = _PaintColor.Radius, MinValue = 1, MaxValue = 128, Margin = new Thickness(2), Height = 16 }; _SliRadius.Label.text = "Radius"; _SliRadius.Label.tooltip = "Shift + (A/D)";
            _SliStrength = new Skill.Editor.UI.Slider() { Value = _PaintColor.Strength, MinValue = 0.001f, MaxValue = 0.5f, Margin = new Thickness(2), Height = 16 }; _SliStrength.Label.text = "Strength";
            _SliFalloff = new Skill.Editor.UI.Slider() { Value = _PaintColor.Falloff, MinValue = 0.0f, MaxValue = 1.0f, Margin = new Thickness(2), Height = 16 }; _SliFalloff.Label.text = "Falloff";
            _HelpBox = new HelpBox() { Height = 60, Message = "Hold CTRL and drag with Right Click to paint.\nTexture must be read/write enable\nValid texture format:\n    ARGB32, RGBA32, RGB24 and Alpha8" };

            _BtnSaveTexture = new Skill.Framework.UI.Button() { Margin = new Thickness(2), Height = 40 }; _BtnSaveTexture.Content.text = "Save Texture";


            _Panel.Controls.Add(_TextureField);            
            _Panel.Controls.Add(_TbChannels);
            _Panel.Controls.Add(_TbModes);
            _Panel.Controls.Add(_PnlFavoriteColors);
            _Panel.Controls.Add(_ColorField);
            _Panel.Controls.Add(_SliRadius);
            _Panel.Controls.Add(_SliStrength);
            _Panel.Controls.Add(_SliFalloff);
            _Panel.Controls.Add(_HelpBox);
            _Panel.Controls.Add(_BtnSaveTexture);

            _Frame = new Frame("Frame");
            _Frame.Grid.Controls.Add(_Panel);

            _TextureField.ObjectChanged += _TextureField_ObjectChanged;

            _TbChannels.TabChanged += Channel_Changed;

            _TbModes.SelectedTabChanged += PaintMode_Changed;
            _PUV.OptionChanged += _PUV_OptionChanged;
            _ColorField.ColorChanged += _ColorField_ColorChanged;

            _SliRadius.ValueChanged += _SliRadius_ValueChanged;
            _SliStrength.ValueChanged += _SliStrength_ValueChanged;
            _SliFalloff.ValueChanged += _SliFalloff_ValueChanged;
            _BtnSaveTexture.Click += _BtnSaveTexture_Click;            
        }

        private int TabCount
        {
            get
            {
                int count = 0;
                if (_PaintColor.ChannelR) count++;
                if (_PaintColor.ChannelG) count++;
                if (_PaintColor.ChannelB) count++;
                if (_PaintColor.ChannelA) count++;
                return count;
            }
        }        

        void _BtnSaveTexture_Click(object sender, EventArgs e)
        {
            if (_PaintColor.Texture != null)
            {

                var bytes = _PaintColor.Texture.EncodeToPNG();
                string texturePass = AssetDatabase.GetAssetPath(_PaintColor.Texture).Replace("Assets/", string.Empty);
                string filePath = Application.dataPath + "/" + texturePass;
                System.IO.Path.ChangeExtension(filePath, ".png");
                System.IO.FileStream file = null;
                if (System.IO.File.Exists(filePath))
                    file = System.IO.File.Open(filePath, System.IO.FileMode.Open);
                else
                    file = System.IO.File.Open(filePath, System.IO.FileMode.Create);
                var binary = new System.IO.BinaryWriter(file);
                binary.Write(bytes);
                file.Close();
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            Skill.Framework.UI.Button btn = (Skill.Framework.UI.Button)sender;
            int index;
            if (int.TryParse(btn.Tag, out index))
            {
                if (_PaintColor.EraseEnable)
                    _ColorField.Color = _PaintColor.Erase = _FavoriteColors[index];
                else
                    _ColorField.Color = _PaintColor.Paint = _FavoriteColors[index];
            }
        }

        void _SliFalloff_ValueChanged(object sender, EventArgs e)
        {
            _PaintColor.Falloff = _SliFalloff.Value;
            EditorUtility.SetDirty(_PaintColor);
        }

        void _SliStrength_ValueChanged(object sender, EventArgs e)
        {
            _PaintColor.Strength = _SliStrength.Value;
            EditorUtility.SetDirty(_PaintColor);
        }

        void _SliRadius_ValueChanged(object sender, EventArgs e)
        {
            _PaintColor.Radius = _SliRadius.Value;
            EditorUtility.SetDirty(_PaintColor);
        }

        void _ColorField_ColorChanged(object sender, EventArgs e)
        {
            if (_PaintColor.EraseEnable)
                _PaintColor.Erase = _ColorField.Color;
            else
                _PaintColor.Paint = _ColorField.Color;
            EditorUtility.SetDirty(_PaintColor);
        }

        void _PUV_OptionChanged(object sender, EventArgs e)
        {
            _PaintColor.UV2 = _PUV.SelectedIndex == 1;
            EditorUtility.SetDirty(_PaintColor);
        }

        void PaintMode_Changed(object sender, EventArgs e)
        {
            _PaintColor.EraseEnable = _TbModes.SelectedTab == 1;
            _ColorField.Color = _PaintColor.EraseEnable ? _PaintColor.Erase : _PaintColor.Paint;
            EditorUtility.SetDirty(_PaintColor);
        }

        void Channel_Changed(object sender, EventArgs e)
        {
            _PaintColor.ChannelR = _TbChannels.IsTabSelected(0);
            _PaintColor.ChannelG = _TbChannels.IsTabSelected(1);
            _PaintColor.ChannelB = _TbChannels.IsTabSelected(2);
            _PaintColor.ChannelA = _TbChannels.IsTabSelected(3);            
            EditorUtility.SetDirty(_PaintColor);
        }
        void _TextureField_ObjectChanged(object sender, System.EventArgs e)
        {
            if (_TextureField.Object != null)
            {
                if (!CanPaintOnTextureFormat(_TextureField.Object.format))
                {
                    Debug.LogError("Unsupported texture format - needs to be ARGB32, RGBA32, RGB24 or Alpha8");
                    _PaintColor.Texture = _TextureField.Object = null;
                }
                else
                {
                    _PaintColor.Texture = _TextureField.Object;
                    EditorUtility.SetDirty(_PaintColor);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            _Frame.OnInspectorGUI(356);
        }
        #endregion



        private PaintColor _PaintColor;
        private BrushProjector _BrushProjector;
        private TextureBrush _Brush;

        void OnEnable()
        {
            _PaintColor = target as PaintColor;
            CreateUI();

            Texture2D brushTexture = EditorGUIUtility.Load("Brushes/builtin_brush_1.png") as Texture2D;
            _Brush = new TextureBrush(brushTexture, 64);
            _BrushProjector = new BrushProjector();
            _BrushProjector.Brush = _Brush;
        }

        void OnDestroy()
        {
            if (_Brush != null)
            {
                _Brush.Destroy();
                _Brush = null;
            }
            if (this._BrushProjector != null)
            {
                this._BrushProjector.Destroy();
                this._BrushProjector = null;
            }
        }


        void OnSceneGUI()
        {
            if (_PaintColor.gameObject != null && _PaintColor.Texture != null)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.KeyDown:
                        if (!e.functionKey && e.shift)
                        {
                            if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.D)
                            {
                                if (e.keyCode == KeyCode.A)
                                    _SliRadius.Value = _PaintColor.Radius = Mathf.Clamp(_PaintColor.Radius - 0.5f, _SliRadius.MinValue, _SliRadius.MaxValue);
                                else if (e.keyCode == KeyCode.D)
                                    _SliRadius.Value = _PaintColor.Radius = Mathf.Clamp(_PaintColor.Radius + 0.5f, _SliRadius.MinValue, _SliRadius.MaxValue);
                                e.Use();
                                HandleUtility.Repaint();
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        if (e.control || e.command)
                        {
                            if (_PaintColor.ChannelR || _PaintColor.ChannelB || _PaintColor.ChannelG || _PaintColor.ChannelA)
                            {
                                PaintOnTexture();
                            }
                            e.Use();
                        }
                        break;
                    case EventType.MouseMove:
                        HandleUtility.Repaint();
                        break;
                    case EventType.Repaint:
                        if (_BrushProjector != null) _BrushProjector.Projector.enabled = false;
                        break;
                }

                if (_BrushProjector != null)
                    UpdatePreviewBrush();
            }
        }

        private static bool CanPaintOnTextureFormat(TextureFormat format)
        {
            // also must checked for editable in importsetting
            return format == TextureFormat.RGBA32 || format == TextureFormat.ARGB32 || format == TextureFormat.RGB24 || format == TextureFormat.Alpha8;
        }

        private void UpdatePreviewBrush()
        {
            Vector3 normal = Vector3.zero;
            Vector3 hitPos = Vector3.zero;
            Vector2 vector;
            Vector3 vector2 = Vector3.zero;
            float m_Size = _PaintColor.Radius;
            float num = 1f;
            float num2 = _PaintColor.Texture.width / _PaintColor.Texture.height;

            Vector3 size = _PaintColor.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.size;

            int meshSizeX = (int)(size.x * _PaintColor.transform.localScale.x);
            int meshSizeZ = (int)(size.z * _PaintColor.transform.localScale.z);

            Transform PPtransform = _BrushProjector.Projector.transform;
            bool flag = true;

            Vector2 newMousePostion = Event.current.mousePosition;
            newMousePostion.y = Screen.height - (Event.current.mousePosition.y + 35);
            Ray ray = Camera.current.ScreenPointToRay(newMousePostion);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                vector2 = hit.point;
                hitPos = hit.point;
                normal = hit.normal;
                float num4 = ((m_Size % 2) != 0) ? 0.5f : 0f;
                int alphamapWidth = 64;
                int alphamapHeight = 64;

                float u, v;
                if (_PaintColor.UV2)
                {
                    u = Mathf.Repeat(hit.textureCoord2.x, 1.0f);
                    v = Mathf.Repeat(hit.textureCoord2.y, 1.0f);
                }
                else
                {
                    u = Mathf.Repeat(hit.textureCoord.x, 1.0f);
                    v = Mathf.Repeat(hit.textureCoord.y, 1.0f);
                }

                vector.x = (Mathf.Floor(u * alphamapWidth) + num4) / ((float)alphamapWidth);
                vector.y = (Mathf.Floor(v * alphamapHeight) + num4) / ((float)alphamapHeight);
                vector2.x = vector.x * -meshSizeX + (meshSizeX / 2);
                vector2.z = vector.y * -meshSizeZ + (meshSizeZ / 2);
                vector2 += _PaintColor.transform.position;
                num = ((m_Size * 0.5f) / ((float)alphamapWidth)) * meshSizeX;
                num2 = ((float)alphamapWidth) / ((float)alphamapHeight);
            }
            else
            {
                flag = false;
            }

            _BrushProjector.Projector.enabled = flag;
            if (flag)
            {
                PPtransform.position = hitPos + (normal * 10);
                PPtransform.rotation = Quaternion.LookRotation(normal);
            }
            _BrushProjector.Projector.orthographicSize = num / num2;
            _BrushProjector.Projector.aspectRatio = num2;

        }

        private void PaintOnTexture()
        {
            if (_PaintColor.gameObject != null && _PaintColor.Texture != null)
            {
                Event e = Event.current;
                RaycastHit hit;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == _PaintColor.gameObject)
                    {
                        float xCenterNormalized, yCenterNormalized;
                        if (_PaintColor.UV2)
                        {
                            xCenterNormalized = Mathf.Repeat(hit.textureCoord2.x, 1.0f);
                            yCenterNormalized = Mathf.Repeat(hit.textureCoord2.y, 1.0f);
                        }
                        else
                        {
                            xCenterNormalized = Mathf.Repeat(hit.textureCoord.x, 1.0f);
                            yCenterNormalized = Mathf.Repeat(hit.textureCoord.y, 1.0f);
                        }

                        int num = Mathf.FloorToInt(xCenterNormalized * _PaintColor.Texture.width);
                        int num2 = Mathf.FloorToInt(yCenterNormalized * _PaintColor.Texture.height);
                        int num3 = Mathf.RoundToInt(_PaintColor.Radius) / 2;
                        int num4 = Mathf.RoundToInt(_PaintColor.Radius) % 2;
                        int x = Mathf.Clamp(num - num3, 0, _PaintColor.Texture.width - 1);
                        int y = Mathf.Clamp(num2 - num3, 0, _PaintColor.Texture.height - 1);
                        int num7 = Mathf.Clamp((num + num3) + num4, 0, _PaintColor.Texture.width);
                        int num8 = Mathf.Clamp((num2 + num3) + num4, 0, _PaintColor.Texture.height);
                        int width = num7 - x;
                        int height = num8 - y;
                        Color[] srcPixels = _PaintColor.Texture.GetPixels(x, y, width, height, 0);
                        Color targetColor;
                        if (e.shift)
                            targetColor = _PaintColor.EraseEnable ? _PaintColor.Paint : _PaintColor.Erase;
                        else
                            targetColor = _PaintColor.EraseEnable ? _PaintColor.Erase : _PaintColor.Paint;


                        int centerX = width / 2;
                        int centerY = height / 2;

                        float min = _PaintColor.Radius * 0.3f;
                        float maxDistance = Mathf.Sqrt((min * min) * 2);
                        float fallOffDistance = (1.0f - _PaintColor.Falloff) * maxDistance;

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                float deltaX = Mathf.Abs(centerX - j);
                                float deltaY = Mathf.Abs(centerY - i);
                                float distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                                float blendFactor = _PaintColor.Strength;

                                if (distance < maxDistance)
                                {
                                    if (distance > fallOffDistance)
                                        blendFactor *= (1.0f - (distance - fallOffDistance) / (maxDistance - fallOffDistance));
                                }
                                else
                                    blendFactor = 0;

                                int index = (i * width) + j;

                                Color c = srcPixels[index];

                                if (_PaintColor.ChannelR) c.r = Mathf.Lerp(c.r, targetColor.r, blendFactor);
                                if (_PaintColor.ChannelG) c.g = Mathf.Lerp(c.g, targetColor.g, blendFactor);
                                if (_PaintColor.ChannelB) c.b = Mathf.Lerp(c.b, targetColor.b, blendFactor);
                                if (_PaintColor.ChannelA) c.a = Mathf.Lerp(c.a, targetColor.a, blendFactor);
                                
                                srcPixels[index] = c;
                            }
                        }
                        _PaintColor.Texture.SetPixels(x, y, width, height, srcPixels, 0);
                    }
                    _PaintColor.Texture.Apply();
                    EditorUtility.SetDirty(_PaintColor.Texture);

                }
            }
        }        
    }

}