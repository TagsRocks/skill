using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    public class CameraLimitPlane : CameraLimit
    {
        public static CameraLimitPlane Instance { get; private set; }

        public LimitPlane[] Limits;        

        private RaycastHit _Hit;
        private Ray _Ray;


        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public override void ApplyLimit(ref Vector3 cameraPosition, Vector3 preCameraPosition)
        {
            if (Limits != null && Limits.Length > 0)
            {
                for (int i = 0; i < Limits.Length; i++)
                {
                    if (Limits[i] != null)
                        CheckPlane(Limits[i].Plane, ref cameraPosition);
                }
            }           
        }

        private void CheckPlane(Plane plane, ref Vector3 cameraPosition)
        {
            if (!plane.GetSide(cameraPosition))
            {
                _Ray.origin = cameraPosition;
                _Ray.direction = plane.normal;
                float rayDistance;
                if (plane.Raycast(_Ray, out rayDistance))
                    cameraPosition = _Ray.GetPoint(rayDistance);
            }
        }

    }
}
