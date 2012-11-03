using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor.Tools
{
    class ImplantObjectField : EditorControl
    {
        private StackPanel _Panel;
        private ObjectField<GameObject> _PrefabField;
        private FloatField _MinScaleField;
        private FloatField _MaxScaleField;
        private Skill.Editor.UI.Slider _ChanceField;

        private Skill.Editor.UI.DropShadowLabel _RotationLabel;

        private Skill.Editor.UI.SelectionField _RotationSF;
        private Skill.Editor.UI.XYZComponent _RandomRotation;
        private Skill.Editor.UI.Vector3Field _CustomRotation;
        private Skill.UI.ToggleButton _RandomYaw;

        public ImplantObject Object { get; private set; }

        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.UI.Visibility.Collapsed)
                    return _PrefabField.LayoutHeight + _PrefabField.Margin.Vertical +
                           _MinScaleField.LayoutHeight + _MinScaleField.Margin.Vertical +
                           _MaxScaleField.LayoutHeight + _MaxScaleField.Margin.Vertical +
                           _ChanceField.LayoutHeight + _ChanceField.Margin.Vertical +
                           _RotationLabel.LayoutHeight + _RotationLabel.Margin.Vertical +
                           _RotationSF.LayoutHeight + _RotationSF.Margin.Vertical;
                return base.LayoutHeight;
            }
        }

        public ImplantObjectField()
            : this(new ImplantObject() { Chance = 1.0f, Prefab = null, MinScalePercent = 0.8f, MaxScalePercent = 1.0f })
        {
        }
        public ImplantObjectField(ImplantObject obj)
        {
            this.Object = obj;
            if (this.Object == null)
                throw new ArgumentNullException("Invalid ImplantObject");

            this.Margin = new Thickness(0, 0, 0, 8);
            this.Width = 300;
            this._PrefabField = new ObjectField<GameObject>() { AllowSceneObjects = true, Margin = new Thickness(2, 2, 2, 0), Object = Object.Prefab };
            this._PrefabField.Label.text = "Prefab";

            this._MinScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0), Value = Object.MinScalePercent };
            this._MinScaleField.Label.text = "Min Scale Percent";

            this._MaxScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0), Value = Object.MaxScalePercent };
            this._MaxScaleField.Label.text = "Max Scale Percent";

            this._ChanceField = new Skill.Editor.UI.Slider() { MinValue = 0.1f, MaxValue = 1.0f, Value = Object.Chance, Margin = new Thickness(2, 2, 2, 2) };
            this._ChanceField.Label.text = "Chance";

            _RandomRotation = new Skill.Editor.UI.XYZComponent();
            _RandomRotation.XComponent.IsChecked = Object.RandomX;
            _RandomRotation.YComponent.IsChecked = Object.RandomY;
            _RandomRotation.ZComponent.IsChecked = Object.RandomZ;

            _CustomRotation = new Skill.Editor.UI.Vector3Field() { Value = Object.CustomRotation };

            _RandomYaw = new Skill.UI.ToggleButton() { IsChecked = Object.RandomYaw, HorizontalAlignment = Skill.UI.HorizontalAlignment.Left, Margin = new Thickness(20, 0, 0, 0) };
            _RandomYaw.Content.text = "Random Yaw";

            _RotationSF = new Skill.Editor.UI.SelectionField() { Margin = new Thickness(2) };
            _RotationSF.Label.Width = 110;
            _RotationSF.Background.Visibility = Skill.UI.Visibility.Hidden;

            _RotationSF.AddField(_RandomYaw, "Surface Normal ");
            _RotationSF.AddField(_CustomRotation, "Custom ");
            _RotationSF.AddField(_RandomRotation, "Random ");

            if (Object.Rotation == ImplantObjectRotation.SurfaceNormal)
                _RotationSF.SelectField(_RandomYaw);
            else if (Object.Rotation == ImplantObjectRotation.Custom)
                _RotationSF.SelectField(_CustomRotation);
            else
                _RotationSF.SelectField(_RandomRotation);

            _RotationLabel = new DropShadowLabel() { Text = "Rotation", Margin = new Thickness(4, 0, 0, 0) };

            this._Panel = new StackPanel() { Orientation = Orientation.Vertical, Parent = this };
            this._Panel.Controls.Add(_PrefabField);
            this._Panel.Controls.Add(_MinScaleField);
            this._Panel.Controls.Add(_MaxScaleField);
            this._Panel.Controls.Add(_ChanceField);
            this._Panel.Controls.Add(_RotationLabel);
            this._Panel.Controls.Add(_RotationSF);

            this._MinScaleField.ValueChanged += new EventHandler(_MinScaleField_ValueChanged);
            this._MaxScaleField.ValueChanged += new EventHandler(_MaxScaleField_ValueChanged);
            this._ChanceField.ValueChanged += new EventHandler(_ChanceField_ValueChanged);
            this._PrefabField.ObjectChanged += new EventHandler(_PrefabField_ObjectChanged);
            this._RotationSF.SelectedFieldChanged += new EventHandler(_RotationSF_SelectedFieldChanged);

            this._RandomRotation.XComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.XComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.YComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.YComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.ZComponent.Checked += new EventHandler(RandomXYZComponent_Checked);
            this._RandomRotation.ZComponent.Unchecked += new EventHandler(RandomXYZComponent_Checked);

            this._CustomRotation.ValueChanged += new EventHandler(_CustomRotation_ValueChanged);
            this._RandomYaw.Changed += new EventHandler(_RandomYaw_Changed);
            

            this._Panel.LayoutChanged += Panel_LayoutChanged;
        }

        void _RandomYaw_Changed(object sender, EventArgs e)
        {
            Object.RandomYaw = _RandomYaw.IsChecked;
        }

        void _CustomRotation_ValueChanged(object sender, EventArgs e)
        {
            Object.CustomRotation = _CustomRotation.Value;
        }

        void RandomXYZComponent_Checked(object sender, EventArgs e)
        {
            Object.RandomX = _RandomRotation.XComponent.IsChecked;
            Object.RandomY = _RandomRotation.YComponent.IsChecked;
            Object.RandomZ = _RandomRotation.ZComponent.IsChecked;
        }

        void _RotationSF_SelectedFieldChanged(object sender, EventArgs e)
        {
            if (_RotationSF.SelectedField == _RandomYaw)
                Object.Rotation = ImplantObjectRotation.SurfaceNormal;
            else if (_RotationSF.SelectedField == _CustomRotation)
                Object.Rotation = ImplantObjectRotation.Custom;
            else
                Object.Rotation = ImplantObjectRotation.Random;
        }

        void _PrefabField_ObjectChanged(object sender, EventArgs e)
        {
            Object.Prefab = _PrefabField.Object;
        }

        void _ChanceField_ValueChanged(object sender, EventArgs e)
        {
            Object.Chance = _ChanceField.Value;
        }

        void _MaxScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_MaxScaleField.Value < _MinScaleField.Value)
                _MaxScaleField.Value = _MinScaleField.Value;
            Object.MaxScalePercent = _MaxScaleField.Value;
        }

        void _MinScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_MinScaleField.Value > _MaxScaleField.Value)
                _MinScaleField.Value = _MaxScaleField.Value;
            Object.MinScalePercent = _MinScaleField.Value;
        }

        protected override void OnRenderAreaChanged()
        {
            this._Panel.RenderArea = RenderArea;
        }

        private void Panel_LayoutChanged(object sender, EventArgs e)
        {
            OnLayoutChanged();
        }

        protected override void Render()
        {
            this._Panel.OnGUI();
        }
    }
}
