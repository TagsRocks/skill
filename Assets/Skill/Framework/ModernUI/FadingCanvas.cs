using UnityEngine;
using System.Collections;
using Skill.Framework;


namespace Skill.Framework.ModernUI
{

    [RequireComponent(typeof(CanvasGroup))]
    public class FadingCanvas : DynamicBehaviour
    {
        public bool FadeInOnAwake;

        /// <summary> Fading  component </summary>
        public Fading Fading;

        private CanvasGroup _CanvasGroup;

        /// <summary> Get references </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            if (Fading == null)
                this.Fading = GetComponent<Fading>();
            this._CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void Awake()
        {
            base.Awake();
            if (FadeInOnAwake)
            {
                _CanvasGroup.alpha = 1.0f;
                Fading.FadeToZero(true);
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