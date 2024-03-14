using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int strength, wisdom, armor, resistance, dexterity, luck;

    [SerializeField]
    private TMP_Text strengthText, wisdomText, armorText, resistanceText, dexterityText, luckText;

    [SerializeField]
    private TMP_Text strengthPreText, wisdomPreText, armorPreText, resistancePreText, dexterityPreText, luckPreText;

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
        strengthText.text = strength.ToString();
        wisdomText.text = wisdom.ToString();
        armorText.text = armor.ToString();
        resistanceText.text = resistance.ToString();
        dexterityText.text = dexterity.ToString();
        luckText.text = luck.ToString();
    }

    public void PreviewEquipmentStats(int strength, int wisdom, int armor, int resistance, int dexterity, int luck, Sprite itemSprite)
    {
        strengthPreText.text = strength.ToString();
        wisdomPreText.text = wisdom.ToString();
        armorPreText.text = armor.ToString();
        resistancePreText.text = resistance.ToString();
        dexterityPreText.text = dexterity.ToString();
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
