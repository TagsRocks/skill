using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Editor.Tools
{
    [System.Serializable]
    public class ImplantObject
    {
        public GameObject Prefab;
        public float MinScalePercent;
        public float MaxScalePercent;
        public float Chance;
    }
}
