using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour {
    [SerializeField]
    Transform toMove = null;
    Vector3 initialPosition;
    Transform initialParent;
    GameObject startPanel = null;
    GameObject destinationPanel = null;
    PlayerManager playerManager;
    bool isDragging = false;
    bool isSplitResult = false; //If the item dragged is a result from splitting a stackable item
    GameObject originalItem = null; //The item the split have been applied to
    Chest currentChest = null;
	// Use this for initialization
	void Start () {
        playerManager = GetComponent<PlayerManager>();
        isDragging = false;
        currentChest = null;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.G))
            GenerateRandomItem();
        if(GetComponent<PlayerUIController>().IsUIActive)
        {
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                toMove = null;
                originalItem = null;
                RaycastHit2D hit;
                hit = Physics2D.Raycast(Input.mousePosition, Vector3.zero, 1.0f, LayerMask.GetMask("UI"));
                if (hit)
                {
                    if (hit.collider.GetComponent<UIItemHolder>() != null)
                    {
                        toMove = hit.collider.transform;
                        initialParent = hit.collider.transform.parent;
                    }
                    else if (hit.collider.GetComponentInChildren<UIItemHolder>() != null)
                    {
                        toMove = hit.collider.GetComponentInChildren<UIItemHolder>().transform;
                        initialParent = hit.collider.transform;
                    }
                    if (toMove != null)
                    {
                        isSplitResult = false;
                        
                        startPanel = hit.collider.transform.parent.parent.gameObject;
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            Item item = hit.collider.GetComponentInChildren<UIItemHolder>().Item;
                            if (item.Stackable && item.StackedAmount > 1)
                            {
                                int movedAmount = (int)Mathf.Ceil(item.StackedAmount / 2.0f);
                                item.StackedAmount -= movedAmount;
                                originalItem = hit.collider.gameObject;

                                GameObject go = PlayerUIController.CreateUIItemByCopy(item);
                                go.transform.SetParent(hit.collider.transform.parent);
                                PlayerUIController.UpdateUIItemHolderSize(go.GetComponent<UIItemHolder>());
                                go.GetComponent<UIItemHolder>().Item.StackedAmount = movedAmount;
                                go.GetComponent<UIItemHolder>().UpdateUIText();
                                toMove = go.transform;
                                toMove.position = Input.mousePosition;
                                
                                isSplitResult = true;
                            }
                        }
                        

                        initialPosition = toMove.position;
                        toMove.SetParent(hit.collider.transform.root);
                        toMove.SetAsLastSibling(); //This line and  the previous are used to draw the moved item in front on everything in the canvas
                        toMove.GetComponent<BoxCollider2D>().enabled = false;
                        isDragging = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && isDragging && toMove != null)
            {
                RaycastHit2D hit;
                bool reset = false;
                hit = Physics2D.Raycast(Input.mousePosition, Vector3.zero, 1.0f, LayerMask.GetMask("UI"));
                if (hit.collider != null)
                {
                    if (hit.collider.GetComponentInParent<Slot>() != null)
                    {
                        Transform slot = hit.collider.GetComponentInParent<Slot>().transform;
                        destinationPanel = slot.parent.gameObject;
                        if (!slot.GetComponent<Slot>().isEmpty())
                        {
                            if (!HandleFilledSlot(slot.GetComponent<Slot>()))
                                reset = true;
                        }
                        else
                        {
                            toMove.SetParent(slot);
                            Item item = toMove.GetComponent<UIItemHolder>().Item;
                            bool moveOk = false;
                            switch (item.GetType().ToString())
                            {
                                case "ItemConsumable":
                                    if (slot.GetComponent<Slot>().AcceptConsumable)
                                    {
                                        moveOk = true;
                                    }
                                    else
                                        reset = true;
                                    break;
                                case "ItemEquipment":
                                    if (slot.GetComponent<Slot>().AcceptEquipment)
                                    {
                                        moveOk = true;
                                    }
                                    else
                                        reset = true;
                                    break;
                                case "ItemMaterial":
                                    if (slot.GetComponent<Slot>().AcceptMaterial)
                                    {
                                        moveOk = true;
                                    }
                                    else
                                        reset = true;
                                    break;
                            }
                            if (destinationPanel.name == "CharacterEquipment" && (item.GetType() != typeof(ItemEquipment) || slot.GetSiblingIndex() != (int)(((ItemEquipment)item).Localisation)))
                            {
                                moveOk = false;
                                reset = true;
                            }
                            if (moveOk && !isSplitResult)
                            {
                                toMove.position = slot.position;
                                if(startPanel.name == destinationPanel.name)
                                {
                                    if (startPanel.name == "Inventory")
                                    {
                                        playerManager.MoveItemToSlot(item, slot.GetSiblingIndex());
                                    }
                                    if(startPanel.name == "ActionBar")
                                    {
                                        playerManager.MoveActionBarItemToSlot(item, slot.GetSiblingIndex());
                                    }
                                    if (startPanel.name == "CharacterEquipment")
                                    {
                                        reset = true;
                                    }
                                }
                                else
                                {
                                    if(destinationPanel.name == "CharacterEquipment")
                                    {
                                        playerManager.EquipItem((ItemEquipment)item);
                                    }
                                    else
                                    {
                                        if(startPanel.name == "CharacterEquipment")
                                        {
                                            Item[] dest = null;
                                            switch (destinationPanel.name)
                                            {
                                                case "Inventory":
                                                    dest = playerManager.Inventory;
                                                    break;
                                                case "ActionBar":
                                                    dest = playerManager.ActionBar;
                                                    break;
                                            }
                                            if(dest != null)
                                            {
                                                playerManager.UnequipItemInPanelAtSlot(((ItemEquipment)item).Localisation, dest, slot.GetSiblingIndex());
                                                if(playerManager.Equipment[(int)((ItemEquipment)item).Localisation] != null)
                                                {
                                                    Debug.Log("Couldn't unequip");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Item[] dest = null;
                                            Item[] src = null;
                                            switch(startPanel.name)
                                            {
                                                case "Inventory":
                                                    src = playerManager.Inventory;
                                                    break;
                                                case "ActionBar":
                                                    src = playerManager.ActionBar;
                                                    break;
                                                case "Chest":
                                                    if(currentChest != null)
                                                        src = currentChest.ChestInventory;
                                                    break;
                                            }

                                            switch (destinationPanel.name)
                                            {
                                                case "Inventory":
                                                    dest = playerManager.Inventory;
                                                    break;
                                                case "ActionBar":
                                                    dest = playerManager.ActionBar;
                                                    break;
                                                case "Chest":
                                                    if (currentChest != null)
                                                        dest = currentChest.ChestInventory;
                                                    break;
                                            }
                                            if(src != null && dest != null)
                                                playerManager.TransferItemBetweenPanelsAtSlot(dest, src, slot.GetSiblingIndex(), initialParent.GetSiblingIndex());
                                        }
                                    }
                                }
                                    
                                
                            }

                            if (!reset && isSplitResult && toMove != null)
                            {
                                playerManager.AddItem(toMove.GetComponent<UIItemHolder>().Item, false);
                                int chosenSlot = slot.GetSiblingIndex();
                                playerManager.MoveItemToSlot(toMove.GetComponent<UIItemHolder>().Item, chosenSlot);
                            }
                        }
                    }
                    else
                    {
                        reset = true;
                    }
                }
                else
                {
                    //Item drop are handled here
                    hit = Physics2D.Raycast(Input.mousePosition, Vector3.zero);
                    if (hit.collider == null && toMove != null)
                    {
                        DropItem(toMove.GetComponent<UIItemHolder>().Item);
                        playerManager.RemoveItem(toMove.GetComponent<UIItemHolder>().Item);
                        DestroyImmediate(toMove.gameObject);
                    }
                    else
                        reset = true;
                }
                if (reset)
                {
                    if(!isSplitResult)
                    {
                        toMove.position = initialPosition;
                        toMove.SetParent(initialParent);
                    }
                    else
                    {
                        Item item = originalItem.GetComponent<UIItemHolder>().Item;
                        item.StackedAmount += toMove.GetComponent<UIItemHolder>().Item.StackedAmount;
                        Destroy(toMove.gameObject);
                    }
                }
                originalItem = null;
                if(toMove != null)
                    toMove.GetComponent<BoxCollider2D>().enabled = true;
                isDragging = false;
            }

            if (isDragging && toMove != null)
            {
                toMove.position = Input.mousePosition;
            }
        }

        if (!isDragging && Input.GetMouseButtonDown(1) && Cursor.lockState == CursorLockMode.None)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(Input.mousePosition, Vector3.zero, 1.0f, LayerMask.GetMask("UI"));
            if (hit)
            {
                if (hit.collider.GetComponentInChildren<UIItemHolder>() != null)
                {
                    UIItemHolder holder = hit.collider.GetComponentInChildren<UIItemHolder>();
                    Transform panel = holder.transform.parent.parent;
                    if(panel.name == "CharacterEquipment")
                    {
                        playerManager.UnequipItem(((ItemEquipment)holder.Item).Localisation);
                    }
                    else
                    {
                        bool destroy = playerManager.UseItem(holder.Item);
                        if (destroy)
                            Destroy(holder.gameObject);
                    }
                }
            }
        }

        for(int i = 0; i < 10; i++)
        {
            if (Input.GetButtonDown("ActionBar"+ (i+1)))
            {
                playerManager.UseItem(playerManager.ActionBar[i]);
                PlayerUIController.ActionBarNeedUpdate = true;
            }
        }
        
    }

    public void OpenChest(Chest chest)
    {
        chest.ToggleChest();
        currentChest = chest;
        GetComponent<PlayerUIController>().UpdateCursor();
    }

    public void CloseCurrentChest()
    {
        currentChest.ToggleChest();
        currentChest = null;
        GetComponent<PlayerUIController>().UpdateCursor();
    }

    public void DropItem(Item toDrop)
    {
        GameObject drop = Instantiate(Resources.Load("Prefabs/" + toDrop.PrefabName, typeof(GameObject))) as GameObject;
        drop.transform.position = transform.position + transform.forward;
        if (toDrop.GetType() == typeof(ItemEquipment))
        {
            drop.GetComponent<ItemHolder>().Item = new ItemEquipment((ItemEquipment)toDrop);
        }
        else if (toDrop.GetType() == typeof(ItemConsumable))
        {
            drop.GetComponent<ItemHolder>().Item = new ItemConsumable((ItemConsumable)toDrop);
        }
        else if (toDrop.GetType() == typeof(ItemMaterial))
        {
            drop.GetComponent<ItemHolder>().Item = new ItemMaterial((ItemMaterial)toDrop);
        }
        
    }

    public void GenerateRandomItem()
    {
        int type = 0;
        Item it = new Item();
        type = Random.Range(0, 3);

        if (type == 0)
            it = ItemDatabase.Equipments[Random.Range(0, ItemDatabase.Equipments.Count)];

        if (type == 1)
            it = ItemDatabase.Consumables[Random.Range(0, ItemDatabase.Consumables.Count)];

        if (type == 2)
            it = ItemDatabase.Materials[Random.Range(0, ItemDatabase.Materials.Count)];

        DropItem(it);
    }

    bool HandleFilledSlot(Slot slot)
    {
        if (isSplitResult && originalItem != null)
        {
            Item item = originalItem.GetComponentInChildren<UIItemHolder>().Item;
            item.StackedAmount += toMove.GetComponentInChildren<UIItemHolder>().Item.StackedAmount;
            Destroy(toMove.gameObject);
            return true;
        }
        destinationPanel = slot.transform.parent.gameObject;
        UIItemHolder fillingItem = slot.GetComponentInChildren<UIItemHolder>();
        if (fillingItem != null)
        {
            UIItemHolder dest = fillingItem;
            UIItemHolder src = toMove.GetComponent<UIItemHolder>();
            if (dest.Item.ID == src.Item.ID && src.Item.Stackable)
            {
                bool leftover = playerManager.MergeStackables(dest.Item, src.Item);
                if (!leftover)
                {
                    playerManager.RemoveItem(src.Item);
                    Destroy(src.gameObject);
                }
                else
                    return false;
            }
            else
            {
                toMove.SetParent(slot.transform);
                toMove.position = fillingItem.transform.position;

                fillingItem.transform.SetParent(initialParent);
                fillingItem.transform.position = initialPosition;

                toMove.SetAsFirstSibling();
                fillingItem.transform.SetAsFirstSibling();
                playerManager.MoveItemToSlot(toMove.GetComponent<UIItemHolder>().Item, toMove.parent.GetSiblingIndex());
            }

        }

        
        return true;
    }

    public bool PickUpItem(ItemHolder itemHolder)
    {
        bool added = false;

        added = playerManager.AddItem(itemHolder.Item);
        
        if(added)
            Destroy(itemHolder.gameObject);
        return added;
    }
}
