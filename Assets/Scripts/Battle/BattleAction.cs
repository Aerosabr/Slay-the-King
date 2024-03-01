using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : MonoBehaviour
{
    //Battle script that represents each action made by players
    public int PlayerNum;
    public string Action;

    public int Energy;
    public string EnergyMovement; //Increase, Decrease, None

    public BattleAction(int playerNum, string action, int energy, string movement)
    {
        PlayerNum = playerNum;
        Action = action;
        Energy = energy;
        EnergyMovement = movement;
        CalculateEnergy();
    }

    public BattleAction(int playerNum, string action)
    {
        PlayerNum = playerNum;
        Action = action;
    }

    public void CalculateEnergy()
    {
        Debug.Log("Calculating energy");
        switch (EnergyMovement)
        {
            case "Increase":
                BattleManager.instance.ActionEnergy.currentEnergy += Energy;
                if (BattleManager.instance.ActionEnergy.currentEnergy > BattleManager.instance.ActionEnergy.maxEnergy)      
                    BattleManager.instance.ActionEnergy.currentEnergy = BattleManager.instance.ActionEnergy.maxEnergy;        
                break;
            case "Decrease":
                BattleManager.instance.ActionEnergy.currentEnergy -= Energy;
                break;
        }
        ActionEnergyUI.instance.UpdateUI();
    }

}

