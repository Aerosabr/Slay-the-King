using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    // SLOT APPEARANCE
    [SerializeField]
    private Image slotImage;
	[SerializeField]
	private GameObject slotIcon;

    // SLOT DATA
    public ItemSO item;

    private InventoryManager inventoryManager;
	public Transform equipmentStatPanel;
	// OTHER VARIABLES
	public bool slotInUse;
    public float xPanelPlacement = -320f;
	[SerializeField]
    public GameObject selectedShader;
    [SerializeField]
    public bool thisItemSelected;
    [SerializeField]
    private Sprite emptySprite;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            if (slotInUse)
            {

                if (item.itemType != ItemType.weapon)
                {
                    UnEquipGear();
                    slotInUse = false;
                }
                if (equipmentStatPanel.gameObject.activeSelf)
                {
                    equipmentStatPanel.gameObject.SetActive(false);
                    inventoryManager.DeselectAllSlots();
                    selectedShader.SetActive(false);
                    thisItemSelected = false;
                }
			}
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
            }
        }
        else
        {
			inventoryManager.DeselectAllSlots();
			if (slotInUse)
            {
                selectedShader.SetActive(true);
                thisItemSelected = true;
                if (!equipmentStatPanel.gameObject.activeSelf)
                {
					equipmentStatPanel.gameObject.SetActive(true);
				}
				equipmentStatPanel.position = new Vector3(xPanelPlacement, -25f, 0) + transform.position;
                EquipmentSO equipment = (EquipmentSO)item;
                equipmentStatPanel.GetComponent<EquipmentStats>().SetEquippedSlot(this);
                equipmentStatPanel.GetComponent<EquipmentStats>().UpdateEquipmentStatPanel(equipment);
				if (item.itemType == ItemType.weapon)
					equipmentStatPanel.GetComponent<EquipmentStats>().disableButton();
			}
		}
    }

    public void OnRightClick()
    {
        UnEquipGear();
    }

    public void EquipGear(ItemSO item)
    {
        if (item.itemType == ItemType.weapon)
            GameObject.Find("PlayerManager").transform.GetChild(0).transform.GetChild(0).GetComponent<Class>().changeWeapon(item.weaponType);

        // Check if item already there
        if (slotInUse)
            UnEquipGear();

        // Update Image
        slotImage.sprite = item.itemSprite;
		slotIcon.SetActive(false);
		// Update Player Stats
		this.item = item;
        EquipmentSO equipment = (EquipmentSO)item;
        equipment.EquipItem(inventoryManager);

        slotInUse = true;
    }

    public void UnEquipGear()
    {
        if (!slotInUse || string.IsNullOrEmpty(item.itemName))
        {
            return;
        }

        inventoryManager.DeselectAllSlots();
        //inventoryManager.AddItem(itemName, 1, itemSprite, itemDescription, itemType);

        slotImage.sprite = emptySprite;
		slotIcon.SetActive(true);
		slotInUse = false;

		EquipmentSO equipment = (EquipmentSO)item;
		equipment.UnEquipItem(inventoryManager);
        inventoryManager.AddItem(item, 1);
        item = null;

	}
}
