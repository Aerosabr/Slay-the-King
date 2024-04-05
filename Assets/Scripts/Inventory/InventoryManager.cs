using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;
    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    public ItemSO[] itemSOs;

	//ITEM DESCRIPTION SLOT
	public Transform itemStatPanel;
	public Image itemDescriptionImage;
	public TMP_Text ItemDescriptionNameText;
	public TMP_Text ItemDescriptionText;

	public Transform equipmentStatPanel;

	public Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("InventoryMenu"))
            Inventory();

    }

    void Inventory()
    {
        if (InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
        }
        else
        {
            InventoryMenu.SetActive(true);
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable;
            }
        }
        return false;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType)
    {
        if(itemType == ItemType.consumable || itemType == ItemType.collectible)
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
                {
                    int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription, itemType);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription, itemType);

                    return leftOverItems;
                }
            }
            return quantity;
        }
        else
        {
            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                if (equipmentSlot[i].isFull == false && equipmentSlot[i].itemName == itemName || equipmentSlot[i].quantity == 0)
                {
                    int leftOverItems = equipmentSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription, itemType);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription, itemType);

                    return leftOverItems;
                }
            }
            return quantity;
        }

    }

    public void DeselectAllSlots()
    {
        for (int i = 0;i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].selectedShader.SetActive(false);
            equipmentSlot[i].thisItemSelected = false;
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            equippedSlot[i].selectedShader.SetActive(false);
            equippedSlot[i].thisItemSelected = false;
        }
    }

    public void PreviewItem()
    {
		if (string.IsNullOrEmpty(itemName) || quantity <= 0)
		{
			if (itemStatPanel.gameObject.activeSelf)
				itemStatPanel.gameObject.SetActive(false);
			inventoryManager.DeselectAllSlots();
			selectedShader.SetActive(false);
			thisItemSelected = false;
			ItemDescriptionNameText.text = "";
			ItemDescriptionText.text = "";
			itemDescriptionImage.sprite = emptySprite;
		}
		else
		{
			Debug.Log($"Using item: {itemName}");
			if (!itemStatPanel.gameObject.activeSelf)
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
		if (!itemStatPanel.gameObject.activeSelf)
			itemStatPanel.gameObject.SetActive(true);
		itemStatPanel.position = new Vector3(-380f, 0, 0) + transform.position;
		inventoryManager.DeselectAllSlots();
		selectedShader.SetActive(true);
		thisItemSelected = true;
		ItemDescriptionNameText.text = itemName;
		ItemDescriptionText.text = itemDescription;
		itemDescriptionImage.sprite = itemSprite ?? emptySprite;
	}
}


public enum ItemType
{
    consumable,
    collectible,
    helmet,
    chest,
    leg,
    weapon,
    amulet,
    ring,
};