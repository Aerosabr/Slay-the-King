using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestStage : MonoBehaviour
{
    public static ChestStage instance;

    public GameObject Chest1;
    public GameObject Chest2;
    public GameObject Chest3;

    public void Awake()
    {
        instance = this;
    }

    public void ChestOpened(int num, Player player)
    {
        switch(num)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}

public class ChestOdds
{
    public int t1;
    public int t2;
    public int t3;

    public int total;

    public ChestOdds()
    {

    }
}
