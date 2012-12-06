using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor.Tools
{
    class SpawnObjectField : EditorControl
    {
        private StackPanel _Panel;
        private ObjectField<GameObject> _PrefabField;
        private Skill.Editor.UI.Slider _ChanceField;

        public SpawnObject Object { get; private set; }

        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.UI.Visibility.Collapsed)
                    return _PrefabField.LayoutHeight + _PrefabField.Margin.Vertical +
                           _ChanceField.LayoutHeight + _ChanceField.Margin.Vertical;
                return base.LayoutHeight;
            }
        }

        public SpawnObjectField()
            : this(new SpawnObject() { Chance = 1.0f, Prefab = null })
        {
        }
        public SpawnObjectField(SpawnObject obj)
        {
            this.Object = obj;
            if (this.Object == null)
                throw new ArgumentNullException("Invalid SpawnData");

            this.Margin = new Thickness(0, 0, 0, 8);
            this.Width = 300;
            this._PrefabField = new ObjectField<GameObject>() { AllowSceneObjects = true, Margin = new Thickness(2, 2, 2, 0), Object = Object.Prefab };
            this._PrefabField.Label.text = "Prefab";            

            this._ChanceField = new Skill.Editor.UI.Slider() { MinValue = 0.1f, MaxValue = 1.0f, Value = Object.Chance, Margin = new Thickness(2, 2, 2, 2) };
            this._ChanceField.Label.text = "Chance";            

            this._Panel = new StackPanel() { Orientation = Orientation.Vertical, Parent = this };
            this._Panel.Controls.Add(_PrefabField);            
            this._Panel.Controls.Add(_ChanceField);            
            
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
