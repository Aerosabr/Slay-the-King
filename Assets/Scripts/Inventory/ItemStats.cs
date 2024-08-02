using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
	public GameObject statPanel;
	public InventoryManager manager;
	public ItemSlot slot;
	public EquippedConsumableSlot consumeSlot;

	public TMP_Text itemName;
	public TMP_Text itemDescription;

	public GameObject equipButton;
	public GameObject unequipButton;
	public GameObject useButton;

	public void UpdateItemView(ItemSO item, ItemSlot slot, EquippedConsumableSlot consumeSlot)
	{
		if (!statPanel.activeSelf)
			statPanel.SetActive(true);
		if (item.itemType == ItemType.consumable)
		{
			if (slot != null)
			{
				equipButton.SetActive(true);
				unequipButton.SetActive(false);
			}
			if (consumeSlot != null)
			{
				unequipButton.SetActive(true);
				equipButton.SetActive(false);
			}
			useButton.SetActive(true);
		}
		else
		{
			equipButton.SetActive(false);
			unequipButton.SetActive(false);
			useButton.SetActive(false);
		}
		this.slot = slot;
		this.consumeSlot = consumeSlot;
		itemName.text = item.itemName;
		itemDescription.text = item.itemDescription;
	}

	public void EquipButton()
	{
		if(slot != null)
			slot.CheckAvailableSlot();
	}
	
	public void UnEquipButton()
	{
		if (consumeSlot != null)
			consumeSlot.UnEquipConsumable();
	}
	
	public void ExitButton()
	{
		statPanel.SetActive(false);
		if (slot != null)
		{
			slot.selectedShader.SetActive(false);
			slot.thisItemSelected = false;
		}
		if (consumeSlot != null)
		{
			consumeSlot.selectedShader.SetActive(false);
			consumeSlot.thisItemSelected = false;
		}
	}

	public void UseButton()
	{
		if (slot != null)
			slot.UseItem();
		if (consumeSlot != null)
			consumeSlot.UseConsumable();
	}

	public void DeleteButton()
	{
		bool isGone = slot.DeleteItem();
		if(isGone)
			statPanel.SetActive(false);
	}

	public void DropButton()
	{
		slot.OnRightClick();
	}

}
