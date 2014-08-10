using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.Sequence;
using Skill.Framework;

namespace Skill.Editor.UI.Extended
{

    public abstract class ExposeProperties : PropertiesPanel
    {
        private List<ControlProperties> _Fields;
        private System.Object _PreObject;
        protected virtual void CreateCustomFileds() { }

        internal void ReplaceObject(System.Object obj)
        {
            if (obj == null)
            {
                _Fields.Clear();
                Controls.Clear();
            }
            else if (_PreObject != obj)
            {
                _PreObject = base.Object = obj;
                Controls.Clear();
                CreateCustomFileds();
                PropertyInfo[] infos = Object.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Type epaType = typeof(ExposePropertyAttribute);

                foreach (PropertyInfo info in infos)
                {
                    if (!(info.CanRead && info.CanWrite))
                        continue;
                    object[] attributes = info.GetCustomAttributes(true);

                    ExposePropertyAttribute exposePropertyAttribute = null;
                    foreach (object o in attributes)
                    {
                        if (o.GetType() == epaType)
                        {
                            exposePropertyAttribute = (ExposePropertyAttribute)o;
                            break;
                        }
                    }
                    if (exposePropertyAttribute == null)
                        continue;

                    PropertyType type;
                    if (GetPropertyType(info.PropertyType, out type))
                    {
                        ControlProperties control = CreateProperties(type, info, exposePropertyAttribute);
                        if (control != null)
                        {
                            _Fields.Add(control);
                        }
                    }
                }

                if (_Fields.Count > 0)
                {
                    _Fields.Sort(new ControlPropertiesComparer());
                    foreach (var item in _Fields)
                    {
                        item.Control.UserData = item;
                        Controls.Add(item.Control);
                    }
                }
            }
        }

        public ExposeProperties(System.Object obj)
            : base(obj)
        {
            _PreObject = null;
            _Fields = new List<ControlProperties>();
            ReplaceObject(obj);
        }

        private ControlProperties CreateProperties(PropertyType type, PropertyInfo info, ExposePropertyAttribute attribute)
        {
            ControlProperties control = null;
            switch (type)
            {
                case PropertyType.AnimationCurve:
                    control = new AnimationCurveProperties(this, info, attribute);
                    break;
                case PropertyType.Boolean:
                    control = new BooleanProperties(this, info, attribute);
                    break;
                case PropertyType.Bounds:
                    control = new BoundsProperties(this, info, attribute);
                    break;
                case PropertyType.Color:
                    control = new ColorProperties(this, info, attribute);
                    break;
                case PropertyType.Enum:
                    control = new EnumProperties(this, info, attribute);
                    break;
                case PropertyType.Float:
                    control = new FloatProperties(this, info, attribute);
                    break;
                case PropertyType.Integer:
                    control = new IntegerProperties(this, info, attribute);
                    break;
                case PropertyType.LayerMask:
                    control = new LayerMaskProperties(this, info, attribute);
                    break;
                case PropertyType.ObjectReference:
                    control = new UnityObjectProperties(this, info, attribute);
                    break;
                case PropertyType.Quaternion:
                    control = new QuaternionProperties(this, info, attribute);
                    break;
                case PropertyType.Rect:
                    control = new RectProperties(this, info, attribute);
                    break;
                case PropertyType.String:
                    control = new StringProperties(this, info, attribute);
                    break;
                case PropertyType.Vector2:
                    control = new Vector2Properties(this, info, attribute);
                    break;
                case PropertyType.Vector3:
                    control = new Vector3Properties(this, info, attribute);
                    break;
                case PropertyType.Vector4:
                    control = new Vector4Properties(this, info, attribute);
                    break;
                case PropertyType.SerializableObject:
                    control = new SerializableObjectProperties(this, info, attribute);
                    break;
                case PropertyType.Array:
                    control = new ArrayProperties(this, info, attribute);
                    break;
            }

            if (control != null)
            {
                var margin = control.Control.Margin;
                margin.Left += 2;
                margin.Top += 2;
                margin.Bottom += 2;
                control.Control.Margin = margin;
            }
            return control;
        }

        protected override void RefreshData()
        {
            foreach (var item in _Fields)
                item.Refresh();
        }

        #region ControlProperties
        protected abstract class ControlProperties
        {
            public ExposeProperties Owner { get; private set; }
            public int Order { get; private set; }
            public PropertyInfo Info { get; private set; }
            public abstract PropertyType Type { get; }
            public abstract Skill.Framework.UI.BaseControl Control { get; }
            public abstract void Refresh();

            public ControlProperties(ExposeProperties owner, int order, PropertyInfo info)
            {
                this.Owner = owner;
                this.Order = order;
                this.Info = info;
                this.Index = -1;
            }
            public int Index { get; set; }

            protected object Value
            {
                get
                {
                    object value = Info.GetValue(Owner.Object, null);
                    if (Index >= 0)
                        return ((Array)value).GetValue(Index);
                    else
                        return value;
                }
                set
                {
                    if (Index >= 0)
                        ((Array)Info.GetValue(Owner.Object, null)).SetValue(value, Index);
                    else
                        Info.SetValue(Owner.Object, value, null);
                }
            }

        }
        protected class ControlPropertiesComparer : IComparer<ControlProperties>
        {
            public int Compare(ControlProperties x, ControlProperties y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }
        #endregion

        #region AnimationCurve

        protected class AnimationCurveProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.AnimationCurve; } }

            private Skill.Editor.UI.CurveField _CurveField;
            public override Framework.UI.BaseControl Control { get { return _CurveField; } }

            public AnimationCurveProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _CurveField = new Skill.Editor.UI.CurveField();
                _CurveField.Label.text = attribute.Name;
                _CurveField.Label.tooltip = attribute.Description;
            }

            public override void Refresh()
            {
                AnimationCurve curve = Value as AnimationCurve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                    Value = curve;
                }
                _CurveField.Curve = (AnimationCurve)Value;
            }
        }
        #endregion

        #region Boolean

        protected class BooleanProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Boolean; } }

            private Skill.Editor.UI.ToggleButton _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }

            public BooleanProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.ToggleButton();
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
                _Field.Changed += ToggleButton_Changed;
            }

            private void ToggleButton_Changed(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.IsChecked;
                Owner.SetDirty();
            }

            public override void Refresh()
            {
                _Field.IsChecked = (bool)Value;
            }
        }

        #endregion

        #region Bounds

        protected class BoundsProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Bounds; } }
            private Skill.Editor.UI.BoundsField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public BoundsProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.BoundsField();
                _Field.ValueChanged += BoundsField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void BoundsField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Bounds)Value;
            }
        }

        #endregion

        #region Color

        protected class ColorProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Color; } }

            private Skill.Editor.UI.ColorField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public ColorProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.ColorField();
                _Field.ColorChanged += ColorField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void ColorField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Color;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Color = (Color)Value;
            }
        }
        #endregion

        #region Enum

        protected class EnumProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Enum; } }

            private Skill.Editor.UI.EnumPopup _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public EnumProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.EnumPopup();
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
                _Field.ValueChanged += EnumPopup_ValueChanged;
            }

            void EnumPopup_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Enum)Value;
            }
        }
        #endregion

        #region Float

        protected class FloatProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Float; } }

            private Skill.Editor.UI.FloatField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public FloatProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.FloatField();
                _Field.ValueChanged += FloatField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void FloatField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (float)Value;
            }
        }
        #endregion

        #region Integer

        protected class IntegerProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Integer; } }

            private Skill.Editor.UI.IntField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public IntegerProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.IntField();
                _Field.ValueChanged += IntField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
                _Field.ChangeOnReturn = true;
            }

            void IntField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (int)Value;
            }
        }
        #endregion

        #region LayerMask

        protected class LayerMaskProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.LayerMask; } }

            private Skill.Editor.UI.LayerMaskField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public LayerMaskProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.LayerMaskField();
                _Field.LayersChanged += LayerMaskField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void LayerMaskField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = new UnityEngine.LayerMask() { value = _Field.Layers };
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Layers = ((UnityEngine.LayerMask)Value).value;
            }
        }
        #endregion

        #region UnityEngine.Object

        protected class UnityObjectProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Integer; } }

            private Skill.Editor.UI.UntypedObjectField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public UnityObjectProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.UntypedObjectField(Info.PropertyType.IsArray ? Info.PropertyType.GetElementType() : Info.PropertyType);
                _Field.ObjectChanged += ObjectField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void ObjectField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Object;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Object = Value as UnityEngine.Object;
            }
        }
        #endregion

        #region Quaternion

        protected class QuaternionProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Quaternion; } }

            private Skill.Editor.UI.Vector3Field _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public QuaternionProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.Vector3Field();
                _Field.ValueChanged += QuaternionField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void QuaternionField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                Quaternion q = (Quaternion)Value;
                _Field.Value = q.eulerAngles;
            }
        }

        #endregion

        #region Rect
        protected class RectProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Rect; } }

            private Skill.Editor.UI.RectField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public RectProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.RectField();
                _Field.ValueChanged += RectField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void RectField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Rect)Value;
            }
        }
        #endregion

        #region String

        protected class StringProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.String; } }

            private Skill.Editor.UI.TextField _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public StringProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.TextField();
                _Field.TextChanged += TextField_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void TextField_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Text;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Text = Value as string;
            }
        }
        #endregion

        #region Vector2

        protected class Vector2Properties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Vector2; } }

            private Skill.Editor.UI.Vector2Field _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public Vector2Properties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.Vector2Field();
                _Field.ValueChanged += Vector2Field_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void Vector2Field_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Vector2)Value;
            }
        }
        #endregion

        #region Vector3
        protected class Vector3Properties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Vector3; } }

            private Skill.Editor.UI.Vector3Field _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public Vector3Properties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.Vector3Field();
                _Field.ValueChanged += Vector3Field_ValueChanged;
                _Field.Label.text = attribute.Name;
                _Field.Label.tooltip = attribute.Description;
            }

            void Vector3Field_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Vector3)Value;
            }
        }

        #endregion

        #region Vector4

        protected class Vector4Properties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Vector4; } }

            private Skill.Editor.UI.Vector4Field _Field;
            public override Framework.UI.BaseControl Control { get { return _Field; } }
            public Vector4Properties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Field = new Skill.Editor.UI.Vector4Field();
                _Field.ValueChanged += Vector4Field_ValueChanged;
                _Field.Label = attribute.Name;
            }

            void Vector4Field_ValueChanged(object sender, EventArgs e)
            {
                if (Owner.IgnoreChanges) return;
                Value = _Field.Value;
                Owner.SetDirty();
            }
            public override void Refresh()
            {
                _Field.Value = (Vector4)Value;
            }
        }

        #endregion

        #region Serializable

        class SerializableObjectExposeProperties : ExposeProperties
        {
            private ExposeProperties _Owner;
            public SerializableObjectExposeProperties(object obj, ExposeProperties owner)
                : base(obj)
            {
                _Owner = owner;
            }
            protected override void SetDirty()
            {
                _Owner.SetDirty();
            }
        }

        protected class SerializableObjectProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.SerializableObject; } }
            private Skill.Editor.UI.VerticalExpander<Skill.Framework.UI.StackPanel> _Expander;
            private SerializableObjectExposeProperties _ExposeProperties;

            private object _Object;
            private string _Name;
            public override Framework.UI.BaseControl Control { get { return _Expander; } }
            public SerializableObjectProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _ExposeProperties = new SerializableObjectExposeProperties(null, Owner);
                _ExposeProperties.Margin = new Framework.UI.Thickness(4, 0, 0, 0);

                _Expander = new VerticalExpander<Framework.UI.StackPanel>(_ExposeProperties);

                _Name = _Expander.Foldout.Content.text = attribute.Name;
                _Expander.Foldout.Content.tooltip = attribute.Description;
            }

            public override void Refresh()
            {
                object obj = Value;
                if (obj == null)
                {
                    try
                    {
                        if (Info.PropertyType.IsArray)
                            obj = Activator.CreateInstance(Info.PropertyType.GetElementType());
                        else
                            obj = Activator.CreateInstance(Info.PropertyType);
                        if (obj != null)
                            Value = obj;
                    }
                    catch (Exception)
                    {
                        Debug.Log(string.Format("Can not create type of serialized object : {0}", Info.PropertyType.ToString()));
                        obj = null;
                    }
                }

                if (obj != _Object)
                {
                    _Object = obj;
                    _ExposeProperties.ReplaceObject(_Object);
                    HookName();
                }
                _ExposeProperties.Refresh();
            }

            public void HookName()
            {
                if (Info.PropertyType.IsArray)
                {
                    foreach (var item in _ExposeProperties.Controls)
                    {
                        if (item is Skill.Editor.UI.TextField)
                        {
                            StringProperties sp = item.UserData as StringProperties;
                            if (sp != null && sp.Info.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
                            {
                                UpdateName(sp.Control);
                                ((Skill.Editor.UI.TextField)item).TextChanged += SerializableObjectProperties_TextChanged;
                            }
                        }
                    }
                }
            }

            void SerializableObjectProperties_TextChanged(object sender, EventArgs e)
            {
                UpdateName(sender);
            }

            private void UpdateName(object sender)
            {
                Skill.Editor.UI.TextField textField = sender as Skill.Editor.UI.TextField;
                if (textField != null)
                {
                    if (!string.IsNullOrEmpty(textField.Text))
                        _Expander.Header = textField.Text;
                    else
                        _Expander.Header = _Name;
                }
            }
        }

        #endregion

        #region Array

        protected class ArrayProperties : ControlProperties
        {
            public override PropertyType Type { get { return PropertyType.Array; } }

            private Skill.Framework.UI.StackPanel _Panel;
            private Skill.Editor.UI.IntField _SizeField;
            private Skill.Editor.UI.VerticalExpander<Skill.Framework.UI.StackPanel> _Expander;
            private List<ControlProperties> _Fields;
            public override Framework.UI.BaseControl Control { get { return _Expander; } }
            public ArrayProperties(ExposeProperties owner, PropertyInfo info, ExposePropertyAttribute attribute)
                : base(owner, attribute.Order, info)
            {
                _Fields = new List<ControlProperties>();
                _SizeField = new IntField() { Value = 0 };
                _SizeField.ValueChanged += _SizeField_ValueChanged;                
                _Panel = new Framework.UI.StackPanel() { Orientation = Framework.UI.Orientation.Vertical };
                _Panel.Margin = new Framework.UI.Thickness(4, 0, 0, 0);
                _Panel.Controls.Add(_SizeField);

                _Expander = new VerticalExpander<Framework.UI.StackPanel>(_Panel);

                _Expander.Foldout.Content.text = attribute.Name;
                _Expander.Foldout.Content.tooltip = attribute.Description;
            }

            void _SizeField_ValueChanged(object sender, EventArgs e)
            {                
                if (_SizeField.Value < 0)
                {
                    _SizeField.Value = 0;
                    return;
                }
                int fCount = _Fields.Count;
                int size = _SizeField.Value;

                if (fCount != size)
                {
                    if (!Owner.IgnoreChanges)
                    {
                        Array preArray = Value as Array;
                        Array newArray = Array.CreateInstance(Info.PropertyType.GetElementType(), size);

                        if (preArray != null)
                        {
                            for (int i = 0; i < Mathf.Min(preArray.Length, newArray.Length); i++)
                                newArray.SetValue(preArray.GetValue(i), i);
                        }
                        Value = newArray;
                    }

                    System.Type elementType = Info.PropertyType.GetElementType();
                    PropertyType elemntPropertyType;
                    if (GetPropertyType(elementType, out elemntPropertyType))
                    {
                        if (fCount < size)
                        {
                            for (int i = fCount; i < size; i++)
                            {
                                var f = Owner.CreateProperties(elemntPropertyType, Info, new ExposePropertyAttribute(i, string.Format("Element {0}", i)));
                                f.Index = i;
                                f.Refresh();
                                _Fields.Add(f);
                                _Panel.Controls.Add(f.Control);
                                f.Control.UserData = f;
                            }
                        }
                        else if (fCount > size)
                        {
                            for (int i = fCount - 1; i >= size; i--)
                                _Panel.Controls.Remove(_Fields[i].Control);
                            _Fields.RemoveRange(size, fCount - size);
                        }
                    }
                    if (!Owner.IgnoreChanges)
                        Owner.SetDirty();
                }
            }

            public override void Refresh()
            {
                ValidateSize();
                for (int i = 0; i < _Fields.Count; i++)
                {
                    _Fields[i].Index = i;
                    _Fields[i].Refresh();
                }

            }

            private void ValidateSize()
            {
                object array = Value;
                if (array == null)
                    array = Array.CreateInstance(Info.PropertyType.GetElementType(), 0);
                _SizeField.Value = ((Array)array).Length;
            }
        }

        #endregion

        #region GetPropertyType


        public enum PropertyType
        {
            None = -1,
            Integer = 0,
            Boolean = 1,
            Float = 2,
            String = 3,
            Color = 4,
            ObjectReference = 5,
            LayerMask = 6,
            Enum = 7,
            Vector2 = 8,
            Vector3 = 9,
            Vector4 = 10,
            Rect = 11,
            AnimationCurve = 12,
            Bounds = 13,
            Quaternion = 14,
            SerializableObject = 15,
            Array = 16
        }

        private struct TypeTag
        {
            public Type Type;
            public PropertyType PropertyType;
        }
        private static TypeTag[] _Types;
        private static void CreateTypes()
        {
            if (_Types == null)
            {
                _Types = new TypeTag[] 
            {
                new TypeTag(){ Type = typeof(float), PropertyType = PropertyType.Float},
                new TypeTag(){ Type = typeof(bool), PropertyType = PropertyType.Boolean},
                new TypeTag(){ Type = typeof(int), PropertyType = PropertyType.Integer},
                new TypeTag(){ Type = typeof(Vector3), PropertyType = PropertyType.Vector3},
                new TypeTag(){ Type = typeof(Vector2), PropertyType = PropertyType.Vector2},                
                new TypeTag(){ Type = typeof(Vector4), PropertyType = PropertyType.Vector4},                
                new TypeTag(){ Type = typeof(string), PropertyType = PropertyType.String},
                new TypeTag(){ Type = typeof(UnityEngine.Quaternion), PropertyType = PropertyType.Quaternion},
                new TypeTag(){ Type = typeof(UnityEngine.Color), PropertyType = PropertyType.Color},                
                new TypeTag(){ Type = typeof(UnityEngine.AnimationCurve), PropertyType = PropertyType.AnimationCurve},
                new TypeTag(){ Type = typeof(UnityEngine.Bounds), PropertyType = PropertyType.Bounds},                
                new TypeTag(){ Type = typeof(UnityEngine.LayerMask), PropertyType = PropertyType.LayerMask},                
                new TypeTag(){ Type = typeof(UnityEngine.Rect), PropertyType = PropertyType.Rect},
            };
            }
        }
        public static bool GetPropertyType(System.Type type, out PropertyType propertyType)
        {
            CreateTypes();
            propertyType = PropertyType.None;

            if (type.IsArray)
            {
                propertyType = PropertyType.Array;
                return true;
            }

            if (type.IsEnum)
            {
                propertyType = PropertyType.Enum;
                return true;
            }

            if (IsObjectReference(type))
            {
                propertyType = PropertyType.ObjectReference;
                return true;
            }
            for (int i = 0; i < _Types.Length; i++)
            {
                if (_Types[i].Type == type)
                {
                    propertyType = _Types[i].PropertyType;
                    return true;
                }
            }
            if (IsSerializable(type))
            {
                propertyType = PropertyType.SerializableObject;
                return true;
            }

            return false;

        }
        private static bool IsObjectReference(Type t)
        {
            Type unityObjectType = typeof(UnityEngine.Object);
            return t.IsSubclassOf(unityObjectType) || t == unityObjectType;
        }
        private static bool IsSerializable(Type t)
        {
            object[] atts = t.GetCustomAttributes(typeof(SerializableAttribute), true);
            return (atts != null && atts.Length > 0);
        }
        #endregion


    }
}