using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Shift uv. for example tank wheel chain
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class UVShift : DynamicBehaviour
    {
        /// <summary> Texture parameters in material shader </summary>
        public string[] Parameters = new string[] { "_MainTex" };
        /// <summary> Speed of shift </summary>
        public float Speed = 1;
        /// <summary> Shift U </summary>
        public bool U;
        /// <summary> Shift V </summary>
        public bool V;

        private Material _Material;
        private Vector2 _Offset;


        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Material = renderer.material;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (Speed != 0)
            {
                float speed = Speed * Time.deltaTime;
                if (U)
                    _Offset.x += speed;
                if (V)
                    _Offset.y += speed;

                //while (_Offset.x > 1.0f) _Offset.x -= 1.0f;
                //while (_Offset.x < -1.0f) _Offset.x += 1.0f;
                //while (_Offset.y > 1.0f) _Offset.y -= 1.0f;
                //while (_Offset.y < -1.0f) _Offset.y += 1.0f;

                foreach (var p in Parameters)
                    _Material.SetTextureOffset(p, _Offset);
            }
            base.Update();
        }
    }
}
