using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Controllers
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
        public float Chance = 1.0f;
    }
}
