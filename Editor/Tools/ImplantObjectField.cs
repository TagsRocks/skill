using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor.Tools
{
    public class ImplantObjectField : EditorControl
    {
        private StackPanel _Panel;
        private ObjectField<GameObject> _PrefabField;
        private FloatField _MinScaleField;
        private FloatField _MaxScaleField;
        private Skill.Editor.UI.Slider _ChanceField;

        public ImplantObject Object { get; private set; }

        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.UI.Visibility.Collapsed)
                    return _PrefabField.Height + _PrefabField.Margin.Vertical +
                           _MinScaleField.Height + _MinScaleField.Margin.Vertical +
                           _MaxScaleField.Height + _MaxScaleField.Margin.Vertical +
                           _ChanceField.Height + _ChanceField.Margin.Vertical;
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

            this._Panel = new StackPanel() { Orientation = Orientation.Vertical, Parent = this };
            this._Panel.Controls.Add(_PrefabField);
            this._Panel.Controls.Add(_MinScaleField);
            this._Panel.Controls.Add(_MaxScaleField);
            this._Panel.Controls.Add(_ChanceField);

            this._MinScaleField.ValueChanged += new EventHandler(_MinScaleField_ValueChanged);
            this._MaxScaleField.ValueChanged += new EventHandler(_MaxScaleField_ValueChanged);
            this._ChanceField.ValueChanged += new EventHandler(_ChanceField_ValueChanged);
            this._PrefabField.ObjectChanged += new EventHandler(_PrefabField_ObjectChanged);

            this._Panel.LayoutChanged += Panel_LayoutChanged;
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
