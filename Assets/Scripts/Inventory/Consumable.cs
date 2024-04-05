using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{
	public int value;

	public bool UseItem()
	{
		//if (statToChange == StatToChange.health)
		/*{
			Player playerHealth = GameObject.FindObjectOfType<Player>();
			if (playerHealth.currentHealth == playerHealth.maxHealth)
			{
				return false;
			}
			else
			{
				playerHealth.Healed(amountToChangeStat);
				return true;
			}
		}*/
		return false;
	}
}
