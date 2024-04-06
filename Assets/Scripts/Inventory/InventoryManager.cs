using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;
    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    public ItemSO[] itemSOs;

	public Transform equipmentStatPanel;

	public Player player;

	[SerializeField]
	private TMP_Text healthText, attackText, defenseText, dexterityText, cooldown_reductionText, attack_speedText, luckText;

	[SerializeField]
	private TMP_Text healthPreText, attackPreText, defensePreText, dexterityPreText, cooldown_reductionPreText, attack_speedPreText, luckPreText;

	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private GameObject selectedItemStats;

	[SerializeField]
	private GameObject selectedItemImage;

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

    public int AddItem(Item item)
    {
        if(item.itemType == ItemType.consumable || item.itemType == ItemType.collectible)
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull == false && itemSlot[i].item.itemName == item.itemName || itemSlot[i].item.quantity == 0)
                {
                    int leftOverItems = itemSlot[i].AddItem(item);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(item);

                    return leftOverItems;
                }
            }
            return item.quantity;
        }
        else
        {
            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                if (equipmentSlot[i].isFull == false && equipmentSlot[i].itemName == itemName || equipmentSlot[i].quantity == 0)
                {
                    int leftOverItems = equipmentSlot[i].AddItem(item);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(item);

                    return leftOverItems;
                }
            }
            return item.quantity;
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

	public void UpdateEquipmentStats(Item item)
	{
		Equipment equipment = (Equipment)item;
		healthText.text = equipment.health.ToString();
		attackText.text = equipment.attack.ToString();
		defenseText.text = equipment.defense.ToString();
		dexterityText.text = equipment.dexterity.ToString();
		cooldown_reductionText.text = equipment.cooldown_reduction.ToString();
		attack_speedText.text = equipment.attack_speed.ToString();
		luckText.text = equipment.luck.ToString();
	}

	public void PreviewEquipmentStats(Item item)
	{
        Equipment equipment = (Equipment)item;
		healthPreText.text = equipment.health.ToString();
		attackPreText.text = equipment.attack.ToString();
		defensePreText.text = equipment.defense.ToString();
		dexterityPreText.text = equipment.dexterity.ToString();
		cooldown_reductionPreText.text = equipment.cooldown_reduction.ToString();
		attack_speedPreText.text = equipment.attack_speed.ToString();
		luckPreText.text = equipment.luck.ToString();

		previewImage.sprite = equipment.sprite;
		selectedItemImage.SetActive(true);
		selectedItemStats.SetActive(true);
	}

	public void TurnOffPreviewStats()
	{
		selectedItemImage.SetActive(false);
		selectedItemStats.SetActive(false);
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