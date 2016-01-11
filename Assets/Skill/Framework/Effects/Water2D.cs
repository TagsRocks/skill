using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Skill.Framework.Effects
{
    public class Water2D : Skill.Framework.DynamicBehaviour
    {
        public float Tension = 0.025f;
        public float Dampening = 0.025f;
        public float Spread = 0.3f;
        public float SplashLenght = 10.0f;
        public float SplashDuration = 5.0f;
        public int SplashVertexCount = 200;
        public int SplashBuffer = 3;
        public int SplashPass = 8;
        public int WaveVertexCount = 60;
        public float Amplitude = 0.05f;
        public float WaveLength = 2.5f;
        public float Speed = 0.6f;

        private float[] _WaveFactors;
        private List<Splash> _Splashes;

        public int SplashCount;
        public float GetHeight(float x)
        {
            float waveH = GetWaveHeight(x);
            float splashH = 0;
            int spCount = 1;
            bool found = false;
            foreach (var sp in _Splashes)
            {
                if (sp.IsEnable && sp.Contains(x))
                {
                    splashH += sp.GetHeight(x);
                    spCount++;
                    found = true;
                }
            }
            if (!found)
                spCount++;
            return (splashH + waveH) / spCount;
        }

        protected override void Awake()
        {
            base.Awake();
            _WaveFactors = CalculateWaveFactors(WaveVertexCount);
        }

        protected override void Start()
        {
            base.Start();
            _Splashes = new List<Splash>();
            for (int i = 0; i < SplashBuffer; i++)
                _Splashes.Add(CreateSplash());
        }
        protected override void Update()
        {
            base.Update();
            if (Global.IsGamePaused) return;
            if (_WaveFactors == null || _WaveFactors.Length != WaveVertexCount)
            {
                WaveVertexCount = Mathf.Max(10, WaveVertexCount);
                _WaveFactors = CalculateWaveFactors(WaveVertexCount);
                if (_WaveFactors == null || _WaveFactors.Length != WaveVertexCount)
                    throw new System.InvalidOperationException("Invalid WaveFactors- length of WaveFactors must be equal to WaveVertexCount");
            }

            int index = 0;
            while (index < _Splashes.Count)
            {
                Splash sp = _Splashes[index];
                if (sp.IsEnable)
                {
                    sp.Update(Dampening, Tension, Spread, SplashPass);
                }
                else if (_Splashes.Count > SplashBuffer)
                {
                    _Splashes.RemoveAt(index);
                    break;
                }
                index++;
            }

            SplashCount = _Splashes.Count;
        }


        private float GetWaveHeight(float x)
        {
            float repeatedX = Mathf.Repeat(x + Time.time * Speed, WaveLength);
            float percent = repeatedX / WaveLength;

            int index1 = Mathf.FloorToInt(percent * (_WaveFactors.Length - 1));
            int index2 = index1 + 1;
            if (index2 >= WaveVertexCount)
                index2 -= WaveVertexCount;

            float step = WaveLength / _WaveFactors.Length;
            float factor = (repeatedX - (index1 * step)) / step;
            float height = Mathf.Lerp(_WaveFactors[index1], _WaveFactors[index2], factor);
            return height * Amplitude;
        }
        protected virtual float[] CalculateWaveFactors(int vertexCount)
        {
            vertexCount = Mathf.Max(10, vertexCount);
            float[] waveFactors = new float[vertexCount];

            float stepAngle = (Mathf.PI * 2) / vertexCount;
            float angle = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                waveFactors[i] = Mathf.Sin(angle);
                angle += stepAngle;
            }
            return waveFactors;
        }
        public void MakeSplash(float x, float speed)
        {
            Splash sp = null;

            foreach (var item in _Splashes)
            {
                if (item.IsEnable && item.Contains(x))
                {
                    sp = item;
                    break;
                }
            }
            if (sp == null)
                sp = GetFreeSplash();
            sp.Apply(x, SplashLenght, speed);
        }
        private Splash GetFreeSplash()
        {
            foreach (var item in _Splashes)
            {
                if (!item.IsEnable)
                    return item;
            }

            Splash sp = CreateSplash();
            _Splashes.Add(sp);
            return sp;
        }
        private Splash CreateSplash()
        {
            return new Splash(SplashVertexCount, SplashDuration);
        }


        #region Splash
        class Splash
        {
            private class Vertex
            {
                public float Height;
                public float Speed;
                public float LeftDelta;
                public float RightDelta;

                public void Update(float dampening, float tension)
                {
                    float x = -Height;
                    Speed += tension * x - Speed * dampening;
                    Height += Speed;
                }
            }

            private List<Vertex> _Vertices;
            private Skill.Framework.TimeWatch _UpdateTW;
            private int _BaseVertexCount;
            private float _StepX;

            public float Duration { get; private set; }
            public float LeftX { get; private set; }
            public float RightX { get; private set; }

            /// <summary> Length </summary>
            public float Length { get; private set; }
            public bool IsEnable { get { return !_UpdateTW.IsOver; } }

            public bool Contains(float x)
            {
                return x > LeftX && x < RightX;
            }

            public float GetHeight(float x)
            {
                if (x < LeftX) return _Vertices[0].Height;
                if (x > RightX) return _Vertices[_Vertices.Count - 1].Height;

                int index = Mathf.Clamp(Mathf.FloorToInt((x - LeftX) / (RightX - LeftX) * (_Vertices.Count - 1)), 0, _Vertices.Count - 1);
                return _Vertices[index].Height;
            }

            public Splash(int vertexCount, float duration = 3.0f)
            {
                if (vertexCount < 3)
                    throw new System.ArgumentException("vertexCount must be at least 3");
                this._BaseVertexCount = vertexCount;
                _Vertices = new List<Vertex>(vertexCount);

                for (int i = 0; i < _BaseVertexCount; i++)
                    _Vertices.Add(new Vertex());

                this.Duration = duration;
                this.LeftX = -1;
                this.RightX = 1;
                this.Length = this.RightX - this.LeftX;
                _StepX = this.Length / (_Vertices.Count - 1);
            }

            public void Apply(float x, float lenght, float speed)
            {
                if (IsEnable)
                {
                    float extent = Mathf.Max(lenght * 0.5f, 0.1f);
                    float leftX = Mathf.Min(this.LeftX, x - extent);
                    float rightX = Mathf.Max(this.RightX, x + extent);

                    if (leftX < this.LeftX)
                        lenght = this.LeftX - leftX;
                    else if (rightX > this.RightX)
                        lenght = rightX - this.RightX;
                    else
                        lenght = 0;

                    int count = Mathf.FloorToInt(lenght / _StepX);
                    if (count > 0)
                    {
                        Vertex[] newVertices = new Vertex[count];
                        for (int i = 0; i < count; i++)
                            newVertices[i] = new Vertex();

                        if (leftX < this.LeftX)
                            _Vertices.InsertRange(0, newVertices);
                        else if (rightX > this.RightX)
                            _Vertices.InsertRange(_Vertices.Count, newVertices);

                    }

                    this.LeftX = leftX;
                    this.RightX = rightX;
                }
                else
                {
                    if (_Vertices.Count != _BaseVertexCount)
                    {
                        if (_Vertices.Count > _BaseVertexCount)
                        {
                            _Vertices.RemoveRange(_BaseVertexCount, _Vertices.Count - _BaseVertexCount);
                        }
                        else
                        {
                            int count = _Vertices.Count;
                            for (int i = count; i < _BaseVertexCount; i++)
                                _Vertices.Add(new Vertex());
                        }
                    }

                    float halfLenght = Mathf.Max(lenght * 0.5f, 0.1f);
                    this.LeftX = x - halfLenght;
                    this.RightX = x + halfLenght;
                }

                this.Length = this.RightX - this.LeftX;
                _StepX = this.Length / (_Vertices.Count - 1);

                int index1, index2;
                float factor;
                GetIndex(x, out index1, out index2, out factor);

                if (index1 == index2)
                {
                    _Vertices[index1].Speed += speed;
                }
                else
                {
                    _Vertices[index1].Speed += factor * speed;
                    _Vertices[index2].Speed += (1.0f - factor) * speed;
                }
                _UpdateTW.Begin(Duration);
            }

            private void GetIndex(float x, out int index1, out int index2, out float factor)
            {
                index1 = Mathf.Clamp(Mathf.FloorToInt(((x - this.LeftX) / this.Length) * (_Vertices.Count - 1)), 0, _Vertices.Count - 1);
                index2 = index1 + 1;
                if (index2 >= _Vertices.Count)
                    index2 = _Vertices.Count - 1;
                factor = (x - this.LeftX - (index1 * _StepX)) / _StepX;
            }


            public void Update(float dampening, float tension, float spread, int pass)
            {
                if (_UpdateTW.IsEnabledAndOver)
                    _UpdateTW.End();
                if (!IsEnable) return;
                for (int i = 0; i < _Vertices.Count; i++)
                    _Vertices[i].Update(dampening, tension);

                // do some passes where columns pull on their neighbours
                for (int j = 0; j < pass; j++)
                {
                    for (int i = 0; i < _Vertices.Count; i++)
                    {
                        if (i > 0)
                        {
                            _Vertices[i].LeftDelta = spread * (_Vertices[i].Height - _Vertices[i - 1].Height);
                            _Vertices[i - 1].Speed += _Vertices[i].LeftDelta;
                        }
                        if (i < _Vertices.Count - 1)
                        {
                            _Vertices[i].RightDelta = spread * (_Vertices[i].Height - _Vertices[i + 1].Height);
                            _Vertices[i + 1].Speed += _Vertices[i].RightDelta;
                        }
                    }

                    for (int i = 0; i < _Vertices.Count; i++)
                    {
                        if (i > 0)
                            _Vertices[i - 1].Height += _Vertices[i].LeftDelta;
                        if (i < _Vertices.Count - 1)
                            _Vertices[i + 1].Height += _Vertices[i].RightDelta;
                    }
                }
            }
        }
        #endregion
    }
}