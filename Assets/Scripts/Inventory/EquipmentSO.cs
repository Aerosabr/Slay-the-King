using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentSO : ItemSO
{
	public int health, attack, defense, dexterity, cooldownReduction, attackSpeed, luck;
	public SubStat mainStat;
	public List<SubStat> subStats;
	public Animator animator;

	public override void BuildItem(ItemSO item)
	{
		EquipmentSO equipment = (EquipmentSO)item;
		this.itemName = item.itemName;
		this.itemSprite = item.itemSprite;
		this.weaponType = item.weaponType;
		this.itemType = item.itemType;
		this.itemDescription = item.itemDescription;
		this.health = equipment.health;
		this.attack = equipment.attack;
		this.defense = equipment.defense;
		this.dexterity = equipment.dexterity;
		this.cooldownReduction = equipment.cooldownReduction;
		this.attackSpeed = equipment.attackSpeed;
		this.luck = equipment.luck;
		this.mainStat = equipment.mainStat;
		this.subStats = new List<SubStat>(equipment.subStats);
		this.animator = equipment.animator;
	}

	public void ReadStats()
	{
		AddStat(mainStat.name, mainStat.value);
		foreach(var stat in  subStats)
		{
			AddStat(stat.name, stat.value);
		}

	}
	public void AddStat(string name, int value)
	{
		if (name == "Health")
			health += value;
		else if(name == "Attack")
			attack += value;
		else if(name == "Defense")
			defense += value;
		else if (name == "Dexterity")
			dexterity += value;
		else if(name == "Cooldown Reduction")
			cooldownReduction += value;
		else if(name == "Attack Speed")
			attackSpeed += value;
		else if(name == "Luck")
			luck += value;
	}
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
