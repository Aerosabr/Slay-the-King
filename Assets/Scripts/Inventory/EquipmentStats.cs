using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentStats : MonoBehaviour
{
    public GameObject statPanel;
    public GameObject deleteBTN;
    public GameObject dropBTN;
    public EquipmentSlot equipmentSlot;
    public EquippedSlot equippedSlot;

    public TMP_Text itemName;
    public TMP_Text mainName;
	public TMP_Text mainNum;
    public List<TMP_Text> subName;
	public List<TMP_Text> subNum;

    public Image itemIcon;
    public Image itemBackground;

    public List<Sprite> itemBackgroundList;

    public GameObject equipButton;
    public GameObject unequipButton;

    public void SetEquipmentSlot(EquipmentSlot slot)
    {
        equipmentSlot = slot;
        equipButton.SetActive(true);
        unequipButton.SetActive(false);
		deleteBTN.SetActive(true);
		dropBTN.SetActive(true);
	}
    public void SetEquippedSlot(EquippedSlot slot)
    {
		equippedSlot = slot;
		equipButton.SetActive(false);
		unequipButton.SetActive(true);
		deleteBTN.SetActive(false);
		dropBTN.SetActive(false);
	}

    public void disableButton()
    {
		equipButton.SetActive(false);
		unequipButton.SetActive(false);
        deleteBTN.SetActive(false);
        dropBTN.SetActive(false);
	}
    public void UpdateEquipmentStatPanel(EquipmentSO item)
    {
        ClearStatPanel();
        int counter = 0;
        itemName.text = item.itemName;

        mainName.text = item.mainStat.name;
        mainNum.text = item.mainStat.value.ToString();
		foreach (var stat in item.subStats)
		{
            subName[counter].text = stat.name;
            subNum[counter].text = stat.value.ToString();
            counter += 1;
		}

        itemBackground.sprite = itemBackgroundList[counter];
        itemIcon.sprite = item.itemSprite;
	}

    public void ClearStatPanel()
    {
        itemName.text = "";
        mainName.text = "";
        mainNum.text = "";
        foreach (var name in subName)
            name.text = "";
        foreach (var num in subNum)
            num.text = "";
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

	public void DeleteButton()
	{
		bool isGone = equipmentSlot.DeleteItem();
		if (isGone)
			statPanel.SetActive(false);
	}

	public void DropButton()
	{
		equipmentSlot.OnRightClick();
	}
}
