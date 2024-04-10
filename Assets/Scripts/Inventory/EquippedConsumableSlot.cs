using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedConsumableSlot : MonoBehaviour, IPointerClickHandler
{
	// SLOT APPEARANCE
	[SerializeField]
	private Image slotImage;
	[SerializeField]
	private GameObject slotIcon;

	// SLOT DATA
	public ItemSO item = null;
	public int quantity = 0;
	public int maxCapacity = 5;
	public bool isFull = false;

	private InventoryManager inventoryManager;
	public Transform itemStatPanel;
	public TMP_Text quantityText;
	public ActivateConsumables ActiveButton;

	// OTHER VARIABLES
	public bool slotInUse;
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
			if (itemStatPanel.gameObject.activeSelf)
				itemStatPanel.gameObject.SetActive(false);
			inventoryManager.DeselectAllSlots();
			selectedShader.SetActive(false);
			thisItemSelected = false;
		}
		else
		{
			if (slotInUse)
			{
				selectedShader.SetActive(true);
				thisItemSelected = true;
				if (!itemStatPanel.gameObject.activeSelf)
					itemStatPanel.gameObject.SetActive(true);
				itemStatPanel.position = new Vector3(-320f, -25, 0) + transform.position;
				itemStatPanel.GetComponent<ItemStats>().UpdateItemView(item, null, this);
			}
			inventoryManager.DeselectAllSlots();
		}
	}

	public void UseConsumable()
	{
		bool usable = item.UseItem(inventoryManager.player);
		if (usable)
		{
			this.quantity -= 1;
			quantityText.text = this.quantity.ToString();
			quantityText.enabled = true;
			ActiveButton.UpdateConsumableHotbar(item, this);
			if (this.quantity == 0)
			{
				if (itemStatPanel.gameObject.activeSelf)
					itemStatPanel.gameObject.SetActive(false);
				quantityText.enabled = false;
				slotImage.sprite = emptySprite;
				selectedShader.SetActive(false);
				slotIcon.SetActive(true);
				item = null;
				slotInUse = false;
			}
		}
	}

	public void OnRightClick()
	{
		UnEquipConsumable();
	}
	public int EquipConsumable(ItemSO item)
	{
		//Check if slot is already full
		if (isFull)
			return 0;
		// Update Player Stats
		this.item = item;
		slotInUse = true;
		slotImage.sprite = item.itemSprite;
		slotIcon.SetActive(false);
		this.quantity += 1;
		ActiveButton.UpdateConsumableHotbar(item, this);
		if (this.quantity >= maxCapacity)
		{
			isFull = true;
		}
		//Update quantity text
		quantityText.text = this.quantity.ToString();
		quantityText.enabled = true;
		return -1;
	}

	public void UnEquipConsumable()
	{
		if (!slotInUse || string.IsNullOrEmpty(item.itemName))
		{
			return;
		}
		this.quantity -= 1;
		ActiveButton.UpdateConsumableHotbar(item, this);
		quantityText.text = this.quantity.ToString();
		inventoryManager.DeselectAllSlots();
		isFull = false;
		inventoryManager.AddItem(item, 1);
		if (this.quantity == 0)
		{
			if (itemStatPanel.gameObject.activeSelf)
				itemStatPanel.gameObject.SetActive(false);
			quantityText.enabled = false;
			slotImage.sprite = emptySprite;
			slotIcon.SetActive(true);
			selectedShader.SetActive(false);
			item = null;
			slotInUse = false;
		}

	}
}
