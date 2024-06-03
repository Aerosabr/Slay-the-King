using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public int num;
    public Animator anim;

    public void Interacted(Player player)
    {
        ChestStage.instance.ChestOpened(num, player);
        anim.Play("Opening");
    }
}
