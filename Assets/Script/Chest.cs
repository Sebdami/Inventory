using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {
    [SerializeField]
    GameObject chestPanel;
    Item[] chestInventory = new Item[20];

    public Item[] ChestInventory
    {
        get
        {
            return chestInventory;
        }

        set
        {
            chestInventory = value;
        }
    }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ToggleChest()
    {
        chestPanel.SetActive(!chestPanel.activeSelf);
        if (chestPanel.activeSelf)
            UpdateChestUI();
    }

    public void UpdateChestUI()
    {
        if (chestPanel.activeSelf)
        {
            GameObject chest = null;
            for (int i = 0; i < chestPanel.transform.childCount; i++)
            {
                if (chestPanel.transform.GetChild(i).name == "Chest")
                {
                    chest = chestPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }

            if (chest != null)
            {
                foreach (UIItemHolder holder in chest.transform.GetComponentsInChildren<UIItemHolder>())
                {
                    DestroyImmediate(holder.gameObject);
                }
                for (int i = 0; i < chestInventory.Length; i++)
                {
                    Slot currentSlot = chest.GetComponentsInChildren<Slot>()[i];
                    if (chestInventory[i] != null)
                    {
                        GameObject go = PlayerUIController.CreateUIItem(chestInventory[i]);
                        go.transform.SetParent(currentSlot.transform);
                        go.transform.position = currentSlot.transform.position;
                        go.transform.SetAsFirstSibling();
                        PlayerUIController.UpdateUIItemHolderSize(go.GetComponent<UIItemHolder>());
                    }
                }
            }
        }
    }
}
