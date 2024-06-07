using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemCreation : MonoBehaviour
{
    public static ItemCreation instance;

	List<EquipmentSO> equipmentList = new List<EquipmentSO>();
    List<ConsumeSO> consumeList = new List<ConsumeSO>();
    List<ItemSO> itemList = new List<ItemSO>();

	private void Awake()
	{
		instance = this;
	}

	public void CreateItem(int index, int quantity)
	{
		ItemSO newItem = ScriptableObject.CreateInstance<ItemSO>();
		newItem.BuildItem(itemList[index]);
		SpawnDropItem(newItem, quantity);
	}

	public void CreateConsumeable(int index, int quantity)
	{
		ConsumeSO newItem = ScriptableObject.CreateInstance<ConsumeSO>();
		newItem.BuildItem(consumeList[index]);
		SpawnDropItem(newItem, quantity);
	}

	public void CreateEquipment(int index, SubStat mainStat, SubStat statList)
	{
		EquipmentSO newItem = ScriptableObject.CreateInstance<EquipmentSO>();
		newItem.BuildItem(equipmentList[index]);
		SpawnDropItem(newItem, 1);
	}


    public void SpawnDropItem(ItemSO item, int quantity)
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
		itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2.0f, 0, 0.0f);
		itemToDrop.transform.localScale = new Vector3(.5f, .5f, .5f);
	}
}
