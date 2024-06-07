using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStage : MonoBehaviour
{
    public static BattleStage instance;

    //Spawner
    public Transform spawnArea;
    public float spawnRadius;
    public int maxEnemies = 0;
    public int enemiesKilled = 0;
    public int Credit = 50;
    public int Wave = 1;
    public GameObject WaveHUD;
    public Text EnemyCountText;
    public List<GameObject> Enemies = new List<GameObject>();
    public bool Active = true;
    public bool Spawnable;

    //Map
    public GameObject Map;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnRadius, spawnRadius));
    }

    private void Awake()
    {
        instance = this;
        if (Spawnable)
        {
            StartCoroutine(Spawning());
            StartCoroutine(CheckAlive());
        }
        else
            TeleportManager.instance.LoadNextStage("Battle");
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
        
        if (Wave == 3)
            TeleportManager.instance.LoadNextStage("Battle");
        else
        {
            Credit = 50 + (Wave * 10);
            enemiesKilled = 0;
            maxEnemies = 0;
            Wave++;
            WaveHUD.GetComponent<Text>().text = "Wave " + Wave;
            StartCoroutine(Spawning());
        }

    }

    public IEnumerator CheckAlive()
    {
        //End game if all players are dead
        bool Alive = true;
        while(Alive)
        {
            Alive = false;
            yield return new WaitForSeconds(.5f);
            for (int i = 0; i < PlayerManager.instance.Players.Count; i++)
            {
                if (PlayerManager.instance.Players[i].GetComponent<Player>().currentHealth > 0)
                    Alive = true;
            }
        }
        Active = false;
        Debug.Log("All players dead");
    }

    //Use this for the EnemyCount UI
    public void UpdateCount()
    {
        EnemyCountText.text = "Enemy: " + 90.ToString() + "/" + 90.ToString(); 
	}
}
 