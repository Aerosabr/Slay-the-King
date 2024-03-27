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

    public void PreviewEquipment()
    {
        GameObject.Find("StatManager").GetComponent<PlayerStats>().PreviewEquipmentStats(health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck, itemSprite);
    }
    
    public void EquipItem()
    {
        //Update Stats
        PlayerStats playerstats = GameObject.Find("StatManager").GetComponent<PlayerStats>();
        playerstats.health += health;
        playerstats.attack += attack;
        playerstats.defense += defense;
        playerstats.dexterity += dexterity;
        playerstats.cooldown_reduction += cooldown_reduction;
        playerstats.attack_speed += attack_speed;
        playerstats.luck += luck;

        playerstats.UpdateEquipmentStats();
    }

    public void UnEquipItem()
    {
        //Update Stats
        PlayerStats playerstats = GameObject.Find("StatManager").GetComponent<PlayerStats>();
        playerstats.health -= health;
        playerstats.attack -= attack;
        playerstats.defense -= defense;
        playerstats.dexterity -= dexterity;
        playerstats.cooldown_reduction -= cooldown_reduction;
        playerstats.attack_speed -= attack_speed;
        playerstats.luck -= luck;

        playerstats.UpdateEquipmentStats();
    }
}
