using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Contains data needed to spawn an object
    /// </summary>
    [System.Serializable]
    public class SpawnObject
    {
        /// <summary> GameObject to spawn </summary>
        public GameObject Prefab;

        /// <summary> Chance to spawn </summary>
        public float Weight = 1.0f;
    }
}
