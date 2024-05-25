using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public Transform spawnArea;
    public float spawnRadius;
    public int maxEnemies = 0;
    public int enemiesKilled = 0;
    public int Credit = 50;
    public int Wave = 1;
    public GameObject WaveHUD;

    public List<GameObject> Enemies = new List<GameObject>();

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnRadius, spawnRadius));
    }

    private void Awake()
    {
        instance = this;
        WaveHUD = GameObject.Find("WaveHUD");
        StartCoroutine(Spawning());
        /*
        for (int i = 1; i < 4; i++)
        {
            GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/FlyingEye"), transform);
            temp.GetComponent<FlyingEye>().Cost = 2 * i;
            Enemies.Add(temp);
            temp.SetActive(false);
            temp.name = "FlyingEye" + i;
        }

        SpawnEnemy(Resources.Load<GameObject>("Prefabs/FlyingEye"));
        */
    }

    public IEnumerator Spawning()
    {
        while (Credit > 0)
        {
            GameObject enemy = Enemies[Random.Range(0, Enemies.Count)];
            if (Credit >= enemy.GetComponent<Entity>().Cost)
            {
                SpawnEnemy(enemy);
                Credit -= enemy.GetComponent<Entity>().Cost;
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            } 
        }
        StartCoroutine(WaitUntilEnemiesKilled());
    }

    public void SpawnEnemy(GameObject Enemy)
    {
        bool searchLocation = true;
        Vector3 SpawnArea = Vector3.zero;
        while (searchLocation)
        {
            searchLocation = false;
            SpawnArea = new Vector3(Random.Range(-spawnRadius / 2, spawnRadius / 2), Random.Range(-spawnRadius / 2, spawnRadius / 2));
            foreach (GameObject player in PlayerManager.instance.Players)
            {
                if (Vector2.Distance(SpawnArea, player.transform.position) < 5)
                {
                    searchLocation = true;
                    break;
                }
            }
        }
        GameObject temp = Instantiate(Enemy, SpawnArea, Quaternion.identity);
        temp.SetActive(true);
        maxEnemies++;
    }

    public IEnumerator WaitUntilEnemiesKilled()
    {
        while (enemiesKilled != maxEnemies)
        {
            yield return new WaitForSeconds(2f);
        }
        Credit = 40 + (Wave * 10);
        enemiesKilled = 0;
        maxEnemies = 0;
        Wave++;
        WaveHUD.GetComponent<Text>().text = "Wave " + Wave;
        StartCoroutine(Spawning());
    }
}
 