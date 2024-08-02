using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    // ITEM DATA
    public ItemSO item;
    public int quantity;
    public bool isFull;
    public Sprite emptySprite;

    // ITEM SLOT
    [SerializeField]
    private Image itemImage;

    // EQUIPMENT SLOT
    [SerializeField]
    private EquippedSlot helmetSlot, chestSlot, legSlot, weaponSlot, amuletSlot, ringSlot;

    public GameObject selectedShader;
    public bool thisItemSelected;

    public Transform equipmentStatPanel; 

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
        this.quantity = 1;
        isFull = true;

        itemImage.sprite = item.itemSprite;


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
        if (!GameManager.instance.canEquip)
            return;

        if (item.itemType == ItemType.helmet)
            helmetSlot.EquipGear(item);
        if (item.itemType == ItemType.chest)
            chestSlot.EquipGear(item);
        if (item.itemType == ItemType.leg)
            legSlot.EquipGear(item);
        if (item.itemType == ItemType.weapon)
            weaponSlot.EquipGear(item);
        if (item.itemType == ItemType.amulet)
            amuletSlot.EquipGear(item);
        if (item.itemType == ItemType.ring)
            ringSlot.EquipGear(item);

        EmptySlot();
    }

    public void OnLeftClick()
    {
        if (isFull)
        {
            if (thisItemSelected)
            {
                if(item.itemType == ItemType.weapon)
                {
                    if (inventoryManager.player.transform.GetComponent<Class>().checkEquippable(item.weaponType))
                        EquipGear();
				}
                else
                {
					EquipGear();
				}
				if (equipmentStatPanel.gameObject.activeSelf)
					equipmentStatPanel.gameObject.SetActive(false);
				inventoryManager.DeselectAllSlots();
				selectedShader.SetActive(false);
				thisItemSelected = false;
			}
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;

				if (!equipmentStatPanel.gameObject.activeSelf)
					equipmentStatPanel.gameObject.SetActive(true);
				equipmentStatPanel.localPosition = new Vector3(-244, 15, 0);
				EquipmentSO equipment = (EquipmentSO) item;
				equipmentStatPanel.GetComponent<EquipmentStats>().SetEquipmentSlot(this);
				equipmentStatPanel.GetComponent<EquipmentStats>().UpdateEquipmentStatPanel(equipment);
			}
        }
        else
        {
            if (thisItemSelected)
            {
                if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
            }
            else
            {
                if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
                inventoryManager.DeselectAllSlots();
            }
        }
    }


    public void EmptySlot()
    {
        itemImage.sprite = emptySprite;

        isFull = false;
        item = null;
        quantity = 0;

        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;

    }


    public void OnRightClick()
    {
        if (this.quantity <= 0 || string.IsNullOrEmpty(item.itemName))
        {
            // If the slot is empty, exit the method early without dropping an item.
            if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
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
        if (this.quantity <= 0)
            EmptySlot();
    }

	public bool DeleteItem()
	{
		// Check if the inventory slot is empty before dropping items.
		if (this.quantity <= 0 || string.IsNullOrEmpty(item.itemName))
		{
			// If the slot is empty, exit the method early without dropping an item.
			if (equipmentStatPanel.gameObject.activeSelf)
				equipmentStatPanel.gameObject.SetActive(false);
            return false;
		}

		this.quantity -= 1;
		if (this.quantity <= 0)
		{
			EmptySlot();
			return true;
		}
		return false;
	}
}