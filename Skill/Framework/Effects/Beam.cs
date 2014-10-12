using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Effects
{

    /// <summary>
    /// Defins parameters for each repeatable section of beam
    /// </summary>
    [System.Serializable]
    public class BeamSection
    {
        /// <summary> Optional name of section</summary>
        public string Name = "Section";
        /// <summary> Number of vertex(by considering resolution) to use for section </summary>
        public int VertexCount = 20;
        /// <summary> Magnitude Scale of section</summary>
        public float Scale = 1.0f;
    }

    /// <summary>
    /// A Beam line
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public abstract class Beam : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Sections of beam </summary>
        public BeamSection[] Sections = new BeamSection[] { new BeamSection() };
        /// <summary> Start pivot to attach beam</summary>
        public Transform StartPivot;
        /// <summary> End pivot to attach beam</summary>
        public Transform EndPivot;
        /// <summary> Minimum amplitude </summary>
        public float MinAmplitude = 0.1f;
        /// <summary> Maximum amplitude </summary>
        public float MaxAmplitude = 0.2f;
        /// <summary> Update interval </summary>
        public float UpdateInterval = 0.01f;
        /// <summary> Length of beam </summary>
        public float Length = 50.0f;

        /// <summary> OffTime after each OffTime interval (zero or negative to disable)</summary>
        public float OffTime = 0.0f;
        /// <summary> Minimum time of between each OffTime</summary>
        public float MinOffInterval = 0.1f;
        /// <summary> Maximum time of between each OffTime </summary>
        public float MaxOffInterval = 0.2f;

        /// <summary> Resolusion of beam vertex per meter </summary>
        public int Resolution = 5;
        /// <summary> Rotate beam towards camera </summary>
        public bool Billboard = true;
        /// <summary> Lock start vertex to StartPosition </summary>
        public bool LockStart = true;
        /// <summary> Lock end vertex to HitPoint </summary>
        public bool LockEnd = true;

        private LineRenderer _LineRenderer;
        private int _VertexCount;
        private Skill.Framework.TimeWatch _UpdateIntervalTW;
        private Skill.Framework.TimeWatch _OffTimeTW;
        private float _PreLength;
        private int _PreResolution;
        private Vector3 _OffsetDirection;
        private Vector3 _Direction;
        private Vector3 _StartPosition;

        /// <summary> Point of hit(if IsHit) </summary>
        public Vector3 HitPoint { get; set; }
        /// <summary> Is beam hit something? </summary>
        public bool IsHit { get; set; }
        /// <summary> Number of vertex at current frame.</summary>
        public int VertexCount { get { return _VertexCount; } }
        /// <summary> Number of vertex used by all sections in first repeat.</summary>
        public int Repeat { get; private set; }
        /// <summary> Direction of beam offset (can be billboard to camera or transform.right )</summary>
        public Vector3 OffsetDirection { get { return _OffsetDirection; } }
        /// <summary> Direction of beam</summary>
        public Vector3 Direction { get { return _Direction; } }
        /// <summary> Where beam start</summary>
        public Vector3 StartPosition { get { return _StartPosition; } }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _LineRenderer = GetComponent<LineRenderer>();
            ValidateParameters();
        }

        /// <summary>
        /// Validate parameters to be in valid range
        /// </summary>
        protected virtual void ValidateParameters()
        {
            if (Resolution < 1) Resolution = 1; // resolution can not be less than 1
            if (Length < 0) Length = 0; // length can not be less than 0
            if (Sections == null || Sections.Length < 1) // at least one section required
                Sections = new BeamSection[] { new BeamSection() };

            // maximum amplitude can not be less than minimum amplitude
            if (MaxAmplitude < MinAmplitude) MaxAmplitude = MinAmplitude;
            // maximum off time interval can not be less than minimum time interval
            if (MaxOffInterval < MinOffInterval) MaxOffInterval = MinOffInterval;

            Repeat = 0;
            for (int i = 0; i < Sections.Length; i++)
            {
                BeamSection section = Sections[i];
                if (section.VertexCount < 2) section.VertexCount = 2; // each section must have 2 vertex at least            
                Repeat += section.VertexCount;
            }

            if (StartPivot != null && EndPivot != null)
            {
                _Direction = EndPivot.position - StartPivot.position;
                Length = _Direction.magnitude;
                _Direction.Normalize();
                _StartPosition = StartPivot.position;

            }
            else
            {
                _Direction = _Transform.forward;
                _StartPosition = _Transform.position;
            }
            if (Billboard && Camera.main != null)
                _OffsetDirection = Vector3.Cross(Camera.main.transform.forward, _Direction).normalized;
            else
                _OffsetDirection = _Transform.right;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (!Skill.Framework.Global.IsGamePaused)
            {
                // check for off time of renderer
                if (OffTime > 0)
                {
                    if (_OffTimeTW.IsOver)
                    {
                        if (_LineRenderer.enabled)
                        {
                            _LineRenderer.enabled = false;
                            _OffTimeTW.Begin(OffTime);
                        }
                        else
                        {
                            _LineRenderer.enabled = true;
                            _OffTimeTW.Begin(Random.Range(MinOffInterval, MaxOffInterval));
                        }
                    }
                }
                else if (!_LineRenderer.enabled) _LineRenderer.enabled = true;

                if (_LineRenderer.enabled)
                {
                    ValidateParameters();

                    //Shoot our laserbeam forwards!
                    UpdateLength();

                    if (_UpdateIntervalTW.IsOver)
                    {
                        // let subclass update beam
                        UpdateBeamRenderer(_LineRenderer, _VertexCount);

                        if (UpdateInterval < 0) UpdateInterval = 0;
                        _UpdateIntervalTW.Begin(UpdateInterval);
                    }

                    if (LockStart)
                    {
                        _LineRenderer.SetPosition(0, _StartPosition);
                    }
                    if (LockEnd)
                    {
                        if (IsHit)
                            _LineRenderer.SetPosition(_VertexCount - 1, HitPoint);
                        else
                            _LineRenderer.SetPosition(_VertexCount - 1, _StartPosition + (_Direction * Length));
                    }
                }
            }
            base.Update();
        }

        private void UpdateLength()
        {
            if (Length < 0.01f) Length = 0.01f;
            if (_PreLength != Length || _PreResolution != Resolution)
            {
                _PreResolution = Resolution;
                _PreLength = Length;
                _VertexCount = Mathf.RoundToInt(Length) + 1;
                _VertexCount = Mathf.Max(_VertexCount, 2);
                _VertexCount *= Resolution;
                _LineRenderer.SetVertexCount(_VertexCount);
            }
        }

        /// <summary>
        /// Update beam renderer
        /// </summary>
        /// <param name="renderer">The LineRenderer use to render beam</param>
        /// <param name="vertexCount">Number of vertex</param>
        protected abstract void UpdateBeamRenderer(LineRenderer renderer, int vertexCount);


    }

}