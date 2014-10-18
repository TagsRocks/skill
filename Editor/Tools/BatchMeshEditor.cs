using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Modules;
using Skill.Framework.UI;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(BatchMesh))]
    public class BatchMeshEditor : UnityEditor.Editor
    {

        #region UI
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.StackPanel _Panel;
        private Button _BtnGenerate;
        private Button _BtnDestroy;

        private void CreateUI()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Panel = new Skill.Framework.UI.StackPanel();
            _BtnGenerate = new Button() { Height = 20, Margin = new Skill.Framework.UI.Thickness(0, 0, 0, 4) }; _BtnGenerate.Content.text = "Generate";
            _BtnDestroy = new Button() { Height = 20, Margin = new Skill.Framework.UI.Thickness(0, 0, 0, 4) }; _BtnDestroy.Content.text = "Clear";

            _Panel.Controls.Add(_BtnGenerate);
            _Panel.Controls.Add(_BtnDestroy);
            _Frame.Grid.Controls.Add(_Panel);

            _BtnGenerate.Click += _BtnGenerate_Click;
            _BtnDestroy.Click += _BtnDestroy_Click;
        }

        void _BtnDestroy_Click(object sender, System.EventArgs e)
        {
            DestroyMeshes();
        }

        void _BtnGenerate_Click(object sender, System.EventArgs e)
        {
            Generate();
        }
        #endregion

        private BatchMesh _Data;
        private List<GameObject> _Meshes;
        private System.Type[] _MeshCompoentns;

        void OnEnable()
        {
            _Data = target as BatchMesh;
            CreateUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.OnInspectorGUI(42);
        }

        private void DestroyMeshes()
        {
            if (_Data.ChildMeshes != null)
            {
                for (int i = 0; i < _Data.ChildMeshes.Length; i++)
                {
                    GameObject obj = _Data.ChildMeshes[i];
                    if (obj != null)
                    {
                        obj.transform.parent = null;
                        DestroyImmediate(obj);
                    }
                }
            }
            _Data.ChildMeshes = null;
        }


        class MeshGroup
        {
            public Mesh SharedMesh { get; private set; }
            public List<MeshFilter> Meshes { get; private set; }
            public MeshGroup(Mesh sharedMesh = null)
            {
                this.SharedMesh = sharedMesh;
                this.Meshes = new List<MeshFilter>();
            }
        }


        private void CheckMinMax(ref Vector3 min, ref Vector3 max, Vector3 pos)
        {
            min.x = Mathf.Min(min.x, pos.x);
            min.y = Mathf.Min(min.y, pos.y);
            min.z = Mathf.Min(min.z, pos.z);
            max.x = Mathf.Max(max.x, pos.x);
            max.y = Mathf.Max(max.y, pos.y);
            max.z = Mathf.Max(max.z, pos.z);
        }
        private void Generate()
        {
            if (_Data.Roots == null || _Data.Roots.Length == 0)
            {
                Debug.LogError("Root is null");
                return;
            }
            if (_Data.UnitLength < 0.2f)
            {
                Debug.LogError("UnitLength is too small, please define larger UnitLength");
                return;
            }

            _Data.MaxPolyCount = Mathf.Max(_Data.MaxPolyCount, 100);


            List<MeshFilter> filters = new List<MeshFilter>(10000);
            foreach (var root in _Data.Roots)
            {
                MeshFilter[] fs = root.GetComponentsInChildren<MeshFilter>();// find all mesh fileters in root
                for (int i = 0; i < fs.Length; i++)
                    filters.Add(fs[i]);
            }

            if (filters.Count == 0) return;

            // find a rect that contains all tese meshes
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (MeshFilter mf in filters)
            {
                Vector3 boundMin = mf.transform.TransformPoint(mf.sharedMesh.bounds.min);
                Vector3 boundMax = mf.transform.TransformPoint(mf.sharedMesh.bounds.max);

                CheckMinMax(ref min, ref max, boundMin);
                CheckMinMax(ref min, ref max, boundMax);
            }

            //seprate world to squares
            int unitX = (int)((max.x - min.x) / _Data.UnitLength) + 1;
            int unitY = (int)((max.y - min.y) / _Data.UnitLength) + 1;
            int unitZ = (int)((max.z - min.z) / _Data.UnitLength) + 1;

            MeshGroup[, ,] units = new MeshGroup[unitX, unitY, unitZ];
            for (int ix = 0; ix < unitX; ix++)
                for (int iy = 0; iy < unitY; iy++)
                    for (int iz = 0; iz < unitZ; iz++)
                        units[ix, iy, iz] = new MeshGroup();

            //place meshes in units
            foreach (MeshFilter mf in filters)
            {
                Vector3 pos = mf.transform.position;
                int ix = Mathf.FloorToInt((pos.x - min.x) / _Data.UnitLength);
                int iy = Mathf.FloorToInt((pos.y - min.y) / _Data.UnitLength);
                int iz = Mathf.FloorToInt((pos.z - min.z) / _Data.UnitLength);
                units[ix, iy, iz].Meshes.Add(mf);
            }

            if (_Meshes == null) _Meshes = new List<GameObject>();
            _Meshes.Clear();
            if (_Data.ChildMeshes != null)
                _Meshes.AddRange(_Data.ChildMeshes);


            foreach (MeshGroup wu in units)
            {
                if (wu.Meshes.Count > 0)
                    GenerateGroups(wu.Meshes);
            }


            _Data.ChildMeshes = _Meshes.ToArray();
            EditorUtility.SetDirty(_Data);
        }

        private void GenerateGroups(List<MeshFilter> filters)
        {
            if (_Data.SeprateByMesh)
            {
                List<MeshGroup> groups = new List<MeshGroup>();
                // group meshes based on sharedMesh
                foreach (MeshFilter mf in filters)
                {
                    MeshGroup group = null;
                    foreach (var g in groups)
                    {
                        if (g.SharedMesh == mf.sharedMesh)
                        {
                            group = g;
                            break;
                        }
                    }
                    if (group == null)
                    {
                        group = new MeshGroup(mf.sharedMesh);
                        groups.Add(group);
                    }
                    group.Meshes.Add(mf);
                }

                foreach (MeshGroup g in groups)
                    GenerateMesh(g.Meshes);
            }
            else
            {
                GenerateMesh(filters);
            }
        }




        class MeshData
        {
            public Mesh Mesh;
            public int SubMeshIndex;
            public Matrix4x4 Transform;
            public Vector3 Min;
            public Vector3 Max;
            public Vector3 Center;
            public int PolyCount;
            public int VertexCount;

            public CombineInstance CI
            {
                get
                {
                    CombineInstance ci = new CombineInstance();
                    ci.subMeshIndex = SubMeshIndex;
                    ci.mesh = Mesh;
                    ci.transform = Transform;
                    return ci;
                }
            }
        }
        class MeshMaterial
        {

            private List<MeshData> _Meshes;
            public Material Material { get; private set; }
            public int Count { get { return _Meshes.Count; } }
            public MeshData this[int index] { get { return _Meshes[index]; } }



            public void Add(MeshFilter mf, int subMeshIndex)
            {
                MeshData md = new MeshData();
                md.Mesh = mf.sharedMesh;
                md.SubMeshIndex = subMeshIndex;
                md.Transform = mf.transform.localToWorldMatrix;
                md.PolyCount = mf.sharedMesh.GetIndices(subMeshIndex).Length / 3;
                md.VertexCount = mf.sharedMesh.vertexCount;
                md.Min = mf.transform.TransformPoint(mf.sharedMesh.bounds.min);
                md.Max = mf.transform.TransformPoint(mf.sharedMesh.bounds.max);
                md.Center = (md.Min + md.Max) * 0.5f;

                _Meshes.Add(md);
            }

            public MeshMaterial(Material material)
            {
                this.Material = material;
                this._Meshes = new List<MeshData>();
            }
        }

        private MeshMaterial FindMeshMaterial(List<MeshMaterial> meshMaterials, Material mat)
        {
            foreach (var mm in meshMaterials)
                if (mm.Material == mat)
                    return mm;

            return null;
        }

        private void GenerateMesh(List<MeshFilter> filters)
        {
            List<MeshMaterial> meshMaterials = new List<MeshMaterial>();
            if (_Data.SeprateByMaterial)
            {
                foreach (MeshFilter mf in filters)
                {
                    MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.sharedMaterials != null)
                    {
                        for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
                        {
                            if (renderer.sharedMaterials.Length > i)
                            {
                                MeshMaterial mm = FindMeshMaterial(meshMaterials, renderer.sharedMaterials[i]);
                                if (mm == null)
                                {
                                    mm = new MeshMaterial(renderer.sharedMaterials[i]);
                                    meshMaterials.Add(mm);
                                }
                                mm.Add(mf, i);
                            }
                        }
                    }
                }
            }
            else
            {
                // just find a material
                Material mat = null;
                foreach (MeshFilter mf in filters)
                {
                    MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        if (renderer.sharedMaterials.Length > 0)
                        {
                            mat = renderer.sharedMaterials[0];
                            break;
                        }
                    }
                }

                MeshMaterial mm = new MeshMaterial(mat);
                foreach (MeshFilter mf in filters) // combine all submesh
                {
                    for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
                        mm.Add(mf, i);
                }
                meshMaterials.Add(mm);
            }

            for (int i = 0; i < meshMaterials.Count; i++)
                CreateMesh(meshMaterials[i]);
        }


        private void CreateMesh(MeshMaterial mm)
        {
            if (mm.Count < 1) return;

            List<CombineInstance> instances = new List<CombineInstance>();
            int polyCount = 0;
            int vertexCount = 0;
            for (int i = 0; i < mm.Count; i++)
            {
                if (instances.Count > 0 && (((polyCount + mm[i].PolyCount) > _Data.MaxPolyCount) || ((vertexCount + mm[i].VertexCount) >= ushort.MaxValue)))
                {
                    CreateMesh(instances.ToArray(), mm.Material);
                    instances.Clear();
                    polyCount = 0;
                    vertexCount = 0;
                }

                instances.Add(mm[i].CI);
                polyCount += mm[i].PolyCount;
                vertexCount += mm[i].VertexCount;
            }

            if (instances.Count > 0)
            {
                CreateMesh(instances.ToArray(), mm.Material);
                instances.Clear();
                polyCount = 0;
            }
        }

        //private void SortBasedOnMinimum(List<MeshData> meshes)
        //{
        //    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        //    foreach (var item in meshes)
        //    {
        //        min.x = Mathf.Min(min.x, item.Min.x);
        //        min.y = Mathf.Min(min.y, item.Min.y);
        //        min.z = Mathf.Min(min.z, item.Min.z);
        //    }

        //    foreach (var item in meshes)
        //        item.Distance = Vector3.Distance(item.Center, min);

        //    if (meshes.Count > 1)
        //        meshes.Sort((IComparer<MeshData>)meshes[0]);
        //}

        private void CreateMesh(CombineInstance[] cis, Material mat)
        {
            if (_MeshCompoentns == null)
                _MeshCompoentns = new System.Type[] { typeof(MeshRenderer), typeof(MeshFilter) };

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(cis, true, true);

            GameObject obj = EditorUtility.CreateGameObjectWithHideFlags(string.Format("Mesh{0}", _Meshes.Count), HideFlags.None, _MeshCompoentns);
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.transform.parent = _Data.transform;
            obj.GetComponent<MeshRenderer>().materials = new Material[] { mat };

            _Meshes.Add(obj);
        }
    }
}
