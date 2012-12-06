using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Skill.Managers;

namespace Skill.Weapons
{
    #region ShootMode
    /// <summary>
    /// Number of bullet at each fire command
    /// </summary>
    public enum ShootMode
    {
        /// <summary> Continue shooting until out of ammo or stop command.</summary>
        Infinite = 0,
        /// <summary> By each fire command it shoots one bullet.</summary>
        One = 1,
        /// <summary> By each fire command it shoots two bullets.</summary>
        Two = 2,
        /// <summary> By each fire command it shoots three bullets.</summary>
        Three = 3
    }
    #endregion

    #region EventHandlers and EventArgs
    /// <summary>
    /// containing Weapon shoot event data.
    /// </summary>
    public class WeaponShootEventArgs : EventArgs
    {
        /// <summary> Amount of consumed ammo </summary>
        public int ConsumedAmmo { get; private set; }


        /// <summary>
        /// Create WeaponShootEventArgs
        /// </summary>
        /// <param name="consumedAmmo"> Amount of consumed ammo </param>
        public WeaponShootEventArgs(int consumedAmmo)
        {
            this.ConsumedAmmo = consumedAmmo;
        }
    }

    /// <summary>
    /// containing Weapon reload event data.
    /// </summary>
    public class WeaponReloadEventArgs : EventArgs
    {
        /// <summary> Is complete reload (when clip is empty and reload happened) </summary>
        public bool IsComplete { get; private set; }


        /// <summary>
        /// Create WeaponReloadEventArgs
        /// </summary>
        /// <param name="isComplete"> Is complete reload (when clip is empty and reload happened) </param>
        public WeaponReloadEventArgs(bool isComplete)
        {
            this.IsComplete = isComplete;
        }
    }

    /// <summary>
    /// containing Weapon change projectile event data.
    /// </summary>
    public class WeaponChangeProjectileEventArgs : EventArgs
    {
        /// <summary> Index of previous projectile </summary>
        public int PreviousProjectileIndex { get; private set; }
        /// <summary> Index of new selected projectile </summary>
        public int NewProjectileIndex { get; private set; }

        /// <summary>
        /// Create WeaponChangeProjectileEventArgs
        /// </summary>
        /// <param name="previousProjectileIndex">Index of previous projectile</param>
        /// <param name="newProjectileIndex">Index of new selected projectile</param>
        public WeaponChangeProjectileEventArgs(int previousProjectileIndex, int newProjectileIndex)
        {
            this.PreviousProjectileIndex = previousProjectileIndex;
            this.NewProjectileIndex = newProjectileIndex;
        }
    }

    /// <summary>
    /// Handle weapon shoot event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> An WeaponShootEventHandler that contains shoot event data. </param>
    public delegate void WeaponShootEventHandler(object sender, WeaponShootEventArgs args);

    /// <summary>
    /// Handle weapon reload event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> An WeaponReloadEventHandler that contains reload event data. </param>
    public delegate void WeaponReloadEventHandler(object sender, WeaponReloadEventArgs args);

    /// <summary>
    /// Handle weapon change projectile event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args"> An WeaponChangeProjectileEventHandler that contains projectile change event data. </param>
    public delegate void WeaponChangeProjectileEventHandler(object sender, WeaponChangeProjectileEventArgs args);

    #endregion

    /// <summary>
    /// Defines base class for weapons
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public abstract class BaseWeapon : DynamicBehaviour
    {
        /// <summary> Name of weapon </summary>
        public string Name = "Weapon";
        /// <summary> Profile of weapon. Useful for AnimationTree </summary>
        public string Profile = "Default";
        /// <summary> Projectiles. should contain at least one projectile </summary>
        public Projectile[] Projectiles;
        /// <summary> sound to play on empty fire </summary>
        public AudioClip EmptySound;
        /// <summary> How long does it take to Equip this weapon </summary>
        public float EquipTime = 2;
        /// <summary> How long does it take to put this weapon down </summary>
        public float PutDownTime = 0.5f;
        /// <summary> Can player toss his weapon out? Typically false for default inventory. </summary>
        public bool CanThrow = false;
        /// <summary> Reload automatically when clip is empty</summary>
        public bool AutoReload = true;
        /// <summary> Local position of weapon when mounted in player hands </summary>
        public Vector3 LocalPosition;
        /// <summary> Local rotation of weapon when mounted in player hands </summary>
        public Vector3 LocalRotation;
        /// <summary> If true, weapon will destory itself when controller is null </summary>
        public bool SelfDestory = true;
        /// <summary> How long does it take to destroy itself after controller is null</summary>
        public float DestroyTime = 20.0f;
        /// <summary> Error in shooting </summary>
        public Vector2 Spread = Vector2.zero;



        /// <summary> State of weapon </summary>
        public WeaponState State { get; private set; }
        /// <summary> This function checks to see if the weapon has any ammo available </summary>
        public virtual bool HasAmmo { get { return CurrentProjectile.TotalAmmo > 0; } }
        /// <summary> Is trigger down and weapon keeps firing </summary>
        public bool IsFiring { get; private set; }
        /// <summary> Number of shot in each fire command. </summary>
        public ShootMode Mode { get; set; }
        /// <summary> Who holds this weapon? modify this value when equip or drop weapon.</summary>
        public virtual Skill.Controller Controller { get; set; }
        /// <summary> Current equipped projectile </summary>
        public Projectile CurrentProjectile { get { return Projectiles[_SelectedProjectile]; } }
        /// <summary> Can fire immediately? </summary>
        public virtual bool CanFire { get { return State == WeaponState.Ready && CurrentClipAmmo > 0; } }
        /// <summary> Number of ammo in current clip </summary>
        public int CurrentClipAmmo { get { return CurrentProjectile.ClipAmmo; } set { CurrentProjectile.ClipAmmo = value; } }
        /// <summary> Target of weapon. can be setted by Controller </summary>
        public Transform Target { get; set; }
        /// <summary> Gets or set damage factor  </summary>
        public float DamageFactor { get; set; }

        // events

        /// <summary> Occurs when a shoot happpened </summary>
        public event WeaponShootEventHandler Shoot;
        /// <summary> Occurs when a shoot happpened </summary>
        protected virtual void OnShoot()
        {
            PlayFireSound();
            int consummed = ConsumeAmmo();
            ShootBullet(consummed);
            if (Shoot != null)
                Shoot(this, new WeaponShootEventArgs(consummed));
        }

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="consummedAmmo">Amount of consumed ammo</param>
        protected abstract Bullet ShootBullet(int consummedAmmo);

        /// <summary> Occurs when a reload happpened </summary>
        public event WeaponReloadEventHandler Reload;
        /// <summary>
        /// Occurs when a reload happpened
        /// </summary>
        /// <param name="isCompleteReload"> Is complete reload (when clip is empty and reload happened) </param>
        protected virtual void OnReload(bool isCompleteReload)
        {
            PlayReloadSound(isCompleteReload);
            if (Reload != null)
                Reload(this, new WeaponReloadEventArgs(isCompleteReload));
        }

        /// <summary> Occurs when projectile of weapon changes </summary>
        public event WeaponChangeProjectileEventHandler ProjectileChanged;
        /// <summary>
        /// Occurs when projectile of weapon changes
        /// </summary>
        /// <param name="previousProjectileIndex">Index of previous projectile</param>
        /// <param name="newProjectileIndex">Index of new selected projectile</param>
        protected virtual void OnProjectileChanged(int previousProjectileIndex, int newProjectileIndex)
        {
            if (ProjectileChanged != null)
                ProjectileChanged(this, new WeaponChangeProjectileEventArgs(previousProjectileIndex, newProjectileIndex));
        }

        // variables

        private AudioSource _AudioSource;
        private int _SelectedProjectile = 0;

        private Skill.TimeWatch _BusyTW;
        private Skill.TimeWatch _DestroyTW;
        private bool _RequestReload;
        private int _SwitchProjectileIndex;
        private int _ShootCount;

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _AudioSource = audio;
            if (_AudioSource == null)
                Debug.LogWarning("Can not find AudioSource of weapon");
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Start()
        {
            base.Start();
            DamageFactor = 1.0f;
            CurrentClipAmmo = CurrentProjectile.ClipSize;
            if (Projectiles == null || Projectiles.Length == 0)
                Debug.LogError("A weapon must have at least one projectile.");
        }

        /// <summary>
        /// Consumes ammunition when firing a shot.
        /// Subclass me to define weapon ammunition consumption. 
        /// </summary>
        /// <returns> Amount actually consummed. </returns>
        protected virtual int ConsumeAmmo()
        {
            if (!CurrentProjectile.InfinitAmmo)
            {
                if (CurrentClipAmmo > 0)
                {
                    CurrentProjectile.ClipAmmo--;
                    return -1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Add ammo to weapon
        /// Subclass me to define ammo addition rules.        
        /// </summary>
        /// <param name="damageType"> Type of ammo. if this weapon contains a projectile with this type of ammo, then add ammo  </param>
        /// <param name="amount">Amount of ammo to add</param>
        /// <returns> Amount actually added. (In case magazine is already full and some ammo is left </returns>
        public virtual int AddAmmo(int damageType, int amount)
        {
            foreach (var item in Projectiles)
            {
                if (item.DamageType == damageType)
                {
                    int preAmmoCount = item.TotalAmmo;
                    if (item.TotalAmmo < item.MaxAmmo)
                    {
                        int count = item.TotalAmmo + amount;
                        if (count > item.MaxAmmo) count = item.MaxAmmo;
                        else if (count < 0) count = 0;
                        item.TotalAmmo = count;
                    }

                    return item.TotalAmmo - preAmmoCount;
                }
            }
            return 0;
        }

        /// <summary>
        /// Request a reload in next update
        /// </summary>
        /// <returns>True if request accepted, otherwise false</returns>
        public bool RequestReload()
        {
            if (_RequestReload == false)
            {
                if (CurrentClipAmmo < CurrentProjectile.ClipSize)
                {
                    if (CurrentProjectile.TotalAmmo > 0 || CurrentProjectile.InfinitClip || CurrentProjectile.InfinitAmmo)
                    {
                        _RequestReload = true;
                    }
                }
                else
                    _RequestReload = false;
            }
            return _RequestReload;
        }

        /// <summary>
        /// Change projectile
        /// </summary>
        /// <param name="projectileName">Name of projectile to change</param>
        public void ChangeProjectile(string projectileName)
        {
            int projectileIndex = -1;
            for (int i = 0; i < Projectiles.Length; i++)
            {
                if (Projectiles[i] != null && Projectiles[i].Name == projectileName)
                {
                    projectileIndex = i;
                    break;
                }
            }
            if (projectileIndex > -1)
                ChangeProjectile(projectileIndex);
        }

        /// <summary>
        /// Change projectile
        /// </summary>
        /// <param name="projectileIndex">Index of projectile to change</param>
        public void ChangeProjectile(int projectileIndex)
        {
            if (projectileIndex >= 0 && projectileIndex < Projectiles.Length &&
                projectileIndex != _SelectedProjectile &&
                Projectiles[projectileIndex] != null)
            {
                _SwitchProjectileIndex = projectileIndex;
            }
        }

        /// <summary>
        /// Start fire command
        /// </summary>
        public virtual void StartFire()
        {
            if (Time.timeScale == 0)
                return;
            if (!IsFiring)
                _ShootCount = 0;
            IsFiring = true;
        }
        /// <summary>
        /// Stop fire command
        /// </summary>
        public virtual void StopFire()
        {
            IsFiring = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;
            if (Controller == null)
            {
                if (_DestroyTW.Enabled)
                {
                    if (_DestroyTW.IsOver)
                    {
                        _DestroyTW.End();
                        CacheSpawner.DestroyCache(gameObject);
                    }
                }
                else if (SelfDestory)
                    _DestroyTW.Begin(DestroyTime);

                _RequestReload = false;
                IsFiring = false;
            }
            else
            {
                _DestroyTW.End();
                if (_BusyTW.EnabledAndOver)
                {
                    _BusyTW.End();
                    State = WeaponState.Ready;
                }

                if (State == WeaponState.Ready)
                {
                    if (_RequestReload)
                    {
                        bool completeReload = CurrentClipAmmo == 0;
                        if (completeReload)
                            _BusyTW.Begin(CurrentProjectile.CompleteReloadTime);
                        else
                            _BusyTW.Begin(CurrentProjectile.ReloadTime);
                        State = WeaponState.Reloading;
                        _RequestReload = false;
                        OnReload(completeReload);
                    }
                    else if (_SwitchProjectileIndex != _SelectedProjectile)
                    {
                        int preIndex = _SwitchProjectileIndex;
                        _SelectedProjectile = _SwitchProjectileIndex;
                        _BusyTW.Begin(CurrentProjectile.EquipTime);
                        State = WeaponState.ChangeProjectile;
                        OnProjectileChanged(preIndex, _SelectedProjectile);
                    }
                    else if (IsFiring)
                    {
                        if (CurrentClipAmmo < 0) CurrentClipAmmo = 0;
                        if (CurrentClipAmmo == 0)
                        {
                            if (AutoReload)
                            {
                                if (!RequestReload())
                                {
                                    PlayEmptySound();
                                }
                                else
                                {
                                    IsFiring = false;
                                }
                            }
                            else
                            {
                                PlayEmptySound();
                                IsFiring = false;
                            }
                        }
                        else
                        {
                            OnShoot();
                            _BusyTW.Begin(CurrentProjectile.FireInterval);
                            State = WeaponState.Refill;
                            _ShootCount++;
                            int mode = (int)this.Mode;
                            if (mode > 0 && _ShootCount > mode)
                                StopFire();
                        }
                    }
                }

            }
            base.Update();
        }

        // play sound

        private void PlayEmptySound()
        {
            PlaySound(EmptySound);
        }

        private void PlayFireSound()
        {
            if (CurrentProjectile.FireSounds != null && CurrentProjectile.FireSounds.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, CurrentProjectile.FireSounds.Length);
                PlaySound(CurrentProjectile.FireSounds[randomIndex]);
            }
        }

        private void PlayReloadSound(bool completeReload)
        {
            if (completeReload)
            {
                if (CurrentProjectile.CompleteReloadSound != null)
                    PlaySound(CurrentProjectile.CompleteReloadSound);
                else
                    PlaySound(CurrentProjectile.ReloadSound);
            }
            else
                PlaySound(CurrentProjectile.ReloadSound);
        }

        private void PlaySound(AudioClip sound)
        {
            if (_AudioSource != null && sound != null)
            {
                if (Global.Instance != null)
                {
                    Global.Instance.PlayOneShot(_AudioSource, sound, Sounds.SoundCategory.FX);
                }
                else
                {
                    _AudioSource.PlayOneShot(sound);
                }
            }
        }
    }

}