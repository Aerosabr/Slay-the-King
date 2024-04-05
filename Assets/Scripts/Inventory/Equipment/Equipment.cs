using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
	public int health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck;
	public Animator animator;
	public void PreviewEquipment(Player player)
	{
		if (player != null)
		{
			player.PreviewEquipmentStats(health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck, sprite);
		}
	}

	public void EquipItem(Player player)
	{
		// Update Stats
		if (player != null)
		{
			player.maxHealth += health;
			player.baseAttack += attack;
			player.baseDefense += defense;
			player.baseDexterity += dexterity;
			player.baseCDR += cooldown_reduction;
			player.baseAttackSpeed += attack_speed;
			player.baseLuck += luck;

			player.UpdateEquipmentStats();
		}
	}

	public void UnEquipItem(Player player)
	{
		// Update Stats
		if (player != null)
		{
			player.baseMaxHealth -= health;
			player.baseAttack -= attack;
			player.baseDefense -= defense;
			player.baseDexterity -= dexterity;
			player.baseCDR -= cooldown_reduction;
			player.baseAttackSpeed -= attack_speed;
			player.baseLuck -= luck;

			player.UpdateEquipmentStats();
		}
	}
}
