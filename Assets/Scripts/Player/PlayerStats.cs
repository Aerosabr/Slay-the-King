using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int strength, wisdom, armor, resistance, dexterity, luck;

    [SerializeField]
    private TMP_Text strengthText, wisdomText, armorText, resistanceText, dexterityText, luckText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateEquipmentStats();
    }

    public void UpdateEquipmentStats()
    {
        strengthText.text = strength.ToString();
        wisdomText.text = wisdom.ToString();
        armorText.text = armor.ToString();
        resistanceText.text = resistance.ToString();
        dexterityText.text = dexterity.ToString();
        luckText.text = luck.ToString();
    }
}
