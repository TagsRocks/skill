using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Skill.Framework.Managers
{
    public abstract class InventoryItem : Skill.Framework.StaticBehaviour
    {
        public int GroupId = 0;
        public int ItemType = 0;
        public bool Portable = true;
        public Vector3 SlotLocalPosition;
        public Vector3 SlotLocalRotation;

        /// <summary> Group of item if picked up, otherwise null </summary>
        public InventoryGroup Group { get; internal set; }


        public virtual void AttachTo(Transform slot)
        {
            _Transform.parent = slot;
            _Transform.localPosition = SlotLocalPosition;
            _Transform.localRotation = Quaternion.Euler(SlotLocalRotation);
            if (rigidbody != null) rigidbody.isKinematic = true;
        }

        public virtual void DeAttach()
        {
            _Transform.parent = null;
            if (rigidbody != null) rigidbody.isKinematic = false;
        }
        public abstract void AddAmmo(InventoryItem sameItem);
        public abstract void Equip();
        public abstract void Disarm();
        public abstract void Pickup();
        public abstract void Drop();
    }

}