using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Shakes camera. arrange components in a way that CameraShake component update after camera controller
    /// </summary>
    [AddComponentMenu("Skill/Camera/Shake")]
    public class CameraShake : DynamicBehaviour
    {

        private CameraShakeInfo _Shake;
        private TimeWatch _ShakeTime;

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
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
        public virtual void Shake(CameraShakeInfo shakeInfo, Vector3 sourceOfShake)
        {

            float distanceToSource = Vector3.Distance(_Transform.position, sourceOfShake);
            if (distanceToSource <= shakeInfo.Range)
            {
                this._Shake = new CameraShakeInfo(shakeInfo);
                float modifier = (1 - (distanceToSource / this._Shake.Range));
                this._Shake.Roll *= modifier;
                this._Shake.SideIntensity *= modifier;
                this._Shake.UpDownIntensity *= modifier;
                //this._Shake.Duration *= modifier;
                this._ShakeTime.Begin(this._Shake.Duration);
                base.enabled = true;
            }
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (_ShakeTime.Enabled)
            {
                if (_ShakeTime.IsOver)
                {
                    _ShakeTime.End();
                    enabled = false;
                }
                else
                {
                    Vector3 right = _Transform.right;
                    Vector3 up = _Transform.up;
                    Vector3 forward = _Transform.forward;
                    Vector3 position = _Transform.position;
                    Quaternion rotation = _Transform.rotation;

                    if (_Shake.SideIntensity > 0)
                        position += right * UnityEngine.Random.Range(-this._Shake.SideIntensity, this._Shake.SideIntensity);
                    if (_Shake.UpDownIntensity > 0)
                        position += up * UnityEngine.Random.Range(-this._Shake.UpDownIntensity, this._Shake.UpDownIntensity);

                    if (_Shake.Roll > 0)
                        rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-_Shake.Roll, _Shake.Roll), forward) * rotation;

                    _Transform.position = position;
                    _Transform.rotation = rotation;
                }

            }
        }
    }

}