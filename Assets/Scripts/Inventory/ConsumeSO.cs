using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConsumeSO : ItemSO
{
	public enum StatToChange
	{
		none,
		health,
		attack,
		defense,
		dexterity,
		cooldown_reduction,
		attack_speed,
		luck,
	};

	public StatToChange statToChange = new StatToChange();
	public int amountToChangeStat;

	public override void BuildItem(ItemSO item)
	{
		ConsumeSO consumeable = (ConsumeSO)item;
		statToChange = consumeable.statToChange;
		itemName = item.itemName;
		itemSprite = item.itemSprite;
		weaponType = item.weaponType;
		itemType = item.itemType;
		itemDescription = item.itemDescription;
	}

	public override bool UseItem(Player player)
	{
		if (statToChange == StatToChange.health)
		{
			if (player.currentHealth == player.maxHealth)
			{
				return false;
			}
			else
			{
				player.Healed(amountToChangeStat);
				return true;
			}
		}
		return false;
	}
}
