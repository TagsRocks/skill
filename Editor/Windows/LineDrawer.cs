using System;
using UnityEngine;
using System.Collections;

namespace Skill.Editor
{
    /// <summary>
    /// Helper class to draw line in EditorWindow
    /// </summary>
    public static class LineDrawer
    {

        /// <summary>
        /// Draw line as another implementation
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>
        /// <param name="color">line color</param>
        /// <param name="thickness"> line thickness</param>        
        /// <param name="lineTexture">The thickness of the line</param>        
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color, float thickness, Texture2D lineTexture)
        {
            Color savedColor = GUI.color;
            Matrix4x4 savedMatrix = GUI.matrix;

            float angle = Vector3.Angle(lineEnd - lineStart, Vector2.right) * (lineStart.y <= lineEnd.y ? 1 : -1);
            float m = (lineEnd - lineStart).magnitude;
            if (m > 0.01f)
            {
                Vector3 dz = new Vector3(lineStart.x, lineStart.y, 0);

                GUI.color = color;
                GUI.matrix = TranslationMatrix(dz) * GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(m, thickness), new Vector3(-0.5f, 0, 0));
                GUI.matrix = TranslationMatrix(-dz) * GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, Vector2.zero);
                GUI.matrix = TranslationMatrix(dz + new Vector3(thickness / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

                GUI.DrawTexture(new Rect(0, 0, 1, 1), lineTexture);
            }
            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }

        /// <summary>
        /// Draw a BezierLine between two point
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="startTangent"> start tangent </param>
        /// <param name="end">End Point</param>
        /// <param name="endTangent">end tangent  </param>
        /// <param name="color">line color</param>
        /// <param name="thickness">line thickness</param>        
        /// <param name="segments">number of segments</param>
        /// <param name="lineTexture">line texture</param>        
        public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float thickness, int segments, Texture2D lineTexture)
        {
            Vector2 lastV = CubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i <= segments; ++i)
            {
                Vector2 v = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
                DrawLine(lastV, v, color, thickness, lineTexture);
                lastV = v;
            }
        }

        private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            float rtt = rt * t;
            return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
        }

        private static Matrix4x4 TranslationMatrix(Vector2 translation)
        {
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.m03 = translation.x;
            matrix.m13 = translation.y;
            matrix.m23 = 0;
            return matrix;
        }

        /// <summary>
        /// Create a texture to use in DrawLine methods
        /// </summary>
        /// <returns>Texture</returns>
        public static Texture2D CreateLineTexture()
        {
            Texture2D lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
            lineTex.SetPixel(0, 1, Color.white);
            lineTex.Apply();
            return lineTex;
        }

        /// <summary>
        /// Create a texture to use in DrawLine method
        /// </summary>        
        /// <param name="height">height of texture</param>
        /// <returns>Texture</returns>
        public static Texture2D CreateAntiAliasLineTexture(int height = 3)
        {
            height = Mathf.Max(3, height);
            Texture2D aaLineTex = new Texture2D(1, height, TextureFormat.ARGB32, true);

            float h2 = (float)(height + 1) * 0.5f;
            float middle = (float)(height - 1) * 0.5f;
            for (int i = 0; i < height; i++)
                aaLineTex.SetPixel(0, i, new Color(1, 1, 1, 1.0f - Mathf.Abs((float)(i + 1) - h2) / middle));

            aaLineTex.Apply();
            return aaLineTex;
        }


        /*  Old Methods
          
        /// <summary>
        /// Draw a line between two points with the thickness of 1
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>        
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color)
        {
            DrawLine(lineStart, lineEnd, color, 1);
        }

        /// <summary>
        /// Draw a line between two points the specified thickness
        /// Inspired by code posted by Sylvan 
        /// <!-- http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005 --> 
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>        
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color, int thickness)
        {
            DrawLineStretched(lineStart, lineEnd, UnityEditor.EditorGUIUtility.whiteTexture, color, thickness);
        }

        /// <summary>
        /// Draw a line between two points with the specified texture and thickness.
        /// The texture will be stretched to fill the drawing rectangle.
        /// Inspired by code posted by Sylvan
        /// <!-- http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005 -->
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>
        /// <param name="texture">The texture of the line</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLineStretched(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, Color color, int thickness)
        {
            Vector2 lineVector = lineEnd - lineStart;
            float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
            if (lineVector.x < 0)
            {
                angle += 180;
            }

            if (thickness < 1)
            {
                thickness = 1;
            }

            // The center of the line will always be at the center
            // regardless of the thickness.
            int thicknessOffset = (int)Mathf.Ceil(thickness / 2);

            Matrix4x4 preGUIMatrix = GUI.matrix;
            Color preColor = GUI.color;

            GUIUtility.RotateAroundPivot(angle, lineStart);
            GUI.DrawTexture(new Rect(lineStart.x, lineStart.y - thicknessOffset, lineVector.magnitude, thickness), texture);

            GUI.color = preColor;
            GUI.matrix = preGUIMatrix;
        }


        /// <summary>
        /// Draw a line between two points with the specified texture and a thickness of 1
        /// The texture will be repeated to fill the drawing rectangle.
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>
        /// <param name="texture">The texture of the line</param>
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, Color color)
        {
            DrawLine(lineStart, lineEnd, texture, color, 1);
        }

        /// <summary>
        /// Draw a line between two points with the specified texture and thickness.
        /// The texture will be repeated to fill the drawing rectangle.
        /// Inspired by code posted by Sylvan and ArenMook
        /// <!-- http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005 -->
        /// <!-- http://forum.unity3d.com/threads/28247-Tile-texture-on-a-GUI?p=416986&viewfull=1#post416986 -->
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>
        /// <param name="texture">The texture of the line</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, Color color, int thickness)
        {
            Vector2 lineVector = lineEnd - lineStart;
            float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
            if (lineVector.x < 0)
            {
                angle += 180;
            }
            if (thickness < 1)
            {
                thickness = 1;
            }

            // The center of the line will always be at the center
            // regardless of the thickness.
            int thicknessOffset = (int)Mathf.Ceil(thickness / 2);

            Rect drawingRect = new Rect(lineStart.x, lineStart.y - thicknessOffset, Vector2.Distance(lineStart, lineEnd), (float)thickness);

            Matrix4x4 preGUIMatrix = GUI.matrix;
            Color preColor = GUI.color;

            GUI.color = color;
            GUIUtility.RotateAroundPivot(angle, lineStart);


            GUI.BeginGroup(drawingRect);
            {
                int drawingRectWidth = Mathf.RoundToInt(drawingRect.width);
                int drawingRectHeight = Mathf.RoundToInt(drawingRect.height);
                for (int y = 0; y < drawingRectHeight; y += texture.height)
                {
                    for (int x = 0; x < drawingRectWidth; x += texture.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, texture.width, texture.height), texture);
                    }
                }
            }
            GUI.EndGroup();

            GUI.color = preColor;
            GUI.matrix = preGUIMatrix;
        }
         */
    }





}
