using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Effects
{
    /// <summary>
    /// A Beam line
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public abstract class Beam : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Start pivot to attach beam</summary>
        public Transform StartPivot;
        /// <summary> End pivot to attach beam</summary>
        public Transform EndPivot;
        /// <summary> Update interval </summary>
        public float UpdateInterval = 0.1f;
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

        /// <summary> Fade in percent start of beam (0.0 - 1.0) </summary>
        public float FadeIn = 0;
        /// <summary> Fade out percent end of beam (0.0 - 1.0) </summary>
        public float FadeOut = 0;


        private LineRenderer _LineRenderer;
        private int _VertexCount;
        private Skill.Framework.TimeWatch _UpdateIntervalTW;
        private Skill.Framework.TimeWatch _OffTimeTW;
        private float _PreLength;
        private int _PreResolution;
        private Vector3 _Direction;
        private Vector3 _StartPosition;
        private Vector3 _EndPosition;
        private bool _IsHit;
        private Vector3 _HitPoint;
        private Vector3[] _PrePoints;
        private Vector3[] _NewPoints;
        private float[] _Values;

        /// <summary> Point of hit(if IsHit) </summary>
        public Vector3 HitPoint { get { return _HitPoint; } }

        /// <summary> Is beam hit something? </summary>
        public bool IsHit { get { return _IsHit; } }

        /// <summary> Number of vertex at current frame.</summary>
        public int VertexCount { get { return _VertexCount; } }

        /// <summary> Direction of beam</summary>
        public Vector3 Direction { get { return _Direction; } }
        /// <summary> Where beam starts</summary>
        public Vector3 StartPosition { get { return _StartPosition; } }

        /// <summary> Where beam ends</summary>
        public Vector3 EndPosition { get { return _EndPosition; } }

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
            if (UpdateInterval < 0.0f) UpdateInterval = 0.0f;
            if (Resolution < 1) Resolution = 1; // resolution can not be less than 1
            if (Length < 0) Length = 0; // length can not be less than 0            

            if (OffTime < 0) OffTime = 0;
            if (MinOffInterval < 0) MinOffInterval = 0;
            // maximum off time interval can not be less than minimum time interval
            if (MaxOffInterval < MinOffInterval) MaxOffInterval = MinOffInterval;

            bool updateLength = true;
            if (StartPivot != null && EndPivot != null)
            {
                _StartPosition = StartPivot.position;
                _EndPosition = EndPivot.position;

            }
            else
            {
                if (StartPivot != null)
                    _StartPosition = StartPivot.position;
                else
                    _StartPosition = _Transform.position;

                if (IsHit)
                    _EndPosition = HitPoint;
                else if (EndPivot != null)
                    _EndPosition = EndPivot.position;
                else
                {
                    _EndPosition = _StartPosition + (_Transform.forward * Length);
                    updateLength = false;
                }
            }

            _Direction = _EndPosition - _StartPosition;
            float len = MathHelper.NormalizeAndMagnitude(ref _Direction);
            if (updateLength) Length = len;
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
                        if (FadeIn < 0) FadeIn = 0;
                        if (FadeIn > 1.0f) FadeIn = 1.0f;
                        float maxFadeOut = 1.0f - FadeIn;
                        if (FadeOut < 0) FadeOut = 0;
                        if (FadeOut > maxFadeOut) FadeOut = maxFadeOut;

                        // let subclass update beam                        
                        UpdatePoints(_Values);
                        System.Array.Copy(_NewPoints, 0, _PrePoints, 0, _VertexCount);

                        Vector3 sideDirection;
                        if (Billboard && Camera.main != null)
                            sideDirection = Vector3.Cross(Camera.main.transform.forward, _Direction).normalized;
                        else
                            sideDirection = _Transform.right;


                        Vector3 step = _Direction * (Length / (VertexCount - 1));
                        Vector3 pos = _StartPosition; // start width StartPosition                                    
                        float percentStep = 1.0f / (_VertexCount - 1);
                        float p = 0;
                        for (int i = 0; i < _VertexCount; i++)
                        {
                            _NewPoints[i] = pos + ((_Values[i] * CalcScale(p)) * sideDirection);

                            p += percentStep;
                            pos += step;
                        }
                        
                        _UpdateIntervalTW.Begin(UpdateInterval);
                    }


                    if (_UpdateIntervalTW.Length > 0.001f)
                    {
                        float percent = _UpdateIntervalTW.Percent;
                        for (int i = 0; i < _VertexCount; i++)
                            _LineRenderer.SetPosition(i, Vector3.Lerp(_PrePoints[i], _NewPoints[i], percent));
                    }
                    else
                    {
                        for (int i = 0; i < _VertexCount; i++)
                            _LineRenderer.SetPosition(i, _NewPoints[i]);
                    }

                    if (LockStart)
                        _LineRenderer.SetPosition(0, _StartPosition);
                    if (LockEnd)
                        _LineRenderer.SetPosition(_VertexCount - 1, _EndPosition);

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

            if (_NewPoints == null || _NewPoints.Length != _VertexCount)
            {
                var preValues = _NewPoints;
                _NewPoints = new Vector3[_VertexCount];

                if (preValues != null && preValues.Length > 0)
                    System.Array.Copy(preValues, 0, _NewPoints, 0, Mathf.Min(_VertexCount, preValues.Length));
            }

            if (_PrePoints == null || _PrePoints.Length != _VertexCount)
            {
                var preValues = _PrePoints;
                _PrePoints = new Vector3[_VertexCount];

                if (preValues != null && preValues.Length > 0)
                    System.Array.Copy(preValues, 0, _PrePoints, 0, Mathf.Min(_VertexCount, preValues.Length));
            }

            if (_Values == null || _Values.Length != _VertexCount)
            {
                var preValues = _Values;
                _Values = new float[_VertexCount];

                if (preValues != null && preValues.Length > 0)
                    System.Array.Copy(preValues, 0, _Values, 0, Mathf.Min(_VertexCount, preValues.Length));
            }
        }


        /// <summary>
        /// Calc scale
        /// </summary>
        /// <param name="percent">percent vertex throgh beam</param>
        /// <returns>Scale</returns>
        private float CalcScale(float percent)
        {
            if (percent < FadeIn)
                return percent / FadeIn;
            else if (percent > 1.0f - FadeOut)
                return (1.0f - percent) / FadeOut;
            else
                return 1.0f;
        }

        /// <summary>
        /// Set hit point
        /// </summary>
        /// <param name="hit">where to hit</param>
        public void SetHitPoint(Vector3 hit)
        {
            _IsHit = true;
            _HitPoint = hit;
        }

        /// <summary>
        /// Ignore hit point
        /// </summary>
        public void FreeHit()
        {
            _IsHit = false;
        }

        /// <summary>
        /// Update beam points
        /// </summary>
        /// <param name="points">The array of values to update</param>        
        protected abstract void UpdatePoints(float[] values);


    }

}