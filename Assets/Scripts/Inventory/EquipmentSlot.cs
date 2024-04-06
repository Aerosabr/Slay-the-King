using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    // ITEM DATA
    public Item item;
	public Sprite emptySprite;
	public bool isFull;
	// ITEM SLOT
	[SerializeField]
    private Image itemImage;

    // EQUIPMENT SLOT
    [SerializeField]
    private EquippedSlot helmetSlot, chestSlot, legSlot, weaponSlot, amuletSlot, ringSlot;

    public GameObject selectedShader;
    public bool thisItemSelected;

	public GameObject equipmentStatPanel;

	private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(Item item)
    {
        //Check if slot is already full
        if (isFull)
            return this.item.quantity;

		isFull = true;
		this.item = item;
        itemImage.sprite = item.sprite;
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

                Player player = FindObjectOfType<Player>(); // Find the player in the scene
                for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
                {
                    if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                    {
                        if(!equipmentStatPanel.gameObject.activeSelf)
                            equipmentStatPanel.gameObject.SetActive(true);
                        equipmentStatPanel.transform.position = new Vector3(-380f, 0, 0) + transform.position;
                        equipmentSOLibrary.equipmentSO[i].PreviewEquipment(player);
                    }       
                }
            }
        }
        else
        {
            if (thisItemSelected)
            {
                if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
                TurnOffPreviewStats();
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
            }
            else
            {
                if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
                TurnOffPreviewStats();
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
            }
        }
    }

    private void TurnOffPreviewStats()
    {
        Player player = FindObjectOfType<Player>(); // Find the player in the scene
        if (player != null)
        {
            player.TurnOffPreviewStats();
        }
    }

    private void EmptySlot()
    {
        itemImage.sprite = emptySprite;

        isFull = false;
        item = null;

        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;

    }


    public void OnRightClick()
    {
        if (item.quantity <= 0 || string.IsNullOrEmpty(item.itemName))
        {
            // If the slot is empty, exit the method early without dropping an item.
            if(equipmentStatPanel.gameObject.activeSelf)
                    equipmentStatPanel.gameObject.SetActive(false);
            return;
        }

        // Create a new item
        GameObject itemToDrop = new GameObject(item.itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem = item;

		// Create and modify the SpriteRenderer
		SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = item.sprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Environment";

        // Add a collider
        itemToDrop.AddComponent<BoxCollider2D>();

        // Set the location
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2.0f, 0, 0.0f);
        itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);

        // Subtract the item
        item.quantity -= 1;
        if (item.quantity <= 0)
            EmptySlot();
    }
}