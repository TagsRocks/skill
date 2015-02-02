using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    public class Point3D : MonoBehaviour
    {
        public Color Color = Color.red;
        public float GizmoRadius = 0.1f;

        public Vector3 Position { get { return transform.position; } }

        void OnDrawGizmos()
        {
            Gizmos.color = this.Color;
            Gizmos.DrawSphere(Position, GizmoRadius);
        }
    }
}