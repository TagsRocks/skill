using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules.Vehicles
{
    /// <summary>
    /// This behaviour is useful for situations like when a static car placed besides of street and you want it's wheel be puncturable but without using RigidBodies.
    /// assign this components to chassis and set reference of chassis to each child wheel.
    /// </summary>
    [AddComponentMenu("Skill/Modules/Vehicles/PuncturableChassis")]
    public class PuncturableChassis : DynamicBehaviour
    {
        /// <summary> Parameters of how smooth rotation of chassis when a wheel is punctured </summary>
        public SmoothingParameters PunctureSmoothing;

        private Smoothing3 _Euler;
        private TimeWatch _UpdateTW;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            enabled = false;
            _Euler.Current = _Euler.Target = transform.rotation.eulerAngles;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;
            _Euler.Update(PunctureSmoothing);
            transform.rotation = Quaternion.Euler(_Euler.Current);

            if (!_UpdateTW.IsEnabled || _UpdateTW.IsEnabledAndOver)
            {
                _UpdateTW.End();
                enabled = false;
            }

            base.Update();
        }

        /// <summary>
        /// Notify chassis that a wheel is punctured
        /// </summary>
        /// <param name="puncturedWheel"> Punctured wheel </param>
        public virtual void NotifyWheelPuncture(PuncturableWheel puncturedWheel)
        {
            _Euler.Target += puncturedWheel.PunctureRotation;
            //transform.position -= new Vector3(0, wheel.DeadRadius, 0);
            _UpdateTW.Begin(PunctureSmoothing.SmoothTime + 0.5f);
            enabled = true;
        }
    }
}