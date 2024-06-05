using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeStage : MonoBehaviour
{
    public static TreeStage instance;
    public Animator axe;
    public float timeElapsed;
    public bool treeCut;
    [SerializeField] private GameObject Timer;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();

    public void Awake()
    {
        instance = this;
        loadAxes();
    }

    public void FixedUpdate()
    {
        if (!treeCut)
        {
            timeElapsed += Time.deltaTime;
            Timer.GetComponent<Text>().text = timeElapsed.ToString("#.00");
        }
    }

    public void loadAxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Axe>();
            RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = axe.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Axe");
    }

    public void unequipAxes()
    {
        int increment = 0;
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Axe>());
            player.GetComponent<Class>().equipCurrent();
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
            increment++;
        }
        CooldownManager.instance.LoadCooldowns();
    }

    public IEnumerator treeFelled()
    {
        yield return new WaitForSeconds(1f);
        TeleportManager.instance.LoadNextStage("Tree");
        treeCut = true;
        unequipAxes();
    }
}
