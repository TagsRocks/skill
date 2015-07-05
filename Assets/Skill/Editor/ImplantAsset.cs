using System;
using UnityEngine;
using System.Collections.Generic;

namespace Skill.Editor
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "newImplantAsset", menuName = "Skill/Implant Asset", order = 48)]
    public class ImplantAsset : ScriptableObject
    {
        /// <summary>
        /// Array of Implant Objects
        /// </summary>
        public ImplantObject[] Objects;

        /// <summary>
        /// Default properties
        /// </summary>
        public ImplantObject DefaultObject = new ImplantObject()
       {
           Prefab = null,
           MinScalePercent = 0.8f,
           MaxScalePercent = 1.0f,
           Weight = 1.0f,
           Rotation = Skill.Editor.ImplantObjectRotation.SurfaceNormal,
       };
    }
}
