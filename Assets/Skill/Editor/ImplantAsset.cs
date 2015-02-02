using System;
using UnityEngine;
using System.Collections.Generic;

namespace Skill.Editor
{
    /// <summary>
    /// Defines serializable data asset required for Implant tool
    /// </summary>
    [System.Serializable]
    public class ImplantAsset : ScriptableObject
    {        
        /// <summary>
        /// Array of Implant Objects
        /// </summary>
        public ImplantObject[] Objects;

        /// <summary>
        /// Default properties
        /// </summary>
        public ImplantObject DefaultObject;
    }
}
