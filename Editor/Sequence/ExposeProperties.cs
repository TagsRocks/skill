using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.Sequence;
using Skill.Framework;

namespace Skill.Editor.Sequence
{



    public abstract class ExposeProperties : Properties
    {
        protected static Skill.Framework.UI.Thickness ControlMargin = new Skill.Framework.UI.Thickness(2);

        class ControlTag
        {
            public Skill.Framework.UI.BaseControl Field;
            public int Order;
            public PropertyType Type;
            public PropertyInfo Info;
        }
        class ControlTagComparer : IComparer<ControlTag>
        {
            public int Compare(ControlTag x, ControlTag y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }
        class PropertyData
        {
            public MethodInfo Setter;
            public MethodInfo Getter;
        }

        private List<ControlTag> _Fields;

        public ExposeProperties(System.Object obj)
            : base(obj)
        {
            CreateCustomFileds();

            _Fields = new List<ControlTag>();
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
                if (GetPropertyType(info, out type))
                {
                    Skill.Framework.UI.BaseControl control = null;
                    switch (type)
                    {
                        case PropertyType.AnimationCurve:
                            control = CreateAnimationCurve(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Boolean:
                            control = CreateBoolean(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Bounds:
                            control = CreateBounds(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Color:
                            control = CreateColor(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Enum:
                            control = CreateEnum(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Float:
                            control = CreateFloat(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Integer:
                            control = CreateInt(info, exposePropertyAttribute);
                            break;
                        case PropertyType.LayerMask:
                            control = CreateLayerMask(info, exposePropertyAttribute);
                            break;
                        case PropertyType.ObjectReference:
                            control = CreateObjectField(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Quaternion:
                            control = CreateQuaternion(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Rect:
                            control = CreateRect(info, exposePropertyAttribute);
                            break;
                        case PropertyType.String:
                            control = CreateString(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Vector2:
                            control = CreateVector2(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Vector3:
                            control = CreateVector3(info, exposePropertyAttribute);
                            break;
                        case PropertyType.Vector4:
                            control = CreateVector4(info, exposePropertyAttribute);
                            break;
                        case PropertyType.SerializableObject:
                            Debug.Log("SerializableObject peoperties not implemented");
                            break;
                    }                    

                    if (control != null)
                    {
                        control.Margin = new Skill.Framework.UI.Thickness(2);
                        PropertyData pd = new PropertyData();
                        pd.Getter = info.GetGetMethod();
                        pd.Setter = info.GetSetMethod();
                        control.UserData = pd;
                        ControlTag ct = new ControlTag() { Info = info, Field = control, Order = ((ExposePropertyAttribute)info.GetCustomAttributes(epaType, true)[0]).Order, Type = type };
                        _Fields.Add(ct);
                    }
                }
            }

            if (_Fields.Count > 0)
            {
                _Fields.Sort(new ControlTagComparer());
                foreach (var item in _Fields)
                    Controls.Add(item.Field);
            }
        }

        protected virtual void CreateCustomFileds() { }

        protected override void RefreshData()
        {
            foreach (var item in _Fields)
            {
                switch (item.Type)
                {
                    case PropertyType.AnimationCurve:
                        RefreshCurveField((Skill.Editor.UI.CurveField)item.Field, item.Info);
                        break;
                    case PropertyType.Boolean:
                        RefreshToggleButton((Skill.Editor.UI.ToggleButton)item.Field, item.Info);
                        break;
                    case PropertyType.Bounds:
                        RefreshBoundsField((Skill.Editor.UI.BoundsField)item.Field, item.Info);
                        break;
                    case PropertyType.Color:
                        RefreshColorField((Skill.Editor.UI.ColorField)item.Field, item.Info);
                        break;
                    case PropertyType.Enum:
                        RefreshEnumMaskField((Skill.Editor.UI.EnumPopup)item.Field, item.Info);
                        break;
                    case PropertyType.Float:
                        RefreshFloatField((Skill.Editor.UI.FloatField)item.Field, item.Info);
                        break;
                    case PropertyType.Integer:
                        RefreshIntField((Skill.Editor.UI.IntField)item.Field, item.Info);
                        break;
                    case PropertyType.LayerMask:
                        RefreshLayerMaskField((Skill.Editor.UI.LayerMaskField)item.Field, item.Info);
                        break;
                    case PropertyType.ObjectReference:
                        RefreshObjectField((Skill.Editor.UI.ObjectField<UnityEngine.Object>)item.Field, item.Info);
                        break;
                    case PropertyType.Quaternion:
                        RefreshQuaternionField((Skill.Editor.UI.Vector3Field)item.Field, item.Info);
                        break;
                    case PropertyType.Rect:
                        RefreshRectField((Skill.Editor.UI.RectField)item.Field, item.Info);
                        break;
                    case PropertyType.String:
                        RefreshTextField((Skill.Editor.UI.TextField)item.Field, item.Info);
                        break;
                    case PropertyType.Vector2:
                        RefreshVector2Field((Skill.Editor.UI.Vector2Field)item.Field, item.Info);
                        break;
                    case PropertyType.Vector3:
                        RefreshVector3Field((Skill.Editor.UI.Vector3Field)item.Field, item.Info);
                        break;
                    case PropertyType.Vector4:
                        RefreshVector4Field((Skill.Editor.UI.Vector4Field)item.Field, item.Info);
                        break;
                    default:
                        break;
                }
            }
        }


        #region CurveField
        private Skill.Editor.UI.CurveField CreateAnimationCurve(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.CurveField control = new Skill.Editor.UI.CurveField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            return control;
        }
        private void RefreshCurveField(Skill.Editor.UI.CurveField curveField, PropertyInfo info)
        {
            AnimationCurve curve = info.GetGetMethod().Invoke(Object, null) as AnimationCurve;
            if (curve == null)
            {
                curve = new AnimationCurve();
                info.GetSetMethod().Invoke(Object, new object[] { curve });
            }
            curveField.Curve = (AnimationCurve)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region ToggleButton
        private Skill.Editor.UI.ToggleButton CreateBoolean(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.ToggleButton control = new Skill.Editor.UI.ToggleButton() { IsChecked = (bool)info.GetGetMethod().Invoke(Object, null) };
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.Changed += ToggleButton_Changed;
            return control;
        }

        private void ToggleButton_Changed(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.ToggleButton control = (Skill.Editor.UI.ToggleButton)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.IsChecked });
            SetDirty();
        }

        private void RefreshToggleButton(Skill.Editor.UI.ToggleButton toggleButton, PropertyInfo info)
        {
            toggleButton.IsChecked = (bool)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region BoundsField
        private Skill.Editor.UI.BoundsField CreateBounds(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.BoundsField control = new Skill.Editor.UI.BoundsField();
            control.ValueChanged += BoundsField_ValueChanged;
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            return control;
        }

        void BoundsField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.BoundsField control = (Skill.Editor.UI.BoundsField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshBoundsField(Skill.Editor.UI.BoundsField boundsField, PropertyInfo info)
        {
            boundsField.Value = (Bounds)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region ColorField
        private Skill.Editor.UI.ColorField CreateColor(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.ColorField control = new Skill.Editor.UI.ColorField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ColorChanged += ColorField_ValueChanged;
            return control;
        }

        void ColorField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.ColorField control = (Skill.Editor.UI.ColorField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Color });
            SetDirty();
        }

        private void RefreshColorField(Skill.Editor.UI.ColorField colorField, PropertyInfo info)
        {
            colorField.Color = (Color)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region EnumMaskField
        private Skill.Editor.UI.EnumPopup CreateEnum(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.EnumPopup control = new Skill.Editor.UI.EnumPopup();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ValueChanged += EnumPopup_ValueChanged;
            return control;
        }

        void EnumPopup_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.EnumPopup control = (Skill.Editor.UI.EnumPopup)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshEnumMaskField(Skill.Editor.UI.EnumPopup enumMaskField, PropertyInfo info)
        {
            enumMaskField.Value = (Enum)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region FloatField
        private Skill.Editor.UI.FloatField CreateFloat(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.FloatField control = new Skill.Editor.UI.FloatField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ValueChanged += FloatField_ValueChanged;
            return control;
        }

        void FloatField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.FloatField control = (Skill.Editor.UI.FloatField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshFloatField(Skill.Editor.UI.FloatField floatField, PropertyInfo info)
        {
            floatField.Value = (float)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region IntField
        private Skill.Editor.UI.IntField CreateInt(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.IntField control = new Skill.Editor.UI.IntField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ValueChanged += IntField_ValueChanged;
            return control;
        }

        void IntField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.IntField control = (Skill.Editor.UI.IntField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshIntField(Skill.Editor.UI.IntField intField, PropertyInfo info)
        {
            intField.Value = (int)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region LayerMaskField
        private Skill.Editor.UI.LayerMaskField CreateLayerMask(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.LayerMaskField control = new Skill.Editor.UI.LayerMaskField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.LayersChanged += LayerMaskField_ValueChanged;
            return control;
        }

        void LayerMaskField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.LayerMaskField control = (Skill.Editor.UI.LayerMaskField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { new UnityEngine.LayerMask() { value = control.Layers } });
            SetDirty();
        }

        private void RefreshLayerMaskField(Skill.Editor.UI.LayerMaskField layerMaskField, PropertyInfo info)
        {
            layerMaskField.Layers = ((UnityEngine.LayerMask)info.GetGetMethod().Invoke(Object, null)).value;
        }
        #endregion

        #region ObjectField
        private Skill.Editor.UI.ObjectField<UnityEngine.Object> CreateObjectField(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.ObjectField<UnityEngine.Object> control = new Skill.Editor.UI.ObjectField<UnityEngine.Object>();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ObjectChanged += ObjectField_ValueChanged;
            return control;
        }

        void ObjectField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.ObjectField<UnityEngine.Object> control = (Skill.Editor.UI.ObjectField<UnityEngine.Object>)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Object });
            SetDirty();
        }

        private void RefreshObjectField(Skill.Editor.UI.ObjectField<UnityEngine.Object> objectField, PropertyInfo info)
        {
            objectField.Object = info.GetGetMethod().Invoke(Object, null) as UnityEngine.Object;
        }
        #endregion

        #region QuaternionField
        private Skill.Editor.UI.Vector3Field CreateQuaternion(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.Vector3Field control = new Skill.Editor.UI.Vector3Field();
            control.Label = attribute.Name;
            control.ValueChanged += QuaternionField_ValueChanged;
            return control;
        }

        void QuaternionField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.Vector3Field control = (Skill.Editor.UI.Vector3Field)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { Quaternion.Euler(control.Value) });
            SetDirty();
        }

        private void RefreshQuaternionField(Skill.Editor.UI.Vector3Field quaternionField, PropertyInfo info)
        {
            Quaternion q = (Quaternion)info.GetGetMethod().Invoke(Object, null);
            quaternionField.Value = q.eulerAngles;
        }
        #endregion

        #region RectField
        private Skill.Editor.UI.RectField CreateRect(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.RectField control = new Skill.Editor.UI.RectField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.ValueChanged += RectField_ValueChanged;
            return control;
        }

        void RectField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.RectField control = (Skill.Editor.UI.RectField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshRectField(Skill.Editor.UI.RectField rectField, PropertyInfo info)
        {
            rectField.Value = (Rect)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region TextField
        private Skill.Editor.UI.TextField CreateString(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.TextField control = new Skill.Editor.UI.TextField();
            control.Label.text = attribute.Name;
            control.Label.tooltip = attribute.Description;
            control.TextChanged += TextField_ValueChanged;
            return control;
        }

        void TextField_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.TextField control = (Skill.Editor.UI.TextField)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Text });
            SetDirty();
        }

        private void RefreshTextField(Skill.Editor.UI.TextField textField, PropertyInfo info)
        {
            textField.Text = info.GetGetMethod().Invoke(Object, null) as string;
        }
        #endregion

        #region Vector2Field
        private Skill.Editor.UI.Vector2Field CreateVector2(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.Vector2Field control = new Skill.Editor.UI.Vector2Field();
            control.Label = attribute.Name;
            control.ValueChanged += Vector2Field_ValueChanged;
            return control;
        }

        void Vector2Field_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.Vector2Field control = (Skill.Editor.UI.Vector2Field)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshVector2Field(Skill.Editor.UI.Vector2Field vector2Field, PropertyInfo info)
        {
            vector2Field.Value = (Vector2)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region Vector3Field
        private Skill.Editor.UI.Vector3Field CreateVector3(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.Vector3Field control = new Skill.Editor.UI.Vector3Field();
            control.Label = attribute.Name;
            control.ValueChanged += Vector3Field_ValueChanged;
            return control;
        }

        void Vector3Field_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.Vector3Field control = (Skill.Editor.UI.Vector3Field)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshVector3Field(Skill.Editor.UI.Vector3Field vector3Field, PropertyInfo info)
        {
            vector3Field.Value = (Vector3)info.GetGetMethod().Invoke(Object, null);
        }
        #endregion

        #region Vector4Field
        private Skill.Editor.UI.Vector4Field CreateVector4(PropertyInfo info, ExposePropertyAttribute attribute)
        {
            Skill.Editor.UI.Vector4Field control = new Skill.Editor.UI.Vector4Field();
            control.Label = attribute.Name;
            control.ValueChanged += Vector4Field_ValueChanged;
            return control;
        }

        void Vector4Field_ValueChanged(object sender, EventArgs e)
        {
            if (IgnoreChanges) return;
            Skill.Editor.UI.Vector4Field control = (Skill.Editor.UI.Vector4Field)sender;
            ((PropertyData)control.UserData).Setter.Invoke(Object, new object[] { control.Value });
            SetDirty();
        }

        private void RefreshVector4Field(Skill.Editor.UI.Vector4Field vector4Field, PropertyInfo info)
        {
            vector4Field.Value = (Vector4)info.GetGetMethod().Invoke(Object, null);
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
            SerializableObject = 15
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
        public static bool GetPropertyType(PropertyInfo info, out PropertyType propertyType)
        {
            CreateTypes();
            propertyType = PropertyType.None;
            Type type = info.PropertyType;

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
            //Type objectType = typeof(object);
            Type unityObjectType = typeof(UnityEngine.Object);


            return t.IsSubclassOf(unityObjectType);

            //while (t != objectType)
            //{
            //    if (t == unityObjectType)
            //        return true;
            //    t = t.BaseType;
            //}
            //return false;
        }

        private static bool IsSerializable(Type t)
        {
            object[] atts = t.GetCustomAttributes(typeof(SerializableAttribute), true);
            return (atts != null && atts.Length > 0);
        }
        #endregion
    }
}