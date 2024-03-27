using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int health, attack, defense, dexterity, cooldown_reduction, attack_speed, luck;

    [SerializeField]
    private TMP_Text healthText, attackText, defenseText, dexterityText, cooldown_reductionText, attack_speedText, luckText;

    [SerializeField]
    private TMP_Text healthPreText, attackPreText, defensePreText, dexterityPreText, cooldown_reductionPreText, attack_speedPreText, luckPreText;

    [SerializeField]
    private Image previewImage;

    [SerializeField]
    private GameObject selectedItemStats;

    [SerializeField]
    private GameObject selectedItemImage;

    // Start is called before the first frame update
    void Start()
    {
        UpdateEquipmentStats();
    }

    public void UpdateEquipmentStats()
    {
        healthText.text = health.ToString();
        attackText.text = attack.ToString();
        defenseText.text = defense.ToString();
        dexterityText.text = dexterity.ToString();
        cooldown_reductionText.text = cooldown_reduction.ToString();
        attack_speedText.text = attack_speed.ToString();
        luckText.text = luck.ToString();
    }

    public void PreviewEquipmentStats(int health, int attack, int defense, int dexterity, int cooldown_reduction, int attack_speed, int luck, Sprite itemSprite)
    {
        healthPreText.text = health.ToString();
        attackPreText.text = attack.ToString();
        defensePreText.text = defense.ToString();
        dexterityPreText.text = dexterity.ToString();
        cooldown_reductionPreText.text = cooldown_reduction.ToString();
        attack_speedPreText.text = attack_speed.ToString();
        luckPreText.text = luck.ToString();

        previewImage.sprite = itemSprite;
        selectedItemImage.SetActive(true);
        selectedItemStats.SetActive(true);
    }

    public void TurnOffPreviewStats()
    {
        selectedItemImage.SetActive(false);
        selectedItemStats.SetActive(false);
    }
}
