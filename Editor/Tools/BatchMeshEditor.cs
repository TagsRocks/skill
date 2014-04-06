using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Framework.Modules;

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
            if (_Data.UnitLength.x < 1.0f || _Data.UnitLength.y < 1.0f || _Data.UnitLength.z < 1.0f)
            {
                Debug.LogError("UnitLength is too small, please define larger UnitLength");
                return;
            }

            List<MeshFilter> filters = new List<MeshFilter>(10000);
            foreach (var root in _Data.Roots)
            {
                MeshFilter[] fs = root.GetComponentsInChildren<MeshFilter>();// find all mesh fileters in root
                for (int i = 0; i < fs.Length; i++)
                    filters.Add(fs[i]);
            }


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
            int unitX = (int)((max.x - min.x) / _Data.UnitLength.x) + 1;
            int unitY = (int)((max.y - min.y) / _Data.UnitLength.y) + 1;
            int unitZ = (int)((max.z - min.z) / _Data.UnitLength.z) + 1;

            MeshGroup[, ,] units = new MeshGroup[unitX, unitY, unitZ];
            for (int ix = 0; ix < unitX; ix++)
                for (int iy = 0; iy < unitY; iy++)
                    for (int iz = 0; iz < unitZ; iz++)
                        units[ix, iy, iz] = new MeshGroup();

            //place meshes in units
            foreach (MeshFilter mf in filters)
            {
                Vector3 pos = mf.transform.position;
                int ix = (int)((pos.x - min.x) / _Data.UnitLength.x);
                int iy = (int)((pos.y - min.y) / _Data.UnitLength.y);
                int iz = (int)((pos.z - min.z) / _Data.UnitLength.z);
                units[ix, iy, iz].Meshes.Add(mf);
            }

            List<GameObject> meshes = new List<GameObject>();
            if (_Data.ChildMeshes != null)
                meshes.AddRange(_Data.ChildMeshes);


            foreach (MeshGroup wu in units)
            {
                if (wu.Meshes.Count > 0)
                    GenerateGroups(wu.Meshes, ref meshes);
            }


            _Data.ChildMeshes = meshes.ToArray();
            EditorUtility.SetDirty(_Data);
        }

        private void GenerateGroups(List<MeshFilter> filters, ref List<GameObject> meshes)
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
                    GenerateMesh(g.Meshes, ref meshes);
            }
            else
            {
                GenerateMesh(filters, ref meshes);
            }
        }

        private void GenerateMesh(List<MeshFilter> filters, ref List<GameObject> meshes)
        {
            int maxSubmesh = 1;
            if (_Data.SeprateBySubmesh)
            {
                foreach (MeshFilter mf in filters)
                    maxSubmesh = Mathf.Max(maxSubmesh, mf.sharedMesh.subMeshCount);
            }

            Material[] materials = new Material[maxSubmesh];
            List<CombineInstance>[] instances = new List<CombineInstance>[maxSubmesh];
            for (int i = 0; i < maxSubmesh; i++)
            {
                instances[i] = new List<CombineInstance>();
                foreach (MeshFilter mf in filters)
                {
                    MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        if (renderer.sharedMaterials.Length > i)
                        {
                            materials[i] = renderer.sharedMaterials[i];
                            break;
                        }
                    }
                }
            }

            foreach (MeshFilter mf in filters)
            {
                for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.subMeshIndex = i;
                    ci.mesh = mf.sharedMesh;
                    ci.transform = mf.transform.localToWorldMatrix;
                    instances[Mathf.Min(i, maxSubmesh - 1)].Add(ci);
                }
            }

            System.Type[] compoentns = new System.Type[] { typeof(MeshRenderer), typeof(MeshFilter) };
            for (int i = 0; i < instances.Length; i++)
            {
                Mesh mesh = new Mesh();
                mesh.CombineMeshes(instances[i].ToArray(), true, true);

                GameObject obj = EditorUtility.CreateGameObjectWithHideFlags(string.Format("Mesh{0}", meshes.Count), HideFlags.None, compoentns);
                obj.GetComponent<MeshFilter>().mesh = mesh;
                obj.transform.parent = _Data.transform;
                obj.GetComponent<MeshRenderer>().materials = materials;

                meshes.Add(obj);

            }
        }
    }
}
