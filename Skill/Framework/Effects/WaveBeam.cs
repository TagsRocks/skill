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
        /// <summary> Interval between amplitude change</summary>
        public float PulseInterval = 1.0f;
        /// <summary> Noise factor </summary>
        public float Noise = 0;
        /// <summary> apply noise relative to wave amplitude or absolute to wave </summary>
        public bool RelativeNoise = true;
        /// <summary> -/+ wave between intervals </summary>
        public bool ZigZag;
        /// <summary> do not move between min and max amplitudes </summary>
        public bool Fixed;
        /// <summary> shift wave each update interval </summary>
        public bool Shift;
        /// <summary> Max buffer to use for shift</summary>
        public int ShiftBuffer = 512; // 512 * 4(sizr of float)  = 2048(2kb)

        private Skill.Framework.TimeWatch _PulseIntervalTW;
        private bool _Inverse = false;
        private float _FixedLerpValue;
        private float[] _ShiftValues;// buffer of shift values
        private int _ShiftIndex = 0; // index of value in _ShiftSection
        private int _ShiftSection = 0; // index of section that active when shift enable
        private float _ShiftSign = 1.0f; // sign to change when _ShiftSection changed

        class WaveFactorData { public float[] Factors;    }
        private WaveFactorData[] _WaveFactorsData;


        /// <summary>
        /// Validate parameters
        /// </summary>
        protected override void ValidateParameters()
        {
            base.ValidateParameters();
            if (ShiftBuffer < Repeat)
                ShiftBuffer = Repeat;

            if (Noise < 0) Noise = 0;

            // make sure _WaveFactorsData is peer-to-peer to Sections
            if (_WaveFactorsData == null || _WaveFactorsData.Length != Sections.Length)
            {
                var preFactors = _WaveFactorsData; // save previous calculated data
                _WaveFactorsData = new WaveFactorData[Sections.Length];

                for (int i = 0; i < _WaveFactorsData.Length; i++)
                {
                    if (preFactors != null && i < preFactors.Length)
                        _WaveFactorsData[i] = preFactors[i]; // copy from previous data
                    if (_WaveFactorsData[i] == null)
                        _WaveFactorsData[i] = new WaveFactorData();
                }
            }

            for (int i = 0; i < Sections.Length; i++)
            {
                BeamSection section = Sections[i];
                WaveFactorData waveFactors = _WaveFactorsData[i];
                if (waveFactors.Factors == null || waveFactors.Factors.Length != section.VertexCount)// if VertexCount changed by script rebuild wavefactors
                {
                    // rebuild wavefactors
                    waveFactors.Factors = CalculateWaveFactors(section.VertexCount);
                    if (waveFactors == null || waveFactors.Factors.Length != section.VertexCount)
                        throw new System.Exception("Invalid WaveFactors. lenght of WaveFactors must be equal to VertexCount");
                }
            }
        }

        /// <summary>
        /// Update beam renderer
        /// </summary>
        /// <param name="renderer">The LineRenderer use to render beam</param>
        /// <param name="vertexCount">Number of vertex</param>
        protected override void UpdateBeamRenderer(LineRenderer renderer, int vertexCount)
        {
            // change amplitude with pulse
            if (_PulseIntervalTW.IsOver)
            {
                if (PulseInterval < 0) PulseInterval = 0;
                _PulseIntervalTW.Begin(PulseInterval);
                _Inverse = !_Inverse; // change direction

                if (Fixed)
                    _FixedLerpValue = Random.Range(0.0f, 1.0f);
            }

            float noiseValue = 0;
            if (Noise > 0)
                noiseValue = Random.Range(0.0f, Noise);

            // calc step between points
            Vector3 step = Direction * (Length / (vertexCount - 1));
            Vector3 pos = StartPosition; // start width StartPosition
            Vector3 offsetDirection = base.OffsetDirection;// create local variable for better performance


            float lerpValue;
            if (Fixed)
                lerpValue = _FixedLerpValue;
            else
                lerpValue = _PulseIntervalTW.Percent; // between 0.0f - 1.0f
            if (_Inverse) lerpValue = 1.0f - lerpValue;

            int valueIndex = 0;
            float amplitude = Mathf.Lerp(MinAmplitude, MaxAmplitude, lerpValue);
            if (Shift)
            {
                // make sure _ShiftValues is correct
                if (_ShiftValues == null || _ShiftValues.Length != ShiftBuffer)
                {
                    var preValues = _ShiftValues;
                    _ShiftValues = new float[ShiftBuffer];

                    if (preValues != null && preValues.Length > 0)
                        System.Array.Copy(preValues, 0, _ShiftValues, 0, Mathf.Min(Repeat, preValues.Length));
                }

                // make sure _ShiftSection is in correct range
                if (_ShiftSection < 0 || _ShiftSection >= Sections.Length) _ShiftSection = Sections.Length - 1;

                BeamSection section = Sections[_ShiftSection];
                _ShiftIndex--;
                // make sure _ShiftIndex is in correct ranged
                if (_ShiftIndex >= section.VertexCount) _ShiftIndex = section.VertexCount - 1;
                else if (_ShiftIndex < 0) // go to next section
                {
                    _ShiftSection--;
                    // make sure _ShiftSection is in correct range
                    if (_ShiftSection < 0 || _ShiftSection >= Sections.Length) _ShiftSection = Sections.Length - 1;
                    section = Sections[_ShiftSection];
                    _ShiftIndex = section.VertexCount - 1;
                    _ShiftSign *= -1;
                }

                float amplitudeScaled = amplitude * section.Scale;
                float value = CalcValue(ref amplitudeScaled, ref _WaveFactorsData[_ShiftSection].Factors[_ShiftIndex], ref noiseValue, ref _ShiftSign);


                bool reachEnd = false; // reach end of shiftbuffer
                valueIndex = 1;
                renderer.SetPosition(0, pos + value * offsetDirection);// first index is updated each frame and other values shifts
                pos += step;// step forward
                float preValue = _ShiftValues[0]; // save previous value
                _ShiftValues[0] = value;
                for (int i = 1; i < vertexCount; i++) // update renderer positions and shift values to right
                {
                    if (valueIndex >= ShiftBuffer) // make sure valueIndex  is in correct range
                    {
                        valueIndex = 0;
                        reachEnd = true;
                    }

                    value = _ShiftValues[valueIndex]; // save previous value
                    if (!reachEnd)
                        _ShiftValues[valueIndex] = preValue;// assign previous(i- 1) value. (shift right)
                    renderer.SetPosition(i, pos + preValue * offsetDirection); // update renderer position

                    preValue = value;// save value for next itration

                    valueIndex++; // advance valueIndex
                    pos += step; // step forward

                }
            }
            else
            {
                float sign = 1.0f;
                if (ZigZag) sign = _Inverse ? -1.0f : 1.0f; // if ZigZag, change sign at each pulse
                int vertexIndex = 0; // index of vertex (0 - vertexCount)
                int sectionIndex = 0; // index of section (0 - Sections.Length)
                BeamSection section = Sections[sectionIndex];
                float[] waveFactors = _WaveFactorsData[sectionIndex].Factors;// waveFactors corresponding to section
                float amplitudeScaled = amplitude * section.Scale;
                for (int i = 0; i < vertexCount; i++)
                {
                    if (vertexIndex >= section.VertexCount)
                    {
                        // go to next section
                        vertexIndex = 0;
                        sectionIndex++;
                        if (sectionIndex >= Sections.Length)
                            sectionIndex = 0;
                        section = Sections[sectionIndex];
                        waveFactors = _WaveFactorsData[sectionIndex].Factors;
                        sign *= -1;
                        amplitudeScaled = amplitude * section.Scale;
                    }

                    float value = CalcValue(ref amplitudeScaled, ref waveFactors[vertexIndex], ref noiseValue, ref sign);
                    renderer.SetPosition(i, pos + value * offsetDirection);
                    vertexIndex++; // advance vertexIndex
                    pos += step;  // step forward
                }
            }

        }

        private float CalcValue(ref float amplitude, ref float waveFactor, ref float noise, ref float sign)
        {
            float value = amplitude * waveFactor;
            if (RelativeNoise)
                return (value + (noise * value)) * sign;
            else
                return value * sign + noise;
        }

        /// <summary>
        /// Calculate wave factors. by default calc sin(0 - PI)
        /// </summary>    
        /// <param name="vertexCount">Number of vertex to use in wavefactors</param>
        /// <returns>Wavefactors</returns>
        public virtual float[] CalculateWaveFactors(int vertexCount)
        {
            float[] waveFactors = new float[vertexCount];
            float stepAngle = Mathf.PI / (waveFactors.Length);
            float angle = 0;
            for (int i = 0; i < waveFactors.Length; i++)
            {
                waveFactors[i] = Mathf.Sin(angle);
                angle += stepAngle;
            }

            return waveFactors;
        }
    }

}