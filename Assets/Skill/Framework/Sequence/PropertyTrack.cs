using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public abstract class PropertyTrack<V> : Track
    {
        [SerializeField]
        [HideInInspector]
        public GameObject Object;

        [SerializeField]
        [HideInInspector]
        public Component Component;

        [SerializeField]
        [HideInInspector]
        public string PropertyName;

        /// <summary>
        /// Default value before Matinee
        /// </summary>  
        [SerializeField]
        [HideInInspector]
        public V DefaultValue;

        [ExposeProperty(100, "Default Value")]
        public virtual V ExDefaultValue { get { return DefaultValue; } set { DefaultValue = value; } }

        public abstract Type PropertyType { get; }

        private System.Reflection.MethodInfo _Setter;
        private System.Reflection.MethodInfo _Getter;
        private System.Reflection.FieldInfo _Field;


        public override void Stop() { }

        public override void Rollback()
        {
            SetValue(DefaultValue);
        }

        private void ValidateAccessMethods()
        {
            if (_Setter == null && _Field == null)
            {
                if (Component != null && !string.IsNullOrEmpty(PropertyName))
                {
                    PropertyInfo info = Component.GetType().GetProperty(PropertyName);
                    if (info != null)
                    {
                        if (info.PropertyType == PropertyType && info.CanWrite)
                        {
                            _Setter = info.GetSetMethod();
                            _Getter = info.GetGetMethod();
                        }
                    }
                }
                if (_Setter == null)
                    _Field = Component.GetType().GetField(PropertyName);

                if (_Setter == null && _Field == null && Application.isPlaying)
                    Debug.LogWarning("Invalid property :" + PropertyName);
            }
        }

        public virtual void SetValue(V value)
        {
            if (Component != null)
            {
                ValidateAccessMethods();

                if (_Setter != null)
                    _Setter.Invoke(Component, new System.Object[] { value });
                else if (_Field != null)
                    _Field.SetValue(Component, value);
            }
        }

        public virtual object GetValue()
        {
            object value = null;
            if (Component != null)
            {
                ValidateAccessMethods();
                if (_Getter != null)
                    value = _Getter.Invoke(Component, null);
                else if (_Field != null)
                    value = _Field.GetValue(Component);
            }
            return value;
        }

        public void Invalidate()
        {
            _Setter = null;
            _Getter = null;
            _Field = null;

        }
    }

}