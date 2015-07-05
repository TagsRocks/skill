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
        /// set valid rect to clip lines out of this area
        /// </summary>
        public static Rect? ClipArea { get; set; }


        private static Vector2 LineRectIntersection(Vector2 lineStart, Vector2 lineEnd, Rect rect)
        {
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMin);
            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMax);

            Vector2 intersection;
            Vector2 result = lineEnd;
            float minDistance = Vector2.Distance(lineStart, lineEnd);

            // left side
            if (Skill.Framework.MathHelper.LineLineSegmentIntersection(lineStart, lineEnd, topLeft, bottomLeft, out intersection))
            {
                float d = Vector2.Distance(intersection, lineStart);
                if (d <= minDistance)
                {
                    result = intersection;
                    minDistance = d;
                }
            }

            // right side
            if (Skill.Framework.MathHelper.LineLineSegmentIntersection(lineStart, lineEnd, topRight, bottomRight, out intersection))
            {
                float d = Vector2.Distance(intersection, lineStart);
                if (d <= minDistance)
                {
                    result = intersection;
                    minDistance = d;
                }
            }

            // top side            
            if (Skill.Framework.MathHelper.LineLineSegmentIntersection(lineStart, lineEnd, topLeft, topRight, out intersection))
            {
                float d = Vector2.Distance(intersection, lineStart);
                if (d <= minDistance)
                {
                    result = intersection;
                    minDistance = d;
                }
            }

            // bottom side
            if (Skill.Framework.MathHelper.LineLineSegmentIntersection(lineStart, lineEnd, bottomLeft, bottomRight, out intersection))
            {
                float d = Vector2.Distance(intersection, lineStart);
                if (d <= minDistance)
                {
                    result = intersection;
                    minDistance = d;
                }
            }

            return result;
        }


        // Clip line inside of ClipArea and return true if line is valid                
        private static bool ClipLine(ref Vector2 lineStart, ref Vector2 lineEnd)
        {
            if (ClipArea != null && ClipArea.HasValue)
            {
                bool startIn = ClipArea.Value.Contains(lineStart);
                bool endIn = ClipArea.Value.Contains(lineEnd);

                if (!startIn && !endIn) return false;
                if (startIn && !endIn)
                {
                    lineEnd = LineRectIntersection(lineStart, lineEnd, ClipArea.Value);
                }
                else if (!startIn && endIn)
                {
                    lineStart = LineRectIntersection(lineEnd, lineStart, ClipArea.Value);
                }
            }
            return true;
        }

        // Clip line inside of ClipArea and return true if line is valid                
        private static bool ClipLine(ref Vector3 lineStart, ref Vector3 lineEnd)
        {
            Vector2 lineStart2 = lineStart;
            Vector2 lineEnd2 = lineEnd;
            bool result = ClipLine(ref lineStart2, ref lineEnd2);

            lineStart.x = lineStart2.x;
            lineStart.y = lineStart2.y;

            lineEnd.x = lineEnd2.x;
            lineEnd.y = lineEnd2.y;

            return result;
        }


        /// <summary>
        /// Draw line as another implementation
        /// </summary>
        /// <param name="lineStart">The start of the line</param>
        /// <param name="lineEnd">The end of the line</param>
        /// <param name="color">line color</param>        
        public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color)
        {
            if (!ClipLine(ref lineStart, ref lineEnd)) return;
            Color savedColor = UnityEditor.Handles.color;
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawAAPolyLine(lineStart, lineEnd);
            UnityEditor.Handles.EndGUI();
            UnityEditor.Handles.color = savedColor;
        }

        /// <summary>
        /// /// Draw polyline between points
        /// </summary>
        /// <param name="points">Points</param>                
        /// <param name="color">line color</param>          
        public static void DrawPolyLine(IList<Vector2> points, Color color, int startIndex, int count)
        {
            if (points == null)
                throw new ArgumentNullException("Invalid points to DrawPolyLine");
            if (points.Count < 2)
                throw new ArgumentException("Invalid points to DrawPolyLine");

            Color savedColor = UnityEditor.Handles.color;
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.color = color;


            Vector3 lineStart = points[startIndex];
            Vector3 lineEnd;

            count += startIndex;

            for (int i = startIndex + 1; i < count; i++)
            {
                lineEnd = points[i];

                if (ClipLine(ref lineStart, ref lineEnd))
                    UnityEditor.Handles.DrawAAPolyLine(lineStart, lineEnd);
                lineStart = lineEnd;
            }

            UnityEditor.Handles.EndGUI();
            UnityEditor.Handles.color = savedColor;
        }

        /// <summary>
        /// Draw a BezierLine between two point
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="startTangent"> start tangent </param>
        /// <param name="end">End Point</param>
        /// <param name="endTangent">end tangent  </param>
        /// <param name="color">line color</param>        
        /// <param name="segments">number of segments</param>
        public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, int segments)
        {
            Vector2[] points = new Vector2[segments + 1];
            points[0] = CubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i <= segments; ++i)
                points[i] = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
            DrawPolyLine(points, color, 0, points.Length);
        }

        private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            float rtt = rt * t;
            return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
        }


        private static Material _LineMaterialGL;
        private static void CreateMaterial()
        {
            if (_LineMaterialGL == null)
            {
                Shader shader = Skill.Editor.Resources.GetShader("LineGL");
                _LineMaterialGL = new Material(shader);
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
            if (!ClipLine(ref lineStart, ref lineEnd)) return;

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

                if (ClipLine(ref lineStart, ref lineEnd))
                {
                    GL.Vertex(lineStart);
                    GL.Vertex(lineEnd);
                }
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
            DrawPolyLine(points, color, 0, points.Count);
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
                Vector2 lineStart = points[i];
                Vector2 lineEnd = points[i + 1];

                if (!ClipLine(ref lineStart, ref lineEnd)) continue;

                GL.Vertex(lineStart);
                GL.Vertex(lineEnd);
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
