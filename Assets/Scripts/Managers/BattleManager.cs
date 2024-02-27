using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
