using UnityEngine;
using System.Collections;


namespace Skill.Framework.Weapons
{
    [RequireComponent(typeof(Weapon))]
    public class WeaponItem : Skill.Framework.Managers.InventoryItem
    {
        public GameObject Trigger;

        public Weapon Weapon { get; private set; }

        private Skill.Framework.Modules.DelayRender _DelayRender;
        private Renderer _Renderer;
        private Collider _Collider;

        protected override void GetReferences()
        {
            base.GetReferences();
            Weapon = GetComponent<Weapon>();
            _DelayRender = GetComponent<Skill.Framework.Modules.DelayRender>();
            _Renderer = GetComponent<Renderer>();
            _Collider = GetComponent<Collider>();
        }

        public override void Disarm()
        {
            if (Weapon != null)
            {
                if (Weapon.IsFiring) Weapon.StopFire();
            }

        }

        public override void Equip()
        {
            if (_Collider != null)
                _Collider.enabled = false;
            if (Trigger != null)
                Trigger.gameObject.SetActive(false);
            if (Weapon != null)
            {
                Weapon.tag = this.Group.Inventory.tag;
                if (_DelayRender != null && _Renderer != null)
                {
                    _Renderer.enabled = false;
                    _DelayRender.enabled = true;
                }
            }
        }

        public override void Pickup()
        {
            if (_Collider != null)
                _Collider.enabled = false;
            if (Trigger != null)
                Trigger.gameObject.SetActive(false);
            if (Weapon != null)
            {
                Collider[] weaponColliders = this.Group.Inventory.GetComponents<Collider>();
                if (weaponColliders != null && weaponColliders.Length > 0)
                {
                    foreach (var c in weaponColliders)
                        Weapon.AddIgnoreCollider(c);
                }
            }
        }

        public override void Drop()
        {
            if (_Collider != null)
                _Collider.enabled = true;
            if (Trigger != null)
                Trigger.gameObject.SetActive(true);
            if (Weapon != null)
            {
                if (Weapon.IsFiring) Weapon.StopFire();
                Collider[] weaponColliders = this.Group.Inventory.GetComponents<Collider>();
                if (weaponColliders != null && weaponColliders.Length > 0)
                {
                    foreach (var c in weaponColliders)
                        Weapon.RemoveIgnoreCollider(c);
                }
            }
        }

        public override void AddAmmo(Skill.Framework.Managers.InventoryItem sameItem)
        {
            var wp = ((WeaponItem)sameItem).Weapon;
            for (int i = 0; i < wp.Projectiles.Length; i++)
            {
                this.Weapon.AddAmmo(wp.Projectiles[i].DamageType, wp.Projectiles[i].TotalAmmo);
            }
        }
    }
}