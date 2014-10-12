using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Effects
{
    /// <summary>
    /// Draw wrapped rain texture on screen
    /// </summary>
    public class RainScreen : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Wrapped rain texture </summary>
        public Texture2D RainTexture;
        /// <summary> Rain direction </summary>
        public Vector2 Direction = new Vector2(-1, -1);
        /// <summary> Rain Speed </summary>
        public float Speed = 100;
        /// <summary> Size of each part relative to screen size </summary>
        public float PartSize = 0.2f;
        /// <summary> Rotate textures? </summary>
        public bool UseRotation;
        /// <summary> Rotation of textures </summary>
        public float Rotation;

        private Vector2[] _Points;
        private Vector2[] _Offsets;
        private float _Size;

        private Skill.Framework.ScreenSizeChange _ScreenSizeChecker;
        private float _PartSize;

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            useGUILayout = false;
            base.Awake();
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            if (_ScreenSizeChecker.IsChanged || _PartSize != PartSize)
            {
                _PartSize = PartSize;
                Rebuild();
            }

            for (int i = 0; i < _Points.Length; i++)
            {
                Vector2 p = _Offsets[i];
                Vector2 dir = Direction;
                dir.y *= -1;
                p += dir * (Speed * Time.deltaTime);

                if (dir.x < 0 && p.x < 0)
                    p.x += _Size;
                else if (dir.x > 0 && p.x > _Size)
                    p.x -= _Size;

                if (dir.y < 0 && p.y < 0)
                    p.y += _Size;
                else if (dir.y > 0 && p.y > _Size)
                    p.y -= _Size;

                _Offsets[i] = p;
            }

            base.Update();
        }

        private void Rebuild()
        {
            _Size = (Screen.width + Screen.height) * 0.5f * PartSize;
            int xCount = Mathf.FloorToInt((float)Screen.width / _Size) + 3;
            int yCount = Mathf.FloorToInt((float)Screen.height / _Size) + 3;

            _Points = new Vector2[xCount * yCount];
            _Offsets = new Vector2[xCount * yCount];
            int index = 0;
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    _Points[index++] = new Vector2((i - 1) * _Size, (j - 1) * _Size);
                }
            }
        }

        /// <summary> OnGUI </summary>
        protected virtual void OnGUI()
        {
            Matrix4x4 savedMatrix = GUI.matrix;
            if (_Points != null)
            {
                Rect rect = new Rect(0, 0, _Size, _Size);
                float size2 = _Size * 0.5f;
                for (int i = 0; i < _Points.Length; i++)
                {
                    Vector2 p = _Points[i] + _Offsets[i];
                    if (UseRotation)
                        GUIUtility.RotateAroundPivot(Rotation, p);
                    rect.x = p.x - size2;
                    rect.y = p.y - size2;
                    GUI.DrawTexture(rect, RainTexture, ScaleMode.StretchToFill, true);
                }
            }
            GUI.matrix = savedMatrix;
        }
    }
}
