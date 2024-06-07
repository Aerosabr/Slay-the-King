using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningStage : MonoBehaviour
{
    public static MiningStage instance;
    public Animator pickaxe;
    public float timeRemaining;
    public int rocksBroken;
    private bool Active = true;

    public Text rockText;
    public Slider starRating;

    [SerializeField] private Text Timer;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();

    public void Awake()
    {
        instance = this;
        loadPickaxes();
    }

    public void FixedUpdate()
    {
        if(Active)
        {
			UpdateRocksMineUI();
			if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                Timer.text = timeRemaining.ToString("F2") + "s";
            }
            else
                unequipPickaxes();
        }
    }

    public void loadPickaxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Pickaxe>();
            RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = pickaxe.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Axe");
    }

    public void unequipPickaxes()
    {
        Active = false;
        int increment = 0;
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Pickaxe>());
            player.GetComponent<Class>().equipCurrent();
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
            increment++;
        }
        CooldownManager.instance.LoadCooldowns();
        TeleportManager.instance.LoadNextStage("Mining");
    }

    public void UpdateRocksMineUI()
    {
        rockText.text = "Rocks broken: " + rocksBroken.ToString();
        if (rocksBroken == 6)
            starRating.value = 1f;
        else if (rocksBroken == 4)
            starRating.value = 0.7f;
        else if (rocksBroken == 2)
            starRating.value = 0.4f;
    }

}
