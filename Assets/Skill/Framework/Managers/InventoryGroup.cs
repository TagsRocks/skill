using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.Managers
{
    public enum InventoryItemDisarmMode
    {
        Deactive = 0,
        AttachToSlot = 1
    }

    [System.Serializable]
    public class InventoryGroup : IEnumerable<InventoryItem>
    {
        public string Name;
        public Transform EquipSlot;
        public Transform[] DisarmSlots;
        public InventoryItemDisarmMode DisarmMode = InventoryItemDisarmMode.Deactive;
        public int GroupId = 0;
        public bool DestroySameItems = true; // if picked up same item, after add ammo destroy it
        public bool AutoEquip;

        public Inventory Inventory { get; internal set; }
        public InventoryItem EquippedItem { get; internal set; }
        public InventoryItem PreviousEquippedItem { get; internal set; }

        private List<InventoryItem> _Items;
        public InventoryItem this[int index] { get { return _Items[index]; } }
        public int SelectedSlot { get; set; }


        public InventoryGroup()
        {
            _Items = new List<InventoryItem>();
        }

        public IEnumerator<InventoryItem> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Items).GetEnumerator();
        }

        internal void Add(InventoryItem item)
        {
            //if (_Items.Count >= MaxItem)
            //    throw new InvalidOperationException("Group is full");
            if (item == null) throw new ArgumentNullException("InventoryItem is null");
            if (item.Group != null)
            {
                if (item.Group != this)
                {
                    Debug.LogWarning("Inventory Item belongs to another group");
                    return;
                }
                else
                    return;
            }

            item.Group = this;
            _Items.Add(item);
        }

        public bool Contains(InventoryItem item)
        {
            return item.Group == this;
        }

        public int Count { get { return _Items.Count; } }

        internal bool Remove(InventoryItem item)
        {
            bool result = false;
            if (item.Group == this)
            {
                item.Group = null;
                result = _Items.Remove(item);
                if (item == EquippedItem)
                    EquippedItem = null;
                if (item == PreviousEquippedItem)
                    PreviousEquippedItem = null;
            }
            return result;
        }

        public InventoryItem FindByItemType(int itemType)
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                if (_Items[i].ItemType == itemType) return _Items[i];
            }
            return null;
        }
    }

}