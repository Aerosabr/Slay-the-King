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
    private EquippedSlot helmetSlot, chestSlot, legSlot, weaponSlot, amuletSlot, ringSlot;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;
    private EquipmentSOLibrary equipmentSOLibrary;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        equipmentSOLibrary = GameObject.Find("InventoryCanvas").GetComponent<EquipmentSOLibrary>();
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
            OnLeftClick();
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
        if (itemType == ItemType.leg)
            legSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.weapon)
            weaponSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.amulet)
            amuletSlot.EquipGear(itemSprite, itemName, itemDescription);
        if (itemType == ItemType.ring)
            ringSlot.EquipGear(itemSprite, itemName, itemDescription);

        EmptySlot();
    }

    public void OnLeftClick()
    {
        if (isFull)
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
                for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
                {
                    if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                        equipmentSOLibrary.equipmentSO[i].PreviewEquipment();
                }
            }
        }
        else
        {
            if (thisItemSelected)
            {
                GameObject.Find("StatManager").GetComponent<PlayerStats>().TurnOffPreviewStats();
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
            }
            else
            {
                GameObject.Find("StatManager").GetComponent<PlayerStats>().TurnOffPreviewStats();
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
            }
        }
    }



    private void EmptySlot()
    {
        itemImage.sprite = emptySprite;

        isFull = false;

        itemName = "";
        itemDescription = "";
        itemSprite = null;
        quantity = 0;

        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;

    }


    public void OnRightClick()
    {
        if (this.quantity <= 0 || string.IsNullOrEmpty(itemName))
        {
            // If the slot is empty, exit the method early without dropping an item.
            return;
        }

        // Create a new item
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;
        newItem.itemType = this.itemType;

        // Create and modify the SpriteRenderer
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Environment";

        // Add a collider
        itemToDrop.AddComponent<BoxCollider2D>();

        // Set the location
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2.0f, 0, 0.0f);
        itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);

        // Subtract the item
        this.quantity -= 1;
        if (this.quantity <= 0)
            EmptySlot();
    }
}