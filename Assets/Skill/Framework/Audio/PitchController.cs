using UnityEngine;
using System.Collections;

namespace Skill.Framework.Audio
{
    /// <summary>
    /// Place one instance of this component in scene to control pitch of sounds.
    /// Add PitchListener to every GameObjects that affets global pitch.
    /// </summary>    
    public class PitchController : DynamicBehaviour
    {
        /// <summary>
        /// Global instance of PitchController 
        /// </summary>
        public static PitchController Instance { get; private set; }

        /// <summary>
        /// How to smooth pitch
        /// </summary>
        public SmoothingParameters PitchSmoothing;

        /// <summary>
        /// Gets of sets global pitch value
        /// </summary>
        public float Pitch
        {
            get { return _PitchSmoothing.Current; }
            set
            {
                _PitchSmoothing.Target = value;
            }
        }

        private Smoothing _PitchSmoothing;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (Instance != null)
                Debug.LogWarning("More than one instance of PitchController found.");
            Instance = this;
            _PitchSmoothing.Reset(1.0f);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_PitchTimeTW.IsEnabledAndOver)
            {
                Pitch = _PrePitch;
                _PitchTimeTW.End();
            }
            _PitchSmoothing.Update(PitchSmoothing);
            base.Update();
        }


        private TimeWatch _PitchTimeTW;
        private float _PrePitch;

        /// <summary>
        /// Change pitch for specific time than fallback
        /// </summary>
        /// <param name="targetPitch">Target pitch</param>
        /// <param name="length">lenght of change</param>
        /// <param name="realtime"> use realtime or gametime?</param>
        public void Change(float targetPitch, float length, bool realtime = true)
        {
            _PrePitch = Pitch;
            Pitch = targetPitch;
            _PitchTimeTW.Begin(length, realtime);
        }
    }
}