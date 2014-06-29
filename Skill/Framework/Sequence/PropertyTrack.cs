using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public interface IPropertyKey<V> : ITrackKey
    {
        /// <summary> Value key</summary>
        V ValueKey { get; set; }
    }

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

        public override float Length
        {
            get
            {
                if (PropertyKeys != null && PropertyKeys.Length > 0)
                {
                    if (!Application.isPlaying)
                        SortKeys();

                    return PropertyKeys[PropertyKeys.Length - 1].FireTime;
                }
                return 0;
            }
        }

        public abstract IPropertyKey<V>[] PropertyKeys { get; set; }
        public abstract Type PropertyType { get; }
        protected float CurrecntTime { get; private set; }

        private System.Reflection.MethodInfo _Setter;
        private System.Reflection.MethodInfo _Getter;
        private System.Reflection.FieldInfo _Field;
        private int _Index;

        public override void Evaluate(float time)
        {
            int evaluatedIndex = -1;
            float preTime = CurrecntTime;
            CurrecntTime = time;
            float deltaTime = CurrecntTime - preTime;
            if (deltaTime > 0)
            {
                if (PropertyKeys != null)
                {
                    if (_Index < 0) _Index = 0;
                    while (_Index < PropertyKeys.Length)
                    {
                        float t = PropertyKeys[_Index].FireTime;
                        if (t <= CurrecntTime)
                        {
                            evaluatedIndex = _Index;
                            if (t < CurrecntTime && t >= preTime)
                            {
                                Execute(PropertyKeys[_Index]);
                            }
                            _Index++;
                        }
                        else
                        {
                            _Index--;
                            break;
                        }
                    }
                }
            }
            else if (deltaTime < 0)
            {
                if (PropertyKeys != null)
                {
                    if (_Index >= PropertyKeys.Length) _Index = PropertyKeys.Length - 1;
                    while (_Index >= 0)
                    {
                        float t = PropertyKeys[_Index].FireTime;
                        if (t < CurrecntTime)
                            break;
                        else if (t >= CurrecntTime && t < preTime)
                        {
                            evaluatedIndex = _Index;
                            Evaluate(PropertyKeys[_Index]);
                        }
                        _Index--;
                    }
                }
            }

            Evaluate(evaluatedIndex);
        }

        private void Evaluate(int evaluatedIndex)
        {
            if (_Index < 0)
                Rollback();
            else
            {
                if (_Index >= PropertyKeys.Length)
                    _Index = PropertyKeys.Length - 1;
                if (evaluatedIndex != _Index && _Index >= 0 && _Index < PropertyKeys.Length)
                {
                    Evaluate(PropertyKeys[_Index]);
                }
            }
        }

        public override void Seek(float time)
        {
            CurrecntTime = time;
            if (PropertyKeys != null && PropertyKeys.Length > 0)
                _Index = FindMaxIndexBeforeTime(PropertyKeys, time);
            else
                _Index = -1;
            Evaluate(-1);
        }
        public override void SortKeys()
        {
            if (PropertyKeys != null && PropertyKeys.Length > 1)
            {
                Skill.Framework.MathHelper.QuickSort(PropertyKeys, new TrackKeyComparer<IPropertyKey<V>>());
            }
        }


        /// <summary>
        /// When time is paused but make sure key applied (for curve tracks)
        /// </summary>
        /// <param name="key">Key to Verify</param>
        protected virtual void Execute(IPropertyKey<V> key)
        {
            SetValue(key.ValueKey);
        }

        /// <summary>
        /// When time is paused but make sure key applied (for curve tracks)
        /// </summary>
        /// <param name="key">Key to Evaluate</param>
        protected virtual void Evaluate(IPropertyKey<V> key)
        {
            Execute(key);
        }

        public override void Stop()
        {
            SetValue(DefaultValue);
        }

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

        protected IPropertyKey<V> GetPreviousKey()
        {
            if (PropertyKeys == null) return null;
            if (_Index > 0)
                return PropertyKeys[_Index - 1];
            else
                return null;
        }

        protected IPropertyKey<V> GetNextKey()
        {
            if (PropertyKeys == null) return null;
            if (_Index < PropertyKeys.Length - 1)
                return PropertyKeys[_Index + 1];
            else
                return null;
        }

        public void Invalidate()
        {
            _Setter = null;
        }
    }

}