using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStage : MonoBehaviour
{
    public static TreeStage instance;
    public Animator axe;
    public float timeElapsed;
    public bool treeCut;

    public void Awake()
    {
        instance = this;
        loadAxes();
    }

    public void FixedUpdate()
    {
        if (!treeCut)
            timeElapsed += Time.deltaTime;
    }

    public void loadAxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Axe>();
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = axe.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Axe");
    }

    public void unequipAxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Axe>());
            player.GetComponent<Class>().equipCurrent();
            
        }
        CooldownManager.instance.LoadCooldowns();
    }

    public void treeFelled()
    {
        TeleportManager.instance.LoadNextStage("Tree");
        treeCut = true;
    }
}
