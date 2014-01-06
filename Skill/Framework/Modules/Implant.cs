using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Defines implant data
    /// </summary>    
    public class Implant : MonoBehaviour
    {
        public ScriptableObject ImplantAsset = null;
        public int LayerMask = 1; // default layer    
        public float MinRadius = 0.0f;
        public float MaxRadius = 1.0f;
        public int Density = 1;
        public float RectWidth = 1;
        public float RectHeight = 1;
        public float Rotation = 0;
        public int BrushMode = 0;
        public float OffsetY = 0;
        public Vector3[] Points;
    }
}
