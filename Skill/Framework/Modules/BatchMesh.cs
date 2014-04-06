using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    public class BatchMesh : MonoBehaviour
    {
        /// <summary> root of all meshfilter to merge </summary>
        public Transform[] Roots;
        /// <summary> unit length in world space. the space containing meshes seperate to unit squares then mehses inside each square will be merged </summary>
        public Vector3 UnitLength = new Vector3(100, 100, 100);
        /// <summary> Sperate by sharedMesh </summary>
        public bool SeprateByMesh = false;
        /// <summary> Sperate by submesh </summary>
        public bool SeprateBySubmesh = false;

        [HideInInspector]
        public GameObject[] ChildMeshes;
    }
}