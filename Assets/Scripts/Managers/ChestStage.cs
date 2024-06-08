using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestStage : MonoBehaviour
{
    public static ChestStage instance;

    [SerializeField] private GameObject Chest1;
    [SerializeField] private GameObject Chest2;
    [SerializeField] private GameObject Chest3;

    public Text chestOddText;
    public Slider starRating;

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
            player.GetComponent<PlayerSpriteController>().Sprintable = false;
        }
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

        ProcessRewards(player);
        TeleportManager.instance.LoadNextStage("Chests");
        foreach (GameObject i in PlayerManager.instance.Players)
        {
            i.GetComponent<Player>().CameraZoomOutSlow(8);
            i.GetComponent<PlayerSpriteController>().Sprintable = true;
        }
    }

    private void ProcessRewards(Player player)
    {
        int num = Random.Range(1, 101) + player.Luck;

        if (num <= 55)
        {
            ItemCreation.instance.OneStarLoot();
            UpdateChestOddUI(1);
        }
        else if (num <= 85)
        {
            ItemCreation.instance.TwoStarLoot();
            UpdateChestOddUI(2);
        }
        else
        {
            ItemCreation.instance.ThreeStarLoot();
            UpdateChestOddUI(3);
        }
    }

    public void UpdateChestOddUI(int tier)
    {
        chestOddText.text = "Reward: ";
        if (tier == 1)
            starRating.value = 0.4f;
        else if (tier == 2)
            starRating.value = 0.7f;
        else if (tier == 3)
            starRating.value = 1f;
    }

}

