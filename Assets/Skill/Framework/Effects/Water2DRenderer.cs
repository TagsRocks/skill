using UnityEngine;
using System.Collections;

namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Water2DRenderer : DynamicBehaviour
    {
        public Water2D Water;
        public int VertexCount = 160;
        public float Width = 20;
        public float Height = 1.0f;
        public float H1 = 0.9f;
        public float Noise = 0.01f;
        public float V0 = 0.06f;
        public float V1 = 0.1f;
        public float Tiling = 1.0f;
        public float USpeed = 0.0f;


        private MeshFilter _MeshFilter;
        private Mesh _Mesh;
        private Renderer _Renderer;
        private Vector3[] _Vertices;
        private Vector2[] _Uvs;
        private ParticleSystem[] _Particles;
        private float _UOffset;
        private int _SavedVertexCount;

        [ContextMenu("Rebuild")]
        public void RebuildMesh()
        {
            if (VertexCount < 3)
                VertexCount = 3;

            _SavedVertexCount = VertexCount;
            _MeshFilter = gameObject.GetComponent<MeshFilter>();
            if (_MeshFilter == null)
                _MeshFilter = gameObject.AddComponent<MeshFilter>();

            _Mesh = _MeshFilter.sharedMesh;

            if (_Mesh != null)
                _Mesh.Clear();
            else
                _Mesh = new Mesh();

            _Vertices = new Vector3[_SavedVertexCount * 3];
            _Uvs = new Vector2[_SavedVertexCount * 3];

            float xStep = Width / (_SavedVertexCount - 1);
            float uStep = (Tiling) / (_SavedVertexCount - 1);
            float x = Width * -0.5f;
            float u = 0;
            int vIndex = 0;
            for (int i = 0; i < _SavedVertexCount; i++)
            {
                _Vertices[vIndex].y = Height;
                _Vertices[vIndex + 1].y = H1;
                _Vertices[vIndex + 2].y = 0;
                _Vertices[vIndex].x = _Vertices[vIndex + 1].x = _Vertices[vIndex + 2].x = x;
                x += xStep;


                _Uvs[vIndex].y = 1.0f - V0;
                _Uvs[vIndex + 1].y = 1.0f - V1;
                _Uvs[vIndex + 2].y = 0.0f;
                _Uvs[vIndex].x = _Uvs[vIndex + 1].x = _Uvs[vIndex + 2].x = u;
                u += uStep;
                vIndex += 3;
            }

            int faceCount = (_SavedVertexCount - 1) * 4;
            int[] triangles = new int[faceCount * 3];

            int index = 0;
            for (int i = 0; i < _SavedVertexCount * 3 - 3; i += 3)
            {
                triangles[index++] = i;
                triangles[index++] = i + 3;
                triangles[index++] = i + 1;

                triangles[index++] = i + 1;
                triangles[index++] = i + 3;
                triangles[index++] = i + 4;

                triangles[index++] = i + 1;
                triangles[index++] = i + 4;
                triangles[index++] = i + 2;

                triangles[index++] = i + 2;
                triangles[index++] = i + 4;
                triangles[index++] = i + 5;

            }

            _Mesh.vertices = _Vertices;
            _Mesh.uv = _Uvs;
            _Mesh.triangles = triangles;
            _Mesh.RecalculateBounds();

            _MeshFilter.mesh = _Mesh;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            _Particles = GetComponentsInChildren<ParticleSystem>();
            _MeshFilter = GetComponent<MeshFilter>();
            _Renderer = GetComponent<Renderer>(); 
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();            
            RebuildMesh();
            EnabledParticles(false);
        }


        private void EnabledParticles(bool enable)
        {
            for (int i = 0; i < _Particles.Length; i++)
            {
                var emission = _Particles[i].emission;
                emission.enabled = enable;                
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_Mesh != null && Water != null && _Renderer.isVisible)
            {
                float posX = transform.position.x;
                float nNoise = -Noise;
                float u = _UOffset;
                _UOffset += Time.deltaTime * USpeed;
                float uStep = (Tiling) / (_SavedVertexCount - 1);
                for (int i = 0; i < _SavedVertexCount * 3; i += 3)
                {
                    float noise = 0;
                    if (this.Noise != 0)
                        noise = Random.Range(nNoise, Noise);

                    float waterH = Water.GetHeight(_Vertices[i].x + posX);
                    _Vertices[i].y = Height + waterH + noise;
                    _Vertices[i + 1].y = H1 + waterH;
                    _Vertices[i + 2].y = 0;

                    _Uvs[i].y = 1.0f - V0;
                    _Uvs[i + 1].y = 1.0f - V1;
                    _Uvs[i + 2].y = 0.0f;

                    _Uvs[i].x = u;
                    _Uvs[i + 1].x = u;
                    _Uvs[i + 2].x = u;

                    u += uStep;
                }
                _Mesh.vertices = _Vertices;
                _Mesh.uv = _Uvs;
            }
            base.Update();
        }

        protected virtual void OnBecameVisible()
        {
            enabled = true;
            EnabledParticles(true);
        }
        protected virtual void OnBecameInvisible()
        {
            enabled = false;
            EnabledParticles(false);
        }


    }
}
