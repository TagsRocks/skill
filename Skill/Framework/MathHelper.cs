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
            if (sourceAngle >= 180)
            {
                float f = sourceAngle - 180;
                return angle >= f && angle <= sourceAngle;
            }
            else
            {
                if (angle >= 0)
                {
                    if (angle <= sourceAngle)
                        return true;
                }
                else
                {
                    float f = sourceAngle - 180 + 360;
                    if (angle >= f)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calc angle rotation around y axis
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <returns>Angle</returns>
        public static float HorizontalAngle(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }

        /// <summary>
        /// Calc delta angle base on  0 - 360
        /// </summary>
        /// <param name="angle1">Angle 1</param>
        /// <param name="angle2">Angle 2</param>
        /// <returns></returns>
        public static float DeltaAngle(float angle1, float angle2)
        {
            float delta = Mathf.Abs(angle1 - angle2);

            if (delta > 180)
                return 360 - delta;
            else
                return delta;
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
    }
}
