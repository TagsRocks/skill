using UnityEngine;
using System.Collections;

namespace Skill.Framework.Dynamics
{
    /// <summary>
    /// Move around a circle
    /// </summary>
    public class MoveAround : DynamicBehaviour
    {
        /// <summary> Number of points in circle - greater value : smoother circle </summary>
        public int Resolution = 16;
        /// <summary> How long takes each round </summary>
        public float Period = 5.0f;
        /// <summary> Radius of circle </summary>
        public float Radius = 1.0f;
        /// <summary> Start time (0 - Period) </summary>
        public float StartTime = 0.0f;

        private Vector3[] _Points;
        private float _Timer;

        /// <summary> Center of circle to move around </summary>
        /// <remarks>
        /// Modify it at Start
        /// </remarks>
        public Vector3 Center { get; set; }

        /// <summary> Is moving around center </summary>
        public bool IsMoving { get; private set; }

        private Vector3 _CirclePos;

        protected override void Awake()
        {
            base.Awake();
            _Points = MathHelper.GetCirclePath(Resolution);
            while (StartTime > Period) StartTime -= Period;
            if (transform.parent == null)
                Center = transform.position;
            else
                Center = Vector3.zero;
            _CirclePos = _Points[0];
        }

        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (IsMoving)
            {
                _Timer += UnityEngine.Time.deltaTime;
                while (_Timer >= Period) _Timer -= Period;
                float t = _Timer / Period;

                float step = Period / Resolution;

                int index = Mathf.FloorToInt(t * Resolution) % Resolution;
                int nextIndex = (index + 1) % Resolution;

                Vector3 pos1 = _Points[index];
                Vector3 pos2 = _Points[nextIndex];
                _CirclePos = Vector3.Lerp(pos1, pos2, (_Timer - (index * step)) / step);
            }
            transform.localPosition = Center + (_CirclePos * Radius);
            base.Update();
        }


        /// <summary> Reset timer to start time </summary>
        public void ResetTime()
        {
            _Timer = StartTime;
        }

        /// <summary> Start moving around  </summary>
        public void Move() { IsMoving = true; }
        /// <summary> Stop moving around  </summary>
        public void Stop() { IsMoving = false; }
    }
}
