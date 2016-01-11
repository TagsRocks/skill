using UnityEngine;
using System.Collections;
using Skill.Framework.Weapons;

namespace Skill.Framework.Audio
{

    [RequireComponent(typeof(AudioSource))]
    public class WeaponSound : StaticBehaviour
    {
        public Weapon Weapon;
        public int Projectile = 0;// index of projectile
        public AudioClip FireSound;
        public AudioClip EndSound;

        private AudioSource _Audio;
        private bool _ShootStarted;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Audio = GetComponent<AudioSource>();
        }
        protected override void HookEvents()
        {
            base.HookEvents();
            if (this.Weapon != null)
            {
                this.Weapon.Shoot += Weapon_Shoot;
                this.Weapon.StopShoot += Weapon_StopShoot;
            }
        }

        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (this.Weapon != null)
            {
                this.Weapon.Shoot -= Weapon_Shoot;
                this.Weapon.StopShoot -= Weapon_StopShoot;
            }
        }

        void Weapon_Shoot(object sender, WeaponShootEventArgs args)
        {
            if (!_ShootStarted)
            {
                _ShootStarted = true;
                if (_Audio != null)
                {
                    if (_Audio.isPlaying)
                        _Audio.Stop();
                    _Audio.clip = FireSound;
                    _Audio.loop = true;
                    _Audio.Play();
                }
            }
        }
        void Weapon_StopShoot(object sender, System.EventArgs e)
        {
            _ShootStarted = false;
            if (_Audio != null)
            {
                if (_Audio.isPlaying)
                    _Audio.Stop();
                _Audio.clip = null;
                _Audio.loop = false;
                if (EndSound != null)
                    _Audio.PlayOneShot(EndSound);
            }
        }
    }
}