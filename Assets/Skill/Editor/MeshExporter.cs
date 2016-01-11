using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System;


namespace Skill.Editor
{
    public class MeshExporter
    {
        private int _StartIndex;
        private void MeshToString(MeshFilter mf, StringBuilder buffer)
        {
            Transform t = mf.transform;
            Quaternion r = t.localRotation;

            int numVertices = 0;
            Mesh m = mf.sharedMesh;
            if (m == null)
                return;


            foreach (Vector3 vv in m.vertices)
            {
                Vector3 v = t.TransformPoint(vv);
                numVertices++;
                buffer.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            buffer.Append("\n");
            foreach (Vector3 nn in m.normals)
            {
                Vector3 v = r * nn;
                buffer.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
            }
            buffer.Append("\n");
            foreach (Vector3 v in m.uv)            
                buffer.Append(string.Format("vt {0} {1}\n", v.x, v.y));

            buffer.Append("\n");
            if (m.uv2 != null)
            {                
                foreach (Vector3 v in m.uv2)                
                    buffer.Append(string.Format("vt2 {0} {1}\n", v.x, v.y));
                buffer.Append("\n");
            }
            if (m.uv3 != null)
            {
                foreach (Vector3 v in m.uv3)
                    buffer.Append(string.Format("vt3 {0} {1}\n", v.x, v.y));
                buffer.Append("\n");
            }
            if (m.uv4 != null)
            {
                foreach (Vector3 v in m.uv4)
                    buffer.Append(string.Format("vt4 {0} {1}\n", v.x, v.y));
                buffer.Append("\n");
            }

            Material[] materials = new Material[0];
            Renderer renderer = mf.GetComponent<Renderer>();
            if (renderer != null)
                materials = renderer.sharedMaterials;
            for (int material = 0; material < m.subMeshCount; material++)
            {
                buffer.Append("\n");
                buffer.Append("usemtl ").Append(materials[material].name).Append("\n");
                buffer.Append("usemap ").Append(materials[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    buffer.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + _StartIndex, triangles[i + 1] + 1 + _StartIndex, triangles[i + 2] + 1 + _StartIndex));
                }
            }

            _StartIndex += numVertices;
        }
        public void ExportObject(string fileName, params MeshFilter[] meshes)
        {
            _StartIndex = 0;
            StringBuilder meshString = new StringBuilder();
            foreach (var mesh in meshes)
            {

                meshString.Append("g ").Append(mesh.name).Append("\n");
                MeshToString(mesh, meshString);
            }
            if (File.Exists(fileName)) File.Delete(fileName);
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(meshString);
            }
        }
    }
}