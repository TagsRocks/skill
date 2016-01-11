using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.Framework.UI;
using UnityEngine;

namespace Skill.Editor
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
        private Skill.Editor.UI.SelectionField _RotationSF;
		private Skill.Framework.UI.Grid _RandomRotationPanel;
        private Skill.Editor.UI.Vector3Field _MinRandomRotation;
        private Skill.Editor.UI.Vector3Field _MaxRandomRotation;
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

                    _MinRandomRotation.Value = _Object.MinRandomRotation;
                    _MaxRandomRotation.Value = _Object.MaxRandomRotation;

                    if (_Object.Rotation == ImplantObjectRotation.SurfaceNormal)
                        _RotationSF.SelectField(_RandomYaw);
                    else if (_Object.Rotation == ImplantObjectRotation.Custom)
                        _RotationSF.SelectField(_CustomRotation);
                    else
                        _RotationSF.SelectField(_RandomRotationPanel);

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

           _RandomRotationPanel = new Grid() { Height = 45 };

            _RandomRotationPanel.ColumnDefinitions.Add(30, GridUnitType.Pixel);
            _RandomRotationPanel.ColumnDefinitions.Add(1, GridUnitType.Star);

            _RandomRotationPanel.RowDefinitions.Add(1, GridUnitType.Star);
            _RandomRotationPanel.RowDefinitions.Add(1, GridUnitType.Star);

            _MinRandomRotation = new Vector3Field() { Row = 0, Column = 1 };
            _MaxRandomRotation = new Vector3Field() { Row = 1, Column = 1 };
            _RandomRotationPanel.Controls.Add(_MinRandomRotation);
            _RandomRotationPanel.Controls.Add(_MaxRandomRotation);

            Label lblMin = new Label() { Row = 0, Column = 0, Text = "Min" };
            Label lblMax = new Label() { Row = 1, Column = 0, Text = "Max" };

            _RandomRotationPanel.Controls.Add(lblMin);
            _RandomRotationPanel.Controls.Add(lblMax);
            _CustomRotation = new Skill.Editor.UI.Vector3Field();

            _RandomYaw = new Skill.Framework.UI.ToggleButton() { HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, Margin = new Thickness(20, 0, 0, 0) };
            _RandomYaw.Content.text = "Random Yaw";

            _RotationSF = new Skill.Editor.UI.SelectionField() { Margin = new Thickness(2) };
            _RotationSF.Label.Width = 110;
            _RotationSF.Background.Visibility = Skill.Framework.UI.Visibility.Hidden;

            _RotationSF.AddField(_RandomYaw, "Surface Normal ");
            _RotationSF.AddField(_CustomRotation, "Custom ");
            _RotationSF.AddField(_RandomRotationPanel, "Random ");

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

            this._MinRandomRotation.ValueChanged += _MinRandomRotation_ValueChanged;
            this._MaxRandomRotation.ValueChanged += _MaxRandomRotation_ValueChanged;

            this._CustomRotation.ValueChanged += new EventHandler(_CustomRotation_ValueChanged);
            this._RandomYaw.Changed += new EventHandler(_RandomYaw_Changed);


            this.Object = null;
            this.Height = _MinScaleField.LayoutHeight + _MinScaleField.Margin.Vertical +
                          _MaxScaleField.LayoutHeight + _MaxScaleField.Margin.Vertical +
                          _ChanceField.LayoutHeight + _ChanceField.Margin.Vertical +
                          _RotationLabel.LayoutHeight + _RotationLabel.Margin.Vertical +
                          _RotationSF.LayoutHeight + _RotationSF.Margin.Vertical + 20;
						       }
							   
							   		void _MaxRandomRotation_ValueChanged(object sender, EventArgs e)
									       {
										               if (_Object != null)
													                   _Object.MaxRandomRotation = _MaxRandomRotation.Value;
																	           }
																			   
																			           void _MinRandomRotation_ValueChanged(object sender, EventArgs e)
																					           {
																							               if (_Object != null)
																										                   _Object.MinRandomRotation = _MinRandomRotation.Value;
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
