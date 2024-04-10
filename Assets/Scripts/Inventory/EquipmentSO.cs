using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentSO : ItemSO
{
    public int health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck;
    public Animator animator;

    public void EquipItem(InventoryManager manager)
    {
        manager.AddStatsToPlayer(this, +1);
    }

    public void UnEquipItem(InventoryManager manager)
    {
		// Update Stats
		manager.AddStatsToPlayer(this, -1);
	}
}
