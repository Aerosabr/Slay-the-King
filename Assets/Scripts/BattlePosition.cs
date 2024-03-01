using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePosition : MonoBehaviour
{
    public bool hasCharacter;
    public Entity Entity;
    
    public BattlePosition()
    {
        hasCharacter = false;
        Entity = null;
    }
}
