using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIItemHolder : MonoBehaviour {
    [SerializeField]
    Item item;
    [SerializeField]
    int id;
    Text amount;
    public Item Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
            UpdateUIText();
        }
    }

    // Use this for initialization
    void Start () {
        if(item == null)
            item = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().Inventory[id];
        UpdateUIText();

	}

    public void UpdateUIText()
    {
        if(amount == null)
            amount = GetComponentInChildren<Text>();
        if (!item.Stackable && amount != null)
            amount.enabled = false;
        else
            if (item.Stackable && amount != null)
                amount.enabled = true;
        if(amount != null && item.Stackable && item.ID != -1)
            amount.text = item.StackedAmount.ToString();
    }

    // Update is called once per frame
    void Update () {
        //temporaire
        if (amount != null && item.Stackable && item.ID != -1)
            amount.text = item.StackedAmount.ToString();
    }
}
