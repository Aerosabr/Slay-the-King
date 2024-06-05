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
    [SerializeField] private GameObject Timer;
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
            if (timeElapsed >= 0)
            {
                timeElapsed -= Time.deltaTime;
                Timer.GetComponent<Text>().text = timeElapsed.ToString("#.00") + "  Coins Collected: " + numCollected;
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
}
