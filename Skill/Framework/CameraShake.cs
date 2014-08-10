﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Shakes camera. arrange components in a way that CameraShake component update after camera controller
    /// </summary>    
    public class CameraShake : DynamicBehaviour
    {
        public Transform RelativeTo;
        public float Smoothing = 50.0f;

        private CameraShakeParams _Shake;
        private TimeWatch _ShakeTimeTW;
        private TimeWatch _TickTimeTW;
        private TimeWatch _DisableTW;
        private Vector3 _DeltaPosition;
        private Vector3 _CurrentDeltaPosition;
        private float _DeltaRoll;
        private float _CurrentDeltaRoll;

        protected override void Start()
        {
            base.Start();
            Global.CameraShake += OnCameraShake;
        }

        /// <summary>
        /// Unhook hooked events 
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            Global.CameraShake -= OnCameraShake;
        }

        // binded to Global.CameraShake event
        private void OnCameraShake(object sender, CameraShakeEventArgs args)
        {
            Shake(args.Shake, args.Source);
        }

        /// <summary>
        /// Shake camera
        /// </summary>
        /// <param name="shakeInfo">Parameters of shake</param>
        /// <param name="sourceOfShake"> source of shake </param>
        public virtual void Shake(CameraShakeParams shakeInfo, Vector3 sourceOfShake)
        {
            if (shakeInfo.Enable)
            {
                if (this._Shake == null)
                    this._Shake = new CameraShakeParams();

                if (RelativeTo == null) RelativeTo = _Transform;

                float distanceToSource = Vector3.Distance(RelativeTo.position, sourceOfShake);
                if (distanceToSource <= shakeInfo.Range)
                {
                    this._Shake.CopyFrom(shakeInfo);
                    if (this._Shake.Range <= Mathf.Epsilon) this._Shake.Range = Mathf.Epsilon;
                    float modifier = (this._Shake.ByDistance) ? (1 - Mathf.Clamp01(distanceToSource / this._Shake.Range)) : 1.0f;
                    this._Shake.Roll *= modifier;
                    this._Shake.Intensity *= modifier;
                    if (this._Shake.TickTime < 0) this._Shake.TickTime = 0;
                    this._ShakeTimeTW.Begin(this._Shake.Duration);
                    this.Tick();
                    base.enabled = true;
                }
            }
        }

        private void Tick()
        {
            Vector3 right = _Transform.right;
            Vector3 up = _Transform.up;
            Vector3 forward = _Transform.forward;
            Vector3 position = _Transform.position;
            Quaternion rotation = _Transform.rotation;

            _DeltaPosition = Vector3.zero;
            if (_Shake.Intensity.x > 0)
                _DeltaPosition += right * UnityEngine.Random.Range(-this._Shake.Intensity.x, this._Shake.Intensity.x);
            if (_Shake.Intensity.y > 0)
                _DeltaPosition += up * UnityEngine.Random.Range(-this._Shake.Intensity.y, this._Shake.Intensity.y);
            if (_Shake.Intensity.z > 0)
                _DeltaPosition += forward * UnityEngine.Random.Range(-this._Shake.Intensity.z, this._Shake.Intensity.z);

            if (_Shake.Roll > 0)
                _DeltaRoll = UnityEngine.Random.Range(-_Shake.Roll, _Shake.Roll);

            _TickTimeTW.Begin(UnityEngine.Random.Range(this._Shake.TickTime * 0.5f, this._Shake.TickTime));
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (Global.IsGamePaused) return;
            if (_DisableTW.IsEnabledAndOver)
            {
                _DisableTW.End();
                enabled = false;
            }
            if (_ShakeTimeTW.IsEnabled)
            {
                if (_ShakeTimeTW.IsEnabledAndOver)
                {
                    _ShakeTimeTW.End();
                    _TickTimeTW.End();
                    if (Smoothing > 0.001f)
                        _DisableTW.Begin(1.0f / Smoothing);
                    else
                        _DisableTW.Begin(0);
                }

                if (_ShakeTimeTW.IsEnabled && _TickTimeTW.IsOver)
                    Tick();

                _CurrentDeltaPosition = Vector3.Lerp(_CurrentDeltaPosition, _DeltaPosition, Time.deltaTime * Smoothing);
                _Transform.position += _CurrentDeltaPosition;

                if (_Shake.Roll > 0)
                {
                    _CurrentDeltaRoll = Mathf.Lerp(_CurrentDeltaRoll, _DeltaRoll, Time.deltaTime * Smoothing);
                    _Transform.rotation = Quaternion.AngleAxis(_CurrentDeltaRoll, _Transform.forward) * _Transform.rotation;
                }


            }
        }
    }

}