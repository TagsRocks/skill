using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Editor;
using Skill.Editor.UI;
using Skill.Framework.UI;
using System;
using System.Collections.Generic;

namespace Skill.Editor
{
    public class BrushProjector
    {
        private TextureBrush _Brush;
        public TextureBrush Brush
        {
            get { return _Brush; }
            set
            {
                _Brush = value;
                if (_Projector != null)
                {
                    _Projector.material.SetTexture("_MainTex", _Brush != null ? _Brush.Preview : null);
                    _Projector.material.SetTexture("_CutoutTex", _Brush != null ? _Brush.Brush : null);
                }
            }
        }

        private Projector _Projector;
        public Projector Projector { get { return _Projector; } }

        public BrushProjector()
        {
            Type[] components = new Type[] { typeof(Projector) };
            GameObject obj2 = EditorUtility.CreateGameObjectWithHideFlags("BrushPreview", HideFlags.HideAndDontSave, components);
            this._Projector = obj2.GetComponent<Projector>();
            this._Projector.enabled = false;
            this._Projector.nearClipPlane = -1000f;
            this._Projector.farClipPlane = 1000f;
            this._Projector.orthographic = true;
            this._Projector.orthographicSize = 10f;
            obj2.transform.Rotate((float)90f, 0f, (float)180f);
            Material material = new Material(Skill.Editor.Resources.GetShader("BrushPreview"));            

            material.shader.hideFlags = HideFlags.HideAndDontSave;
            material.hideFlags = HideFlags.HideAndDontSave;
            this._Projector.material = material;
            this._Projector.enabled = false;
            
        }

        public void Destroy()
        {
            if (this._Projector != null)
            {
                UnityEngine.Object.DestroyImmediate(this._Projector.material);
                UnityEngine.Object.DestroyImmediate(this._Projector.gameObject);
                this._Projector = null;
            }
        }
    }
}