using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    public class BatchMesh : MonoBehaviour
    {
        public enum SortAxis
        {
            None,
            X,
            Y,
            Z,
            InverseX,
            InverseY,
            InverseZ,
        }

        /// <summary> root of all meshfilter to merge </summary>
        public Transform[] Roots;
        /// <summary> unit length in world space. the space containing meshes seperate to unit cubes then mehses inside each square will be merged </summary>
        public Vector3 UnitLength = new Vector3(30, 30, 30);
        /// <summary> Sperate by sharedMesh </summary>
        public bool SeprateByMesh = false;
        /// <summary> Sperate by material </summary>
        public bool SeprateByMaterial = true;
        /// <summary> Maximum number of polygons in each batch part </summary>
        public int MaxPolyCount = 30000;
        /// <summary> sort objects in vertexbuffer by axis </summary>
        public SortAxis Sort = SortAxis.None;

        [HideInInspector]
        public GameObject[] ChildMeshes;
    }
}