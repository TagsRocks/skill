using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    [RequireComponent(typeof(Matinee))]
    [RequireComponent(typeof(CheckPoint))]
    public class MatineeBreak : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Optional FadeScreen to fadeout before apply CheckPoint </summary>
        public Skill.Framework.UI.FadeScreen FadeScreen;
        /// <summary> Delay by start of matinee </summary>
        public float StartDelay = 2.0f;
        /// <summary> key to break matinee </summary>
        public KeyCode BreakKey = KeyCode.Escape;
        /// <summary> how long to hold key </summary>
        public float KeyHoldTime = 0;
        /// <summary> how long to delay between break start and break operation (exp: let finish screen fade) </summary>
        public float Delay = 0;
        /// <summary> Pause matinee on break start</summary>
        public bool PauseOnBreak;
        /// <summary> Reference to Matinee component </summary>
        public Matinee Matinee { get; private set; }

        private bool _IsBreaking;
        private TimeWatch _BreakDelay;
        private TimeWatch _KeyHoldTW;
        private CheckPoint _CheckPoint;

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            Matinee = GetComponent<Matinee>();
            _CheckPoint = GetComponent<CheckPoint>();
            if (Matinee == null) throw new MissingComponentException("MatineeBreak needs a Matinee component");
            if (_CheckPoint == null) throw new MissingComponentException("MatineeBreak needs a CheckPoint component");
        }


        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_IsBreaking)
            {
                if (_BreakDelay.IsOver)
                {
                    Break();
                    _BreakDelay.End();
                    _IsBreaking = false;
                    enabled = false;
                }
            }
            else if (Matinee != null && Matinee.IsPlaying)
            {
                if (Matinee.PlaybackTime > StartDelay)
                {
                    if (Input.GetKey(BreakKey))
                    {
                        if (!_KeyHoldTW.IsEnabled)
                        {
                            _KeyHoldTW.Begin(KeyHoldTime);
                        }
                        else if (_KeyHoldTW.IsEnabledAndOver)
                        {
                            _KeyHoldTW.End();
                            BreakStart();
                            _BreakDelay.Begin(Delay);
                            _IsBreaking = true;
                        }
                    }
                    else
                    {
                        _KeyHoldTW.End();
                    }
                }
            }
            base.Update();
        }

        /// <summary> Start break operation before stop matinee </summary>
        protected virtual void BreakStart()
        {
            if (PauseOnBreak)
                Matinee.Pause();
            if (this.FadeScreen != null)
            {
                this.FadeScreen.Fading.FadeOutTime = this.Delay - 0.2f;
                this.FadeScreen.Fading.FadeToOne();
            }
        }

        /// <summary> Stop matinee and do required changes </summary>
        protected virtual void Break()
        {
            Matinee.Stop();
            _CheckPoint.Apply();
            if (this.FadeScreen != null)
                this.FadeScreen.Fading.FadeToZero();
        }
    }
}
