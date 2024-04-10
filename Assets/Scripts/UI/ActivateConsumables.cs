using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivateConsumables : MonoBehaviour
{
    public Image spriteImage;
	public TMP_Text consumableCounter;
	public EquippedConsumableSlot slot;


	public void UpdateConsumableHotbar(ItemSO item, EquippedConsumableSlot slot)
	{
		spriteImage.sprite = item.itemSprite;
		consumableCounter.text = slot.quantity.ToString();
		this.slot = slot;
	}

	public void Activate()
	{
		slot.UseConsumable();
	}
}
