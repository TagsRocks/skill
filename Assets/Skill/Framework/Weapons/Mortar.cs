using UnityEngine;
using System.Collections;
using Skill.Framework;

namespace Skill.Framework.Weapons
{
    public class Mortar : DynamicBehaviour
    {
        public GameObject BulletPrefab;
        public GameObject ExplosionPrefab;
        public GameObject IndicatorPrefab;// a game object with ExplosionIndicator component    
        /// <summary> Source of sound </summary>
        public AudioSource AudioSource;
        public AudioClip[] IncomingSounds;
        /// <summary> Set LifeTime to lenght of incoming sound </summary>
        public bool SoundLifeTime = true;
        public float BulletDistance = 50;
        public float ExplosionRadius = 5;
        public Vector3 ExplosionOffset = new Vector3(0, 0.1f, 0);

        private Vector3 _BulletDirection;
        private TimeWatch _MoveTimeTW;
        private Vector3 _BulletStartPosition;
        private AudioClip _Clip;

        private Transform _BulletObject;

        public float LifeTime { get; set; }

        protected override void GetReferences()
        {
            base.GetReferences();
            if (AudioSource == null)
                AudioSource = GetComponent<AudioSource>();
            if (AudioSource != null)
            {
                AudioSource.playOnAwake = false;
                AudioSource.Stop();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IncomingSounds != null && IncomingSounds.Length > 0)
            {
                _Clip = IncomingSounds[Random.Range(0, IncomingSounds.Length)];
                if (_Clip != null)
                {
                    if (SoundLifeTime)
                        LifeTime = _Clip.length - 0.05f;
                }
            }
        }

        public void Fire()
        {
            if (BulletPrefab != null && _Clip != null)
            {
                _MoveTimeTW.Begin(LifeTime);
                _BulletDirection = -transform.forward;

                _BulletStartPosition = transform.position + transform.forward * BulletDistance;

                if (BulletPrefab != null)
                {
                    GameObject go = Skill.Framework.Managers.Cache.Spawn(BulletPrefab, _BulletStartPosition, Quaternion.LookRotation(_BulletDirection));
                    _BulletObject = go.transform;
                }

                if (IndicatorPrefab != null)
                {
                    GameObject go = Skill.Framework.Managers.Cache.Spawn(IndicatorPrefab, transform.position, IndicatorPrefab.transform.rotation);
                    Skill.Framework.Effects.ExplosionIndicator indicator = go.GetComponent<Skill.Framework.Effects.ExplosionIndicator>();
                    if (indicator != null)
                    {
                        indicator.LifeTime = LifeTime;
                        indicator.Radius = ExplosionRadius;
                        indicator.Show();
                    }
                }

                if (AudioSource != null)
                    AudioSource.PlayOneShot(_Clip);
            }
            else
            {
                Debug.LogWarning("Invalid Mortar parameters");
            }
        }

        protected virtual void ShowExplosion()
        {
            if (ExplosionPrefab != null)
                Skill.Framework.Managers.Cache.Spawn(ExplosionPrefab, transform.position + ExplosionOffset, ExplosionPrefab.transform.rotation);

            if (_BulletObject != null)
                Skill.Framework.Managers.Cache.DestroyCache(_BulletObject.gameObject);
        }
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_MoveTimeTW.IsEnabled)
            {
                if (_MoveTimeTW.IsOver)
                {
                    _MoveTimeTW.End();
                    ShowExplosion();
                    DestroySelf();
                }
                else
                    _BulletObject.position = Vector3.Lerp(_BulletStartPosition, transform.position, _MoveTimeTW.Percent);
            }
            base.Update();
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            GizmosHelper.DrawArrow(transform.position + transform.forward * 3, transform.position + transform.forward * 0.35f, 0.3f);
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
