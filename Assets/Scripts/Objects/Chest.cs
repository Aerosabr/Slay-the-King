using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public int num;
    public Animator anim;

    public void OpenChest(Player player)
    {
        ChestStage.instance.ChestOpened(num, player);
        anim.Play("Opening");
    }
}
