using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
	public Player player;
    public ItemSlot[] itemSlot;
    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    public ItemSO[] itemSOs;

    public Transform StatPanel;
	public TMP_Text itemName;
	public TMP_Text healthNum;
	public TMP_Text attackNum;
	public TMP_Text defenseNum;
	public TMP_Text dexNum;
	public TMP_Text cdrNum;
	public TMP_Text attackspdNum;
	public TMP_Text luckNum;

	// Start is called before the first frame update
	void Start()
    {
        player = GameObject.Find("PlayerManager").transform.GetChild(0).GetChild(0).GetComponent<Player>();
		UpdatePlayerStatPanel();
		equipCurrentWeapon();
        InventoryMenu.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("InventoryMenu"))
            Inventory();

    }

    public void equipCurrentWeapon()
    {
        foreach(var weapon in itemSOs)
        {
            if(weapon.itemName == GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player1Weapon)
            {
                equippedSlot[3].EquipGear(weapon);
			}
        }
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

    public int AddItem(ItemSO item, int quantity)
    {
        if(item.itemType == ItemType.consumable || item.itemType == ItemType.collectible)
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull == false && itemSlot[i].quantity == 0 || itemSlot[i].item.itemName == item.itemName)
                {
                    int leftOverItems = itemSlot[i].AddItem(item, quantity);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(item, leftOverItems);

                    return leftOverItems;
                }
            }
            return quantity;
        }
        else
        {
            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                if (equipmentSlot[i].isFull == false)
                {
                    int leftOverItems = equipmentSlot[i].AddItem(item, quantity);
                    if (leftOverItems > 0)
                        leftOverItems = AddItem(item, leftOverItems);

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

    public void AddStatsToPlayer(EquipmentSO equipment, int change)
    {
        player.UpdateEquipmentStats(equipment, change);
        UpdatePlayerStatPanel();
    }

	public void UpdatePlayerStatPanel()
	{
		healthNum.text = player.baseMaxHealth.ToString();
		attackNum.text = player.baseAttack.ToString();
		defenseNum.text = player.baseDefense.ToString();
		dexNum.text = player.baseDexterity.ToString();
		cdrNum.text = player.CDR.ToString();
		attackspdNum.text = player.baseAttackSpeed.ToString();
		luckNum.text = player.Luck.ToString();
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