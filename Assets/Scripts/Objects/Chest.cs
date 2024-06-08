using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private int num;
    private bool Activated = false;
    [SerializeField] private Animator anim;

    public void Interacted(Player player)
    {
        if (!Activated)
        {
            Debug.Log(num + " Activated");
            Activated = true;
            ChestStage.instance.ChestOpened(num, player);
            anim.Play("Opening");
        } 
    }

    public void Disappear()
    {
        Debug.Log(num + " Disappearing");
        Activated = true;
        anim.Play("Disappear");
        Debug.Log(num + " Disappearing");
    }
}
