using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Editor.Tools
{
    /// <summary>
    /// Defines type of rotation algorithm for implant objects
    /// </summary>
    public enum ImplantObjectRotation
    {
        /// <summary> Objects align with surface normal </summary>
        SurfaceNormal = 0,
        /// <summary> Custom rotation </summary>
        Custom = 1,
        /// <summary> Rotation for specified x,y,z components calculated by generating random number between 0 - 360 </summary>
        Random = 2,
    }

    /// <summary>
    /// Information about an ImplantObject to use in Implant tool
    /// </summary>
    [System.Serializable]
    public class ImplantObject
    {
        /// <summary> Prefab to implent </summary>
        public GameObject Prefab;
        /// <summary> Minimum scale percent of original scale </summary>
        public float MinScalePercent;
        /// <summary> Maximum scale percent of original scale </summary>
        public float MaxScalePercent;
        /// <summary> Chance of this object to implant </summary>
        public float Weight;

        // ******** Rotation *********

        /// <summary> Rotation of implanted object </summary>
        public ImplantObjectRotation Rotation;
        /// <summary> if Rotation is Random, whether x value of rotation is random between 0-360 or same as Prefab  </summary>
        public bool RandomX;
        /// <summary> if Rotation is Random, whether y value of rotation is random between 0-360 or same as Prefab  </summary>
        public bool RandomY;
        /// <summary> if Rotation is Random, whether z value of rotation is random between 0-360 or same as Prefab  </summary>
        public bool RandomZ;

        /// <summary> if Rotation is Custom, Custom rotation value for implanted object</summary>
        public Vector3 CustomRotation;

        /// <summary> if Rotation is SurfaceNormal whether yaw value of rotation is random between 0-360 or same as Prefab </summary>
        public bool RandomYaw;
    }


}
