using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Framework.Managers
{
    public class Inventory : Skill.Framework.StaticBehaviour
    {
        public InventoryGroup[] Groups;

        protected override void Awake()
        {
            base.Awake();
            if (Groups != null)
            {
                for (int i = 0; i < Groups.Length; i++)
                    if (Groups[i] != null) Groups[i].Inventory = this;
            }
        }
        private InventoryGroup FindGroup(int id)
        {
            if (Groups != null)
            {
                for (int i = 0; i < Groups.Length; i++)
                {
                    if (Groups[i].GroupId == id) return Groups[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Find appropriate disarm slot to attach item
        /// by default
        /// first look for slot with childCount == 0
        /// if not, if selectedSlot is valid : selectedSlot
        /// if not, look for slot with minimum childCount
        /// </summary>
        /// <param name="slots">Disarm slots</param>
        /// <param name="item">InventoryItem supposed to attach to slot</param>
        /// <returns>appropriate disarm slot</returns>
        protected virtual Transform GetDisarmSlot(Transform[] slots, int selectedSlot)
        {
            if (slots != null)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].childCount == 0) return slots[i];
                }

                if (selectedSlot >= 0 && selectedSlot < slots.Length)
                    return slots[selectedSlot];

                if (slots.Length > 0)
                {
                    Transform minSlot = null;
                    int minCount = int.MaxValue;

                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (minCount > slots[i].childCount)
                        {
                            minSlot = slots[i];
                            minCount = slots[i].childCount;
                        }
                    }
                    return minSlot;
                }
            }
            return null;
        }

        /// <summary>
        /// Equip item
        /// </summary>
        /// <param name="item">Item to equip</param>
        public virtual void Equip(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException("InventoryItem is null");
            InventoryGroup group = FindGroup(item.GroupId);
            if (group != null)
            {
                if (item.Group == null)
                {
                    Pickup(item);
                }
                if (item.Group != group)
                {
                    Debug.LogWarning("Inventory Item belongs to another group");
                    return;
                }

                if (group.EquippedItem != item)
                {
                    if (group.EquippedItem != null)
                    {
                        Disarm(group.EquippedItem);
                    }
                    group.EquippedItem = item;
                    if (group.EquippedItem.Portable)
                    {
                        group.EquippedItem.gameObject.SetActive(true);
                        if (group.EquipSlot != null)
                            group.EquippedItem.AttachTo(group.EquipSlot);
                    }
                    group.EquippedItem.Equip();
                }
            }
            else
            {
                Debug.LogError("Invalid Inventory Group Id");
            }
        }

        public virtual void Disarm(InventoryItem item)
        {
            if (item.Group == null || item.Group.EquippedItem != item)
            {
                Debug.LogWarning("Invalid item to Disarm");
                return;
            }

            if (item.Portable)
            {
                item.DeAttach();
                if (item.Group.DisarmMode == InventoryItemDisarmMode.Deactive)
                    item.gameObject.SetActive(false);
                else if (item.Group.DisarmMode == InventoryItemDisarmMode.AttachToSlot)
                {
                    if (item.Group.DisarmSlots == null || item.Group.DisarmSlots.Length < 1)
                    {
                        Debug.LogWarning(string.Format("Invalid DisarmSlots of InventoryGroup id : {0} ", item.Group.GroupId));
                        return;
                    }
                    item.AttachTo(GetDisarmSlot(item.Group.DisarmSlots, item.Group.SelectedSlot));
                }
            }

            item.Group.PreviousEquippedItem = item.Group.EquippedItem;
            item.Group.EquippedItem = null;

            item.Disarm();
            if (item.Group.AutoEquip)
            {
                for (int i = 0; i < item.Group.Count; i++)
                {
                    if (item.Group[i] != item)
                    {
                        Equip(item.Group[i]);
                        break;
                    }
                }
            }

        }

        public InventoryGroup GetGroup(InventoryItem item)
        {
            return FindGroup(item.GroupId);
        }

        public bool HasItem(InventoryItem item)
        {
            InventoryGroup group = FindGroup(item.GroupId);
            if (group != null)
            {
                InventoryItem preItem = group.FindByItemType(item.ItemType);
                return preItem != null;
            }
            return false;
        }

        public virtual bool Pickup(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException("InventoryItem is null");
            InventoryGroup group = FindGroup(item.GroupId);
            if (group != null)
            {
                if (item.Group != null)
                {
                    if (item.Group != group)
                        Debug.LogWarning("Inventory Item belongs to another group");
                    return false;
                }

                InventoryItem preItem = group.FindByItemType(item.ItemType);
                if (preItem != null)
                {
                    preItem.AddAmmo(item);
                    if (group.DestroySameItems)
                        Skill.Framework.Managers.Cache.DestroyCache(item.gameObject);
                    return true;
                }
                else
                {
                    if (item.Portable)
                    {
                        if (group.DisarmMode == InventoryItemDisarmMode.AttachToSlot)
                        {
                            item.gameObject.SetActive(true);
                            Transform slot = GetDisarmSlot(group.DisarmSlots, group.SelectedSlot);
                            if (slot != null)
                                item.AttachTo(slot);
                        }
                        else if (group.DisarmMode == InventoryItemDisarmMode.Deactive)
                            item.gameObject.SetActive(false);
                    }
                    group.Add(item);
                    item.Pickup();
                    if (group.AutoEquip && item.Group.EquippedItem == null || !item.Portable)
                    {
                        Equip(item);
                    }
                    return true;
                }
            }
            return false;
        }

        protected virtual InventoryItem GetExtraItemToDrop(InventoryGroup group)
        {
            if (group.SelectedSlot >= 0 && group.SelectedSlot < group.Count)
                return group[group.SelectedSlot];
            else if (group.Count > 0)
                return group[0];
            else
                return null;
        }

        public virtual void Drop(InventoryItem item)
        {
            if (item.Group != null)
            {
                if (item.Group.EquippedItem == item)
                {
                    Disarm(item);
                }

                if (item.Group.DisarmMode == InventoryItemDisarmMode.AttachToSlot)
                    item.DeAttach();

                item.Drop();
                item.Group.Remove(item);
                item.gameObject.SetActive(true);
            }
        }

        public void DropAll()
        {
            List<InventoryItem> itemlist = new List<InventoryItem>();
            foreach (var g in this.Groups)
            {
                foreach (InventoryItem item in g)
                {
                    if (!itemlist.Contains(item))
                        itemlist.Add(item);
                }
            }
            foreach (var item in itemlist)
                Drop(item);
            itemlist.Clear();
        }
    }

}