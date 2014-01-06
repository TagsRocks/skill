using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    public class Meter : MonoBehaviour
    {
        public string MeterName = "Meter";
        public Transform LockStartPoint;
        public Transform LockEndPoint;
        public Color Color = Color.yellow;
        public float GizmoRadius = 0.1f;
        public bool ScaleToPixels = false;
        public int PixelPerUnit = 128;

        [HideInInspector]
        public Vector3 FreeStartPoint;
        [HideInInspector]
        public Vector3 FreeEndPoint;

        /// <summary> Retrieves distance between StartPosition and EndPosition.</summary>
        public float Distance { get { return Vector3.Distance(StartPosition, EndPosition); } }
        /// <summary> Gets or set StartPosition.</summary>
        public Vector3 StartPosition
        {
            get
            {
                if (LockStartPoint != null)
                    return LockStartPoint.position;
                else
                    return FreeStartPoint;
            }
            set
            {
                if (LockStartPoint != null)
                    LockStartPoint.position = value;
                else
                    FreeStartPoint = value;
            }
        }
        /// <summary> Gets or set EndPosition.</summary>
        public Vector3 EndPosition
        {
            get
            {
                if (LockEndPoint != null)
                    return LockEndPoint.position;
                else
                    return FreeEndPoint;
            }
            set
            {
                if (LockEndPoint != null)
                    LockEndPoint.position = value;
                else
                    FreeEndPoint = value;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = this.Color;
            Gizmos.DrawWireSphere(StartPosition, GizmoRadius);
            Gizmos.DrawWireSphere(EndPosition, GizmoRadius);
            Gizmos.DrawLine(StartPosition, EndPosition);
        }
    }
}