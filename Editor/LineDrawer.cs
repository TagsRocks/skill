using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        /// /// Draw polyline between points
        /// </summary>
        /// <param name="points">Points</param>                
        /// <param name="color">line color</param>
        /// <param name="thickness"> line thickness</param>        
        /// <param name="lineTexture">The thickness of the line</param>    
        public static void DrawPolyLine(IList<Vector2> points, Color color, float thickness, Texture2D lineTexture)
        {
            if (points == null)
                throw new ArgumentNullException("Invalid points to DrawPolyLine");
            if (points.Count < 2)
                throw new ArgumentException("Invalid points to DrawPolyLine");
            Color savedColor = GUI.color;
            Matrix4x4 savedMatrix = GUI.matrix;

            Vector3 lineStart = points[0];
            Vector3 lineEnd;

            for (int i = 1; i < points.Count; i++)
            {
                lineEnd = points[i];

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
                lineStart = lineEnd;
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
            Vector2[] points = new Vector2[segments + 1];
            points[0] = CubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i <= segments; ++i)
                points[i] = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
            DrawPolyLine(points, color, thickness, lineTexture);
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
            return Resources.CreateTexture(Color.white, 1);
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



        private static Material _LineMaterialGL;
        private static void CreateMaterial()
        {
            if (_LineMaterialGL == null)
            {
                _LineMaterialGL = new Material("Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }");
                _LineMaterialGL.hideFlags = HideFlags.HideAndDontSave;
                _LineMaterialGL.shader.hideFlags = HideFlags.HideAndDontSave;
            }
        }


        /// <summary>
        /// Draw line between points using GL
        /// </summary>
        /// <param name="lineStart">Line start</param>
        /// <param name="lineEnd">Line end</param>
        /// <param name="color">Line color</param>        
        public static void DrawLineGL(Vector2 lineStart, Vector2 lineEnd, Color color)
        {
            CreateMaterial();

            GL.PushMatrix();
            _LineMaterialGL.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex(lineStart);
            GL.Vertex(lineEnd);

            GL.End();
            GL.PopMatrix();
        }

        /// <summary>
        /// Draw a BezierLine between two point useing GL
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="startTangent"> start tangent </param>
        /// <param name="end">End Point</param>
        /// <param name="endTangent">end tangent  </param>
        /// <param name="color">line color</param>        
        /// <param name="segments">number of segments</param>        
        public static void DrawBezierLineGL(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, int segments)
        {
            Vector2[] points = new Vector2[segments + 1];
            points[0] = CubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i <= segments; ++i)
                points[i] = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
            DrawPolyLineGL(points, color);
        }

        /// <summary>
        /// Draw polyline between points using GL
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="color">line color</param>  
        /// <param name="startIndex">start index</param>
        /// <param name="pointCount">number of points to use in drawing</param>      
        public static void DrawPolyLineGL(IList<Vector2> points, Color color, int startIndex, int pointCount)
        {
            if (points == null)
                throw new ArgumentNullException("Invalid points to DrawPolyLineGL");
            if (points.Count < 2)
                throw new ArgumentException("Invalid points to DrawPolyLineGL");

            startIndex = Mathf.Max(startIndex, 0);
            int endIndex = startIndex + Mathf.Min(pointCount, points.Count - startIndex) - 1;
            CreateMaterial();

            GL.PushMatrix();
            _LineMaterialGL.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            Vector3 lineStart = points[startIndex];
            Vector3 lineEnd;
            startIndex++;
            for (int i = startIndex; i <= endIndex; i++)
            {
                lineEnd = points[i];
                lineStart.z = lineEnd.z = 0;

                GL.Vertex(lineStart);
                GL.Vertex(lineEnd);

                lineStart = lineEnd;
            }

            GL.End();
            GL.PopMatrix();
        }

        /// <summary>
        /// Draw polyline between points using GL
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="color">line color</param>          
        public static void DrawPolyLineGL(IList<Vector2> points, Color color)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentNullException("Invalid points to DrawPolyLineGL");
            DrawPolyLineGL(points, color, 0, points.Count);
        }
        /// <summary>
        /// Draw lines between each pair of points using GL
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="color">line color</param>
        /// <param name="lineCount">Number of lines</param>          
        public static void DrawLinesGL(IList<Vector2> points, Color color, int lineCount)
        {
            if (points == null)
                throw new ArgumentNullException("Invalid points to DrawLinesGL");
            if (points.Count % 2 != 0)
                throw new ArgumentException("Invalid points to DrawLinesGL");

            if (points.Count < lineCount * 2)
                throw new ArgumentException("Invalid lineCount to DrawLinesGL");

            CreateMaterial();

            GL.PushMatrix();
            _LineMaterialGL.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            lineCount *= 2;

            for (int i = 0; i < lineCount; i += 2)
            {
                GL.Vertex(points[i]);
                GL.Vertex(points[i + 1]);
            }

            GL.End();
            GL.PopMatrix();
        }

        /// <summary>
        /// Draw lines between each pair of points using GL
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="color">line color</param>          
        public static void DrawLinesGL(IList<Vector2> points, Color color)
        {
            if (points == null)
                throw new ArgumentNullException("Invalid points to DrawLinesGL");

            DrawLinesGL(points, color, points.Count / 2);
        }




    }





}
