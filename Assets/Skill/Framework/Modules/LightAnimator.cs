using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    [RequireComponent(typeof(Light))]
    public class LightAnimator : DynamicBehaviour
    {
        public AnimationCurve Range;
        public AnimationCurve Intencity;
        public float Speed = 0.1f;
        private float _Time;
        private Light _Light;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            if (Range.length > 1)
                _Time = Random.Range(Range.keys[0].time, Range.keys[Range.length - 1].time);
            else if (Intencity.length > 1)
                _Time = Random.Range(Intencity.keys[0].time, Intencity.keys[Intencity.length - 1].time);
            else
                _Time = 0;

            _Light = GetComponent<Light>();

            Range.postWrapMode = WrapMode.Loop;
            Range.preWrapMode = WrapMode.Loop;
            Intencity.postWrapMode = WrapMode.Loop;
            Intencity.preWrapMode = WrapMode.Loop;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (!Global.IsGamePaused)
            {
                _Time += Time.deltaTime * Speed;
                if (_Time > 99999999.0f) _Time -= 99999999.0f;
                if (Range.length > 1)
                    _Light.range = Range.Evaluate(_Time);
                if (Intencity.length > 1)
                    _Light.intensity = Intencity.Evaluate(_Time);
                base.Update();
            }
        }
    }
}