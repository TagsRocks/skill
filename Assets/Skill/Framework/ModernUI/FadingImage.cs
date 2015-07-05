using UnityEngine;
using System.Collections;
using Skill.Framework;


namespace Skill.Framework.ModernUI
{

    [RequireComponent(typeof(Fading))]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadingImage : DynamicBehaviour
    {
        public bool FadeInOnStart;

        /// <summary> Fading  component </summary>
        public Fading Fading { get; private set; }

        private CanvasGroup _CanvasGroup;

        /// <summary> Get references </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            this.Fading = GetComponent<Fading>();
            this._CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void Awake()
        {
            base.Awake();
            if (FadeInOnStart)
            {
                _CanvasGroup.alpha = 1.0f;
                Fading.FadeIn(true);
            }
        }

        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            if (Fading != null && _CanvasGroup)
                _CanvasGroup.alpha = Fading.Alpha;
            base.Update();
        }


    }
}