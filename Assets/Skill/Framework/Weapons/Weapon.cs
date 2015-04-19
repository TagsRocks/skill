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
        Continuous = 0,
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
        public ShootMode Mode = ShootMode.Continuous;
        /// <summary> Can player toss his weapon out? Typically false for default inventory. </summary>
        public bool CanThrow = false;
        /// <summary> Reload automatically when clip is empty</summary>
        public bool AutoReload = true;
        /// <summary> Error in shooting </summary>
        public Vector2 Spread = Vector2.zero;



        /// <summary> State of weapon </summary>
        public WeaponState State { get; private set; }
        /// <summary> This function checks to see if the weapon has any ammo available </summary>
        public virtual bool HasAmmo { get { return SelectedProjectile.TotalAmmo > 0; } }
        /// <summary> Is trigger down and weapon keeps firing </summary>
        public bool IsFiring { get; private set; }
        /// <summary> Current equipped projectile </summary>
        public Projectile SelectedProjectile { get { return Projectiles[_SelectedProjectileIndex]; } }
        /// <summary> Current equipped projectile </summary>
        public int SelectedProjectileIndex { get { return _SelectedProjectileIndex; } }
        /// <summary> Can fire immediately? </summary>
        public virtual bool CanFire { get { return State == WeaponState.Ready && (SelectedProjectile.ClipAmmo > 0 || SelectedProjectile.InfinitAmmo || SelectedProjectile.InfinitClip); } }
        /// <summary> Target of weapon. can be setted by Controller </summary>
        public Vector3? Target { get; set; }
        /// <summary> Gets or set damage factor  </summary>
        public float DamageFactor { get; set; }

        /// <summary> If true, weapon try to calculate initial speed of curve projectiles to hit Target(if valid) </summary>
        public bool ThrowCurveProjectilesOnTarget { get; set; }


        private bool _AutoUpdateSpawnPosition = true;
        /// <summary> Automatic update bullet spawn positions from SpawnPoint </summary>
        /// <remarks>
        /// If weapon is attached to hand of an actor, it is possible to handle direction of hand with IK, in most case ik applied at LateUpdate so if
        /// we shoot a bullet it start from wrong place because position of SpawnPoint updated in Update and IK applied in LateUpdate
        /// in situation like this disable auto update of spawn position and call UpdateSpawnPosition() when IK applied.
        /// </remarks>
        public bool AutoUpdateSpawnPosition { get { return _AutoUpdateSpawnPosition; } set { _AutoUpdateSpawnPosition = value; } }

        private List<Collider> _IgnoreColliders;
        /// <summary>
        /// Add collider to ignored by bullets
        /// </summary>
        /// <param name="collider">Collider to ignore</param>
        public void AddIgnoreCollider(Collider collider)
        {
            if (_IgnoreColliders == null) _IgnoreColliders = new List<Collider>();
            if (_IgnoreColliders == null) _IgnoreColliders = new List<Collider>();
            if (!_IgnoreColliders.Contains(collider)) _IgnoreColliders.Add(collider);
        }
        /// <summary>
        /// remove collider to ignored by bullets
        /// </summary>
        /// <param name="collider">Collider to ignore</param>
        /// <returns></returns>
        public bool RemoveIgnoreCollider(Collider collider)
        {
            if (_IgnoreColliders == null) return false;
            return _IgnoreColliders.Remove(collider);
        }
        /// <summary>
        /// Remove all ignored colliders
        /// </summary>
        public void ClearIgnoreColliders()
        {
            if (_IgnoreColliders != null)
                _IgnoreColliders.Clear();
        }

        // events
        /// <summary> Occurs when a shoot happpened </summary>
        public event WeaponShootEventHandler Shoot;
        /// <summary> Occurs when a shoot happpened </summary>
        protected virtual void OnShoot()
        {
            if (AutoUpdateSpawnPosition)
                UpdateSpawnPosition();
            PlayFireSound();
            int consummed = ConsumeAmmo();

            if (!SelectedProjectile.InfinitAmmo)
            {
                if (SelectedProjectile.ClipAmmo > consummed)
                {
                    SelectedProjectile.ClipAmmo -= consummed;
                }
                else
                {
                    SelectedProjectile.ClipAmmo = 0;
                }
            }

            Bullet[] bullets = ShootBullets(consummed);
            if (bullets != null)
            {
                for (int i = 0; i < bullets.Length; i++)
                    PrepareBullet(bullets[i]);
            }

            if (Shoot != null)
                Shoot(this, new WeaponShootEventArgs(consummed));
        }

        /// <summary>
        /// Update bullet spawn position from SpawnPoint.Position
        /// </summary>
        public void UpdateSpawnPosition()
        {
            foreach (var p in Projectiles)
            {
                if (p.SpawnPoint != null)
                    p.SpawnPosition = p.SpawnPoint.position;
            }
        }

        /// <summary>
        /// Prepare bullet parameters just after spawn
        /// </summary>
        /// <param name="bullet">Bullet to prepare</param>
        protected virtual void PrepareBullet(Bullet bullet)
        {
            bullet.DamageType = SelectedProjectile.DamageType;
            bullet.Target = Target;
            bullet.StartJourney();
            bullet.gameObject.SetActive(true);
        }

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        private Bullet[] ShootBullets(int bulletCount)
        {
            if (SelectedProjectile != null)
            {
                switch (SelectedProjectile.Type)
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
            int ammoToFillClip = SelectedProjectile.ClipSize - SelectedProjectile.ClipAmmo;

            if (!SelectedProjectile.InfinitClip)
            {
                if (SelectedProjectile.Ammo < ammoToFillClip)
                    ammoToFillClip = SelectedProjectile.Ammo;
                SelectedProjectile.Ammo -= ammoToFillClip;
            }
            SelectedProjectile.ClipAmmo += ammoToFillClip;

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
        private Collider _Collider;
        private int _SelectedProjectileIndex = 0;

        private TimeWatch _BusyTW;
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
            _AudioSource = GetComponent<AudioSource>();
            _Collider = GetComponent<Collider>();
            if (_AudioSource == null)
                Debug.LogWarning("Can not find AudioSource of weapon");
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            DamageFactor = 1.0f;
            if (Projectiles == null || Projectiles.Length == 0)
                Debug.LogError("A weapon must have at least one projectile.");

            foreach (var p in Projectiles)
            {
                if (p.MaxAmmo < 0)
                    p.MaxAmmo = 0;
                if (p.DefaultAmmo > p.MaxAmmo)
                    p.DefaultAmmo = p.MaxAmmo;
                if (p.Ammo == 0)
                {
                    p.Ammo = p.DefaultAmmo - p.ClipSize;
                    p.ClipAmmo = Mathf.Max(0, p.DefaultAmmo - p.Ammo);
                }
            }
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
        /// <returns> Amount actually added. (In case magazine is already full and some ammo is left) </returns>
        public virtual int AddAmmo(int damageType, int amount)
        {
            foreach (var item in Projectiles)
            {
                if (item.DamageType == damageType)
                {
                    int preAmmoCount = item.TotalAmmo;
                    if (item.TotalAmmo < item.MaxAmmo)
                    {
                        int totalAmmo = item.TotalAmmo + amount;
                        if (totalAmmo > item.MaxAmmo) totalAmmo = item.MaxAmmo;
                        else if (totalAmmo < 0) totalAmmo = 0;
                        item.Ammo = totalAmmo;
                    }
                    return item.Ammo - preAmmoCount;
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
                if (SelectedProjectile.ClipAmmo < SelectedProjectile.ClipSize)
                {
                    if (SelectedProjectile.Ammo > 0 || SelectedProjectile.InfinitClip || SelectedProjectile.InfinitAmmo)
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
                projectileIndex != _SelectedProjectileIndex &&
                Projectiles[projectileIndex] != null)
            {
                _SwitchProjectileIndex = projectileIndex;
            }
        }

        private bool _StopFireCommand = true;
        /// <summary>
        /// Start fire command
        /// </summary>
        public virtual void StartFire()
        {
            if (State == WeaponState.Ready)
            {
                if (!IsFiring)
                {
                    if (Mode != ShootMode.Continuous && !_StopFireCommand)
                    {
                        return;
                    }
                    else
                    {
                        _ShootCount = 0;
                        IsFiring = true;
                        _StopFireCommand = false;
                    }
                }
            }
        }
        /// <summary>
        /// Stop fire command
        /// </summary>
        public virtual void StopFire()
        {
            _StopFireCommand = true;
            if (IsFiring)
            {
                int mode = (int)this.Mode;
                if (mode <= 0 || _ShootCount >= mode || SelectedProjectile.ClipAmmo == 0)
                    IsFiring = false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_BusyTW.IsEnabledAndOver)
            {
                _BusyTW.End();
                if (State == WeaponState.Reloading)
                {
                    OnReloadCompleted();
                }
                else if (State == WeaponState.ChangeProjectile)
                {
                    OnProjectileChanged(_SelectedProjectileIndex, _SwitchProjectileIndex);
                    _SelectedProjectileIndex = _SwitchProjectileIndex;
                }
                State = WeaponState.Ready;
            }

            if (State == WeaponState.Ready)
            {
                if (_RequestReload && (SelectedProjectile.Ammo > 0 || SelectedProjectile.InfinitClip || SelectedProjectile.InfinitAmmo))
                {
                    bool completeReload = SelectedProjectile.ClipAmmo == 0;
                    if (completeReload)
                        _BusyTW.Begin(SelectedProjectile.CompleteReloadTime);
                    else
                        _BusyTW.Begin(SelectedProjectile.ReloadTime);

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
                else if (_SwitchProjectileIndex != _SelectedProjectileIndex)
                {
                    _BusyTW.Begin(SelectedProjectile.EquipTime);
                    State = WeaponState.ChangeProjectile;
                }
                else if (IsFiring)
                {
                    if (SelectedProjectile.ClipAmmo < 0) SelectedProjectile.ClipAmmo = 0;
                    if (SelectedProjectile.ClipAmmo == 0)
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

                        if (SelectedProjectile.ClipAmmo == 0)
                        {
                            if (AutoReload)
                            {
                                RequestReload();
                                _ShootCount = 0;
                            }
                        }
                        else
                        {
                            _BusyTW.Begin(SelectedProjectile.FireInterval);
                            State = WeaponState.Refill;
                        }
                        _ShootCount++;
                        int mode = (int)this.Mode;
                        if (mode > 0 && _ShootCount >= mode)
                            IsFiring = false;
                    }
                }
                else if (AutoReload && SelectedProjectile.ClipAmmo == 0)
                {
                    if (!_RequestReload)
                    {
                        RequestReload();
                        _ShootCount = 0;
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
            if (SelectedProjectile.FireSounds != null && SelectedProjectile.FireSounds.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, SelectedProjectile.FireSounds.Length);
                PlaySound(SelectedProjectile.FireSounds[randomIndex]);
            }
        }

        private void PlayReloadSound(bool completeReload)
        {
            if (completeReload)
            {
                if (SelectedProjectile.CompleteReloadSound != null)
                    PlaySound(SelectedProjectile.CompleteReloadSound);
                else
                    PlaySound(SelectedProjectile.ReloadSound);
            }
            else
                PlaySound(SelectedProjectile.ReloadSound);
        }

        private void PlaySound(AudioClip sound)
        {
            if (_AudioSource != null && sound != null)
            {
                if (Global.Instance != null)
                {
                    Global.Instance.PlayOneShot(_AudioSource, sound, Audio.SoundCategory.FX);
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

            switch (SelectedProjectile.CurveParams.InitialRotation)
            {
                case InitialCurveProjectileRotation.Forward:
                    Quaternion.LookRotation(SelectedProjectile.SpawnPoint.forward, transform.up);
                    break;
                case InitialCurveProjectileRotation.AbsoluteCustom:
                    iniRotation = SelectedProjectile.CurveParams.Rotation;
                    break;
                case InitialCurveProjectileRotation.RelativeCustom:
                    iniRotation = SelectedProjectile.CurveParams.Rotation * transform.rotation;
                    break;
            }

            for (int i = 0; i < bulletCount; i++)
            {
                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(SelectedProjectile.BulletPrefab, SelectedProjectile.SpawnPosition, iniRotation) as GameObject;

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = SelectedProjectile.InstantHitDamage * DamageFactor;
                    bullet.Range = SelectedProjectile.Range;
                    bullet.Speed = SelectedProjectile.InitialSpeed;
                }

                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Collider rbc = go.GetComponent<Collider>();
                    if (rbc != null)
                    {
                        IgnoreBulletColliders(rbc);
                    }

                    float speed = SelectedProjectile.InitialSpeed;
                    Vector3 dir = SelectedProjectile.SpawnPoint.forward;
                    dir.y = 0;

                    if (ThrowCurveProjectilesOnTarget && Target != null && Target.HasValue)
                    {
                        float range = SelectedProjectile.Range;
                        if (range < 0) range = 0.0f;

                        Vector3 target = Target.Value;
                        Vector3 position = transform.position;
                        dir = target - position;  // get target direction
                        float h = dir.y;  // get height difference
                        dir.y = 0;  // retain only the horizontal direction
                        range = dir.magnitude;  // get horizontal distance
                        if (range > SelectedProjectile.Range) range = SelectedProjectile.Range;
                        if (range < Mathf.Epsilon || float.IsNaN(range)) range = Mathf.Epsilon;
                        dir.y = range * SelectedProjectile.CurveParams.TangentThrowAngle;  // set dir to the elevation angle
                        range += h / SelectedProjectile.CurveParams.TangentThrowAngle;  // correct for small height differences
                        // calculate the velocity magnitude
                        speed = Mathf.Sqrt(range * Physics.gravity.magnitude / SelectedProjectile.CurveParams.Sin2ThrowAngle);
                        dir.Normalize();
                    }

                    float euler = Skill.Framework.MathHelper.HorizontalAngle(dir);
                    Quaternion rotation = Quaternion.Euler(-SelectedProjectile.CurveParams.ThrowAngle, euler, 0);
                    dir = rotation * Vector3.forward;

                    if (float.IsNaN(speed))
                        speed = Mathf.Epsilon;
                    rb.AddForce(dir * speed, ForceMode.Impulse);

                    bullet.Direction = dir;
                    bullet.Speed = speed;
                }
                bullets[i] = bullet;
            }
            return bullets;
        }

        public static Vector3 CalculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
        {
            // calculate vectors
            Vector3 toTarget = target - origin;
            Vector3 toTargetXZ = toTarget;
            toTargetXZ.y = 0;

            // calculate xz and y
            float y = toTarget.y;
            float xz = toTargetXZ.magnitude;

            // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
            // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
            // so xz = v0xz * t => v0xz = xz / t
            // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
            float t = timeToTarget;
            float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
            float v0xz = xz / t;

            // create result vector for calculated starting speeds
            Vector3 result = toTargetXZ.normalized; // get direction of xz but with magnitude 1
            result *= v0xz; // set magnitude of xz to v0xz (starting speed in xz plane)
            result.y = v0y; // set y to v0y (starting speed of y plane)

            return result;
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
                direction = (Target.Value - SelectedProjectile.SpawnPosition).normalized;
            else
                direction = SelectedProjectile.SpawnPoint.forward;

            for (int i = 0; i < bulletCount; i++)
            {
                Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(SelectedProjectile.BulletPrefab, SelectedProjectile.SpawnPosition, bulletRotation) as GameObject;
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    Collider rbc = go.GetComponent<Collider>();
                    if (rbc != null)
                    {
                        IgnoreBulletColliders(rbc);
                    }

                    if (rb.useGravity)
                    {
                        rb.AddForce(bulletDirection * SelectedProjectile.InitialSpeed, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddForce(bulletDirection * SelectedProjectile.InitialSpeed, ForceMode.VelocityChange);
                    }
                }

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = SelectedProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = SelectedProjectile.Range;
                    bullet.Speed = SelectedProjectile.InitialSpeed;
                }
                bullets[i] = bullet;
            }
            return bullets;
        }

        protected void IgnoreBulletColliders(Collider bulletCollider)
        {
            if (this._Rigidbody != null && this._Collider != null && this._Collider.enabled)
                Physics.IgnoreCollision(this._Collider, bulletCollider);
            if (_IgnoreColliders != null && _IgnoreColliders.Count > 0)
            {
                for (int cIndex = 0; cIndex < _IgnoreColliders.Count; cIndex++)
                {
                    if (_IgnoreColliders[cIndex].enabled)
                        Physics.IgnoreCollision(_IgnoreColliders[cIndex], bulletCollider);
                }
            }
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
                direction = (Target.Value - SelectedProjectile.SpawnPosition).normalized;
            else
                direction = SelectedProjectile.SpawnPoint.forward;

            for (int i = 0; i < bulletCount; i++)
            {
                Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = Cache.Spawn(SelectedProjectile.BulletPrefab, SelectedProjectile.SpawnPosition, bulletRotation, false) as GameObject;
                StraightLineBullet bullet = go.GetComponent<StraightLineBullet>();// get reference to bullet

                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = this.gameObject;
                    bullet.Damage = SelectedProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = SelectedProjectile.Range;
                    bullet.Speed = SelectedProjectile.InitialSpeed;
                    bullet.LayerMask = SelectedProjectile.LayerMask;

                    if (SelectedProjectile.HitAtSpawn) // if is is needed to check hit at spawn time
                    {
                        _Ray.direction = bulletDirection;
                        _Ray.origin = SelectedProjectile.SpawnPosition;

                        if (Physics.Raycast(_Ray, out _HitInfo, SelectedProjectile.Range, SelectedProjectile.LayerMask))
                        {
                            bullet.Range = _HitInfo.distance;

                            EventManager events = _HitInfo.collider.GetComponent<EventManager>();
                            if (events != null)
                            {
                                RaycastHitEventArgs info = new RaycastHitEventArgs(bullet.Shooter, HitType.Bullet | HitType.Raycast, _HitInfo.collider);
                                info.Damage = bullet.Damage;
                                info.DamageType = bullet.DamageType;
                                info.Tag = this.tag;
                                info.Normal = _HitInfo.normal;
                                info.Point = _HitInfo.point;
                                info.RaycastHit = _HitInfo;
                                events.RaiseHit(this, info);
                            }
                        }
                    }
                }
                bullets[i] = bullet;
            }
            return bullets;
        }
    }

}