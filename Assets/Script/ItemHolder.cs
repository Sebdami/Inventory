using UnityEngine;
using System.Collections;

public class ItemHolder : MonoBehaviour {
    [SerializeField]
    Item item;

    public Item Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
        }
    }
    // Use this for initialization
    void Start () {
	    if(item == null)
        {
            bool found = false;
            foreach (ItemEquipment it in ItemDatabase.Equipments)
            {
                if(it.PrefabName == name)
                {
                    item = new ItemEquipment(it);
                    found = true;
                    break;
                }
            }
            if(!found)
                foreach (ItemConsumable it in ItemDatabase.Consumables)
                {
                    if (it.PrefabName == name)
                    {
                        item = new ItemConsumable(it);
                        found = true;
                        break;
                    }
                }

            if (!found)
                foreach (ItemMaterial it in ItemDatabase.Materials)
                {
                    if (it.PrefabName == name)
                    {
                        item = new ItemMaterial(it);
                        found = true;
                        break;
                    }
                }
            if (!found)
                item = new Item();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
