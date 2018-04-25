using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayerManager : MonoBehaviour {
    Item[] inventory = new Item[20];
    Item[] equipment = new Item[5];
    Item[] actionBar = new Item[10];
    const int maxInventorySlots = 20;
    const int maxHP = 100;
    const int maxMana = 100;
    Stats playerStats = new Stats() { hp = maxHP, mana = maxMana, strength = 10, defense = 10, agility = 10 };
    public delegate void StatChangedEvent();
    public static StatChangedEvent StatsChanged;

    /*public List<Item> Inventory
    {
        get
        {
            return inventory;
        }
    }*/

    public Item[] Inventory
    {
        get
        {
            return inventory;
        }
    }

    public Item[] Equipment
    {
        get
        {
            return equipment;
        }
    }

    public Item[] ActionBar
    {
        get
        {
            return actionBar;
        }
    }

    public Stats PlayerStats
    {
        get
        {
            return playerStats;
        }
    }

    public Stats EquipmentStats
    {
        
        get
        {
            Stats sum = new Stats() { hp = 0, mana = 0, strength = 0, defense = 0, agility = 0 };
            foreach (ItemEquipment equip in equipment)
            {
                if(equip != null)
                {
                    sum.hp += equip.Stats.hp;
                    sum.mana += equip.Stats.mana;
                    sum.strength += equip.Stats.strength;
                    sum.defense += equip.Stats.defense;
                    sum.agility += equip.Stats.agility;
                }
            }
            return sum;
        }
    }

    public Stats TotalStats
    {
        get
        {
            Stats sum = playerStats;
            foreach (ItemEquipment equip in equipment)
            {
                if (equip != null)
                {
                    sum.hp += equip.Stats.hp;
                    sum.mana += equip.Stats.mana;
                    sum.strength += equip.Stats.strength;
                    sum.defense += equip.Stats.defense;
                    sum.agility += equip.Stats.agility;
                }
            }
            return sum;
        }
    }

    public int MaxHP
    {
        get
        {
            return maxHP + EquipmentStats.hp;
        }
    }

    public int MaxMana
    {
        get
        {
            return maxMana + EquipmentStats.mana;
        }
    }

    public bool UseItem(Item item)
    {
        if(item.GetType() == typeof(ItemEquipment))
        {
            EquipItem((ItemEquipment)item);
        }
        else if (item.GetType() == typeof(ItemConsumable))
        {
            ItemConsumable cons = (ItemConsumable)item;
            switch (cons.Action)
            {
                case ItemConsumable.UseAction.HEAL:
                    Heal(cons.Value);
                    break;
                case ItemConsumable.UseAction.DAMAGE:
                    GetHit(cons.Value);
                    break;
                case ItemConsumable.UseAction.ADD_MANA:
                    AddMana(cons.Value);
                    break;
                case ItemConsumable.UseAction.USE_MANA:
                    UseMana(cons.Value);
                    break;
            }
            bool destroy = false;
            if (item.Stackable)
            {
                item.StackedAmount--;
                if (item.StackedAmount <= 0)
                    destroy = true;
            }
            else
            {
                destroy = true;
            }

            if (destroy)
            {
                if(CheckIfItemIsInInventory(item))
                {
                    RemoveItem(item);
                }
                else if(CheckIfItemIsInActionBar(item))
                {
                    RemoveActionBarItem(item);
                }
            }
            return destroy;
        }
        else if (item.GetType() == typeof(ItemMaterial))
        {
        }
        return false;
    }

    public void EquipItem(ItemEquipment item)
    {
        if(!CheckIfItemIsInActionBar(item) && !CheckIfItemIsInInventory(item))
        {
            Debug.Log("Can't find Item to equip in Inventory or ActionBar");
            return;
        }
        Item[] sourcePanel;
        int index = GetActionBarItemIndex(item);
        if (index != -1)
            sourcePanel = actionBar;
        else
        {
            index = GetInventoryItemIndex(item);
            if(index != -1)
                sourcePanel = inventory;
            else
            {
                Debug.Log("Can't find item");
                return;
            }
        }
        
        if(equipment[(int)item.Localisation] != null)
        {
            Item temp = equipment[(int)item.Localisation];
            equipment[(int)item.Localisation] = item;
            sourcePanel[index] = temp;
        }
        else
        {
            equipment[(int)item.Localisation] = item;
            sourcePanel[index] = null;
        }
        PlayerUIController.UpdateEveryPanel();
        StatsChanged.Invoke();
    }

    public void UnequipItem(ItemEquipment.EquipSlot equipSlot)
    {
        int index = FindFreeInventorySlot();
        if (index == -1)
        {
            Debug.Log("No free inventory slot to unequip");
            return;
        }
            
        inventory[index] = equipment[(int)equipSlot];
        equipment[(int)equipSlot] = null;
        PlayerUIController.InventoryNeedUpdate = true;
        PlayerUIController.CharacterNeedUpdate = true;
        StatsChanged.Invoke();
    }

    public void UnequipItemInPanelAtSlot(ItemEquipment.EquipSlot equipSlot, Item[] panel, int slot)
    {
        if (slot < 0 || slot > panel.Length || panel[slot] != null)
            return;
        panel[slot] = equipment[(int)equipSlot];
        equipment[(int)equipSlot] = null;
        PlayerUIController.UpdateEveryPanel();
        StatsChanged.Invoke();
    }

    // Use this for initialization
    void Start () {
        Array.Clear(inventory, 0, inventory.Length);
        AddItem(new ItemEquipment(ItemDatabase.Equipments[0]));
        AddItem(new ItemConsumable(ItemDatabase.Consumables[0]));
        AddItem(new ItemMaterial(ItemDatabase.Materials[0]));
        AddItem(new ItemConsumable(ItemDatabase.Consumables[0]));

        actionBar[0] = new ItemConsumable(ItemDatabase.Consumables[0]);
        actionBar[6] = new ItemConsumable(ItemDatabase.Consumables[0]);

        equipment[(int)ItemEquipment.EquipSlot.WEAPON] = new ItemEquipment(ItemDatabase.Equipments[0]);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public bool MergeStackables(Item dest, Item src) //return true if there are still remaining items in the source
    {
        if(dest.ID != src.ID)
        {
            Debug.Log("Error: The two items are not the same");
            return true;
        }

        if (!dest.Stackable || !src.Stackable)
        {
            Debug.Log("Error: The two items are not both stackable");
            return true;
        }

        int newAmount = Mathf.Clamp(dest.StackedAmount + src.StackedAmount, 0, dest.MaxStackable);
        int usedAmount = newAmount - dest.StackedAmount;
        dest.StackedAmount = newAmount;
        src.StackedAmount -= usedAmount;
        if (src.StackedAmount <= 0)
            return false;
        return true;
    }

    public void MoveItemToSlot(Item item, int slot)
    {
        if (slot >= 20 || slot < 0 || item == null || !CheckIfItemIsInInventory(item))
            return;
        int startIndex = GetInventoryItemIndex(item);
        if(startIndex != slot)
        {
            if (inventory[slot] != null)
            {
                Item temp = inventory[startIndex];
                inventory[startIndex] = inventory[slot];
                inventory[slot] = temp;
            }
            else
            {

                inventory[slot] = inventory[startIndex];
                inventory[startIndex] = null;
            }
        }
        /*if (InventoryChanged != null)
            InventoryChanged.Invoke();*/
    }

    public void MoveActionBarItemToSlot(Item item, int slot)
    {
        if (slot >= 10 || slot < 0 || item == null || !CheckIfItemIsInActionBar(item))
            return;
        int startIndex = GetActionBarItemIndex(item);
        if (startIndex != slot)
        {
            if (actionBar[slot] != null)
            {
                Item temp = actionBar[startIndex];
                actionBar[startIndex] = actionBar[slot];
                actionBar[slot] = temp;
            }
            else
            {

                actionBar[slot] = actionBar[startIndex];
                actionBar[startIndex] = null;
            }
        }
        /*if (InventoryChanged != null)
            InventoryChanged.Invoke();*/
    }
    public void TransferItemBetweenPanelsAtSlot(Item[] dest, Item[] src, int slotDest, int slotSrc)
    {
        if(dest[slotDest] != null)
        {
            Item temp = dest[slotDest];
            dest[slotDest] = src[slotSrc];
            src[slotSrc] = temp;
        }
        else
        {
            dest[slotDest] = src[slotSrc];
            src[slotSrc] = null;
        }
        PlayerUIController.UpdateEveryPanel();
    }

    public bool AddItem(Item item, bool stack = true)
    {
        bool add = true;
        if(stack)
            if(item.Stackable && CheckIfItemTypeIsInInventory(item))
            {
                for(int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] != null && inventory[i].ID == item.ID)
                    {
                        add = MergeStackables(inventory[i], item);
                        if (!add)
                        break;
                    }
                
                }
            
            }
        if(add)
        {
            int freeIndex = FindFreeInventorySlot();
            if (freeIndex != -1)
            {
                inventory[freeIndex] = item;
            }
            else
            {
                Debug.Log("No free slot");
                return false;
            }
                
        }
        PlayerUIController.InventoryNeedUpdate = true;
        return true;
    }

    public int FindFreeInventorySlot()
    {
        int freeIndex = -1;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                freeIndex = i;
                break;
            }
        }

        return freeIndex;
    }

    public bool CheckIfItemTypeIsInInventory(Item item) //Check if an item with the same ID already exists in the inventory
    {
        return Array.Exists<Item>(inventory, x => 
        {
            if(x != null)
                return x.ID == item.ID;
            return false;
        });
    }

    public bool CheckIfItemIsInInventory(Item item) //Check if the item itself is in the inventory
    {
        return Array.Exists<Item>(inventory, x =>
        {
            if (x != null)
                return x == item;
            return false;
        });
    }

    public bool CheckIfItemIsInActionBar(Item item) //Check if the item itself is in the action bar
    {
        return Array.Exists<Item>(actionBar, x =>
        {
            if (x != null)
                return x == item;
            return false;
        });
    }

    public int GetInventoryItemIndex(Item item)
    {
        return Array.FindIndex<Item>(inventory, x => x == item);
    }

    public int GetActionBarItemIndex(Item item)
    {
        return Array.FindIndex<Item>(actionBar, x => x == item);
    }

    public int GetInventoryFirstItemTypeIndex(Item item)
    {
        return Array.FindIndex<Item>(inventory, x => x.ID == item.ID);
    }

    public void RemoveItem(Item item)
    {
        int index = GetInventoryItemIndex(item);
        if(index == -1)
        {
            Debug.Log("Item does not exist in inventory");
            return;
        }
        inventory[index] = null;
    }

    public void RemoveActionBarItem(Item item)
    {
        int index = GetActionBarItemIndex(item);
        if (index == -1)
        {
            Debug.Log("Item does not exist in action bar");
            return;
        }
        actionBar[index] = null;
    }

    public void GetHit(int damage)
    {
        damage = damage < 0 ? 0 : damage;
        playerStats.hp -= damage;
        playerStats.hp = playerStats.hp < 0 ? 0 : playerStats.hp;
        StatsChanged.Invoke();
    }

    public void Heal(int value)
    {
        value = value < 0 ? 0 : value;
        playerStats.hp += value;
        playerStats.hp = playerStats.hp > MaxHP ? MaxHP : playerStats.hp;
        StatsChanged.Invoke();
    }

    public void UseMana(int value)
    {
        value = value < 0 ? 0 : value;
        playerStats.mana -= value;
        playerStats.mana = playerStats.mana < 0 ? 0 : playerStats.mana;
        StatsChanged.Invoke();
    }

    public void AddMana(int value)
    {
        value = value < 0 ? 0 : value;
        playerStats.mana += value;
        playerStats.mana = playerStats.mana > MaxMana ? MaxMana : playerStats.mana;
        StatsChanged.Invoke();
    }
}
