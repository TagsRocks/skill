using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    public class BatchMesh : MonoBehaviour
    {
        /// <summary> root of all meshfilter to merge </summary>
        public Transform[] Roots;
        /// <summary> unit length in world space. the space containing meshes seperate to unit cubes then mehses inside each square will be merged </summary>
        public float UnitLength = 30;
        /// <summary> Sperate by sharedMesh </summary>
        public bool SeprateByMesh = false;
        /// <summary> Sperate by material </summary>
        public bool SeprateByMaterial = true;
        /// <summary> Maximum number of polygons in each batch part </summary>
        public int MaxPolyCount = 30000;

        [HideInInspector]
        public GameObject[] ChildMeshes;
    }
}