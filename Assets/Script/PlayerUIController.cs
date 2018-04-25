using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerUIController : MonoBehaviour {
    [SerializeField]
    GameObject inventoryPanel;
    [SerializeField]
    GameObject characterPanel;
    [SerializeField]
    GameObject actionBarPanel;
    [SerializeField]
    GameObject chestPanel;
    [SerializeField]
    GameObject useText;
    [SerializeField]
    GameObject inventoryFullText;
    [SerializeField]
    GameObject hpPanel;
    [SerializeField]
    GameObject manaPanel;
    [SerializeField]
    GameObject itemHoverPanel;

    private static bool inventoryNeedUpdate = true;
    private static bool characterNeedUpdate = true;
    private static bool actionBarNeedUpdate = true;
    PlayerManager playerManager;

    FirstPersonController fps;
    float BaseMouseSensitivityX;
    float BaseMouseSensitivityY;
    bool isUIActive = false;
    public bool IsUIActive
    {
        get
        {
            return isUIActive;
        }
    }

    public static bool InventoryNeedUpdate
    {
        get
        {
            return inventoryNeedUpdate;
        }

        set
        {
            inventoryNeedUpdate = value;
        }
    }
    public static bool CharacterNeedUpdate
    {
        get
        {
            return characterNeedUpdate;
        }

        set
        {
            characterNeedUpdate = value;
        }
    }
    public static bool ActionBarNeedUpdate
    {
        get
        {
            return actionBarNeedUpdate;
        }

        set
        {
            actionBarNeedUpdate = value;
        }
    }

    // Use this for initialization
    void Start () {
        playerManager = GetComponent<PlayerManager>();
        isUIActive = (inventoryPanel.activeSelf || characterPanel.activeSelf || chestPanel.activeSelf);
        fps = GetComponent<FirstPersonController>();
        BaseMouseSensitivityX = fps.m_MouseLook.XSensitivity;
        BaseMouseSensitivityY = fps.m_MouseLook.YSensitivity;
        UpdateCursor();
        PlayerManager.StatsChanged += UpdateUI;
        if (characterPanel.activeSelf)
            UpdateEquipmentPanel();
        if (inventoryPanel.activeSelf)
            UpdateInventoryPanel();
        UpdateActionBarPanel();
        UpdateUI();
    }



    public static GameObject CreateUIItemByCopy(Item item)
    {
        GameObject go = Instantiate(Resources.Load("InventoryItem", typeof(GameObject))) as GameObject;
        go.GetComponent<Image>().sprite = Resources.Load("Sprites/" + item.SpriteName, typeof(Sprite)) as Sprite;
        if (item.GetType() == typeof(ItemEquipment))
            go.GetComponent<UIItemHolder>().Item = new ItemEquipment((ItemEquipment)item);
        if (item.GetType() == typeof(ItemConsumable))
            go.GetComponent<UIItemHolder>().Item = new ItemConsumable((ItemConsumable)item);
        if (item.GetType() == typeof(ItemMaterial))
            go.GetComponent<UIItemHolder>().Item = new ItemMaterial((ItemMaterial)item);
        return go;
    }

    public static GameObject CreateUIItem(Item item)
    {
        GameObject go = Instantiate(Resources.Load("InventoryItem", typeof(GameObject))) as GameObject;
        go.GetComponent<Image>().sprite = Resources.Load("Sprites/" + item.SpriteName, typeof(Sprite)) as Sprite;
        go.GetComponent<UIItemHolder>().Item = item;
        return go;
    }
    public void UpdateActionBarPanel()
    {
        GameObject actionBar = null;
        for (int i = 0; i < actionBarPanel.transform.childCount; i++)
        {
            if (actionBarPanel.transform.GetChild(i).name == "ActionBar")
            {
                actionBar = actionBarPanel.transform.GetChild(i).gameObject;
                break;
            }
        }

        if (actionBar != null)
        {
            foreach (UIItemHolder holder in actionBar.transform.GetComponentsInChildren<UIItemHolder>())
            {
                DestroyImmediate(holder.gameObject);
            }
            for (int i = 0; i < playerManager.ActionBar.Length; i++)
            {
                Slot currentSlot = actionBar.GetComponentsInChildren<Slot>()[i];
                if (playerManager.ActionBar[i] != null)
                {
                    GameObject go = CreateUIItem(playerManager.ActionBar[i]);
                    go.transform.SetParent(currentSlot.transform);
                    go.transform.position = currentSlot.transform.position;
                    go.transform.SetAsFirstSibling();
                    UpdateUIItemHolderSize(go.GetComponent<UIItemHolder>());
                }
            }
        }
    }

    public void UpdateInventoryPanel()
    {
        if (inventoryPanel.activeSelf)
        {
            GameObject inventory = null;
            for (int i = 0; i < inventoryPanel.transform.childCount; i++)
            {
                if (inventoryPanel.transform.GetChild(i).name == "Inventory")
                {
                    inventory = inventoryPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }

            if (inventory != null)
            {
                foreach (UIItemHolder holder in inventory.transform.GetComponentsInChildren<UIItemHolder>())
                {
                    DestroyImmediate(holder.gameObject);
                }
                for (int i = 0; i < playerManager.Inventory.Length; i++)
                {
                    Slot currentSlot = inventory.GetComponentsInChildren<Slot>()[i];
                    if (playerManager.Inventory[i] != null)
                    {
                        GameObject go = CreateUIItem(playerManager.Inventory[i]);
                        go.transform.SetParent(currentSlot.transform);
                        go.transform.position = currentSlot.transform.position;
                        go.transform.SetAsFirstSibling();
                        UpdateUIItemHolderSize(go.GetComponent<UIItemHolder>());
                    }
                }
            }
        }
    }
    public static void UpdateEveryPanel()
    {
        inventoryNeedUpdate = true;
        characterNeedUpdate = true;
        actionBarNeedUpdate = true;
    }

    public static void UpdateUIItemHolderSize(UIItemHolder go)
    {
        RectTransform trans = go.GetComponent<RectTransform>();
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5.0f, go.GetComponentsInParent<RectTransform>()[1].sizeDelta.y - 10);
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5.0f, go.GetComponentsInParent<RectTransform>()[1].sizeDelta.x - 10);
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void UpdateEquipmentPanel()
    {
        if (characterPanel.activeSelf)
        {
            GameObject equipment = null;
            GameObject stats = null;
            for (int i = 0; i < characterPanel.transform.childCount; i++)
            {
                if (characterPanel.transform.GetChild(i).name == "CharacterEquipment")
                {
                    equipment = characterPanel.transform.GetChild(i).gameObject;
                }
                if (characterPanel.transform.GetChild(i).name == "CharacterStats")
                {
                    stats = characterPanel.transform.GetChild(i).gameObject;
                    
                }
                if(equipment != null && stats != null)
                    break;
            }

            if (equipment != null)
            {
                foreach (UIItemHolder holder in equipment.transform.GetComponentsInChildren<UIItemHolder>())
                {
                    DestroyImmediate(holder.gameObject);
                }
                for (int i = 0; i < playerManager.Equipment.Length; i++)
                {
                    Slot currentSlot = equipment.GetComponentsInChildren<Slot>()[i];
                    if (playerManager.Equipment[i] != null)
                    {
                        GameObject go = CreateUIItem(playerManager.Equipment[i]);
                        go.transform.SetParent(currentSlot.transform);
                        go.transform.position = currentSlot.transform.position;
                        go.transform.SetAsFirstSibling();
                        UpdateUIItemHolderSize(go.GetComponent<UIItemHolder>());
                    }
                }
            }

            if(stats != null)
            {
                stats.transform.GetChild(0).GetComponent<Text>().text = "Strength: " + playerManager.PlayerStats.strength + " (" + playerManager.EquipmentStats.strength + ") : " + playerManager.TotalStats.strength;
                stats.transform.GetChild(1).GetComponent<Text>().text = "Defense: " + playerManager.PlayerStats.defense + " (" + playerManager.EquipmentStats.defense + ") : " + playerManager.TotalStats.defense;
                stats.transform.GetChild(2).GetComponent<Text>().text = "Agility:    " + playerManager.PlayerStats.agility + " (" + playerManager.EquipmentStats.agility + ") : " + playerManager.TotalStats.agility;
            }


        }
    }

    public void UpdateUI()
    {
        
        hpPanel.GetComponentInChildren<Text>().text = "Health: " + playerManager.PlayerStats.hp + "/" + playerManager.MaxHP;
        hpPanel.GetComponentsInChildren<Image>()[1].fillAmount = (float)(playerManager.PlayerStats.hp) / playerManager.MaxHP;
        manaPanel.GetComponentInChildren<Text>().text = "Mana: " + playerManager.PlayerStats.mana + "/" + playerManager.MaxMana;
        manaPanel.GetComponentsInChildren<Image>()[1].fillAmount = (float)(playerManager.PlayerStats.mana) / playerManager.MaxMana;
    }
	
	// Update is called once per frame
	void Update () {
        if(inventoryNeedUpdate && inventoryPanel.activeSelf)
        {
            UpdateInventoryPanel();
            inventoryNeedUpdate = false;
        }
        if (characterNeedUpdate && characterPanel.activeSelf)
        {
            UpdateEquipmentPanel();
            characterNeedUpdate = false;
        }
        if (actionBarNeedUpdate)
        {
            UpdateActionBarPanel();
            actionBarNeedUpdate = false;
        }
        isUIActive = (inventoryPanel.activeSelf || characterPanel.activeSelf || chestPanel.activeSelf);
        if (Input.GetButtonDown("Inventory"))
        {
            ToggleInventoryPanel();
        }

        if (Input.GetButtonDown("Character"))
        {
            ToggleCharacterPanel();
        }

        if(Input.GetButtonDown("ShowCursor"))
        {
            fps.m_MouseLook.lockCursor = false;
            BaseMouseSensitivityX = fps.m_MouseLook.XSensitivity;
            BaseMouseSensitivityY = fps.m_MouseLook.YSensitivity;
            fps.m_MouseLook.XSensitivity = 0.0f;
            fps.m_MouseLook.YSensitivity = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetButtonUp("ShowCursor") && !isUIActive)
        {
            fps.m_MouseLook.XSensitivity = BaseMouseSensitivityX;
            fps.m_MouseLook.YSensitivity = BaseMouseSensitivityY;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (!isUIActive)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.GetComponent<ItemHolder>() || hit.collider.GetComponent<Chest>())
                {
                    if (hit.distance < 1.5f)
                    {
                        useText.SetActive(true);
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            if(hit.collider.GetComponent<ItemHolder>())
                            {
                                bool added = GetComponent<DragAndDrop>().PickUpItem(hit.collider.GetComponent<ItemHolder>());
                                if (!added)
                                    inventoryFullText.SetActive(true);
                            }
                            else
                            {
                                Chest chest = hit.collider.GetComponent<Chest>();
                                GetComponent<DragAndDrop>().OpenChest(chest);
                            }
                            
                        }
                    }
                    else
                    {
                        useText.SetActive(false);
                    }
                }
                else
                {
                    useText.SetActive(false);
                }
            }
        }
        if(Cursor.lockState == CursorLockMode.None)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(Input.mousePosition, Vector3.zero, 1.0f, LayerMask.GetMask("UI"));
            if (hit && hit.collider.GetComponentInChildren<UIItemHolder>())
            {
                Item item = hit.collider.GetComponentInChildren<UIItemHolder>().Item;
                itemHoverPanel.SetActive(true);
                itemHoverPanel.transform.position = Input.mousePosition;
                itemHoverPanel.transform.GetChild(0).GetComponent<Text>().text = item.ItemName;
                itemHoverPanel.transform.GetChild(1).GetComponent<Text>().text = item.Description;
                if(item.GetType() == typeof(ItemEquipment))
                {
                    string attributes = "";
                    ItemEquipment equip = (ItemEquipment)item;
                    if (equip.Stats.hp > 0)
                        attributes += "HP: +" + equip.Stats.hp + "\n";
                    if (equip.Stats.hp < 0)
                        attributes += "HP: " + equip.Stats.hp + "\n";

                    if (equip.Stats.mana > 0)
                        attributes += "Mana: +" + equip.Stats.mana + "\n";
                    if (equip.Stats.mana < 0)
                        attributes += "Mana: " + equip.Stats.mana + "\n";

                    if (equip.Stats.strength > 0)
                        attributes += "Strength: +" + equip.Stats.strength + "\n";
                    if (equip.Stats.strength < 0)
                        attributes += "Strength: " + equip.Stats.strength + "\n";

                    if (equip.Stats.defense > 0)
                        attributes += "Defense: +" + equip.Stats.defense + "\n";
                    if (equip.Stats.defense < 0)
                        attributes += "Defense: " + equip.Stats.defense + "\n";

                    if (equip.Stats.agility > 0)
                        attributes += "Agility: +" + equip.Stats.agility + "\n";
                    if (equip.Stats.agility < 0)
                        attributes += "Agility: " + equip.Stats.agility + "\n";

                    itemHoverPanel.transform.GetChild(2).GetComponent<Text>().text = attributes;
                }
                else if (item.GetType() == typeof(ItemConsumable))
                {
                    string attributes = "";
                    ItemConsumable cons = (ItemConsumable)item;
                    switch(cons.Action)
                    {
                        case ItemConsumable.UseAction.HEAL:
                            attributes = "Heal: +" + cons.Value + " HP";
                            break;
                        case ItemConsumable.UseAction.DAMAGE:
                            attributes = "Heal: -" + cons.Value + " HP";
                            break;
                        case ItemConsumable.UseAction.ADD_MANA:
                            attributes = "Heal: +" + cons.Value + " MP";
                            break;
                        case ItemConsumable.UseAction.USE_MANA:
                            attributes = "Heal: -" + cons.Value + " MP";
                            break;
                    }

                    if(cons.Stackable)
                    {
                        attributes += "\n\n Quantity: " + cons.StackedAmount;
                    }

                    itemHoverPanel.transform.GetChild(2).GetComponent<Text>().text = attributes;
                }
                else if (item.GetType() == typeof(ItemMaterial))
                {
                    string attributes = "";
                    if (item.Stackable)
                        attributes += "Quantity: " + item.StackedAmount;
                    itemHoverPanel.transform.GetChild(2).GetComponent<Text>().text = attributes;
                }
            }
            else
            {
                if (itemHoverPanel.activeSelf)
                    itemHoverPanel.SetActive(false);
            }
        }
        else
        {
            if (itemHoverPanel.activeSelf)
                itemHoverPanel.SetActive(false);
        }
        if (isUIActive && Input.GetButtonDown("Cancel"))
        {
            if(inventoryPanel.activeSelf)
                ToggleInventoryPanel();
            if (characterPanel.activeSelf)
                ToggleCharacterPanel();
            if (chestPanel.activeSelf)
                chestPanel.SetActive(false);
            UpdateCursor();
            isUIActive = false;
        }
        
    }
    public void UpdateCursor()
    {
        bool hideCursor = !(inventoryPanel.activeSelf || characterPanel.activeSelf || chestPanel.activeSelf);
        if (!hideCursor)
        {
            if (fps.m_MouseLook.XSensitivity != 0.0f && fps.m_MouseLook.YSensitivity != 0.0f)
            {
                fps.m_MouseLook.lockCursor = false;
                BaseMouseSensitivityX = fps.m_MouseLook.XSensitivity;
                BaseMouseSensitivityY = fps.m_MouseLook.YSensitivity;
                fps.m_MouseLook.XSensitivity = 0.0f;
                fps.m_MouseLook.YSensitivity = 0.0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            fps.m_MouseLook.XSensitivity = BaseMouseSensitivityX;
            fps.m_MouseLook.YSensitivity = BaseMouseSensitivityY;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnDestroy()
    {
        PlayerManager.StatsChanged -= UpdateUI;
    }

    public void ToggleInventoryPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        UpdateCursor();
        if (inventoryPanel.activeSelf)
            InventoryNeedUpdate = true;
    }

    public void ToggleCharacterPanel()
    {
        characterPanel.SetActive(!characterPanel.activeSelf);
        UpdateCursor();
    }
}
