using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimNodeBlend2D : AnimNodeSingleLayer
    {
        private bool _IsChanged;
        private Vertex[] _Vertices;
        private Triangle[] _Triangles;
        private List<Triangle> _InsideTriangles;

        private Vector2 _Value;
        /// <summary>
        /// Get or set value of actor
        /// </summary>
        public Vector2 Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    _IsChanged = true;
                }
            }
        }

        public float ValueX
        {
            get { return _Value.x; }
            set
            {
                if (_Value.x != value)
                {
                    _Value.x = value;
                    _IsChanged = true;
                }
            }
        }

        public float ValueY
        {
            get { return _Value.y; }
            set
            {
                if (_Value.y != value)
                {
                    _Value.y = value;
                    _IsChanged = true;
                }
            }
        }

        /// <summary> Thresholds </summary>
        public Vector2[] Thresholds { get; private set; }

        /// <summary>
        /// Create new instance of AnimNodeBlendBySpeed
        /// </summary>
        /// <param name="childCoun">number of children</param>
        public AnimNodeBlend2D(int childCoun)
            : base(childCoun)
        {
            _IsChanged = true;
            Thresholds = new Vector2[childCoun];
            _Vertices = new Vertex[childCoun];
            for (int i = 0; i < childCoun; i++)
                _Vertices[i] = new Vertex(i);

            int triangleCount = (_Vertices.Length - 2) * (_Vertices.Length - 2) * (_Vertices.Length - 2);
            _InsideTriangles = new List<Triangle>(triangleCount);

            if (_Vertices.Length >= 3) // find all possible triangles
            {
                for (int i = 0; i < _Vertices.Length - 2; i++)
                    for (int j = 1; j < _Vertices.Length - 1; j++)
                        for (int k = 2; k < _Vertices.Length; k++)
                            _InsideTriangles.Add(new Triangle(_Vertices[i], _Vertices[j], _Vertices[k]));                
            }            
            _Triangles = _InsideTriangles.ToArray();
            _InsideTriangles.Clear();
        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            if (_IsChanged)
            {
                _IsChanged = false;
                if (_Vertices.Length > 1)
                {
                    // update positions
                    for (int i = 0; i < ChildCount; i++)
                    {
                        _Vertices[i].Position = Thresholds[_Vertices[i].Index];
                        _Vertices[i].Weight = 0;
                    }

                    _InsideTriangles.Clear();
                    foreach (var t in _Triangles)
                    {
                        if (t.Contains(_Value))
                            _InsideTriangles.Add(t);
                    }

                    if (_InsideTriangles.Count > 0) // value is inside at least  one triangle
                    {
                        foreach (var t in _InsideTriangles)
                        {
                            t.Barycentric(_Value);
                            t.V1.Weight += t.Weight1;
                            t.V2.Weight += t.Weight2;
                            t.V3.Weight += t.Weight3;
                        }

                        if (_InsideTriangles.Count > 1)
                        {
                            for (int i = 0; i < _Vertices.Length; i++)
                                _Vertices[i].Weight /= _InsideTriangles.Count;
                        }
                    }
                    else // value is not inside of any triangle
                    {

                        for (int i = 0; i < ChildCount; i++)
                            _Vertices[i].Distance = Vector2.Distance(_Value, _Vertices[i].Position);

                        Utility.QuickSort(_Vertices, _Vertices[0]);
                        // find two nearest vertex

                        Vertex v1 = _Vertices[0];
                        Vertex v2 = _Vertices[1];

                        float sumDistance = v1.Distance + v2.Distance;
                        if (sumDistance > 0)
                        {
                            v1.Weight = 1.0f - (v1.Distance / sumDistance);
                            v2.Weight = 1.0f - (v2.Distance / sumDistance);
                        }
                    }

                    for (int i = 0; i < _Vertices.Length; i++)
                        blendWeights[_Vertices[i].Index].SetBoth(_Vertices[i].Weight);
                }
                else if (_Vertices.Length > 0)
                {
                    blendWeights[0].SetBoth(1);
                }
            }
        }        

        class Vertex : IComparer<Vertex>
        {
            public int Index { get; private set; }
            public Vector2 Position;
            public float Distance;
            public float Weight;

            public Vertex(int vertexIndex)
            {
                this.Index = vertexIndex;
            }

            public int Compare(Vertex x, Vertex y)
            {
                return x.Distance.CompareTo(y.Distance);
            }
        }

        class Triangle
        {
            public Vertex V1 { get; private set; }
            public Vertex V2 { get; private set; }
            public Vertex V3 { get; private set; }


            private float _Weight1;
            private float _Weight2;
            private float _Weight3;

            public float Weight1 { get { return _Weight1; } }
            public float Weight2 { get { return _Weight2; } }
            public float Weight3 { get { return _Weight3; } }

            public Triangle(Vertex v1, Vertex v2, Vertex v3)
            {
                this.V1 = v1;
                this.V2 = v2;
                this.V3 = v3;
            }

            public bool Contains(Vector2 point)
            {
                return MathHelper.IsPointInTriangle(point, V1.Position, V2.Position, V3.Position);
            }

            public void Barycentric(Vector2 point)
            {
                MathHelper.Barycentric(point, V1.Position, V2.Position, V3.Position, out _Weight1, out _Weight2, out _Weight3);
            }
        }
    }
}
