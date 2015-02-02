using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Effects
{
    /// <summary>
    /// A beam that use random between min and max amplitude each frame.
    /// </summary>    
    public class RandomizeBeam : Beam
    {
        /// <summary> Minimum amplitude </summary>
        public float MinAmplitude = 0.1f;
        /// <summary> Maximum amplitude </summary>
        public float MaxAmplitude = 0.2f;


        protected override void ValidateParameters()
        {
            base.ValidateParameters();
            // maximum amplitude can not be less than minimum amplitude
            if (MaxAmplitude < MinAmplitude) MaxAmplitude = MinAmplitude;
        }

        /// <summary>
        /// Update beam points
        /// </summary>
        /// <param name="points">The array of values to update</param>        
        protected override void UpdatePoints(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = Random.Range(MinAmplitude, MaxAmplitude);
        }
    }

}