using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentStats : MonoBehaviour
{
    public GameObject statPanel;
    public EquipmentSlot equipmentSlot;
    public EquippedSlot equippedSlot;

    public TMP_Text itemName;
    public TMP_Text healthNum;
	public TMP_Text attackNum;
	public TMP_Text defenseNum;
    public TMP_Text dexNum;
    public TMP_Text cdrNum;
    public TMP_Text attackspdNum;
    public TMP_Text luckNum;

    public GameObject equipButton;
    public GameObject unequipButton;

    public void SetEquipmentSlot(EquipmentSlot slot)
    {
        equipmentSlot = slot;
        equipButton.SetActive(true);
        unequipButton.SetActive(false);
    }
    public void SetEquippedSlot(EquippedSlot slot)
    {
		equippedSlot = slot;
		equipButton.SetActive(false);
		unequipButton.SetActive(true);
	}

    public void disableButton()
    {
		equipButton.SetActive(false);
		unequipButton.SetActive(false);
	}
    public void UpdateEquipmentStatPanel(EquipmentSO item)
    {
        itemName.text = item.itemName;
        healthNum.text = item.health.ToString();
        attackNum.text = item.attack.ToString();
        defenseNum.text = item.defense.ToString();
        dexNum.text = item.dexterity.ToString();
        cdrNum.text = item.cooldown_reduction.ToString();
        attackspdNum.text = item.attack_speed.ToString();
        luckNum.text = item.luck.ToString();
	}

    public void EquipButton()
    {
        equipmentSlot.OnLeftClick();
    }

    public void UnequipButton() 
    {
        equippedSlot.OnRightClick();
	}

    public void ExitButton()
    {
        statPanel.SetActive(false);
        if (equipmentSlot)
        {
            equipmentSlot.selectedShader.SetActive(false);
            equipmentSlot.thisItemSelected = false;

		}
        if (equippedSlot)
        {
            equippedSlot.selectedShader.SetActive(false);
            equippedSlot.thisItemSelected = false;
        }
    }
}
