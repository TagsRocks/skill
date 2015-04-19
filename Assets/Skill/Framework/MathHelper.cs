using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Some helper methods for math
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Wether given angle is between (sourceAngle) and (sourceAngle - 180)
        /// </summary>
        /// <param name="angle">Angle</param>
        /// <param name="sourceAngle">Source angle</param>
        /// <returns>True if angle is left side, otherwise false</returns>
        public static bool IsAngleLeftOf(float angle, float sourceAngle)
        {
            float validAngle = ClampAngle(angle);
            float validSourceAngle = ClampAngle(sourceAngle);

            return Mathf.Abs(validSourceAngle - validAngle) < 180;
        }

        /// <summary>
        /// Calc angle rotation around y axis
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <returns>Angle</returns>
        public static float HorizontalAngle(Vector3 direction)
        {
            return HorizontalAngle(direction.x, direction.z);
        }

        /// <summary>
        /// Calc angle rotation around y axis
        /// </summary>
        /// <param name="x">Direction.x</param>
        /// <param name="z">Direction.z</param>
        /// <returns>Angle</returns>
        public static float HorizontalAngle(float x, float z)
        {
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
            ClampAngle(ref angle);
            return angle;
        }


        /// <summary>
        /// Calc vertical angle rotation relative to y axis (relative to xz plane)
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <returns>Angle</returns>
        public static float VerticalAngle(Vector3 direction)
        {
            Vector3 flat = direction;
            flat.y = 0;
            return Mathf.Abs(Vector3.Angle(direction, flat)) * Mathf.Sign(direction.y);
        }

        /// <summary>
        /// Keep angle between -180 to 180
        /// </summary>
        /// <param name="angle">Angle to validate</param>
        /// <returns>angle between -180 to 180</returns>
        public static float ClampAngle(float angle)
        {
            ClampAngle(ref angle);// angle is a value copy
            return angle;
        }

        /// <summary>
        /// Keep angle between -180 to 180
        /// </summary>
        /// <param name="angle">Angle to validate</param>        
        public static void ClampAngle(ref float angle)
        {
            while (angle < -180) angle += 360;
            while (angle > 180) angle -= 360;
        }


        /// <summary>
        /// The angle between dirA and dirB around axis
        /// </summary>
        /// <param name="dirA">Direction A</param>
        /// <param name="dirB">Direction B</param>
        /// <param name="axis">Axis</param>
        /// <returns>The angle between dirA and dirB around axis</returns>
        public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
        {
            // Project A and B onto the plane orthogonal target axis
            dirA = dirA - Vector3.Project(dirA, axis);
            dirB = dirB - Vector3.Project(dirB, axis);

            // Find (positive) angle between A and B
            float angle = Vector3.Angle(dirA, dirB);

            // Return angle multiplied with 1 or -1
            return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
        }


        /// <summary>
        /// Converts 'Meter Per Second' to 'Kilometer Per Hour'
        /// </summary>
        /// <param name="mps">value in meter per second</param>
        /// <returns></returns>
        public static float MPS_To_KPH(float mps)
        {
            return mps * 3.6f;
        }


        /// <summary>
        /// calc projection of a point on a line
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="lineStart">Start of line</param>
        /// <param name="lineDirection">Direction of line</param>
        /// <returns></returns>
        public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineDirection)
        {
            return Vector3.Project((point - lineStart), lineDirection) + lineStart;
        }

        /// <summary>
        /// returns a point which is a projection from a point to a plane.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="point">Point</param>
        /// <returns>Projection of point on plane</returns>
        public static Vector3 ProjectPointOnPlane(Plane plane, Vector3 point)
        {
            //First calculate the distance from the point to the plane:
            float distance = plane.GetDistanceToPoint(point);

            //Reverse the sign of the distance
            distance *= -1;

            //Translate the point to form a projection
            return point + plane.normal * distance;
        }

        /// <summary>
        /// Calculate 32bit integer layermask
        /// </summary>
        /// <param name="layerIndices">Index of layers</param>
        /// <returns>layermask</returns>
        public static int GetLayerMask(params int[] layerIndices)
        {
            int mask = 0;
            if (layerIndices != null)
            {
                foreach (var item in layerIndices)
                {
                    mask |= 1 << item;
                }
            }
            return mask;
        }

        /// <summary>
        /// Calculate distance to camera where height of frustum is specified value
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="frustumHeight">Height of frustum</param>
        /// <returns>distance to camera</returns>
        public static float FrameFrustumHeight(Camera camera, float frustumHeight)
        {
            return frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Calculate distance to camera where width of frustum is specified value
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="frustumWidth">Width of frustum</param>
        /// <returns>distance to camera</returns>
        public static float FrameFrustumWidth(Camera camera, float frustumWidth)
        {
            float frustumHeight = frustumWidth / camera.aspect;
            return frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Calculate height of frustum at specified distance
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="distance">Distance to camera</param>
        /// <returns>height of frustum</returns>
        public static float FrustumHeightAtDistance(Camera camera, float distance)
        {
            return distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f;
        }

        /// <summary>
        /// Calculate width of frustum at specified distance
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="distance">Distance to camera</param>
        /// <returns>width of frustum</returns>
        public static float FrustumWidthAtDistance(Camera camera, float distance)
        {
            return FrustumHeightAtDistance(camera, distance) * camera.aspect;
        }

        /// <summary>
        /// Calculate width of orthogonal camera
        /// </summary>
        /// <param name="camera">orthogonal camera</param>
        /// <returns> width of camera</returns>
        public static float OrthographicWidth(Camera camera)
        {
            return camera.orthographicSize * Screen.width / Screen.height * 2;
        }

        /// <summary>
        /// Interpolates between min and max with smoothing at the limits.
        /// </summary>
        /// <param name="from">min</param>
        /// <param name="to">max</param>
        /// <param name="t">Time between 0.0 - 1.0</param>
        /// <returns>Smooth Step</returns>
        /// <remarks>
        /// This function interpolates between min and max in a similar way to Lerp.
        /// However, the interpolation will gradually speed up from the start and slow down toward the end.
        /// This is useful for creating natural-looking animation, fading and other transitions.
        /// </remarks>
        public static Vector3 SmoothStep(Vector3 from, Vector3 to, float t)
        {
            from.x = Mathf.SmoothStep(from.x, to.x, t);
            from.y = Mathf.SmoothStep(from.y, to.y, t);
            from.z = Mathf.SmoothStep(from.z, to.z, t);
            return from;
        }


        private static Dictionary<int, Vector3[]> _Circles;

        /// <summary>
        /// Calclate points on a xzplane circle with specified resolution
        /// </summary>
        /// <param name="resolution">number of points (at least 3)</param>
        /// <returns>Circle path</returns>
        public static Vector3[] GetCirclePath(int resolution)
        {
            if (resolution < 2)
                throw new ArgumentException("Invalid resolution - must be at least 2");

            if (_Circles == null) _Circles = new Dictionary<int, Vector3[]>();

            Vector3[] result = null;
            if (_Circles.TryGetValue(resolution, out result))
                return result;

            result = new Vector3[resolution];
            float angle = 0;
            float deltaAngel = (Mathf.PI * 2) / resolution;
            int i = 0;
            for (i = 0; i < resolution; i++)
            {
                result[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                angle += deltaAngel;
            }

            _Circles.Add(resolution, result);
            return result;
        }


        /// <summary>
        /// Normalize vector3 and calc magnitude
        /// </summary>
        /// <param name="v">Vector3</param>
        /// <returns>Magnitude of Vector3</returns>
        public static float NormalizeAndMagnitude(ref Vector3 v)
        {
            float magnitude = Mathf.Sqrt((v.x * v.x) + (v.y * v.y) + (v.z * v.z));
            if (magnitude > 0.00001f)
            {
                float f = 1.0f / magnitude;
                v.x *= f; v.y *= f; v.z *= f;
            }
            else
            {
                v = Vector3.zero;
                magnitude = 0;
            }
            return magnitude;
        }


        /// <summary>
        /// Returns the horizontal distance between a and b (ignore y).
        /// </summary>
        /// <param name="v1">Vector3</param>
        /// <param name="v2">Vector3</param>
        /// <returns>horizontal distance between a and b</returns>
        public static float HorizontalDistance(Vector3 v1, Vector3 v2)
        {
            Vector2 v11 = new Vector2(v1.x, v1.z);
            Vector2 v22 = new Vector2(v2.x, v2.z);
            return Vector2.Distance(v11, v22);
        }


        /// <summary>
        /// Returns the horizontal direction of v2 - v1 (ignore y).
        /// </summary>
        /// <param name="v1">Vector3</param>
        /// <param name="v2">Vector3</param>
        /// <returns>horizontal direction of v2 - v1</returns>
        public static Vector3 HorizontalDirection(Vector3 v1, Vector3 v2)
        {
            Vector3 dir = v2 - v1;
            dir.y = 0;
            return dir;
        }

        /// <summary>
        /// Returns the normalized horizontal direction of v2 - v1 (ignore y).
        /// </summary>
        /// <param name="v1">Vector3</param>
        /// <param name="v2">Vector3</param>
        /// <returns>normalized horizontal direction of v2 - v1</returns>
        public static Vector3 HorizontalDirectionNormalized(Vector3 v1, Vector3 v2)
        {
            Vector2 v11 = new Vector2(v1.x, v1.z);
            Vector2 v22 = new Vector2(v2.x, v2.z);
            Vector2 dir = v22 - v11;
            dir.Normalize();
            return new Vector3(dir.x, 0, dir.y);
        }

        /// <summary>
        /// Returns the horizontal angle between a and b (ignore y).
        /// </summary>
        /// <param name="v1">Vector3</param>
        /// <param name="v2">Vector3</param>
        /// <returns>horizontal angle between a and b</returns>
        public static float HorizontalAngle(Vector3 v1, Vector3 v2)
        {
            Vector2 v11 = new Vector2(v1.x, v1.z);
            Vector2 v22 = new Vector2(v2.x, v2.z);
            return Vector2.Angle(v11, v22);
        }




        /// <summary>
        /// Determines the intersection point of the lines
        /// </summary>
        /// <param name="line1Start"> line1 start</param>
        /// <param name="line1End">line1 end</param>
        /// <param name="line2Start">line2 start</param>
        /// <param name="line2End">line2 end</param>
        /// <param name="intersection">intersection point of two lines</param>
        /// <returns>
        ///  Returns True if the intersection point was found, and stores that point in intersection.
        ///  Returns False if there is no determinable intersection point, in which case intersection will
        ///  be unmodified.</returns>
        public static bool LineLineIntersection(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End, out Vector2 intersection)
        {
            intersection = line1Start;
            float distAB, theCos, theSin, newX, ABpos;

            //  Fail if either line is undefined.
            if (line1Start.x == line1End.x && line1Start.y == line1End.y || line2Start.x == line2End.x && line2Start.y == line2End.y) return false;

            //  (1) Translate the system so that point A is on the origin.
            line1End.x -= line1Start.x; line1End.y -= line1Start.y;
            line2Start.x -= line1Start.x; line2Start.y -= line1Start.y;
            line2End.x -= line1Start.x; line2End.y -= line1Start.y;

            //  Discover the length of segment A-B.
            distAB = line1End.magnitude;

            //  (2) Rotate the system so that point B is on the positive X axis.
            theCos = line1End.x / distAB;
            theSin = line1End.y / distAB;
            newX = line2Start.x * theCos + line2Start.y * theSin;
            line2Start.y = line2Start.y * theCos - line2Start.x * theSin; line2Start.x = newX;
            newX = line2End.x * theCos + line2End.y * theSin;
            line2End.y = line2End.y * theCos - line2End.x * theSin; line2End.x = newX;

            //  Fail if the lines are parallel.
            if (line2Start.y == line2End.y) return false;

            //  (3) Discover the position of the intersection point along line A-B.
            ABpos = line2End.x + (line2Start.x - line2End.x) * line2End.y / (line2End.y - line2Start.y);

            //  (4) Apply the discovered position to line A-B in the original coordinate system.
            intersection.x = line1Start.x + ABpos * theCos;
            intersection.y = line1Start.y + ABpos * theSin;

            //  Success.
            return true;
        }


        /// <summary>
        /// Determines the intersection point of the line segments
        /// </summary>
        /// <param name="line1Start"> line1 start</param>
        /// <param name="line1End">line1 end</param>
        /// <param name="line2Start">line2 start</param>
        /// <param name="line2End">line2 end</param>
        /// <param name="intersection">intersection point of two line segments</param>
        /// <returns>
        ///  Returns True if the intersection point was found, and stores that point in intersection.
        ///  Returns False if there is no determinable intersection point, in which case intersection will
        ///  be unmodified.</returns>
        public static bool LineLineSegmentIntersection(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End, out Vector2 intersection)
        {
            intersection = line1Start;
            float distAB, theCos, theSin, newX, ABpos;

            //  Fail if either line segment is zero-length.
            if (line1Start.x == line1End.x && line1Start.y == line1End.y || line2Start.x == line2End.x && line2Start.y == line2End.y) return false;

            //  Fail if the segments share an end-point.
            if (line1Start.x == line2Start.x && line1Start.y == line2Start.y || line1End.x == line2Start.x && line1End.y == line2Start.y || line1Start.x == line2End.x && line1Start.y == line2End.y || line1End.x == line2End.x && line1End.y == line2End.y)
            {
                return false;
            }

            //  (1) Translate the system so that point A is on the origin.
            line1End.x -= line1Start.x; line1End.y -= line1Start.y;
            line2Start.x -= line1Start.x; line2Start.y -= line1Start.y;
            line2End.x -= line1Start.x; line2End.y -= line1Start.y;

            //  Discover the length of segment A-B.
            distAB = line1End.magnitude;

            //  (2) Rotate the system so that point B is on the positive X axis.
            theCos = line1End.x / distAB;
            theSin = line1End.y / distAB;
            newX = line2Start.x * theCos + line2Start.y * theSin;
            line2Start.y = line2Start.y * theCos - line2Start.x * theSin; line2Start.x = newX;
            newX = line2End.x * theCos + line2End.y * theSin;
            line2End.y = line2End.y * theCos - line2End.x * theSin; line2End.x = newX;

            //  Fail if segment C-D doesn't cross line A-B.
            if (line2Start.y < 0 && line2End.y < 0 || line2Start.y >= 0 && line2End.y >= 0) return false;

            //  (3) Discover the position of the intersection point along line A-B.
            ABpos = line2End.x + (line2Start.x - line2End.x) * line2End.y / (line2End.y - line2Start.y);

            //  Fail if segment C-D crosses line A-B outside of segment A-B.
            if (ABpos < 0 || ABpos > distAB) return false;

            //  (4) Apply the discovered position to line A-B in the original coordinate system.
            intersection.x = line1Start.x + ABpos * theCos;
            intersection.y = line1Start.y + ABpos * theSin;

            //  Success.
            return true;
        }


        /// <summary>
        /// Check if given point is inside specified triangle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <param name="v1">v1 of triagle</param>
        /// <param name="v2">v2 of triagle</param>
        /// <param name="v3">v3 of triagle</param>
        /// <returns> true if point is inside triangle, otherwise false </returns>
        public static bool IsPointInTriangle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;

            b1 = Sign(point, v1, v2) < 0.0f;
            b2 = Sign(point, v2, v3) < 0.0f;
            b3 = Sign(point, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }
        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }



        /// <summary>
        /// Compute barycentric coordinates (u, v, w) for point p with respect to triangle (a, b, c)
        /// </summary>
        /// <param name="p">P</param>
        /// <param name="a">triangle point 1</param>
        /// <param name="b">triangle point 2</param>
        /// <param name="c">triangle point 3</param>
        /// <param name="u">weight of a</param>
        /// <param name="v">weight of b</param>
        /// <param name="w">weight of c</param>
        public static void Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float u, out float v, out float w)
        {
            Vector2 v0 = b - a, v1 = c - a, v2 = p - a;

            if (v2.sqrMagnitude < Mathf.Epsilon)
            {
                u = 1.0f;
                v = 0.0f;
                w = 0.0f;
            }
            else if ((p - b).sqrMagnitude < Mathf.Epsilon)
            {
                u = 0.0f;
                v = 1.0f;
                w = 0.0f;
            }
            else if ((p - c).sqrMagnitude < Mathf.Epsilon)
            {
                u = 0.0f;
                v = 0.0f;
                w = 1.0f;
            }
            else
            {

                float d00 = Vector2.Dot(v0, v0);
                float d01 = Vector2.Dot(v0, v1);
                float d11 = Vector2.Dot(v1, v1);
                float d20 = Vector2.Dot(v2, v0);
                float d21 = Vector2.Dot(v2, v1);
                float denom = d00 * d11 - d01 * d01;
                v = (d11 * d20 - d01 * d21) / denom;
                w = (d00 * d21 - d01 * d20) / denom;
                u = 1.0f - v - w;
            }
        }


        /// <summary>
        /// Calculate abs of Vector3
        /// </summary>
        /// <param name="vector">Vector3</param>
        /// <returns>Abs Vector3</returns>
        public static Vector3 Abs(Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);
            return vector;
        }

        /// <summary>
        /// Calculate abs of Vector2
        /// </summary>
        /// <param name="vector">Vector2</param>
        /// <returns>Abs Vector2</returns>
        public static Vector2 Abs(Vector2 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            return vector;
        }

        /// <summary>
        /// Do a per component division
        /// </summary>
        /// <param name="divide">Vector2 divide</param>
        /// <param name="divisor">Vector2 divisor</param>
        /// <returns>Divided vector</returns>
        public static Vector2 Divide(Vector2 divide, Vector2 divisor)
        {
            divide.x /= divisor.x;
            divide.y /= divisor.y;
            return divide;
        }

        /// <summary>
        /// Do a per component division
        /// </summary>
        /// <param name="divide">Vector3 divide</param>
        /// <param name="divisor">Vector3 divisor</param>
        /// <returns>Divided vector</returns>
        public static Vector3 Divide(Vector3 divide, Vector3 divisor)
        {
            divide.x /= divisor.x;
            divide.y /= divisor.y;
            divide.z /= divisor.z;
            return divide;
        }

        /// <summary>
        /// Calculate intersection point of a ray from center to specified point with rectangle
        /// </summary>
        /// <param name="rect">rect</param>
        /// <param name="pointOnRay">a point on ray from center</param>
        /// <returns>intersection point</returns>
        public static Vector2 IntersectRectWithRayFromCenter(Rect rect, Vector2 pointOnRay)
        {
            Vector2 pointOnRayLocal = pointOnRay - rect.center;
            Vector2 edgeToRayRatios = Divide((rect.max - rect.center), Abs(pointOnRayLocal));

            if (edgeToRayRatios.x < edgeToRayRatios.y)
                return new Vector2(pointOnRayLocal.x > 0 ? rect.xMax : rect.xMin, pointOnRayLocal.y * edgeToRayRatios.x + rect.center.y);
            else
                return new Vector2(pointOnRayLocal.x * edgeToRayRatios.y + rect.center.x, pointOnRayLocal.y > 0 ? rect.yMax : rect.yMin);
        }


        /// <summary>
        /// Creates a Rect defining the area where one rect overlaps another rect.
        /// </summary>
        /// <param name="rect1">Rect 1</param>
        /// <param name="rect2">Rect 2</param>
        /// <returns>intersection rect</returns>
        public static Rect Intersect(Rect rect1, Rect rect2)
        {
            Rect rectangle = new Rect();
            float num8 = rect1.x + rect1.width;
            float num7 = rect2.x + rect2.width;
            float num6 = rect1.y + rect1.height;
            float num5 = rect2.y + rect2.height;
            float num2 = (rect1.x > rect2.x) ? rect1.x : rect2.x;
            float num = (rect1.y > rect2.y) ? rect1.y : rect2.y;
            float num4 = (num8 < num7) ? num8 : num7;
            float num3 = (num6 < num5) ? num6 : num5;
            if ((num4 > num2) && (num3 > num))
            {
                rectangle.x = num2;
                rectangle.y = num;
                rectangle.width = num4 - num2;
                rectangle.height = num3 - num;
                return rectangle;
            }
            rectangle.x = 0;
            rectangle.y = 0;
            rectangle.width = 0;
            rectangle.height = 0;
            return rectangle;
        }

        /// <summary>
        /// Creates a new Rect that exactly contains two other rects.
        /// </summary>
        /// <param name="rect1">Rect 1</param>
        /// <param name="rect2">Rect 2</param>
        /// <returns>Union rect</returns>
        public static Rect Union(Rect rect1, Rect rect2)
        {
            Rect rectangle = new Rect();
            float num6 = rect1.x + rect1.width;
            float num5 = rect2.x + rect2.width;
            float num4 = rect1.y + rect1.height;
            float num3 = rect2.y + rect2.height;
            float num2 = (rect1.x < rect2.x) ? rect1.x : rect2.x;
            float num = (rect1.y < rect2.y) ? rect1.y : rect2.y;
            float num8 = (num6 > num5) ? num6 : num5;
            float num7 = (num4 > num3) ? num4 : num3;
            rectangle.x = num2;
            rectangle.y = num;
            rectangle.width = num8 - num2;
            rectangle.height = num7 - num;
            return rectangle;
        }

    }
}
