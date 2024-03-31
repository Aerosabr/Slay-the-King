using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentSO : ScriptableObject
{
    public string itemName;
    public int health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck;
    [SerializeField]
    private Sprite itemSprite;

    public void PreviewEquipment(Player player)
    {
        if (player != null)
        {
            player.PreviewEquipmentStats(health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck, itemSprite);
        }
    }

    public void EquipItem(Player player)
    {
        // Update Stats
        if (player != null)
        {
            player.health += health;
            player.attack += attack;
            player.defense += defense;
            player.dexterity += dexterity;
            player.cooldown_reduction += cooldown_reduction;
            player.attack_speed += attack_speed;
            player.luck += luck;

            player.UpdateEquipmentStats();
        }
    }

    public void UnEquipItem(Player player)
    {
        // Update Stats
        if (player != null)
        {
            player.health -= health;
            player.attack -= attack;
            player.defense -= defense;
            player.dexterity -= dexterity;
            player.cooldown_reduction -= cooldown_reduction;
            player.attack_speed -= attack_speed;
            player.luck -= luck;

            player.UpdateEquipmentStats();
        }
    }
}
