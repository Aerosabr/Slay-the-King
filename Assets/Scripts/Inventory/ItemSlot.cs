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
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    public ItemType itemType;

    [SerializeField]
    private int maxNumberOfItems;

    //ITEM SLOT
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    //ITEM DESCRIPTION SLOT
    public Transform itemStatPanel;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(ItemSO item, string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType)
    {
        //Check if slot is already full
        if (isFull)
            return quantity;
        this.item = item;
        this.itemType = itemType;
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        this.quantity += quantity;

        itemImage.sprite = itemSprite;

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
            if (string.IsNullOrEmpty(itemName) || quantity <= 0)
            {
                if(itemStatPanel.gameObject.activeSelf)
                    itemStatPanel.gameObject.SetActive(false);
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
                ItemDescriptionNameText.text = "";
                ItemDescriptionText.text = "";
            }
            else
            {
                Debug.Log($"Using item: {itemName}");
                if(!itemStatPanel.gameObject.activeSelf)
                    itemStatPanel.gameObject.SetActive(true);
                bool usable = inventoryManager.UseItem(itemName);
                if (usable)
                {
                    this.quantity -= 1;
                    quantityText.text = this.quantity.ToString();
                    if (this.quantity <= 0)
                        EmptySlot();
                }
            }
        }
        else
        {
            if(!itemStatPanel.gameObject.activeSelf)
                itemStatPanel.gameObject.SetActive(true);
            itemStatPanel.position = new Vector3(-380f, 0, 0) + transform.position;
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
        }
    }


    private void EmptySlot()
    {
        quantityText.enabled = false;
        quantityText.text = "";

        itemImage.sprite = emptySprite;

        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";

        itemName = "";
        itemDescription = "";
        itemSprite = null;
        quantity = 0;
        isFull = false;


        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;
    }


    public void OnRightClick()
    {
        // Check if the inventory slot is empty before dropping items.
        if (this.quantity <= 0 || string.IsNullOrEmpty(itemName))
        {
            // Slot is empty, so do nothing and return early.
            if(itemStatPanel.gameObject.activeSelf)
                    itemStatPanel.gameObject.SetActive(false);
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
        quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
            EmptySlot();
    }

}
