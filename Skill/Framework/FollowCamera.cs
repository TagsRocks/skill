using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// This camera smoothes out rotation around the y-axis and height.
    /// Horizontal Distance to the target is always fixed.
    /// There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.
    /// For every of those smoothed values we calculate the wanted value and the current value.
    /// Then we smooth it using the Lerp function.
    /// Then we apply the smoothed values to the transform's position.
    /// </summary>
    public class FollowCamera : StaticBehaviour
    {

        /// <summary> The target we are following </summary>
        public Transform Target;
        /// <summary> The distance in the x-z plane to the target </summary>
        public float Distance = 10.0f;
        /// <summary> the height we want the camera to be above the target </summary>
        public float Height = 5.0f;
        /// <summary> HeightDamping </summary>
        public float HeightDamping = 2.0f;
        /// <summary> RotationDamping </summary>
        public float RotationDamping = 3.0f;


        private void LateUpdate()
        {
            if (Global.IsGamePaused) return;
            // Early out if we don't have a target
            if (Target == null)
                return;

            // Calculate the current rotation angles
            float wantedRotationAngle = Target.eulerAngles.y;
            float wantedHeight = Target.position.y + Height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            Vector3 position = Target.position - (currentRotation * Vector3.forward * Distance);

            // Set the height of the camera
            position.y = currentHeight;
            transform.position = position;

            // Always look at the target
            transform.LookAt(Target);
        }
    }
}
