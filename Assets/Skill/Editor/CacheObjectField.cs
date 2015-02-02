using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.UI;
using Skill.Framework.UI;
using UnityEngine;

namespace Skill.Editor
{
    class CacheObjectField : Grid
    {
        private ObjectField<GameObject> _PrefabField;
        private Skill.Editor.UI.IntField _CacheSizeField;
        private Skill.Editor.UI.ToggleButton _TbGrowable;


        private Skill.Framework.Managers.CacheObject _Object;
        public Skill.Framework.Managers.CacheObject Object
        {
            get { return _Object; }
            set
            {
                _Object = value;
                if (_Object != null)
                {
                    this._PrefabField.Object = _Object.Prefab;
                    this._CacheSizeField.Value = _Object.CacheSize;
                    this._TbGrowable.IsChecked = _Object.Growable;
                    IsEnabled = true;
                }
                else
                {
                    this._PrefabField.Object = null;
                    this._CacheSizeField.Value = 0;
                    this._TbGrowable.IsChecked = false;
                    IsEnabled = false;
                }
            }
        }
        public CacheGroupEditor Editor { get; private set; }        

        public CacheObjectField(CacheGroupEditor editor)
        {
            this.Editor = editor;

            this._PrefabField = new ObjectField<GameObject>() { Row = 0, Column = 0, AllowSceneObjects = false, Margin = new Thickness(4, 2, 4, 0) };
            this._PrefabField.Label.text = "Prefab";

            this._CacheSizeField = new Skill.Editor.UI.IntField() { Row = 1, Column = 0, Margin = new Thickness(4, 2, 4, 2) };
            this._CacheSizeField.Label.text = "CacheSize";

            this._TbGrowable = new Skill.Editor.UI.ToggleButton() { Row = 2, Column = 0, Margin = new Thickness(4, 2, 4, 2) };
            this._TbGrowable.Label.text = "Growable?";
            
            this.Padding = new Thickness(2);
            this.AutoHeight = true;
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.RowDefinitions.Add(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(1, GridUnitType.Star);

            this.Controls.Add(_PrefabField);
            this.Controls.Add(_CacheSizeField);
            this.Controls.Add(_TbGrowable);

            this._CacheSizeField.ValueChanged += _CacheSizeField_ValueChanged;
            this._TbGrowable.Changed += _TbGrowable_Changed;
            this._PrefabField.ObjectChanged += _PrefabField_ObjectChanged;

            this.Object = null;
            this.Height = _PrefabField.LayoutHeight + _PrefabField.Margin.Vertical +
                          _CacheSizeField.LayoutHeight + _CacheSizeField.Margin.Vertical +
                          _TbGrowable.LayoutHeight + _TbGrowable.Margin.Vertical + 2;
        }
        void _TbGrowable_Changed(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Growable = _TbGrowable.IsChecked;
        }

        void _PrefabField_ObjectChanged(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Prefab = _PrefabField.Object;
            Editor.UpdateNames();
        }

        void _CacheSizeField_ValueChanged(object sender, EventArgs e)
        {
            if (_Object != null) _Object.CacheSize = _CacheSizeField.Value;
        }
    }
}
