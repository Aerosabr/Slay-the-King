using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestStage : MonoBehaviour
{
    public static ChestStage instance;

    public GameObject Chest1;
    public GameObject Chest2;
    public GameObject Chest3;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Player>().canInteract = true;
        }
    }

    public void ChestOpened(int num, Player player)
    {
        switch(num)
        {
            case 1:
                Chest2.GetComponent<Chest>().anim.Play("Disappear");
                Chest3.GetComponent<Chest>().anim.Play("Disappear");
                break;
            case 2:
                Chest1.GetComponent<Chest>().anim.Play("Disappear");
                Chest3.GetComponent<Chest>().anim.Play("Disappear");
                break;
            case 3:
                Chest1.GetComponent<Chest>().anim.Play("Disappear");
                Chest2.GetComponent<Chest>().anim.Play("Disappear");
                break;
        }
        TeleportManager.instance.LoadNextStage("Chests");
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
