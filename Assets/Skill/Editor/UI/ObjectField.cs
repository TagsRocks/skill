﻿using System;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{

    /// <summary>
    /// Make an object field. You can assign objects either by drag and drop objects or by selecting an object using the Object Picker.
    /// </summary>    
    public class ObjectField<T> : EditorControl where T : UnityEngine.Object
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

#pragma warning disable 0067 // this warning is only inside unity (???)
        /// <summary>
        /// Occurs when object of ObjectField changes
        /// </summary>
        public event EventHandler ObjectChanged;

#pragma warning restore 0067
        /// <summary>
        /// when object of ObjectField changes
        /// </summary>
        protected virtual void OnObjectChanged()
        {
            if (ObjectChanged != null) ObjectChanged(this, EventArgs.Empty);
        }

        private T _Object;
        /// <summary>
        /// UnityEngine.Object - The object that has been set by the user.
        /// </summary>
        public T Object
        {
            get { return _Object; }
            set
            {
                if (_Object != value)
                {
                    _Object = value;
                    OnObjectChanged();
                }
            }
        }

        /// <summary>
        /// Type of object
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Allow assigning scene objects. See Description for more info. (default true)
        /// </summary>
        public bool AllowSceneObjects { get; set; }

        /// <summary>
        /// Create an instance of ObjectField
        /// </summary>
        public ObjectField()
        {
            Label = new GUIContent();
            ObjectType = typeof(T);
            this.Height = 16;
            this.AllowSceneObjects = true;
        }

        /// <summary>
        /// Render ObjectField
        /// </summary>
        protected override void Render()
        {
            UnityEngine.Object obj = EditorGUI.ObjectField(RenderArea, Label, _Object, ObjectType, AllowSceneObjects);
            if (obj != null)
                Object = (T)obj;
            else
                Object = null;
        }
    }


    public class UntypedObjectField : EditorControl 
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when object of ObjectField changes
        /// </summary>
        public event EventHandler ObjectChanged;
        /// <summary>
        /// when object of ObjectField changes
        /// </summary>
        protected virtual void OnObjectChanged()
        {
            if (ObjectChanged != null) ObjectChanged(this, EventArgs.Empty);
        }

        private UnityEngine.Object _Object;
        /// <summary>
        /// UnityEngine.Object - The object that has been set by the user.
        /// </summary>
        public UnityEngine.Object Object
        {
            get { return _Object; }
            set
            {
                if (_Object != value)
                {
                    _Object = value;
                    OnObjectChanged();
                }
            }
        }

        /// <summary>
        /// Type of object
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Allow assigning scene objects. See Description for more info. (default true)
        /// </summary>
        public bool AllowSceneObjects { get; set; }

        /// <summary>
        /// Create an instance of ObjectField
        /// </summary>
        public UntypedObjectField(Type type)
        {
            Label = new GUIContent();
            ObjectType = type;
            this.Height = 16;
            this.AllowSceneObjects = true;
        }

        /// <summary>
        /// Render ObjectField
        /// </summary>
        protected override void Render()
        {
            UnityEngine.Object obj = EditorGUI.ObjectField(RenderArea, Label, _Object, ObjectType, AllowSceneObjects);
            if (obj != null)
                Object = obj;
            else
                Object = null;
        }
    }
}
