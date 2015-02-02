using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public static class HandleHelper
    {
        public static void DrawWireCube(Vector3 position, Vector3 size)
        {
            Vector3 half = size * 0.5f;

            Vector3 v1 = position + new Vector3(half.x, half.y, half.z);
            Vector3 v2 = position + new Vector3(half.x, half.y, -half.z);
            Vector3 v3 = position + new Vector3(half.x, -half.y, half.z);
            Vector3 v4 = position + new Vector3(half.x, -half.y, -half.z);
            Vector3 v5 = position + new Vector3(-half.x, half.y, half.z);
            Vector3 v6 = position + new Vector3(-half.x, half.y, -half.z);
            Vector3 v7 = position + new Vector3(-half.x, -half.y, half.z);
            Vector3 v8 = position + new Vector3(-half.x, -half.y, -half.z);

            // draw front
            Handles.DrawLine(v7, v3);
            Handles.DrawLine(v7, v5);
            Handles.DrawLine(v1, v3);
            Handles.DrawLine(v1, v5);
            // draw back
            Handles.DrawLine(v8, v4);
            Handles.DrawLine(v8, v6);
            Handles.DrawLine(v2, v4);
            Handles.DrawLine(v2, v6);
            // draw corners
            Handles.DrawLine(v8, v7);
            Handles.DrawLine(v4, v3);
            Handles.DrawLine(v6, v5);
            Handles.DrawLine(v2, v1);
        }

        public static void DrawWireCube(Vector3 position, Vector3 size, Quaternion rotation)
        {
            Vector3 half = size * 0.5f;

            Vector3 v1 = position + (rotation * new Vector3(half.x, half.y, half.z));
            Vector3 v2 = position + (rotation * new Vector3(half.x, half.y, -half.z));
            Vector3 v3 = position + (rotation * new Vector3(half.x, -half.y, half.z));
            Vector3 v4 = position + (rotation * new Vector3(half.x, -half.y, -half.z));
            Vector3 v5 = position + (rotation * new Vector3(-half.x, half.y, half.z));
            Vector3 v6 = position + (rotation * new Vector3(-half.x, half.y, -half.z));
            Vector3 v7 = position + (rotation * new Vector3(-half.x, -half.y, half.z));
            Vector3 v8 = position + (rotation * new Vector3(-half.x, -half.y, -half.z));

            // draw front
            Handles.DrawLine(v7, v3);
            Handles.DrawLine(v7, v5);
            Handles.DrawLine(v1, v3);
            Handles.DrawLine(v1, v5);
            // draw back
            Handles.DrawLine(v8, v4);
            Handles.DrawLine(v8, v6);
            Handles.DrawLine(v2, v4);
            Handles.DrawLine(v2, v6);
            // draw corners
            Handles.DrawLine(v8, v7);
            Handles.DrawLine(v4, v3);
            Handles.DrawLine(v6, v5);
            Handles.DrawLine(v2, v1);
        }

        public static void DrawWireRect(Vector3 position, Vector2 size)
        {
            Vector2 half = size * 0.5f;

            Vector3 v1 = position + new Vector3(half.x, half.y);
            Vector3 v2 = position + new Vector3(half.x, -half.y);
            Vector3 v3 = position + new Vector3(-half.x, half.y);
            Vector3 v4 = position + new Vector3(-half.x, -half.y);

            Handles.DrawLine(v1, v2);
            Handles.DrawLine(v3, v4);
            Handles.DrawLine(v1, v3);
            Handles.DrawLine(v2, v4);
        }

        public static void DrawWireRect(Vector3 position, Vector2 size, Quaternion rotation)
        {
            Vector2 half = size * 0.5f;

            Vector3 v1 = position + (rotation * new Vector3(half.x, 0, half.y));
            Vector3 v2 = position + (rotation * new Vector3(half.x, 0, -half.y));
            Vector3 v3 = position + (rotation * new Vector3(-half.x, 0, half.y));
            Vector3 v4 = position + (rotation * new Vector3(-half.x, 0, -half.y));

            Handles.DrawLine(v1, v2);
            Handles.DrawLine(v3, v4);
            Handles.DrawLine(v1, v3);
            Handles.DrawLine(v2, v4);
        }
    }
    
}
