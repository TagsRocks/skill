using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill
{
    public static class MathHelper
    {
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

        public static float HorizontalAngle(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            return angle;
        }

        // calc delta angle base on  0 - 360
        public static float DeltaAngle(float angle1, float angle2)
        {
            float delta = Mathf.Abs(angle1 - angle2);

            if (delta > 180)
                return 360 - delta;
            else
                return delta;
        }


        // The angle between dirA and dirB around axis
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
    }
}
