using UnityEngine;
using System.Collections;
namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(LineRenderer))]
    public class ProjectileCurve : Skill.Framework.StaticBehaviour
    {
        public int VertexCount = 20; //
        public float Gravity = -9.8f;

        private LineRenderer _LineRenderer;
        private int _PreVertexCount;

        protected virtual void Start()
        {
            _LineRenderer = GetComponent<LineRenderer>();
            if (_LineRenderer != null)
                _LineRenderer.SetVertexCount(VertexCount);
            else
                Debug.LogError("Can not find LineRenderer component");

            _PreVertexCount = VertexCount;
        }

        /// <summary>
        /// Simulate projectile path
        /// </summary>
        /// <param name="direction">Initial direction of projectile</param>
        /// <param name="position">Initial position of projectile</param>
        /// <param name="speed">Initial speed of projectile</param>
        /// <param name="time"> time of path to simulate</param>
        public void Simulate(Vector3 direction, Vector3 offset, float speed, float time)
        {
            if (_LineRenderer != null)
            {
                Vector3 position;
                if (_LineRenderer.useWorldSpace)
                    position = _Transform.position + offset;
                else
                    position = offset;

                if (_PreVertexCount != VertexCount)
                {
                    _LineRenderer.SetVertexCount(VertexCount);
                    _PreVertexCount = VertexCount;
                }

                Vector3 xzDirection;
                if (!_LineRenderer.useWorldSpace)
                {
                    _Transform.rotation = Quaternion.Euler(0, Skill.Framework.MathHelper.HorizontalAngle(direction), 0);
                    xzDirection = Vector3.forward;
                }
                else
                {
                    xzDirection = direction;
                }

                float v0SinAlpha, v0CosAlpha;

                if (this.Gravity != 0f)
                {
                    xzDirection.y = 0f;
                    xzDirection.Normalize();
                    float angle = Vector3.Angle(xzDirection, direction) * Mathf.Sign(direction.y);
                    v0SinAlpha = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
                    v0CosAlpha = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
                }
                else // avoid sin,cos math calculation if gravity is zero
                {
                    v0SinAlpha = 0f;
                    v0CosAlpha = speed;
                }

                Simulate(ref position, ref xzDirection, ref v0CosAlpha, ref v0SinAlpha, ref time);
            }
        }

        public void Simulate(float yaw, float pitch, Vector3 offset, float distance, float gravity, float time)
        {
            if (_LineRenderer != null)
            {
                Vector3 position;
                if (_LineRenderer.useWorldSpace)
                    position = _Transform.position + offset;
                else
                    position = offset;

                if (_PreVertexCount != VertexCount)
                {
                    _LineRenderer.SetVertexCount(VertexCount);
                    _PreVertexCount = VertexCount;
                }


                Vector3 xzDirection;
                if (!_LineRenderer.useWorldSpace)
                {
                    _Transform.rotation = Quaternion.Euler(0, yaw, 0);
                    xzDirection = Vector3.forward;
                }
                else
                {
                    xzDirection = Quaternion.Euler(0, yaw, 0) * Vector3.forward;// direction in xz plane
                }

                float speed = Mathf.Sqrt(distance * Mathf.Abs(gravity));
                float v0SinAlpha, v0CosAlpha;

                if (this.Gravity != 0f)
                {
                    xzDirection.y = 0f;
                    xzDirection.Normalize();
                    v0SinAlpha = speed * Mathf.Sin(pitch * Mathf.Deg2Rad);
                    v0CosAlpha = speed * Mathf.Cos(pitch * Mathf.Deg2Rad);
                }
                else // avoid sin,cos math calculation if gravity is zero
                {
                    v0SinAlpha = 0f;
                    v0CosAlpha = speed;
                }

                Simulate(ref position, ref xzDirection, ref v0CosAlpha, ref v0SinAlpha, ref time);
            }
        }

        private void Simulate(ref Vector3 position, ref Vector3 xzDirection, ref float v0CosAlpha, ref float v0SinAlpha, ref float time)
        {
            float timeStep = time / VertexCount;
            float startY = position.y;
            float timer = 0;
            float g = 0.5f * this.Gravity;
            xzDirection *= v0CosAlpha;

            for (int i = 0; i < VertexCount; i++)
            {
                Vector3 vector = position + ((Vector3)(xzDirection * timer));
                vector.y = startY + (((g * timer) + v0SinAlpha) * timer);
                _LineRenderer.SetPosition(i, vector);
                timer += timeStep;
            }
        }
    }

}