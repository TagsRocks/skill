using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.Effects
{
    public abstract class ExplosionIndicator : Skill.Framework.DynamicBehaviour
    {
        public float LifeTime = 4.0f;
        public float Radius = 5.0f;
        public bool SelfDestroy = false;
        public Vector3 StickOffset;
        public bool ShowOnAwake = false;

        private Skill.Framework.TimeWatch _LifeTimeTW;
        private Transform _StickedTransform;

        public float Percent { get { return _LifeTimeTW.Percent; } }

        public void StickTo(Transform transform)
        {
            _StickedTransform = transform;
        }

        protected override void Awake()
        {
            base.Awake();
            if (ShowOnAwake)
                Show();
        }

        public void Show()
        {
            _LifeTimeTW.Begin(LifeTime);
            StartIndicator();
        }

        protected abstract void StartIndicator();
        protected abstract void EndIndicator();
        protected abstract void UpdateIndicator();

        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_LifeTimeTW.IsEnabledAndOver)
            {
                _LifeTimeTW.End();
                EndIndicator();
                _StickedTransform = null;
                if (SelfDestroy)
                    DestroySelf();
            }
            else
            {
                if (_StickedTransform != null)
                    transform.position = _StickedTransform.position + StickOffset;
                UpdateIndicator();
            }
            base.Update();
        }
    }

}