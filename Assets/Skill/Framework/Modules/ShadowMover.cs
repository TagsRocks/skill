using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    public class ShadowMover : Skill.Framework.DynamicBehaviour
    {
        public AnimationCurve MovementX;
        public AnimationCurve MovementZ;
        public float Scale = 0.1f;
        public float Speed = 0.1f;
        private float _Time;
        private Vector3 _InitialPos;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _Time = 0;
            MovementX.postWrapMode = WrapMode.Loop;
            MovementX.preWrapMode = WrapMode.Loop;
            MovementZ.postWrapMode = WrapMode.Loop;
            MovementZ.preWrapMode = WrapMode.Loop;

            _InitialPos = transform.position;
        }

        // Update is called once per frame
        protected override void Update()
        {
            _Time += Time.deltaTime * Speed;
            transform.position = _InitialPos + (new Vector3(MovementX.Evaluate(_Time), 0, MovementZ.Evaluate(_Time)) * Scale);

            base.Update();
        }
    }
}