using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerStage : MonoBehaviour
{
    public GameObject Coin;
    public int spawnRate;
    public int numSpawned;
    public int numCollected;

    private void Awake()
    {
        SpawnCoins();
    }

    public void SpawnCoins()
    {
        for (int i = -9; i < 10; i++)
        {
            for (int j = -7; j < 8; j++)
            {
                if (Random.Range(1, 101) <= spawnRate)
                {
                    Instantiate(Coin, new Vector2(i, j), Quaternion.identity);
                    numSpawned++;
                }
            }
        }
    }
}
