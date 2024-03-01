using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionEnergyUI : MonoBehaviour
{
    public static ActionEnergyUI instance;
    public List<GameObject> Energy = new List<GameObject>();
    public GameObject EnergyText;

    void Awake()
    {
        instance = this;  
    }

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        int currentEnergy = BattleManager.instance.ActionEnergy.currentEnergy;
        EnergyText.GetComponent<Text>().text = currentEnergy.ToString();
        foreach (GameObject i in Energy)
        {
            if (currentEnergy > 0)
            {
                i.GetComponent<Image>().color = Color.yellow;
                currentEnergy--;
            }
            else
                i.GetComponent<Image>().color = Color.white;
        }
    }
}
