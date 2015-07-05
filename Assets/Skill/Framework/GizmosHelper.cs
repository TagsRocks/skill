using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    public static class GizmosHelper
    {
        /// <summary>
        /// Draw a cube between two points
        /// </summary>
        /// <param name="pos1">Point 1</param>
        /// <param name="pos2">Point 2</param>
        /// <param name="width">Width of cube</param>
        /// <param name="height">Height of cube</param>
        public static void DrawCubeBetween(Vector3 pos1, Vector3 pos2, float width, float height)
        {
            Matrix4x4 preMatrix = Gizmos.matrix;// save previous matrix
            Vector3 dir = pos2 - pos1;
            Gizmos.matrix = Matrix4x4.TRS((pos1 + pos2) * 0.5f, Quaternion.LookRotation(dir.normalized), Vector3.one);
            Gizmos.DrawCube(Vector3.zero, new Vector3(width, height, dir.magnitude));
            Gizmos.matrix = preMatrix; // apply previous matrix        
        }


        /// <summary>
        /// Draw an arrow with lines
        /// </summary>
        /// <param name="from">Arrow start</param>
        /// <param name="to">Arrow end</param>
        /// <param name="capSize">Cap size</param>
        /// <param name="capResolution">Number of lines to draw cap</param>
        public static void DrawArrow(Vector3 from, Vector3 to, float capSize, int capResolution = 10)
        {
            Gizmos.DrawLine(from, to);
            Vector3 dir = (to - from).normalized;
            Vector3 capPos = to - dir * capSize;

            capResolution = Mathf.Max(4, capResolution);
            float angle = -45;
            float deltaAngle = 360 / capResolution;
            Vector3 up = Vector3.up * capSize * 0.4f;
            Quaternion rotation = Quaternion.LookRotation(dir);
            for (int i = 0; i < capResolution; i++)
            {
                Vector3 pos = capPos + (rotation * Quaternion.Euler(0, 0, angle) * up);
                Gizmos.DrawLine(pos, to);
                angle += deltaAngle;

            }

        }

        /// <summary>
        /// Collider gizmo draw mode
        /// </summary>
        public enum DrawColliderMode
        {
            /// <summary> None </summary>
            None = 0,
            /// <summary> Solid </summary>
            Solid = 1,
            /// <summary> Wireframe </summary>
            Wire = 2,
            /// <summary> Solid and Wireframe </summary>
            SolidAndWire = 3
        }

        /// <summary>
        /// Draw collider gizmo
        /// </summary>
        /// <param name="collider">Collider</param>
        /// <param name="color">Color</param>
        /// <param name="drawMode">Draw mode</param>
        public static void DrawCollider(Collider collider, Color color, DrawColliderMode drawMode)
        {
            if (collider == null) return;
            if (drawMode == DrawColliderMode.None) return;
            if (collider is BoxCollider)
                DrawBoxCollider((BoxCollider)collider, color, drawMode);
            else if (collider is SphereCollider)
                DrawSphereCollider((SphereCollider)collider, color, drawMode);
            //else if (collider is CapsuleCollider)
            //    DrawCapsuleCollider((CapsuleCollider)collider, color, drawMode);
        }


        public static void DrawBox(Transform transform, Vector3 size, Vector3 center, Color color, DrawColliderMode drawMode)
        {
            Gizmos.color = color;
            Matrix4x4 savedMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            if ((drawMode & DrawColliderMode.Solid) == DrawColliderMode.Solid)
                Gizmos.DrawCube(center, size);
            if ((drawMode & DrawColliderMode.Wire) == DrawColliderMode.Wire)
            {
                color.a = 1.0f;
                Gizmos.DrawWireCube(center, size);
            }
            Gizmos.matrix = savedMatrix;
        }

        private static void DrawBoxCollider(BoxCollider collider, Color color, DrawColliderMode drawMode)
        {
            DrawBox(collider.transform, collider.size, collider.center, color, drawMode);
        }

        private static void DrawSphereCollider(SphereCollider collider, Color color, DrawColliderMode drawMode)
        {
            DrawSphere(collider.transform, collider.radius, collider.center, color, drawMode);
        }

        public static void DrawSphere(Transform transform, float radius, Vector3 center, Color color, DrawColliderMode drawMode)
        {
            Gizmos.color = color;
            Matrix4x4 savedMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            if ((drawMode & DrawColliderMode.Solid) == DrawColliderMode.Solid)
                Gizmos.DrawSphere(center, radius);
            if ((drawMode & DrawColliderMode.Wire) == DrawColliderMode.Wire)
            {
                color.a = 1.0f;
                Gizmos.DrawWireSphere(center, radius);
            }
            Gizmos.matrix = savedMatrix;
        }

        //private static void DrawCapsuleCollider(CapsuleCollider collider, Color color, DrawColliderMode drawMode)
        //{
        //    float radius = collider.radius;
        //    Gizmos.color = color;
        //    Matrix4x4 savedMatrix = Gizmos.matrix;

        //    if ((drawMode & DrawColliderMode.Solid) == DrawColliderMode.Solid)
        //    {
        //        Gizmos.matrix = collider.transform.localToWorldMatrix;
        //        Gizmos.DrawSphere(collider.center + new Vector3(0, collider.height * 0.5f - radius, 0), radius);
        //        Gizmos.DrawSphere(collider.center + new Vector3(0, collider.height * -0.5f + radius, 0), radius);

        //        Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, collider.height - radius, 1));
        //        Gizmos.DrawSphere(collider.center, radius);
        //        //Gizmos.DrawCube(collider.center, new Vector3(radius * 2, collider.height - radius, radius * 2));
        //    }
        //    if ((drawMode & DrawColliderMode.Wire) == DrawColliderMode.Wire)
        //    {
        //        Gizmos.matrix = collider.transform.localToWorldMatrix;
        //        color.a = 1.0f;
        //        Gizmos.DrawWireSphere(collider.center + new Vector3(0, collider.height * 0.5f - radius, 0), radius);
        //        Gizmos.DrawWireSphere(collider.center + new Vector3(0, collider.height * -0.5f + radius, 0), radius);
        //        Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, collider.height - radius, 1));
        //        Gizmos.DrawWireSphere(collider.center, radius);
        //        //Gizmos.DrawWireCube(collider.center, new Vector3(radius * 2, collider.height - radius, radius * 2));
        //    }
        //    Gizmos.matrix = savedMatrix;
        //}
    }

}
