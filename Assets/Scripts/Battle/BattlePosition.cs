using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePosition : MonoBehaviour
{
    //Battle script that manages each position on the battlefield
    public bool hasCharacter;
    public Entity Entity;
    
    public BattlePosition()
    {
        hasCharacter = false;
        Entity = null;
    }
}
