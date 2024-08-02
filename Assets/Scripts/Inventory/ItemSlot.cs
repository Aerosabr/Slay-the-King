using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //ITEM DATA
    public ItemSO item;
    public int quantity;
    public bool isFull;
    public Sprite emptySprite;

    [SerializeField]
    private int maxNumberOfItems;

    //ITEM SLOT
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

	public EquippedConsumableSlot[] slots = new EquippedConsumableSlot[3];

    //ITEM DESCRIPTION SLOT
    public Transform itemStatPanel;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(ItemSO item, int quantity)
    {
        //Check if slot is already full
        if (isFull)
            return quantity;
        this.item = item;
        this.quantity += quantity;

        itemImage.sprite = item.itemSprite;

        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;

            //return leftovers
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }
        //Update quantity text
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            if (itemStatPanel.gameObject.activeSelf)
                itemStatPanel.gameObject.SetActive(false);
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(false);
            thisItemSelected = false;
        }
        else
        {
            if (item)
            {
                if (!itemStatPanel.gameObject.activeSelf)
                    itemStatPanel.gameObject.SetActive(true);
                itemStatPanel.position = new Vector3(-320f, -25, 0) + transform.position;
                itemStatPanel.GetComponent<ItemStats>().UpdateItemView(item, this, null);
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
            }
            
        }
    }


    public void EmptySlot()
    {
		quantityText.enabled = false;
        quantityText.text = "";

        itemImage.sprite = emptySprite;

        item = null;
        quantity = 0;
        isFull = false;

		if (itemStatPanel.gameObject.activeSelf)
			itemStatPanel.gameObject.SetActive(false);
		if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;
    }


    public void OnRightClick()
    {
        // Check if the inventory slot is empty before dropping items.
        if (this.quantity <= 0 || string.IsNullOrEmpty(item.itemName))
        {
            // Slot is empty, so do nothing and return early.
            if(itemStatPanel.gameObject.activeSelf)
                    itemStatPanel.gameObject.SetActive(false);
            return;
        }

        // Create a new item
        GameObject itemToDrop = new GameObject(item.itemName);
        ItemPlaceholder newItem = itemToDrop.AddComponent<ItemPlaceholder>();
        newItem.quantity = 1;
        newItem.itemSO = item;
        // Create and modify the SpriteRenderer
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = item.itemSprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Environment";

        // Add a collider
        itemToDrop.AddComponent<BoxCollider2D>();

        // Set the location
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2.0f, 0, 0.0f);
        itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);

        // Subtract the item
        this.quantity -= 1;
        quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
            EmptySlot();
    }

    public bool CheckAvailableSlot()
    {
        if (!GameManager.instance.canEquip)
            return false;
		bool found = false;
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item)
			{
                if (slots[i].item.itemName == this.item.itemName)
                {
                    found = true;
                    if (slots[i].quantity != slots[i].maxCapacity)
                    {
                        EquipItem(slots[i]);
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
			}
		}
        if(!found)
        {
			for (int i = 0; i < slots.Length; i++)
			{
				if (!slots[i].item)
				{
					EquipItem(slots[i]);
                    return true;
				}
			}
		}
		return false;
    }

    public void EquipItem(EquippedConsumableSlot selectedSlot)
    {
		if (!itemStatPanel.gameObject.activeSelf)
			itemStatPanel.gameObject.SetActive(true);
        this.quantity += selectedSlot.EquipConsumable(item);
		quantityText.text = this.quantity.ToString();
		if (this.quantity <= 0)
			EmptySlot();
	}

    public void UseItem()
    {
		if (!itemStatPanel.gameObject.activeSelf)
			itemStatPanel.gameObject.SetActive(true);
		bool usable = item.UseItem(inventoryManager.player);
		if (usable)
		{
			this.quantity -= 1;
			quantityText.text = this.quantity.ToString();
			if (this.quantity <= 0)
				EmptySlot();
		}
	}

    public bool DeleteItem()
    {
		// Check if the inventory slot is empty before dropping items.
		if (this.quantity <= 0 || string.IsNullOrEmpty(item.itemName))
		{
			// Slot is empty, so do nothing and return early.
			if (itemStatPanel.gameObject.activeSelf)
				itemStatPanel.gameObject.SetActive(false);
			return false;
		}

		this.quantity -= 1;
		quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
        {
            EmptySlot();
            return true;
        }
        return false;
	}

}
