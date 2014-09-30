using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.Framework.UI;
using UnityEngine;

namespace Skill.Editor.Tools
{
    class ImplantObjectField : StackPanel
    {
        private ObjectField<GameObject> _PrefabField;
        private Editor.UI.ToggleButton _TbOverride;
        private ImplantObjectPropertiesField _PropertiesField;

        private ImplantObject _Object;
        public ImplantObject Object
        {
            get { return _Object; }
            set
            {
                _Object = value;
                if (_Object != null)
                {
                    this._PrefabField.Object = _Object.Prefab;
                    this._PropertiesField.Object = _Object;
                    this._TbOverride.IsChecked = _Object.OverrideProperties;
                    IsEnabled = true;
                }
                else
                {
                    this._PrefabField.Object = null;
                    this._PropertiesField.Object = null;
                    this._TbOverride.IsChecked = false;
                    IsEnabled = false;
                }
            }
        }
        public ImplantAssetEditor Editor { get; private set; }

        public ImplantObjectField(ImplantAssetEditor editor)
        {
            this.Editor = editor;
            this.Margin = new Thickness(0, 0, 0, 8);
            this.Width = 300;
            this._PrefabField = new ObjectField<GameObject>() { AllowSceneObjects = true, Margin = new Thickness(2, 2, 2, 0) };
            this._PrefabField.Label.text = "Prefab";

            _TbOverride = new UI.ToggleButton() { IsChecked = false }; _TbOverride.Label.text = "Override properties";
            _PropertiesField = new ImplantObjectPropertiesField();

            this.Orientation = Orientation.Vertical;
            this.Controls.Add(_PrefabField);
            this.Controls.Add(_TbOverride);
            this.Controls.Add(_PropertiesField);



            this._PrefabField.ObjectChanged += _PrefabField_ObjectChanged;
            this._TbOverride.Changed += _TbOverride_Changed;

            this.Object = null;
            this.Height = _PrefabField.LayoutHeight + _PrefabField.Margin.Vertical +
                          _TbOverride.LayoutHeight + _TbOverride.Margin.Vertical +
                          _PropertiesField.LayoutHeight + _PropertiesField.Margin.Vertical;
        }

        void _TbOverride_Changed(object sender, EventArgs e)
        {
            _PropertiesField.IsEnabled = _TbOverride.IsChecked;
            if (this._Object != null)
                this._Object.OverrideProperties = _TbOverride.IsChecked;
        }

        void _PrefabField_ObjectChanged(object sender, EventArgs e)
        {
            if (_Object != null)
                _Object.Prefab = _PrefabField.Object;
            Editor.UpdateNames();
        }
    }



    class ImplantObjectPropertiesField : StackPanel
    {
        private FloatField _MinScaleField;
        private FloatField _MaxScaleField;
        private Skill.Editor.UI.Slider _ChanceField;
        private Skill.Editor.UI.DropShadowLabel _RotationLabel;
        private Skill.Editor.UI.Extended.SelectionField _RotationSF;
        private Skill.Editor.UI.Extended.XYZComponent _RandomRotation;
        private Skill.Editor.UI.Vector3Field _CustomRotation;
        private Skill.Framework.UI.ToggleButton _RandomYaw;

        private ImplantObject _Object;
        public ImplantObject Object
        {
            get { return _Object; }
            set
            {
                _Object = value;
                if (_Object != null)
                {
                    this._MinScaleField.Value = _Object.MinScalePercent;
                    this._MaxScaleField.Value = _Object.MaxScalePercent;
                    this._ChanceField.Value = _Object.Weight;

                    _RandomRotation.XComponent.IsChecked = _Object.RandomX;
                    _RandomRotation.YComponent.IsChecked = _Object.RandomY;
                    _RandomRotation.ZComponent.IsChecked = _Object.RandomZ;

                    if (_Object.Rotation == ImplantObjectRotation.SurfaceNormal)
                        _RotationSF.SelectField(_RandomYaw);
                    else if (_Object.Rotation == ImplantObjectRotation.Custom)
                        _RotationSF.SelectField(_CustomRotation);
                    else
                        _RotationSF.SelectField(_RandomRotation);

                    this._CustomRotation.Value = _Object.CustomRotation;
                    this._RandomYaw.IsChecked = _Object.RandomYaw;
                    IsEnabled = _Object.OverrideProperties;
                }
                else
                {
                    IsEnabled = false;
                }
            }
        }

        public ImplantObjectPropertiesField()
        {
            this.Margin = new Thickness(0, 0, 0, 8);
            this.Width = 300;

            this._MinScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0) };
            this._MinScaleField.Label.text = "Min Scale Percent";

            this._MaxScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0) };
            this._MaxScaleField.Label.text = "Max Scale Percent";

            this._ChanceField = new Skill.Editor.UI.Slider() { MinValue = 0.1f, MaxValue = 1.0f, Margin = new Thickness(2, 2, 2, 2) };
            this._ChanceField.Label.text = "Chance";

            _RandomRotation = new Skill.Editor.UI.Extended.XYZComponent();
            _CustomRotation = new Skill.Editor.UI.Vector3Field();

            _RandomYaw = new Skill.Framework.UI.ToggleButton() { HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, Margin = new Thickness(20, 0, 0, 0) };
            _RandomYaw.Content.text = "Random Yaw";

            _RotationSF = new Skill.Editor.UI.Extended.SelectionField() { Margin = new Thickness(2) };
            _RotationSF.Label.Width = 110;
            _RotationSF.Background.Visibility = Skill.Framework.UI.Visibility.Hidden;

            _RotationSF.AddField(_RandomYaw, "Surface Normal ");
            _RotationSF.AddField(_CustomRotation, "Custom ");
            _RotationSF.AddField(_RandomRotation, "Random ");

            _RotationLabel = new DropShadowLabel() { Text = "Rotation", Margin = new Thickness(4, 0, 0, 0) };

            this.Orientation = Orientation.Vertical;
            this.Controls.Add(_MinScaleField);
            this.Controls.Add(_MaxScaleField);
            this.Controls.Add(_ChanceField);
            this.Controls.Add(_RotationLabel);
            this.Controls.Add(_RotationSF);

            this._MinScaleField.ValueChanged += new EventHandler(_MinScaleField_ValueChanged);
            this._MaxScaleField.ValueChanged += new EventHandler(_MaxScaleField_ValueChanged);
            this._ChanceField.ValueChanged += new EventHandler(_ChanceField_ValueChanged);
            this._RotationSF.SelectedFieldChanged += new EventHandler(_RotationSF_SelectedFieldChanged);

            this._RandomRotation.XComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.XComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.YComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.YComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.ZComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.ZComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);

            this._CustomRotation.ValueChanged += new EventHandler(_CustomRotation_ValueChanged);
            this._RandomYaw.Changed += new EventHandler(_RandomYaw_Changed);


            this.Object = null;
            this.Height = _MinScaleField.LayoutHeight + _MinScaleField.Margin.Vertical +
                          _MaxScaleField.LayoutHeight + _MaxScaleField.Margin.Vertical +
                          _ChanceField.LayoutHeight + _ChanceField.Margin.Vertical +
                          _RotationLabel.LayoutHeight + _RotationLabel.Margin.Vertical +
                          _RotationSF.LayoutHeight + _RotationSF.Margin.Vertical;
        }

        void _RandomYaw_Changed(object sender, EventArgs e)
        {
            if (_Object != null)
                _Object.RandomYaw = _RandomYaw.IsChecked;
        }

        void _CustomRotation_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null)
                _Object.CustomRotation = _CustomRotation.Value;
        }

        void RandomXYZComponent_Checked(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.RandomX = _RandomRotation.XComponent.IsChecked;
                _Object.RandomY = _RandomRotation.YComponent.IsChecked;
                _Object.RandomZ = _RandomRotation.ZComponent.IsChecked;
            }
        }

        void _RotationSF_SelectedFieldChanged(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                if (_RotationSF.SelectedField == _RandomYaw)
                    _Object.Rotation = ImplantObjectRotation.SurfaceNormal;
                else if (_RotationSF.SelectedField == _CustomRotation)
                    _Object.Rotation = ImplantObjectRotation.Custom;
                else
                    _Object.Rotation = ImplantObjectRotation.Random;
            }
        }

        void _ChanceField_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null)
                _Object.Weight = _ChanceField.Value;
        }

        void _MaxScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.MaxScalePercent = _MaxScaleField.Value;
                if (_Object.MaxScalePercent < _Object.MinScalePercent)
                {
                    _Object.MaxScalePercent = _Object.MinScalePercent;
                    _MaxScaleField.Value = _Object.MaxScalePercent;
                }
            }
        }

        void _MinScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.MinScalePercent = _MinScaleField.Value;
                if (_Object.MinScalePercent > _Object.MaxScalePercent)
                {
                    _Object.MinScalePercent = _Object.MaxScalePercent;
                    _MinScaleField.Value = _Object.MinScalePercent;
                }
            }
        }

        private void Panel_LayoutChanged(object sender, EventArgs e)
        {
            OnLayoutChanged();
        }
    }
}
