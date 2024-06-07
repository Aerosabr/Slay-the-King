using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemCreation : MonoBehaviour
{
    public static ItemCreation instance;

	public List<EquipmentSO> equipmentList = new List<EquipmentSO>();
    public List<ConsumeSO> consumeList = new List<ConsumeSO>();
    public List<ItemSO> itemList = new List<ItemSO>();

	private void Awake()
	{
		instance = this;
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
		sr.sortingLayerName = "Environment";

		// Add a collider
		itemToDrop.AddComponent<BoxCollider2D>();

		// Set the location
		itemToDrop.transform.position = spot.position + new Vector3(2.0f, 0, 0.0f);
		itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);
	}
}
