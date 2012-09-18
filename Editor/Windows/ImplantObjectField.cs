using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor
{
    public class ImplantObjectField : EditorControl
    {
        private StackPanel _Panel;
        private ObjectField<GameObject> _PrefabField;
        private FloatField _MinScaleField;
        private FloatField _MaxScaleField;
        private Skill.Editor.UI.Slider _ChanceField;


        public float Chance { get { return _ChanceField.Value; } }
        public float MinScale { get { return _MinScaleField.Value; } }
        public float MaxScale { get { return _MaxScaleField.Value; } }
        public GameObject Prefab { get { return _PrefabField.Object; } }

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
        {
            this.Margin = new Thickness(0, 0, 0, 8);
            this.Width = 300;
            this._PrefabField = new ObjectField<GameObject>() { AllowSceneObjects = true, Margin = new Thickness(2, 2, 2, 0) };
            this._PrefabField.Label.text = "Prefab";

            this._MinScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0), Value = 0.8f };
            this._MinScaleField.Label.text = "Min Scale Percent";

            this._MaxScaleField = new FloatField() { Margin = new Thickness(2, 2, 2, 0), Value = 1.0f };
            this._MaxScaleField.Label.text = "Max Scale Percent";

            this._ChanceField = new Skill.Editor.UI.Slider() { MinValue = 0.1f, MaxValue = 1.0f, Value = 1.0f, Margin = new Thickness(2, 2, 2, 2) };
            this._ChanceField.Label.text = "Chance";

            this._Panel = new StackPanel() { Orientation = Orientation.Vertical };
            this._Panel.Controls.Add(_PrefabField);
            this._Panel.Controls.Add(_MinScaleField);
            this._Panel.Controls.Add(_MaxScaleField);
            this._Panel.Controls.Add(_ChanceField);

            this._MinScaleField.ValueChanged += new EventHandler(_MinScaleField_ValueChanged);
            this._MaxScaleField.ValueChanged += new EventHandler(_MaxScaleField_ValueChanged);

            this._Panel.LayoutChanged += Panel_LayoutChanged;
        }

        void _MaxScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_MaxScaleField.Value < _MinScaleField.Value)
                _MaxScaleField.Value = _MinScaleField.Value;
        }

        void _MinScaleField_ValueChanged(object sender, EventArgs e)
        {
            if (_MinScaleField.Value > _MaxScaleField.Value)
                _MinScaleField.Value = _MaxScaleField.Value;
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
