using UnityEngine;
using System.Collections;

public struct Stats
{
    public int hp;
    public int mana;
    public int strength;
    public int defense;
    public int agility;
}

public class Item {
    protected int id;
    protected string itemName;
    protected string description;
    protected string prefabName;
    protected string spriteName;
    protected bool stackable;
    protected int maxStackable;
    protected int stackedAmount;

    public int ID { get { return id; } }

    public string ItemName
    {
        get
        {
            return itemName;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public string PrefabName
    {
        get
        {
            return prefabName;
        }
    }

    public string SpriteName
    {
        get
        {
            return spriteName;
        }
    }

    public bool Stackable
    {
        get
        {
            return stackable;
        }
    }

    public int MaxStackable
    {
        get
        {
            return maxStackable;
        }
    }

    public int StackedAmount
    {
        get
        {
            return stackedAmount;
        }

        set
        {
            stackedAmount = value;
        }
    }

    public Item()
    {
        id = -1;
    }

    public Item(int _id, string _itemName, string _description, string _prefabName, string _spriteName)
    {
        id = _id;
        itemName = _itemName;
        description = _description;
        prefabName = _prefabName;
        spriteName = _spriteName;
        stackable = false;
        maxStackable = 1;
        stackedAmount = 1;
    }

    public Item(int _id, string _itemName, string _description, string _prefabName, string _spriteName, int _maxStack)
    {
        id = _id;
        itemName = _itemName;
        description = _description;
        prefabName = _prefabName;
        spriteName = _spriteName;
        stackable = true;
        maxStackable = _maxStack;
        stackedAmount = 1;
    }

    public Item(Item it)
    {
        id = it.id;
        itemName = it.itemName;
        description = it.description;
        prefabName = it.prefabName;
        spriteName = it.spriteName;
        stackable = it.stackable;
        maxStackable = it.maxStackable;
        stackedAmount = it.stackedAmount;
    }
}


public class ItemEquipment : Item
{
    public enum EquipSlot { HELM, CHEST, WEAPON, SHIELD, BOOTS}
    private EquipSlot localisation;
    private Stats stats;


    public EquipSlot Localisation
    {
        get
        {
            return localisation;
        }
    }

    public Stats Stats
    {
        get
        {
            return stats;
        }
    }

    public ItemEquipment()
    {
        id = -1;
    }

    public ItemEquipment(int _id, string _itemName, string _description, string _prefabName, string _spriteName, EquipSlot _local, Stats _stats)
        : base(_id, _itemName, _description, _prefabName, _spriteName)
    {
        localisation = _local;
        stats = _stats;
    }

    public ItemEquipment(int _id, string _itemName, string _description, string _prefabName, string _spriteName, EquipSlot _local, Stats _stats, int _maxStack)
        : base(_id, _itemName, _description, _prefabName, _spriteName, _maxStack)
    {
        localisation = _local;
        stats = _stats;
    }

    public ItemEquipment(ItemEquipment it)
        : base(it)
    {
        localisation = it.localisation;
        stats = it.stats;
    }

}

public class ItemConsumable : Item
{
    public enum UseAction { HEAL, DAMAGE, ADD_MANA, USE_MANA }
    UseAction action;
    private int value;

    public UseAction Action
    {
        get
        {
            return action;
        }
    }

    public int Value
    {
        get
        {
            return value;
        }
    }

    public ItemConsumable()
    {
        id = -1;
    }

    public ItemConsumable(int _id, string _itemName, string _description, string _prefabName, string _spriteName, UseAction _action, int _value)
        : base(_id, _itemName, _description, _prefabName, _spriteName)
    {
        action = _action;
        value = _value;
    }

    public ItemConsumable(int _id, string _itemName, string _description, string _prefabName, string _spriteName, UseAction _action, int _value, int _maxStack)
        : base(_id, _itemName, _description, _prefabName, _spriteName, _maxStack)
    {
        action = _action;
        value = _value;
    }

    public ItemConsumable(ItemConsumable it)
        : base(it)
    {
        action = it.action;
        value = it.value;
    }
}

public class ItemMaterial : Item
{
    // Can be used to implement specific behaviours for materials.
    // For now, there won't be any
    public ItemMaterial()
    {
        id = -1;
    }

    public ItemMaterial(int _id, string _itemName, string _description, string _prefabName, string _spriteName)
        : base(_id, _itemName, _description, _prefabName, _spriteName)
    {}

    public ItemMaterial(int _id, string _itemName, string _description, string _prefabName, string _spriteName, int _maxStack)
        : base(_id, _itemName, _description, _prefabName, _spriteName, _maxStack)
    {}
    public ItemMaterial(ItemMaterial it)
        : base(it)
    {}
}