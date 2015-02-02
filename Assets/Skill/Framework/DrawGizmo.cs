using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Helper class to draw gizmo
    /// </summary>
    public class DrawGizmo : MonoBehaviour
    {
        /// <summary> Color of gizmo </summary>
        public Color Color = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        /// <summary> Draw mode of collider</summary>
        public GizmosHelper.DrawColliderMode DrawMode = GizmosHelper.DrawColliderMode.SolidAndWire;
        /// <summary> Gizmo filename </summary>
        public string Filename = string.Empty;
        /// <summary> Gizmo icon offset </summary>
        public Vector3 Offset = Vector3.zero;


        protected virtual void OnDrawGizmos()
        {
            if (!string.IsNullOrEmpty(Filename))
                Gizmos.DrawIcon(transform.position + transform.TransformDirection(Offset), Filename, false);
            if (collider != null && DrawMode != GizmosHelper.DrawColliderMode.None)
                GizmosHelper.DrawCollider(collider, Color, DrawMode);
        }
    }
}
