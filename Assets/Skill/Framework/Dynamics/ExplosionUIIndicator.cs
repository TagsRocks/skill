using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Dynamics
{
    public class ExplosionUIIndicator : DynamicBehaviour
    {
        public Texture Explosion;
        public Texture Arrow;
        public Color FarColor = Color.white;
        public Color NearColor = Color.red;
        public float SizeFactor = 0.1f;
        public float MaxDistance = 50.0f;

        private bool _ArrowVisible;
        private Rect _ExplosionRect;
        private Rect _ArrowRect;
        private float _ArrowRotation;
        private Color _Color;

        public virtual Vector3 PlayerPosition { get { return Camera.main.transform.position; } }

        protected override void Update()
        {
            _Color = NearColor;

            float distance = Vector3.Distance(transform.position, PlayerPosition);
            _Color = Color.Lerp(NearColor, FarColor, Mathf.Clamp01(distance / MaxDistance));

            Vector3 pos = transform.position;

            Plane cameraPlane = new Plane(Camera.main.transform.forward, Camera.main.transform.position + Camera.main.transform.forward * 0.2f);
            if (!cameraPlane.GetSide(pos))
                pos = Skill.Framework.MathHelper.ProjectPointOnPlane(cameraPlane, pos);

            Rect screenRect = Skill.Framework.Utility.ScreenRect;
            Vector3 screenPos3 = Camera.main.WorldToScreenPoint(pos);

            Vector2 screenPos = new Vector2(screenPos3.x, screenPos3.y);
            if (Skill.Framework.Modules.ScreenQualityCamera.Instance != null)
                screenPos = Skill.Framework.Modules.ScreenQualityCamera.Instance.ToCameraSpace(screenPos);

            float size = (screenRect.width + screenRect.height) * 0.1f * SizeFactor;

            if (!screenRect.Contains(screenPos))
            {
                _ArrowVisible = true;
                Vector2 center = screenRect.center;
                Vector2 dir = new Vector2(center.x - screenPos.x, center.y - screenPos.y).normalized;

                Vector2 intersect = IntersectRectWithRayFromCenter(screenRect, screenPos);
                center = intersect + (dir * (size * 1.1f));
                _ExplosionRect.x = center.x - size * 0.5f;
                _ExplosionRect.y = (screenRect.height - center.y) - size * 0.5f;
                _ExplosionRect.width = size;
                _ExplosionRect.height = size;

                center = intersect + (dir * (size * 0.5f));
                _ArrowRect.x = center.x - size * 0.25f;
                _ArrowRect.y = (screenRect.height - center.y) - size * 0.25f;
                _ArrowRect.width = size * 0.5f;
                _ArrowRect.height = size * 0.5f;
                _ArrowRotation = Skill.Framework.MathHelper.HorizontalAngle(dir.x, dir.y);
            }
            else
            {
                _ArrowVisible = false;
                Vector2 center = screenPos;
                _ExplosionRect.x = center.x - size * 0.5f;
                _ExplosionRect.y = (screenRect.height - center.y) - size * 0.5f;
                _ExplosionRect.width = size;
                _ExplosionRect.height = size;
            }


            base.Update();
        }


        void OnGUI()
        {

            Color savedColor = GUI.color;
            GUI.color = _Color;
            GUI.DrawTexture(_ExplosionRect, Explosion, ScaleMode.StretchToFill);
            if (_ArrowVisible)
            {
                Matrix4x4 saveMatrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(_ArrowRotation, _ArrowRect.center);
                GUI.DrawTexture(_ArrowRect, Arrow, ScaleMode.StretchToFill);
                GUI.matrix = saveMatrix;
            }
            GUI.color = savedColor;

        }



        private static Vector2 Abs(Vector2 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            return vector;
        }

        private static Vector2 Divide(Vector2 vector, Vector2 divisor)
        {
            vector.x /= divisor.x;
            vector.y /= divisor.y;
            return vector;
        }

        private static Vector2 IntersectRectWithRayFromCenter(Rect rect, Vector2 pointOnRay)
        {
            Vector2 pointOnRayLocal = pointOnRay - rect.center;
            Vector2 edgeToRayRatios = Divide((rect.max - rect.center), Abs(pointOnRayLocal));

            if (edgeToRayRatios.x < edgeToRayRatios.y)
                return new Vector2(pointOnRayLocal.x > 0 ? rect.xMax : rect.xMin, pointOnRayLocal.y * edgeToRayRatios.x + rect.center.y);
            else
                return new Vector2(pointOnRayLocal.x * edgeToRayRatios.y + rect.center.x, pointOnRayLocal.y > 0 ? rect.yMax : rect.yMin);
        }
    }
}