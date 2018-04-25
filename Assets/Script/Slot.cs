using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour {
    [SerializeField]
    bool acceptConsumable = true;
    [SerializeField]
    bool acceptEquipment = true;
    [SerializeField]
    bool acceptMaterial = true;

    //Color baseColor;
    //Color hoverColor;

    public bool AcceptConsumable
    {
        get
        {
            return acceptConsumable;
        }

        set
        {
            acceptConsumable = value;
        }
    }

    public bool AcceptEquipment
    {
        get
        {
            return acceptEquipment;
        }

        set
        {
            acceptEquipment = value;
        }
    }

    public bool AcceptMaterial
    {
        get
        {
            return acceptMaterial;
        }

        set
        {
            acceptMaterial = value;
        }
    }
    void Start()
    {
        //baseColor = GetComponent<Image>().color;
        //hoverColor = new Color(0.66f, 0.66f, 0.66f, 0.43f);
    }

    public void ResetColor()
    {
        //GetComponent<Image>().color = baseColor;
    }

    public bool isEmpty()
    {
        return (GetComponentInChildren<UIItemHolder>() == null);
    }
}
