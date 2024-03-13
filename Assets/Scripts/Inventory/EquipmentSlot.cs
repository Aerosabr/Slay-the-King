using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    //ITEM DATA
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    public ItemType itemType;

    //ITEM SLOT

    [SerializeField]
    private Image itemImage;

    //EQUIPMENT SLOT
    [SerializeField]
    private EquippedSlot helmetSlot, chestSlot, legsSlot, weaponSlot, glovesSlot, bootsSlot;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType)
    {
        //Check if slot is already full
        if (isFull)
            return quantity;

        this.itemType = itemType;
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        this.quantity = 1;
        isFull = true;

        itemImage.sprite = itemSprite;


        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (thisItemSelected)
            {
                EquipGear();
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void EquipGear()
    {
        if (itemType == ItemType.helmet)
            helmetSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.chest)
            chestSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.legs)
            legsSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.weapon)
            weaponSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.gloves)
            glovesSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.boots)
            bootsSlot.EquipGear(itemSprite, itemName, itemDescription);

        EmptySlot();
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            Debug.Log($"Using item: {itemName}");
            bool usable = inventoryManager.UseItem(itemName);
            if (usable)
            {
                this.quantity -= 1;
                if (this.quantity <= 0)
                    EmptySlot();
            }
        }

        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
        }
    }

    private void EmptySlot()
    {
        itemImage.sprite = emptySprite;
        isFull = false;
    }

    public void OnRightClick()
    {
        //create a new item
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;
        newItem.itemType = this.itemType;

        //Create and modify the SR
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Environment";

        //Add a collider
        itemToDrop.AddComponent<BoxCollider2D>();

        //Set the location
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2.0f, 0, 0.0f);
        itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);

        //Subtract the item
        this.quantity -= 1;
        if (this.quantity <= 0)
            EmptySlot();
    }
}
