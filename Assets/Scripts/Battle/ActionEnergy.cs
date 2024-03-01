using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEnergy 
{
    //Class for BattleManager that contains energy amounts
    public int maxEnergy;
    public int currentEnergy;

    public ActionEnergy(int max, int current)
    {
        maxEnergy = max;
        currentEnergy = current;
    }
}
