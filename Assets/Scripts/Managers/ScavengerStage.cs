using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerStage : MonoBehaviour
{
    public static ScavengerStage instance;
    public GameObject Coin;
    public int spawnRate;
    public int numSpawned;
    public int numCollected;
    private bool Active = true;
    [SerializeField] private Text Timer;
    public Text coinText;
    public Slider starRating;
    private float timeElapsed = 30f;

    private void Awake()
    {
        instance = this;
        SpawnCoins();
    }

    private void FixedUpdate()
    {
        if (Active)
        {
			UpdateCoinUI();
			if (timeElapsed >= 0)
            {
                timeElapsed -= Time.deltaTime;
                Timer.text = timeElapsed.ToString("F2") + "s";
            }
            else
                EndStage();
        }
    }

    private void SpawnCoins()
    {
        int num = 1;
        for (int i = -9; i < 10; i++)
        {
            for (int j = -7; j < 8; j++)
            {
                if (Random.Range(1, 101) <= spawnRate || num % 9 == 0)
                {
                    Instantiate(Coin, new Vector2(i, j), Quaternion.identity);
                    numSpawned++;
                    Debug.Log("Spawned " + num + " Num spawned: " + numSpawned);
                }
                num++;
            }
        }
    }

    public void CoinCollected()
    {
        numCollected++;
    }

    private void EndStage()
    {
        Active = false;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Item"))
        {
            Debug.Log(obj.name);
            Destroy(obj);
        }
        TeleportManager.instance.LoadNextStage("Scavenger");
    }

    public void UpdateCoinUI()
    {
        coinText.text = "Coins Collected: " + numCollected;
        if (numCollected == 50)
            starRating.value = 1f;
        else if (numCollected == 35)
            starRating.value = 0.7f;
        else if (numCollected == 20)
            starRating.value = 0.4f;
	}
}
