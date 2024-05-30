using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStage : MonoBehaviour
{
    public static TreeStage instance;
    public Animator axe;
    public float timeElapsed;

    public void Awake()
    {
        instance = this;
        loadAxes();
    }

    public void FixedUpdate()
    {
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
    }

    public void unequipAxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Axe>());
            player.GetComponent<Class>().equipCurrent();
            
        }
    }

    public void treeFelled()
    {

    }
}
