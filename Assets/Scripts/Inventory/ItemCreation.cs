using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCreation : MonoBehaviour
{
    public static ItemCreation instance;

	public List<EquipmentSO> equipmentList = new List<EquipmentSO>();
    public List<ConsumeSO> consumeList = new List<ConsumeSO>();
    public List<ItemSO> itemList = new List<ItemSO>();

    public Dictionary<string, int> equipmentDict = new Dictionary<string, int>();
    public Dictionary<string, int> consumeDict = new Dictionary<string, int>();
    public Dictionary<string, int> itemDict = new Dictionary<string, int>();

    public List<string> possibleWeapons = new List<string>();

	private void Awake()
	{
		instance = this;
        equipmentDict.Add("Helmet", 0);
        equipmentDict.Add("Chestplate",1);
        equipmentDict.Add("Leggings",2);
        equipmentDict.Add("Greatsword",3);
        equipmentDict.Add("DualAxes",4);
        equipmentDict.Add("SwordShield",5);
        equipmentDict.Add("Hammer",6);
        equipmentDict.Add("Staff",7);
        equipmentDict.Add("Wand",8);
        equipmentDict.Add("Bow",9);
        equipmentDict.Add("Knives",10);
        equipmentDict.Add("Daggers",11);
        equipmentDict.Add("Scythe",12);
        equipmentDict.Add("MaceShield",13);
        equipmentDict.Add("Tome",14);
        equipmentDict.Add("Ring", 15);
        equipmentDict.Add("Amulet", 16);

        consumeDict.Add("HealthPotion", 0);
        consumeDict.Add("DefensePotion", 1);
        consumeDict.Add("AttackSpeedPotion", 2);

        itemDict.Add("Gold", 0);
    }

    public void LoadPossibleWeapons()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            switch (player.GetComponent<Player>().Class)
            {
                case "Warrior":
                    possibleWeapons.Add("Greatsword");
                    possibleWeapons.Add("DualAxes");
                    break;
                case "Mage":
                    possibleWeapons.Add("Wand");
                    possibleWeapons.Add("Staff");
                    break;
                case "Rogue":
                    possibleWeapons.Add("Scythe");
                    possibleWeapons.Add("Daggers");
                    break;
                case "Ranger":
                    possibleWeapons.Add("Bow");
                    possibleWeapons.Add("Knives");
                    break;
                case "Cleric":
                    possibleWeapons.Add("MaceShield");
                    possibleWeapons.Add("Tome");
                    break;
                case "Knight":
                    possibleWeapons.Add("SwordShield");
                    possibleWeapons.Add("Hammer");
                    break;
            }
        }
    }

    public string GenerateRandomEquipment(int num)
    {
        string equipment = "";
        switch (num)
        {
            case 1: //Armor
                List<string> temp = new List<string> { "Helmet", "Chestplate", "Leggings" };
                equipment = temp[UnityEngine.Random.Range(0, temp.Count)];
                break;
            case 2: //Weapon
                equipment = possibleWeapons[UnityEngine.Random.Range(0, possibleWeapons.Count)];
                break;
            case 3: //Accessory
                List<string> temp2 = new List<string> { "Ring", "Amulet" };
                equipment = temp2[UnityEngine.Random.Range(0, temp2.Count)];
                break;
        }

        return equipment;
    }

    public List<SubStat> GenerateSubstats(int bonus)
    {
        int lv = PlayerManager.instance.GetAverageLevel() + bonus;
        int num = UnityEngine.Random.Range(1, 101) + (int)(PlayerManager.instance.GetTotalLuck() * (2f / (float)PlayerManager.instance.Players.Count));
        List<string> substatNames = new List<string> { "Health", "Attack", "Defense", "Dexterity", "Cooldown Reduction", "Attack Speed", "Luck" };
        List<SubStat> substats = new List<SubStat>();
        int numSubstats;

        if (num < 51) //Common       
            return substats;
        else if (num < 81) //Uncommon
            numSubstats = 1;
        else if (num < 93) //Rare
            numSubstats = 2;
        else if (num < 99) //Epic
            numSubstats = 3;
        else //Legendary
            numSubstats = 4;
 
        int statPoints = lv * numSubstats;
        for (int i = 0; i < numSubstats; i++)
        {
            substats.Add(new SubStat(substatNames[UnityEngine.Random.Range(0, substatNames.Count)], 1));
            statPoints--;
        }

        while (statPoints > 0)
        {
            substats[UnityEngine.Random.Range(0, substats.Count)].increaseValue();
            statPoints--;
        }

        return substats;
    }

	public void CreateItem(int index, int quantity, Transform spot)
	{
		ItemSO newItem = ScriptableObject.CreateInstance<ItemSO>();
		newItem.BuildItem(itemList[index]);
		SpawnDropItem(newItem, quantity, spot);
	}

	public void CreateConsumeable(int index, int quantity, Transform spot)
	{
		ConsumeSO newItem = ScriptableObject.CreateInstance<ConsumeSO>();
		newItem.BuildItem(consumeList[index]);
		SpawnDropItem(newItem, quantity, spot);
	}

	public void CreateEquipment(int index, SubStat mainStat, List<SubStat> statList, Transform spot)
	{
		EquipmentSO newItem = ScriptableObject.CreateInstance<EquipmentSO>();
        Debug.Log("Indexs" + index);
        newItem.BuildItem(equipmentList[index]);
        newItem.mainStat = mainStat;
        newItem.subStats = statList;
        newItem.ReadStats();
        SpawnDropItem(newItem, 1, spot);
    }


    public void SpawnDropItem(ItemSO item, int quantity, Transform spot)
    {
		// Create a new item
		GameObject itemToDrop = new GameObject(item.itemName);
		ItemPlaceholder newItem = itemToDrop.AddComponent<ItemPlaceholder>();
		newItem.quantity = quantity;
		newItem.itemSO = item;
		// Create and modify the SpriteRenderer
		SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
		sr.sprite = item.itemSprite;
		sr.sortingOrder = 5;
		sr.sortingLayerName = "Item";

		// Add a collider
		itemToDrop.AddComponent<BoxCollider2D>();
        BoxCollider2D temp = itemToDrop.GetComponent<BoxCollider2D>();
        temp.includeLayers = LayerMask.GetMask("Player");

        // Set the location
        itemToDrop.transform.position = spot.position;
		itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);
	}

    #region Event Stage Rewards
    public void OneStarLoot()
    {
        //25x Gold, 1 + 1 (50)% -> Weapon/Armor
        CreateItem(itemDict["Gold"], 25, PlayerManager.instance.Players[0].transform);
        List<int> odds = new List<int> { 100, 50 };
        for (int i = 0; i < 2; i++)
        {
            if (UnityEngine.Random.Range(1, 101) <= odds[i])
            {
                string equipment = GenerateRandomEquipment(UnityEngine.Random.Range(1, 3));
                List<SubStat> subStats = GenerateSubstats(0);
                SubStat mainStat;
                switch (equipment)
                {
                    case "Helmet":
                        mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel());
                        break;
                    case "Chestplate":
                        mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel());
                        break;
                    case "Leggings":
                        mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel());
                        break;
                    default:
                        mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel());
                        break;
                }

                CreateEquipment(equipmentDict[equipment], mainStat, subStats, PlayerManager.instance.Players[0].transform);
            }
        }
    }

    public void TwoStarLoot()
    {
        //50x Gold, 2 + 1 (50%) + 1 (25%) -> Weapon/Armor
        CreateItem(itemDict["Gold"], 50, PlayerManager.instance.Players[0].transform);
        List<int> odds = new List<int> { 100, 100, 50, 25 };
        for (int i = 0; i < 2; i++)
        {
            if (UnityEngine.Random.Range(1, 101) <= odds[i])
            {
                string equipment = GenerateRandomEquipment(UnityEngine.Random.Range(1, 3));
                List<SubStat> subStats = GenerateSubstats(0);
                SubStat mainStat;
                switch (equipment)
                {
                    case "Helmet":
                        mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel());
                        break;
                    case "Chestplate":
                        mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel());
                        break;
                    case "Leggings":
                        mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel());
                        break;
                    default:
                        mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel());
                        break;
                }

                CreateEquipment(equipmentDict[equipment], mainStat, subStats, PlayerManager.instance.Players[0].transform);
            }
        }
    }

    public void ThreeStarLoot()
    {
        //100x Gold, 2 + 1 (50%) + 1 (25%) -> Weapon/Armor
        CreateItem(itemDict["Gold"], 100, PlayerManager.instance.Players[0].transform);
        List<int> odds = new List<int> { 100, 100, 50, 25 };
        for (int i = 0; i < 2; i++)
        {
            if (UnityEngine.Random.Range(1, 101) <= odds[i])
            {
                string equipment = GenerateRandomEquipment(UnityEngine.Random.Range(1, 3));
                List<SubStat> subStats = GenerateSubstats(1);
                SubStat mainStat;
                switch (equipment)
                {
                    case "Helmet":
                        mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel() + 1);
                        break;
                    case "Chestplate":
                        mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel() + 1);
                        break;
                    case "Leggings":
                        mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel() + 1);
                        break;
                    default:
                        mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel() + 1);
                        break;
                }

                CreateEquipment(equipmentDict[equipment], mainStat, subStats, PlayerManager.instance.Players[0].transform);
            }
        }
    }
    #endregion
}
