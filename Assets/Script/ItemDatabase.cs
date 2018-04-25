using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    private static List<ItemEquipment> equipments = new List<ItemEquipment>();
    private static List<ItemConsumable> consumables = new List<ItemConsumable>();
    private static List<ItemMaterial> materials = new List<ItemMaterial>();
    private JsonData itemsData;

    public static List<ItemEquipment> Equipments
    {
        get
        {
            return equipments;
        }
    }

    public static List<ItemConsumable> Consumables
    {
        get
        {
            return consumables;
        }
    }

    public static List<ItemMaterial> Materials
    {
        get
        {
            return materials;
        }
    }

    // Use this for initialization
    void Start () {
        itemsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        InitializeDatabases();
    }
	
	void InitializeDatabases()
    {
        equipments = new List<ItemEquipment>();
        consumables = new List<ItemConsumable>();
        materials = new List<ItemMaterial>();

        for(int i = 0; i < itemsData.Count; i++)
        {
            switch(itemsData[i]["type"].ToString())
            {
                case "equipment":
                    string local = itemsData[i]["localisation"].ToString();
                    ItemEquipment.EquipSlot localisation = ItemEquipment.EquipSlot.CHEST;
                    switch (local)
                    {
                        case "CHEST":
                            localisation = ItemEquipment.EquipSlot.CHEST;
                            break;
                        case "BOOTS":
                            localisation = ItemEquipment.EquipSlot.BOOTS;
                            break;
                        case "WEAPON":
                            localisation = ItemEquipment.EquipSlot.WEAPON;
                            break;
                        case "HELM":
                            localisation = ItemEquipment.EquipSlot.HELM;
                            break;
                        case "SHIELD":
                            localisation = ItemEquipment.EquipSlot.SHIELD;
                            break;
                    }
                    Stats stats;
                    stats.hp = (int)itemsData[i]["stats"]["hp"];
                    stats.mana = (int)itemsData[i]["stats"]["mana"];
                    stats.strength = (int)itemsData[i]["stats"]["strength"];
                    stats.defense = (int)itemsData[i]["stats"]["defense"];
                    stats.agility = (int)itemsData[i]["stats"]["agility"];

                    if ((bool)itemsData[i]["stackable"])
                    {
                        Equipments.Add(new ItemEquipment((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"],
                                                        localisation,
                                                        stats,
                                                        (int)itemsData[i]["maxStackable"]));
                    }
                    else
                    {
                        Equipments.Add(new ItemEquipment((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"],
                                                        localisation,
                                                        stats));
                    }
                    break;
                case "consumable":
                    ItemConsumable.UseAction action = ItemConsumable.UseAction.HEAL;
                    switch(itemsData[i]["action"].ToString())
                    {
                        case "HEAL":
                            action = ItemConsumable.UseAction.HEAL;
                            break;
                        case "ADD_MANA":
                            action = ItemConsumable.UseAction.ADD_MANA;
                            break;
                        case "DAMAGE":
                            action = ItemConsumable.UseAction.DAMAGE;
                            break;
                        case "USE_MANA":
                            action = ItemConsumable.UseAction.USE_MANA;
                            break;
                    }

                    if ((bool)itemsData[i]["stackable"])
                    {
                        Consumables.Add(new ItemConsumable((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"],
                                                        action,
                                                        (int)itemsData[i]["value"],
                                                        (int)itemsData[i]["maxStackable"]));
                    }
                    else
                    {
                        Consumables.Add(new ItemConsumable((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"],
                                                        action,
                                                        (int)itemsData[i]["value"]));
                    }
                    break;
                case "material":
                    if ((bool)itemsData[i]["stackable"])
                    {
                        Materials.Add(new ItemMaterial((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"],
                                                        (int)itemsData[i]["maxStackable"]));
                    }
                    else
                    {
                        Materials.Add(new ItemMaterial((int)itemsData[i]["id"],
                                                        (string)itemsData[i]["itemName"],
                                                        (string)itemsData[i]["description"],
                                                        (string)itemsData[i]["prefabName"],
                                                        (string)itemsData[i]["spriteName"]));
                    }
                    break;
            }

        }
        
    }

    public static Item FetchItemByID(int id)
    {
        for(int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i].ID == id)
                return equipments[i];
        }
        for (int i = 0; i < consumables.Count; i++)
        {
            if (consumables[i].ID == id)
                return consumables[i];
        }
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].ID == id)
                return materials[i];
        }
        return null;
    }
}
