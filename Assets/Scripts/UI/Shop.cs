using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject ShopUI;
    [SerializeField] private bool isActive;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isActive)
        {
            ShopUI.SetActive(true);
            isActive = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isActive)
        {
            ShopUI.SetActive(false);
            isActive = false;
        }
    }

    public void BuyEquipment()
    {
        if (PlayerManager.instance.Players[0].GetComponent<Player>().Money >= 25)
        {
            PlayerManager.instance.Players[0].GetComponent<Player>().Money -= 25;
            List<string> substatNames = new List<string> { "Health", "Attack", "Defense", "Dexterity", "Cooldown Reduction", "Attack Speed", "Luck" };
            ItemCreation IC = ItemCreation.instance;
            string equipment = IC.GenerateRandomEquipment(Random.Range(1, 4));
            List<SubStat> subStats = IC.GenerateSubstats(0);
            SubStat mainStat;
            switch (equipment)
            {
                case "Helmet":
                    mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel());
                    break;
                case "Chestplate":
                    mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel());
                    break;
                case "Leggings":
                    mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel());
                    break;
                case "Ring":
                    mainStat = new SubStat(substatNames[Random.Range(0, substatNames.Count)], PlayerManager.instance.GetAverageLevel());
                    break;
                case "Amulet":
                    mainStat = new SubStat(substatNames[Random.Range(0, substatNames.Count)], PlayerManager.instance.GetAverageLevel());
                    break;
                default:
                    mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel());
                    break;
            }

            IC.CreateEquipment(IC.equipmentDict[equipment], mainStat, subStats, PlayerManager.instance.Players[0].transform);

            Debug.Log("Generated");
        }     
    }

    public void BuyHP()
    {
        if (PlayerManager.instance.Players[0].GetComponent<Player>().Money >= 40)
        {
            PlayerManager.instance.Players[0].GetComponent<Player>().Money -= 40;
            ItemCreation IC = ItemCreation.instance;
            IC.CreateConsumeable(0, 1, PlayerManager.instance.Players[0].transform);
        }
    }
}
