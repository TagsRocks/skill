using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Skill.Framework.Effects
{
    /// <summary>
    /// A wave beam
    /// </summary>    
    public class WaveBeam : Beam
    {
        /// <summary> Curve of wave</summary>
        public AnimationCurve Wave;
        /// <summary> Speed of wave</summary>
        public float Speed = 1.0f;
        /// <summary> amount of time per meter</summary>
        public float TimePreMeter = 0.2f;
        /// <summary> min noise factor </summary>
        public float MinNoise = 0;
        /// <summary> min noise factor </summary>
        public float MaxNoise = 0;
        /// <summary> apply noise relative to wave amplitude or absolute to wave </summary>
        public bool RelativeNoise = true;
        /// <summary> Speed of wave</summary>
        public float Scale = 1.0f;

        /// <summary> PingPong postWrapMode or Loop </summary>
        public bool PingPong = false;

        protected override void ValidateParameters()
        {
            base.ValidateParameters();
            if (TimePreMeter < 0.001f) TimePreMeter = 0.001f;
        }

        /// <summary>
        /// Update beam points
        /// </summary>
        /// <param name="points">The array of values to update</param>        
        protected override void UpdatePoints(float[] values)
        {
            WrapMode mode = PingPong ? WrapMode.PingPong : WrapMode.Loop;
            Wave.preWrapMode = Wave.postWrapMode = mode;

            float noise = 1;
            if (RelativeNoise)
                noise = Random.Range(MinNoise, MaxNoise);

            float step = (base.Length / (values.Length - 1)) * TimePreMeter;
            float t = Time.time * Speed;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Wave.Evaluate(t);

                if (RelativeNoise)
                    values[i] += values[i] * noise;
                else
                    values[i] += Random.Range(MinNoise, MaxNoise);

                values[i] *= Scale;
                t += step;
            }
        }
    }

}