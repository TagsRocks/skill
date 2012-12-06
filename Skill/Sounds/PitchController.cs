using UnityEngine;
using System.Collections;
namespace Skill.Sounds
{
    /// <summary>
    /// Place one instance of this component in scene to control pitch of sounds.
    /// Add PitchListener to every GameObjects that affets global pitch.
    /// </summary>
    [AddComponentMenu("Skill/Sounds/PitchController")]
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
            _PitchSmoothing.Target = _PitchSmoothing.Current = 1.0f;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            //if (Time.timeScale < 0.01 && G_PauseMenu.Pause) return;
            if (Time.timeScale == 0.0f) return;
            _PitchSmoothing.Update(PitchSmoothing);
            base.Update();
        }
    }
}