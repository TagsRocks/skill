using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.Framework.UI;
using Skill.Framework.Modules;
using Skill.Editor.UI;

namespace Skill.Editor
{
    [CustomEditor(typeof(Implant))]
    public class ImplantEditor : UnityEditor.Editor
    {

        #region UI

        private float _FrameSize = 300;
        private StackPanel _Panel;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Editor.UI.ToggleButton _TbEnable;
        private Skill.Editor.UI.ObjectField<ImplantAsset> _AssetField;
        private Skill.Editor.UI.ObjectField<Transform> _RootField;
        private Skill.Editor.UI.LayerMaskField _Layers;
        private Skill.Editor.UI.Slider _SliMinRadius;
        private Skill.Editor.UI.Slider _SliMaxRadius;
        private Skill.Editor.UI.IntSlider _SliDensity;
        private Skill.Editor.UI.Slider _SliRotation;
        private Skill.Editor.UI.Slider _SliOffsetY;
        private Skill.Editor.UI.Slider _SliRectWidth;
        private Skill.Editor.UI.Slider _SliRectHeight;
        private Skill.Editor.UI.HelpBox _Help;

        private TabHeader _TbModes;

        private void CreateUI()
        {
            Thickness margin = new Thickness(0, 2, 0, 4);
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Panel = new StackPanel();

            _TbEnable = new UI.ToggleButton() { IsChecked = _Implant.IsEnable, Margin = margin }; _TbEnable.Label.text = "Enable";

            _AssetField = new Skill.Editor.UI.ObjectField<ImplantAsset>() { VerticalAlignment = VerticalAlignment.Center, Margin = margin, AllowSceneObjects = false };
            _AssetField.Label.text = "ImplantAsset";
            if (_Implant.ImplantAsset != null)
            {
                _Asset = _Implant.ImplantAsset as ImplantAsset;
                _Implant.ImplantAsset = _AssetField.Object = _Asset;
            }

            _RootField = new ObjectField<Transform>() { Margin = margin, AllowSceneObjects = true, Object = _Implant.Root }; _RootField.Label.text = "Root";

            _Layers = new Skill.Editor.UI.LayerMaskField() { Layers = _Implant.LayerMask, VerticalAlignment = VerticalAlignment.Center, Margin = margin };
            _Layers.Label.text = "Raycast layers";

            _SliDensity = new Skill.Editor.UI.IntSlider() { Value = _Implant.Density, MinValue = 1, MaxValue = 1000, Margin = margin, Height = 16 }; _SliDensity.Label.text = "Density"; _SliDensity.Label.tooltip = "Shift + (W/S)";
            _SliRotation = new Skill.Editor.UI.Slider() { Value = _Implant.Rotation, MinValue = 0.0f, MaxValue = 360, Margin = margin, Height = 16 }; _SliRotation.Label.text = "Rotation"; _SliRotation.Label.tooltip = "Shift + (A/D)";
            _SliOffsetY = new Skill.Editor.UI.Slider() { Value = _Implant.OffsetY, MinValue = -10.0f, MaxValue = 20, Margin = margin, Height = 16 }; _SliOffsetY.Label.text = "Offset Y"; _SliOffsetY.Label.tooltip = "Offset in Y axis)";
            _SliMinRadius = new Skill.Editor.UI.Slider() { Value = _Implant.MinRadius, MinValue = 0.0f, MaxValue = 200, Margin = margin, Height = 16 }; _SliMinRadius.Label.text = "Min Radius";
            _SliMaxRadius = new Skill.Editor.UI.Slider() { Value = _Implant.MaxRadius, MinValue = 0.0f, MaxValue = 200, Margin = margin, Height = 16 }; _SliMaxRadius.Label.text = "Max Radius";
            _SliRectWidth = new Skill.Editor.UI.Slider() { Value = _Implant.RectWidth, MinValue = 0.0f, MaxValue = 1000, Margin = margin, Height = 16 }; _SliRectWidth.Label.text = "Width";
            _SliRectHeight = new Skill.Editor.UI.Slider() { Value = _Implant.RectHeight, MinValue = 0.0f, MaxValue = 1000, Margin = margin, Height = 16 }; _SliRectHeight.Label.text = "Height";
            _Help = new Skill.Editor.UI.HelpBox() { Message = "Hold Ctrl and Right Click to implant obejcts.\nMouse3 to shuffle.\nShift + R : Make reference point.\nSpace : R reference point", Height = 60, Margin = new Thickness(0, 10, 0, 0) };

            _TbModes = new TabHeader { Margin = new Thickness(0, 2, 0, 10), HorizontalAlignment = HorizontalAlignment.Center, Width = 200, Height = 20 };
            _TbModes[0].text = "Circle";
            _TbModes[1].text = "Rectangle";
            _TbModes.SelectedTab = _Implant.BrushMode;


            _Panel.Controls.Add(_TbEnable);
            _Panel.Controls.Add(_AssetField);
            _Panel.Controls.Add(_RootField);
            _Panel.Controls.Add(_Layers);
            _Panel.Controls.Add(_SliDensity);
            _Panel.Controls.Add(_SliRotation);
            _Panel.Controls.Add(_SliOffsetY);
            _Panel.Controls.Add(_TbModes);
            _Panel.Controls.Add(_SliMinRadius);
            _Panel.Controls.Add(_SliMaxRadius);
            _Panel.Controls.Add(_SliRectWidth);
            _Panel.Controls.Add(_SliRectHeight);
            _Panel.Controls.Add(_Help);
            _Frame.Grid.Controls.Add(_Panel);

            ChangeMode();
            UpdateRotation();

            _AssetField.ObjectChanged += _AssetField_ObjectChanged;
            _RootField.ObjectChanged += _RootField_ObjectChanged;
            _Layers.LayersChanged += _Layers_LayersChanged;
            _SliDensity.ValueChanged += _SliDensity_ValueChanged;
            _SliRotation.ValueChanged += _SliRotation_ValueChanged;
            _TbModes.SelectedTabChanged += _TbModes_SelectedTabChanged;
            _SliMinRadius.ValueChanged += _SliMinRadius_ValueChanged;
            _SliMaxRadius.ValueChanged += _SliMaxRadius_ValueChanged;
            _SliRectWidth.ValueChanged += _SliRectWidth_ValueChanged;
            _SliRectHeight.ValueChanged += _SliRectHeight_ValueChanged;
            _SliOffsetY.ValueChanged += _SliOffsetY_ValueChanged;
            _TbEnable.Changed += _TbEnable_Changed;
        }

        void _TbEnable_Changed(object sender, EventArgs e)
        {
            _Implant.IsEnable = _TbEnable.IsChecked;
            EditorUtility.SetDirty(_Implant);
        }

        void _RootField_ObjectChanged(object sender, EventArgs e)
        {
            _Implant.Root = _RootField.Object;
            EditorUtility.SetDirty(_Implant);
        }

        void _SliOffsetY_ValueChanged(object sender, EventArgs e)
        {
            _Implant.OffsetY = _SliOffsetY.Value;
            EditorUtility.SetDirty(_Implant);
        }

        void _TbModes_SelectedTabChanged(object sender, EventArgs e)
        {
            _Implant.BrushMode = _TbModes.SelectedTab;
            EditorUtility.SetDirty(_Implant);
            _ShuffleNeeded = true;
            ChangeMode();
        }

        void _SliRotation_ValueChanged(object sender, EventArgs e)
        {
            _Implant.Rotation = _SliRotation.Value;
            EditorUtility.SetDirty(_Implant);
            UpdateRotation();
        }

        void _SliMaxRadius_ValueChanged(object sender, EventArgs e)
        {
            _Implant.MaxRadius = _SliMaxRadius.Value;
            if (_Implant.MaxRadius < _Implant.MinRadius)
                _SliMaxRadius.Value = _Implant.MaxRadius = _Implant.MinRadius;
            EditorUtility.SetDirty(_Implant);
            _ShuffleNeeded = true;
        }

        void _SliMinRadius_ValueChanged(object sender, EventArgs e)
        {
            _Implant.MinRadius = _SliMinRadius.Value;
            if (_Implant.MinRadius > _Implant.MaxRadius)
                _SliMinRadius.Value = _Implant.MinRadius = _Implant.MaxRadius;
            EditorUtility.SetDirty(_Implant);
            _ShuffleNeeded = true;
        }

        void _SliRectHeight_ValueChanged(object sender, EventArgs e)
        {
            _Implant.RectHeight = _SliRectHeight.Value;
            EditorUtility.SetDirty(_Implant);
            _ShuffleNeeded = true;
        }

        void _SliRectWidth_ValueChanged(object sender, EventArgs e)
        {
            _Implant.RectWidth = _SliRectWidth.Value;
            EditorUtility.SetDirty(_Implant);
            _ShuffleNeeded = true;
        }

        private void ChangeMode()
        {
            if (_Implant.BrushMode == 0) // circle
            {
                _SliMaxRadius.Visibility = _SliMinRadius.Visibility = Visibility.Visible;
                _SliRectHeight.Visibility = _SliRectWidth.Visibility = Visibility.Collapsed;
            }
            if (_Implant.BrushMode == 1) // rectangle
            {
                _SliMaxRadius.Visibility = _SliMinRadius.Visibility = Visibility.Collapsed;
                _SliRectHeight.Visibility = _SliRectWidth.Visibility = Visibility.Visible;
            }
        }
        private void UpdateRotation()
        {
            _Rotation = Quaternion.Euler(0, _Implant.Rotation, 0);
        }

        void _SliDensity_ValueChanged(object sender, EventArgs e)
        {
            _Implant.Density = _SliDensity.Value;
            _ShuffleNeeded = true;
            EditorUtility.SetDirty(_Implant);
        }
        void _Layers_LayersChanged(object sender, EventArgs e)
        {
            _Implant.LayerMask = _Layers.Layers;
            _ShuffleNeeded = true;
            EditorUtility.SetDirty(_Implant);
        }
        void _AssetField_ObjectChanged(object sender, EventArgs e)
        {
            _Implant.ImplantAsset = _Asset = _AssetField.Object;
            EditorUtility.SetDirty(_Implant);
        }
        public override void OnInspectorGUI()
        {
            _Frame.OnInspectorGUI(_FrameSize);
        }
        #endregion


        private Implant _Implant;
        private ImplantAsset _Asset;
        private bool _ShuffleNeeded;
        private Quaternion _Rotation;
        private RaycastHit _ReferenceHit;
        private bool _ReferenceEnable;

        void OnEnable()
        {
            _Implant = target as Implant;
            CreateUI();

            if (_Implant.Points == null || _Implant.Points.Length != _Implant.Density)
                Shuffle();
        }

        void Shuffle()
        {
            if (_Implant.Points == null || _Implant.Points.Length != _Implant.Density)
                _Implant.Points = new Vector3[_Implant.Density];
            for (int i = 0; i < _Implant.Density; i++)
                _Implant.Points[i] = GetRandomPoint();

            EditorUtility.SetDirty(_Implant);
        }

        private Vector3 GetRandomPoint()
        {
            Vector3 point;
            if (_Implant.BrushMode == 1)// rectangle brush        
            {
                point = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f) * _Implant.RectWidth, 0, UnityEngine.Random.Range(-0.5f, 0.5f) * _Implant.RectHeight);
            }
            else // circle brush        
            {
                Vector2 v = UnityEngine.Random.insideUnitCircle;
                Vector2 v1 = v.normalized * _Implant.MinRadius;
                Vector2 v2 = v * (_Implant.MaxRadius - _Implant.MinRadius);
                point = new Vector3(v1.x + v2.x, 0, v1.y + v2.y);
            }
            return point;
        }

        void OnSceneGUI()
        {
            if (_ShuffleNeeded)
            {
                Shuffle();
                _ShuffleNeeded = false;
            }
            if (_Implant != null && _Implant.IsEnable)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.KeyDown:
                        if (!e.functionKey && e.shift)
                        {
                            if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.D || e.keyCode == KeyCode.S || e.keyCode == KeyCode.W)
                            {
                                float angle = _Implant.Rotation;
                                if (e.keyCode == KeyCode.A)
                                    angle--;
                                else if (e.keyCode == KeyCode.D)
                                    angle++;

                                while (angle < 0) angle += 360;
                                while (angle > 360) angle -= 360;

                                _SliRotation.Value = _Implant.Rotation = angle;

                                if (e.keyCode == KeyCode.W)
                                    _SliDensity.Value = _Implant.Density = Mathf.Clamp(_Implant.Density + 1, _SliDensity.MinValue, _SliDensity.MaxValue);
                                else if (e.keyCode == KeyCode.S)
                                    _SliDensity.Value = _Implant.Density = Mathf.Clamp(_Implant.Density - 1, _SliDensity.MinValue, _SliDensity.MaxValue);

                                e.Use();
                                HandleUtility.Repaint();
                            }
                            else if (e.keyCode == KeyCode.R)
                            {
                                MakeReference();
                                e.Use();
                            }
                        }
                        else if (e.keyCode == KeyCode.R)
                        {
                            _ReferenceEnable = false;
                        }
                        break;
                    case EventType.mouseDown:
                        if (!e.shift)
                        {
                            if (!e.alt && e.button == 2) // middle mouse
                            {
                                Shuffle();
                                e.Use();
                            }
                            if (e.button == 1 && (e.control || e.command)) // right mouse
                            {
                                ImplantObjects();
                                e.Use();
                            }
                        }
                        break;
                    case EventType.MouseMove:
                        HandleUtility.Repaint();
                        break;
                }
                UpdatePreviewHandle();
            }
        }

        private void MakeReference()
        {
            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out _ReferenceHit, float.MaxValue, _Layers.Layers))
                _ReferenceEnable = true;
            else
                _ReferenceEnable = false;
        }


        private ImplantObject GetRandomImplantObject()
        {
            if (_AssetField.Object == null) return null;
            if (_AssetField.Object.Objects == null || _AssetField.Object.Objects.Length == 0) return null;

            float totalWeight = 0;
            foreach (ImplantObject item in _AssetField.Object.Objects)
            {
                if (item.OverrideProperties)
                    totalWeight += item.Weight;
                else
                    totalWeight += _Asset.DefaultObject.Weight;
            }

            float rnd = UnityEngine.Random.Range(0.0f, totalWeight);
            foreach (ImplantObject item in _AssetField.Object.Objects)
            {
                float w = _Asset.DefaultObject.Weight;
                if (item.OverrideProperties)
                    w = item.Weight;

                if (rnd < w) return item;
                rnd -= w;
            }
            return null;

        }
        private void ImplantObject(ImplantObject io, ref RaycastHit hit)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(io.Prefab) as GameObject;
            ImplantObject pr = _Asset.DefaultObject;
            if (io.OverrideProperties) pr = io;
            if (obj != null)
            {
                // scale                               
                float scale = UnityEngine.Random.Range(pr.MinScalePercent, pr.MaxScalePercent);
                obj.transform.localScale *= scale;
                // position
                obj.transform.position = hit.point + (hit.normal * (_Implant.OffsetY * scale));

                // rotation
                Quaternion rotation = obj.transform.rotation;
                if (pr.Rotation == ImplantObjectRotation.Random)
                {
                    Vector3 euler = obj.transform.eulerAngles;
                    if (pr.RandomX) euler.x = UnityEngine.Random.Range(0.0f, 360.0f);
                    if (pr.RandomY) euler.y = UnityEngine.Random.Range(0.0f, 360.0f);
                    if (pr.RandomZ) euler.z = UnityEngine.Random.Range(0.0f, 360.0f);
                    rotation = Quaternion.Euler(euler);
                }
                else if (pr.Rotation == ImplantObjectRotation.Custom)
                {
                    rotation = Quaternion.Euler(pr.CustomRotation);
                }
                else
                {
                    rotation *= Quaternion.FromToRotation(obj.transform.up, hit.normal);

                    if (pr.RandomYaw)
                    {
                        rotation *= Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);
                    }

                }
                obj.transform.rotation = rotation;
                obj.transform.parent = _Implant.Root != null ? _Implant.Root : _Implant.transform;
                Undo.RegisterCreatedObjectUndo(obj, "Create Object");
            }
        }

        private void ImplantObjects()
        {
            bool valid = false;
            Ray ray = new Ray();
            Event e = Event.current;
            if (_ReferenceEnable)
            {
                valid = true;
            }
            else
            {
                ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out _ReferenceHit, float.MaxValue, _Layers.Layers))
                {
                    valid = true;
                }
            }

            if (valid)
            {
                Vector3 center = _ReferenceHit.point + (_ReferenceHit.normal * 100);
                ray.direction = -_ReferenceHit.normal;
                RaycastHit hit;
                for (int i = 0; i < _Implant.Points.Length; i++)
                {
                    ray.origin = center + _Rotation * _Implant.Points[i];
                    if (Physics.Raycast(ray, out hit, float.MaxValue, _Layers.Layers))
                    {
                        ImplantObject randomObj = GetRandomImplantObject();
                        if (randomObj != null && randomObj.Prefab != null)
                        {
                            ImplantObject(randomObj, ref hit);
                        }
                        else
                        {
                            Debug.LogWarning("Please set valid prefab to implant");
                            return;
                        }
                    }
                }
                e.Use();
            }
        }

        private void UpdatePreviewHandle()
        {
            Event e = Event.current;
            Ray ray = new Ray();
            bool valid = false;
            if (_ReferenceEnable)
            {
                valid = true;
            }
            else
            {
                ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out _ReferenceHit, float.MaxValue, _Layers.Layers))
                    valid = true;
            }

            if (valid)
            {

                float handleSize = HandleUtility.GetHandleSize(_ReferenceHit.point);

                if (_ReferenceEnable)
                {
                    Handles.color = Color.blue;
                    Handles.ArrowCap(0, _ReferenceHit.point, Quaternion.LookRotation(_ReferenceHit.normal), handleSize * 0.8f);
                }

                Handles.color = Color.red;
                Vector3 center = _ReferenceHit.point + (_ReferenceHit.normal * 100);
                ray.direction = -_ReferenceHit.normal;

                RaycastHit hit;
                for (int i = 0; i < _Implant.Points.Length; i++)
                {
                    ray.origin = center + (_Rotation * _Implant.Points[i]);
                    if (Physics.Raycast(ray, out hit, float.MaxValue, _Layers.Layers))
                        Handles.ArrowCap(0, hit.point + (hit.normal * _Implant.OffsetY), Quaternion.LookRotation(hit.normal), handleSize * 0.5f);
                }

                Vector3 offset = (_ReferenceHit.normal * _Implant.OffsetY);
                if (_Implant.BrushMode == 0) // circle
                {
                    if (_Implant.MinRadius > 0.01f)
                        Handles.DrawWireDisc(_ReferenceHit.point + offset, _ReferenceHit.normal, _Implant.MinRadius);
                    Handles.DrawWireDisc(_ReferenceHit.point + offset, _ReferenceHit.normal, _Implant.MaxRadius);
                }
                else
                {
                    HandleHelper.DrawWireRect(_ReferenceHit.point + offset, new Vector2(_Implant.RectWidth, _Implant.RectHeight), _Rotation);
                }
            }
        }
    }
}
