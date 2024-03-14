using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentSO : ScriptableObject
{
    public string itemName;
    public int strength, wisdom, armor, resistance, dexterity, luck;
    [SerializeField]
    private Sprite itemSprite;

    public void PreviewEquipment()
    {
        GameObject.Find("StatManager").GetComponent<PlayerStats>().PreviewEquipmentStats(strength, wisdom, armor, resistance, dexterity, luck, itemSprite);
    }
    
    public void EquipItem()
    {
        //Update Stats
        PlayerStats playerstats = GameObject.Find("StatManager").GetComponent<PlayerStats>();
        playerstats.strength += strength;
        playerstats.wisdom += wisdom;
        playerstats.armor += armor;
        playerstats.resistance += resistance;
        playerstats.dexterity += dexterity;
        playerstats.luck += luck;

        playerstats.UpdateEquipmentStats();
    }

    public void UnEquipItem()
    {
        //Update Stats
        PlayerStats playerstats = GameObject.Find("StatManager").GetComponent<PlayerStats>();
        playerstats.strength -= strength;
        playerstats.wisdom -= wisdom;
        playerstats.armor -= armor;
        playerstats.resistance -= resistance;
        playerstats.dexterity -= dexterity;
        playerstats.luck -= luck;

        playerstats.UpdateEquipmentStats();
    }
}
