using UnityEngine;
using System.Collections;
namespace Skill.Framework
{
    public class LimitPlane : MonoBehaviour
    {
        public Color Color = new Color(1.0f, 0.2f, 0.2f, 0.5f);
        public Skill.Framework.GizmosHelper.DrawColliderMode DrawMode = Skill.Framework.GizmosHelper.DrawColliderMode.SolidAndWire;

        public Plane Plane { get; private set; }

        private void Awake()
        {
            Plane = new Plane(transform.forward, transform.position);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Skill.Framework.GizmosHelper.DrawBox(transform, new Vector3(1, 1, 0.02f), transform.position, this.Color, this.DrawMode);
            Color color = this.Color;
            color.a = 1.0f;
            Gizmos.color = color;
            float size = 0.6f;
            Skill.Framework.GizmosHelper.DrawArrow(transform.position, transform.position + transform.forward * size, size * 0.4f);
        }
#endif
    }
}
