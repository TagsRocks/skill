using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace Skill.Framework
{
    /// <summary>
    /// andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    /// </summary>
    public class CRSpline3D : Path3D
    {
        /// <summary> Keys of path </summary>
        [HideInInspector]
        public Vector3[] Keys;

        /// <summary> Times of keys </summary>
        [HideInInspector]
        public float[] Times;


        [HideInInspector]
        public int Interpolations = 2;

        /// <summary> Number of keys in path </summary>
        public override int Length { get { return Keys != null ? Keys.Length : 0; } }
        /// <summary> lenght of path in time</summary>
        public override float TimeLength { get { return (Times != null && Times.Length > 0) ? Times[Times.Length - 1] : 0; } }

        /// <summary>
        /// Calculates the total path length.
        /// <summary>
        public float GetPathLength()
        {
            return GetPathLength(_PathControlPoints);
        }

        private Vector3[] _PathControlPoints;
        /// <summary> Rebuild path after modify keys </summary>
        public override void Rebuild()
        {
            if (Keys == null) Keys = new Vector3[] { transform.position, transform.position + Vector3.forward };
            else if (Keys.Length == 1) Keys = new Vector3[] { Keys[0], Keys[0] + Vector3.forward };

            if (Times == null) Times = new float[] { 0, 1 };
            else if (Times.Length == 1) Times = new float[] { Times[0], Times[0] + 1 };

            _PathControlPoints = GeneratorPathControlPoints(Keys);
        }

        /// <summary>
        /// Smoothes a list of Vector3's based on the number of interpolations. Credits to "Codetastic".
        /// <summary>
        /// <param name="interpolations">number of interpolations</param>
        public void Smooth(int interpolations)
        {
            _PathControlPoints = SmoothCurve(_PathControlPoints, interpolations);
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="time"> Time</param>
        /// <returns>Evaluate position</returns>
        public override Vector3 Evaluate(float time)
        {
            time = ConvertToInterpolationTime(time);
            Vector3 result = Interpolate(_PathControlPoints, time);
            if (!UseWorldSpace)
                result = transform.TransformPoint(result);
            return result;
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="time"> Time</param>
        /// <returns>Evaluate position</returns>
        public Vector3 Evaluate01(float time)
        {
            Vector3 result = Interpolate(_PathControlPoints, time);
            if (!UseWorldSpace)
                result = transform.TransformPoint(result);
            return result;
        }

        /// <summary>
        /// Get direction along path at specified time (0.0f - 1.0f)
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="deltaTime">deltaTime</param>
        /// <returns>Direction</returns>
        public Vector3 GetDirection01(float time, float deltaTime = 0.001f)
        {
            Vector3 direction;
            GetDirection01(time, deltaTime, out direction);
            return direction;
        }
        /// <summary>
        /// Get direction along path at specified time (0.0f - 1.0f)
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="deltaTime">deltaTime</param>    
        /// <param name="direction">Direction</param>
        public void GetDirection01(float time, float deltaTime, out Vector3 direction)
        {
            time = Mathf.Clamp01(time);
            if (deltaTime < 0.0001f) deltaTime = 0.00001f;
            direction = (Evaluate01(time + deltaTime) - Evaluate01(time)).normalized;

        }


        /// <summary>
        /// Get velocity along path at specified time (0.0f - 1.0f)
        /// </summary>
        /// <param name="time">time</param>
        /// <returns>Velocity</returns>
        public Vector3 GetVelocity01(float time)
        {
            time = Mathf.Clamp01(time);
            int numSections = _PathControlPoints.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currPt;

            Vector3 a = _PathControlPoints[currPt];
            Vector3 b = _PathControlPoints[currPt + 1];
            Vector3 c = _PathControlPoints[currPt + 2];
            Vector3 d = _PathControlPoints[currPt + 3];

            return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
                    + (2f * a - 5f * b + 4f * c - d) * u
                    + .5f * c - .5f * a;
        }

        /// <summary>
        /// Get velocity along path at specified time
        /// </summary>
        /// <param name="time">time</param>
        /// <returns>Velocity</returns>
        public Vector3 GetVelocity(float time)
        {
            time = ConvertToInterpolationTime(time);
            return GetVelocity01(time);
        }


        private float ValidateTime(float time)
        {
            if (time < 0)
            {
                switch (PreWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        time = 0.0f;
                        break;
                    case WrapMode.Loop:
                        time = Mathf.Abs(time);
                        time = TimeLength - (time - Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                    case WrapMode.PingPong:
                        time = Mathf.Abs(time);
                        time = time - (Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                }
            }
            else if (time > TimeLength)
            {
                switch (PostWrapMode)
                {
                    case WrapMode.Clamp:
                    case WrapMode.ClampForever:
                    case WrapMode.Default:
                        time = TimeLength;
                        break;
                    case WrapMode.Loop:
                        time = time - (Mathf.FloorToInt(time / TimeLength) * TimeLength);
                        break;
                    case WrapMode.PingPong:
                        time = TimeLength - (time - (Mathf.FloorToInt(time / TimeLength) * TimeLength));
                        break;
                }
            }
            return time;
        }

        /// <summary>
        /// Convert time to range (0.0f - 1.0f)
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>time between (0.0f - 1.0f)</returns>
        public float ConvertToInterpolationTime(float time)
        {
            time = ValidateTime(time);
            int index = FindTimeIndex(time);
            float timeStep = 1.0f / (Length - 1);
            return (index + (time - Times[index]) / (Times[index + 1] - Times[index])) * timeStep;
        }

        int FindTimeIndex(float time)
        {
            int minTimeIndex = 0;
            int maxTimeIndex = Length - 1;

            while (maxTimeIndex - minTimeIndex > 1)
            {
                // calculate the midpoint for roughly equal partition
                int middleIndex = (minTimeIndex + maxTimeIndex) / 2;
                // determine which subarray to search
                if (Times[middleIndex] < time)
                    // change min index to search upper subarray
                    minTimeIndex = middleIndex;
                else
                    // change max index to search lower subarray
                    maxTimeIndex = middleIndex;
            }

            return minTimeIndex;
        }


        /// <summary>
        /// Retrieves time of path at specified point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght)</param>
        /// <returns>Time</returns>
        public override float GetTime(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Times[pointIndex];
        }



        /// <summary>
        /// Retrieves position of path control point
        /// </summary>
        /// <param name="pointIndex">Index of point (0 - Path.Lenght - 1)</param>
        /// <returns>position of path control point</returns>
        public override Vector3 GetPoint(int pointIndex)
        {
            pointIndex = GetValidIndex(pointIndex);
            return Keys[pointIndex];
        }

        /// <summary>
        /// Interpolate between points at specified time
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="time">time (0.0f - 1.0f)</param>
        /// <returns></returns>
        public static Vector3 Interpolate(Vector3[] points, float time)
        {
            if (points.Length < 4) throw new ArgumentException("lenght of points must be at least 4");
            int numSections = points.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currPt;
            Vector3 a = points[currPt];
            Vector3 b = points[currPt + 1];
            Vector3 c = points[currPt + 2];
            Vector3 d = points[currPt + 3];
            return 0.5f * ((-a + 3.0f * b - 3.0f * c + d) * (u * u * u) + (2.0f * a - 5.0f * b + 4.0f * c - d) * (u * u) + (-a + c) * u + 2.0f * b);
        }

        /// <summary>
        /// Generate curve points
        /// </summary>
        /// <param name="pathPoints">points</param>
        /// <returns>curve points</returns>
        public static Vector3[] GeneratorPathControlPoints(Vector3[] pathPoints)
        {
            Vector3[] suppliedPath;
            Vector3[] vector3s;

            //create and store path points:
            suppliedPath = pathPoints;

            //populate calculate path;
            int offset = 2;
            vector3s = new Vector3[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

            //populate start and end control points:
            //vector3s[0] = vector3s[1] - vector3s[2];
            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector3[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
            }

            return (vector3s);
        }

        /// <summary>
        /// Calculates the total path length.
        /// <summary>
        public static float GetPathLength(Vector3[] waypoints)
        {
            float dist = 0f;
            for (int i = 0; i < waypoints.Length - 1; i++)
                dist += Vector3.Distance(waypoints[i], waypoints[i + 1]);
            return dist;
        }

        /// <summary>
        /// Smoothes a list of Vector3's based on the number of interpolations. Credits to "Codetastic".
        /// <summary>
        //http://answers.unity3d.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html
        public static Vector3[] SmoothCurve(Vector3[] pathToCurve, int interpolations)
        {
            Vector3[] tempPoints;
            List<Vector3> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;

            if (interpolations < 1)
                interpolations = 1;

            pointsLength = pathToCurve.Length;
            curvedLength = (pointsLength * Mathf.RoundToInt(interpolations)) - 1;
            curvedPoints = new List<Vector3>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);
                tempPoints = (Vector3[])pathToCurve.Clone();
                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        tempPoints[i] = (1 - t) * tempPoints[i] + t * tempPoints[i + 1];
                    }
                }
                curvedPoints.Add(tempPoints[0]);
            }
            return curvedPoints.ToArray();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color;
            if (ShowPoints && Keys != null)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawSphere(Keys[i], PointRadius);
                    else
                        Gizmos.DrawSphere(transform.TransformPoint(Keys[i]), PointRadius);
                }
            }
            if (ShowPath && Keys != null)
            {
                for (int i = 0; i < Keys.Length - 1; i++)
                {
                    if (UseWorldSpace)
                        Gizmos.DrawLine(Keys[i], Keys[i + 1]);
                    else
                        Gizmos.DrawLine(transform.TransformPoint(Keys[i]), transform.TransformPoint(Keys[i + 1]));
                }
            }
        }
    }


    // features
    /*
     * 
     * 
    [System.Serializable]
public class QuadBez {
	public Vector3 st, en, ctrl;
	
	public QuadBez(Vector3 st, Vector3 en, Vector3 ctrl) {
		this.st = st;
		this.en = en;
		this.ctrl = ctrl;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * st + 2f * d * t * ctrl + t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl);
		Gizmos.DrawLine(ctrl, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
	

}


[System.Serializable]
public class CubicBez {
	public Vector3 st, en, ctrl1, ctrl2;
	
	public CubicBez(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2) {
		this.st = st;
		this.en = en;
		this.ctrl1 = ctrl1;
		this.ctrl2 = ctrl2;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * d * st + 3f * d * d * t * ctrl1 + 3f * d * t * t * ctrl2 + t * t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t
			+  (6f * st - 12f * ctrl1 + 6f * ctrl2) * t
			-  3f * st + 3f * ctrl1;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl1);
		Gizmos.DrawLine(ctrl2, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
}

    */
}