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
            player.changeHealth(health, 0);
            player.changeAttack(attack, 0);
            player.changeDefense(defense, 0);
            player.changeDexterity(dexterity, 0);
            player.changeAttackSpeed(attack_speed, 0);
            player.CDR += cooldown_reduction;
            player.Luck += luck;

            player.UpdateEquipmentStats();
        }
    }

    public void UnEquipItem(Player player)
    {
        // Update Stats
        if (player != null)
        {
            player.changeHealth(-health, 0);
            player.changeAttack(-attack, 0);
            player.changeDefense(-defense, 0);
            player.changeDexterity(-dexterity, 0);
            player.changeAttackSpeed(-attack_speed, 0);
            player.CDR -= cooldown_reduction;
            player.Luck -= luck;

            player.UpdateEquipmentStats();
        }
    }
}
