using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.Managers;

namespace Skill.Framework.Weapons
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
    [AddComponentMenu("Skill/Weapons/Weapon")]
    public class Weapon : DynamicBehaviour
    {
        /// <summary> Name of weapon </summary>
        public string Name = "Weapon";
        /// <summary> Profile of weapon. Useful for AnimationTree </summary>
        public string Profile = "Default";
        /// <summary> Projectiles. should contain at least one projectile </summary>
        public Projectile[] Projectiles;
        /// <summary> sound to play on empty fire </summary>
        public AudioClip EmptySound;
        /// <summary> Number of shot in each fire command. </summary>
        public ShootMode Mode = ShootMode.Infinite;
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
        /// <summary> Is equipped? weapon works when equipped </summary>
        public bool IsEquipped { get; set; }
        /// <summary> Current equipped projectile </summary>
        public Projectile CurrentProjectile { get { return Projectiles[_SelectedProjectile]; } }
        /// <summary> Can fire immediately? </summary>
        public virtual bool CanFire { get { return State == WeaponState.Ready && CurrentClipAmmo > 0; } }
        /// <summary> Number of ammo in current clip </summary>
        public int CurrentClipAmmo { get { return CurrentProjectile.ClipAmmo; } set { CurrentProjectile.ClipAmmo = value; } }
        /// <summary> Target of weapon. can be setted by Controller </summary>
        public Vector3? Target { get; set; }
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

            if (!CurrentProjectile.InfinitAmmo)
            {
                if (CurrentClipAmmo > consummed)
                {
                    CurrentProjectile.ClipAmmo -= consummed;
                }
                else
                {
                    CurrentProjectile.ClipAmmo = 0;
                }
            }

            Bullet[] bullets = ShootBullets(consummed);
            if (bullets != null)
            {
                for (int i = 0; i < bullets.Length; i++)
                {
                    bullets[i].Target = Target;
                }
            }

            if (Shoot != null)
                Shoot(this, new WeaponShootEventArgs(consummed));
        }

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        private Bullet[] ShootBullets(int bulletCount)
        {
            if (CurrentProjectile != null)
            {
                switch (CurrentProjectile.Type)
                {
                    case ProjectileType.StraightLine:
                        return ShootStraightLineBullets(bulletCount);
                    case ProjectileType.Raycast:
                        return ShootRaycastBullets(bulletCount);
                    case ProjectileType.Curve:
                        return ShootCurveBullets(bulletCount);
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Invalid bullet to shoot");
            }
            return null;
        }

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

        /// <summary> Occurs when a reload completed and clip refills </summary>
        public event EventHandler ReloadCompleted;
        /// <summary>
        /// Occurs when a reload completed and clip refills
        /// </summary>        
        protected virtual void OnReloadCompleted()
        {
            int ammoToFillClip = CurrentProjectile.ClipSize - CurrentClipAmmo;

            if (!CurrentProjectile.InfinitClip)
            {
                if (CurrentProjectile.TotalAmmo < ammoToFillClip)
                    ammoToFillClip = CurrentProjectile.TotalAmmo;
                CurrentProjectile.TotalAmmo -= ammoToFillClip;
            }
            CurrentClipAmmo += ammoToFillClip;

            if (ReloadCompleted != null)
                ReloadCompleted(this, EventArgs.Empty);
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

        private TimeWatch _BusyTW;
        private TimeWatch _DestroyTW;
        private bool _RequestReload;
        private float _RequestBusy;
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
            if (Projectiles == null || Projectiles.Length == 0)
                Debug.LogError("A weapon must have at least one projectile.");
            if (CurrentProjectile.TotalAmmo <= 0)
                CurrentProjectile.TotalAmmo = CurrentProjectile.DefaultAmmo;
            CurrentClipAmmo = CurrentProjectile.ClipSize;
        }

        /// <summary>
        /// Consumes ammunition when firing a shot.
        /// Subclass me to define weapon ammunition consumption. 
        /// </summary>
        /// <returns> Amount actually consummed. </returns>
        protected virtual int ConsumeAmmo()
        {
            return 1;
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
        /// Set weapon busy
        /// </summary>
        /// <param name="busyTime"> How long to be busy </param>
        public void SetBusy(float busyTime)
        {
            if (busyTime > 0)
                _RequestBusy = busyTime;
            else
                _RequestBusy = 0;
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

            if (State == WeaponState.Ready)
            {
                if (!IsFiring)
                    _ShootCount = 0;
                IsFiring = true;
            }
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
            if (!IsEquipped)
            {
                if (_DestroyTW.IsEnabled)
                {
                    if (_DestroyTW.IsOver)
                    {
                        _DestroyTW.End();
                        Cache.DestroyCache(gameObject);
                    }
                }
                else if (SelfDestory)
                    _DestroyTW.Begin(DestroyTime);


                if (State == WeaponState.ChangeProjectile)
                    _SelectedProjectile = _SwitchProjectileIndex; //cancel change projectile
                _RequestReload = false; // cancel reload
                IsFiring = false;
                _BusyTW.End();
                State = WeaponState.Busy;
            }
            else
            {
                _DestroyTW.End();
                if (_BusyTW.IsEnabledAndOver)
                {
                    _BusyTW.End();
                    if (State == WeaponState.Reloading)
                    {
                        OnReloadCompleted();
                    }
                    else if (State == WeaponState.ChangeProjectile)
                    {
                        OnProjectileChanged(_SelectedProjectile, _SwitchProjectileIndex);
                        _SwitchProjectileIndex = _SelectedProjectile;
                    }
                    State = WeaponState.Ready;
                }

                if (State == WeaponState.Ready)
                {
                    if (_RequestReload && CurrentProjectile.TotalAmmo > 0)
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
                    else if (_RequestBusy > 0)
                    {
                        _BusyTW.Begin(_RequestBusy);
                        State = WeaponState.Busy;
                        _RequestBusy = 0;
                    }
                    else if (_SwitchProjectileIndex != _SelectedProjectile)
                    {
                        _BusyTW.Begin(CurrentProjectile.EquipTime);
                        State = WeaponState.ChangeProjectile;
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

                            if (CurrentClipAmmo == 0)
                            {
                                if (AutoReload)
                                {
                                    RequestReload();
                                    _ShootCount = 0;
                                }
                            }
                            else
                            {
                                _BusyTW.Begin(CurrentProjectile.FireInterval);
                                State = WeaponState.Refill;
                            }
                            _ShootCount++;
                            int mode = (int)this.Mode;
                            if (mode > 0 && _ShootCount >= mode)
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
                    Global.Instance.PlaySoundOneShot(_AudioSource, sound, Sounds.SoundCategory.FX);
                }
                else
                {
                    _AudioSource.PlayOneShot(sound);
                }
            }
        }


        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        protected virtual Bullet[] ShootCurveBullets(int bulletCount)
        {
            Bullet[] bullets = new Bullet[bulletCount];

            Quaternion iniRotation = Quaternion.identity;

            switch (CurrentProjectile.CurveParams.InitialRotation)
            {
                case InitialCurveProjectileRotation.Forward:
                    Quaternion.LookRotation(_Transform.forward, _Transform.up);
                    break;
                case InitialCurveProjectileRotation.AbsoluteCustom:
                    iniRotation = CurrentProjectile.CurveParams.Rotation;
                    break;
                case InitialCurveProjectileRotation.RelativeCustom:
                    iniRotation = CurrentProjectile.CurveParams.Rotation * _Transform.rotation;
                    break;
            }

            for (int i = 0; i < bulletCount; i++)
            {
                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, iniRotation) as GameObject;

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                }

                Rigidbody rb = go.rigidbody;
                if (rb != null)
                {
                    if (rb.collider != null)
                    {
                        Rigidbody weaponRB = this.rigidbody;
                        if (weaponRB != null && weaponRB.collider)
                            Physics.IgnoreCollision(weaponRB.collider, rb.collider);
                    }
                    float range = CurrentProjectile.Range;
                    Vector3 dir = _Transform.forward;
                    dir.y = 0;

                    if (Target != null && Target.HasValue)
                    {
                        Vector3 target = Target.Value;
                        Vector3 position = _Transform.position;
                        dir = target - position;
                        dir.y = 0;
                        range = dir.magnitude;
                        if (range > CurrentProjectile.Range)
                            range = CurrentProjectile.Range;
                    }

                    dir.Normalize();
                    Quaternion rotation = Quaternion.AngleAxis(CurrentProjectile.CurveParams.ThrowAngle, dir);

                    float speed = (Physics.gravity * range).sqrMagnitude;
                    dir = rotation * Vector3.forward;
                    rb.AddForce(dir * speed * rb.mass, ForceMode.Impulse);

                    bullet.Direction = dir;
                    bullet.Speed = speed;
                }

                bullet.StartJourney();
                bullets[i] = bullet;
            }
            return bullets;
        }

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        protected virtual Bullet[] ShootStraightLineBullets(int bulletCount)
        {
            Bullet[] bullets = new Bullet[bulletCount];

            Vector3 direction;
            if (Target != null && Target.HasValue)
                direction = (Target.Value - CurrentProjectile.SpawnPoint.position).normalized;
            else
                direction = _Transform.forward;

            for (int i = 0; i < bulletCount; i++)
            {
                Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, bulletRotation) as GameObject;
                Rigidbody rb = go.rigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    if (rb.collider != null)
                    {
                        Rigidbody weaponRB = this.rigidbody;
                        if (weaponRB != null && weaponRB.collider)
                            Physics.IgnoreCollision(weaponRB.collider, rb.collider);
                    }

                    if (rb.useGravity)
                    {
                        rb.AddForce(bulletDirection * CurrentProjectile.InitialSpeed, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddForce(bulletDirection * CurrentProjectile.InitialSpeed, ForceMode.VelocityChange);
                    }
                }

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                }

                bullet.StartJourney();
                bullets[i] = bullet;
            }
            return bullets;
        }

        private RaycastHit _HitInfo;
        private Ray _Ray;

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        protected virtual Bullet[] ShootRaycastBullets(int bulletCount)
        {
            Bullet[] bullets = new Bullet[bulletCount];

            Vector3 direction;
            if (Target != null && Target.HasValue)
                direction = (Target.Value - CurrentProjectile.SpawnPoint.position).normalized;
            else
                direction = _Transform.forward;

            for (int i = 0; i < bulletCount; i++)
            {
                Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, bulletRotation, false) as GameObject;
                StraightLineBullet bullet = go.GetComponent<StraightLineBullet>();// get reference to bullet

                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                    bullet.LayerMask = CurrentProjectile.LayerMask;

                    if (CurrentProjectile.HitAtSpawn) // if is is needed to check hit at spawn time
                    {
                        _Ray.direction = bulletDirection;
                        _Ray.origin = CurrentProjectile.SpawnPoint.position;

                        if (Physics.Raycast(_Ray, out _HitInfo, CurrentProjectile.Range, CurrentProjectile.LayerMask))
                        {
                            bullet.Range = _HitInfo.distance;

                            EventManager events = _HitInfo.collider.GetComponent<EventManager>();
                            if (events != null)
                            {
                                RaycastHitEventArgs info = new RaycastHitEventArgs(bullet.Shooter, HitType.Bullet | HitType.Raycast, _HitInfo.collider);
                                info.Damage = bullet.Damage;
                                info.Tag = this.tag;
                                info.Normal = _HitInfo.normal;
                                info.Point = _HitInfo.point;
                                info.RaycastHit = _HitInfo;
                                events.OnHit(this, info);
                            }
                        }
                    }
                }
                bullet.StartJourney();
                go.SetActive(true);
                bullets[i] = bullet;
            }
            return bullets;
        }
    }

}