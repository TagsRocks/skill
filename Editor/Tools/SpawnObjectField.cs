using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.Framework.UI;
using UnityEngine;

namespace Skill.Editor.Tools
{
    class SpawnObjectField : EditorControl
    {
        private Grid _Panel;
        private ObjectField<GameObject> _PrefabField;
        private Skill.Editor.UI.Slider _WeightField;        
        private Skill.Framework.UI.Box _Background;

        private Skill.Framework.SpawnObject _Object;
        public Skill.Framework.SpawnObject Object
        {
            get { return _Object; }
            set
            {
                _Object = value;
                if (_Object != null)
                {
                    this._PrefabField.Object = _Object.Prefab;
                    this._WeightField.Value = _Object.Weight;
                    IsEnabled = true;
                }
                else
                {
                    this._PrefabField.Object = null;
                    this._WeightField.Value = 0;
                    IsEnabled = false;
                }
            }
        }
        public SpawnAssetEditor Editor { get; private set; }        

        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.Framework.UI.Visibility.Collapsed)
                    return _PrefabField.LayoutHeight + _PrefabField.Margin.Vertical +
                           _WeightField.LayoutHeight + _WeightField.Margin.Vertical + 2;
                return base.LayoutHeight;
            }
        }

        public SpawnObjectField(SpawnAssetEditor editor)
        {
            this.Editor = editor;

            this._PrefabField = new ObjectField<GameObject>() { Row = 0, Column = 0, AllowSceneObjects = true, Margin = new Thickness(4, 4, 4, 0) };
            this._PrefabField.Label.text = "Prefab";

            this._WeightField = new Skill.Editor.UI.Slider() { Row = 1, Column = 0, MinValue = 0.1f, MaxValue = 1.0f, Margin = new Thickness(4, 2, 4, 4) };
            this._WeightField.Label.text = "Weight";

            this._Background = new Framework.UI.Box() { Row = 0, Column = 0, RowSpan = 3, ColumnSpan = 2 };

            this._Panel = new Grid() { Parent = this , Padding = new Thickness(2) };
            this._Panel.RowDefinitions.Add(1, GridUnitType.Star);
            this._Panel.RowDefinitions.Add(1, GridUnitType.Star);
            this._Panel.ColumnDefinitions.Add(1, GridUnitType.Star);            

            this._Panel.Controls.Add(_Background);
            this._Panel.Controls.Add(_PrefabField);
            this._Panel.Controls.Add(_WeightField);            

            this._WeightField.ValueChanged += new EventHandler(_WeightField_ValueChanged);
            this._PrefabField.ObjectChanged += new EventHandler(_PrefabField_ObjectChanged);
            this._Panel.LayoutChanged += Panel_LayoutChanged;

            this.Object = null;
        }       

        void _PrefabField_ObjectChanged(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Prefab = _PrefabField.Object;
            Editor.UpdateNames();
        }

        void _WeightField_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Weight = _WeightField.Value;
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
