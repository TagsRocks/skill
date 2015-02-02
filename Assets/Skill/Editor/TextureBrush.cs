using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
namespace Skill.Editor
{
    public class TextureBrush
    {
        // Fields    
        private Texture2D _Brush;
        private Texture2D _Preview;
        private int _Size;
        private float[] _Strength;

        public Texture2D Brush { get { return this._Brush; } }
        public Texture2D Preview { get { return this._Preview; } }
        public int Size { get { return this._Size; } }

        public TextureBrush(Texture2D brushTexture, int size)
        {
            Load(brushTexture, size);
        }

        // Methods    

        private bool Load(Texture2D brushTexture, int size)
        {
            if (((this._Brush == brushTexture) && (size == this._Size)) && (this._Strength != null))
            {
                return true;
            }
            if (brushTexture != null)
            {
                float num = size;
                this._Size = size;
                this._Strength = new float[this._Size * this._Size];
                if (this._Size > 3)
                {
                    for (int j = 0; j < this._Size; j++)
                    {
                        for (int k = 0; k < this._Size; k++)
                        {
                            this._Strength[(j * this._Size) + k] = brushTexture.GetPixelBilinear((k + 0.5f) / num, ((float)j) / num).a;
                        }
                    }
                }
                else
                {
                    for (int m = 0; m < this._Strength.Length; m++)
                    {
                        this._Strength[m] = 1f;
                    }
                }
                UnityEngine.Object.DestroyImmediate(this._Preview);
                this._Preview = new Texture2D(this._Size, this._Size, TextureFormat.ARGB32, false);
                this._Preview.hideFlags = HideFlags.HideAndDontSave;
                this._Preview.wrapMode = TextureWrapMode.Repeat;
                this._Preview.filterMode = FilterMode.Point;
                Color[] colors = new Color[this._Size * this._Size];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new Color(1f, 1f, 1f, this._Strength[i]);
                }
                this._Preview.SetPixels(0, 0, this._Size, this._Size, colors, 0);
                this._Preview.Apply();

                this._Brush = brushTexture;
                return true;
            }
            this._Strength = new float[] { 1f };
            this._Size = 1;
            return false;
        }

        public void Destroy()
        {
            if (this._Preview != null)
                UnityEngine.Object.DestroyImmediate(this._Preview);
            this._Brush = null;
            this._Preview = null;
            this._Strength = null;
        }

        public float GetStrengthInt(int ix, int iy)
        {
            ix = Mathf.Clamp(ix, 0, this._Size - 1);
            iy = Mathf.Clamp(iy, 0, this._Size - 1);
            return this._Strength[(iy * this._Size) + ix];
        }
    }
}
