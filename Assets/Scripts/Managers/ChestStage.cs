using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestStage : MonoBehaviour
{
    public static ChestStage instance;

    public GameObject Chest1;
    public GameObject Chest2;
    public GameObject Chest3;

    public Text chestOddText;
    public Slider starRating;

    public float Timer = 540;
    public Text timerText;

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

	public void FixedUpdate()
	{
		Timer -= Time.deltaTime;
        if (Timer != 0)
		    UpdateTimerUI(Timer);
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

    public void UpdateChestOddUI(int tier)
    {
        if(tier == 1)
        {
            chestOddText.text = "Chances: 50%";
            starRating.value = 0.4f;
        }
        else if(tier == 2)
        {
            chestOddText.text = "Changes: 35%";
            starRating.value = 0.7f;
        }
        else if(tier == 3)
        {
            chestOddText.text = "Chances: 15%";
            starRating.value = 1f;
        }
    }

    public void UpdateTimerUI(float Timer)
    {
        timerText.text = Timer.ToString("F2") + "s";

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

    //Call the "UpdateChestOddUI" with ChestStage.instance.UpdateChestOddUI(tier number);
}
