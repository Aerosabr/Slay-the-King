using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestStage : MonoBehaviour
{
    public static ChestStage instance;

    [SerializeField] private GameObject Chest1;
    [SerializeField] private GameObject Chest2;
    [SerializeField] private GameObject Chest3;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Player>().canInteract = true;
            player.GetComponent<Player>().CameraZoom(5);
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.GetComponent<PlayerSpriteController>().Sprintable = false;
        }

        CooldownManager.instance.LoadCooldowns("None");
    }

    public void ChestOpened(int num, Player player)
    {
        switch(num)
        {
            case 1:
                Chest2.GetComponent<Chest>().Disappear();
                Chest3.GetComponent<Chest>().Disappear();
                break;
            case 2:
                Chest1.GetComponent<Chest>().Disappear();
                Chest3.GetComponent<Chest>().Disappear();
                break;
            case 3:
                Chest1.GetComponent<Chest>().Disappear();
                Chest2.GetComponent<Chest>().Disappear();
                break;
        }
        TeleportManager.instance.LoadNextStage("Chests");
        foreach (GameObject i in PlayerManager.instance.Players)
        {
            i.GetComponent<Player>().CameraZoomOutSlow(8);
            i.GetComponent<Class>().equipCurrent();
            i.GetComponent<PlayerSpriteController>().Sprintable = true;
        }
        CooldownManager.instance.LoadCooldowns();
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
