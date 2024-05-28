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
    public List<GameObject> Enemies = new List<GameObject>();
    public bool Active = true;

    //Map
    public GameObject Map;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnRadius, spawnRadius));
    }

    private void Awake()
    {
        instance = this;
        WaveHUD = GameObject.Find("WaveHUD");
        StartCoroutine(Spawning());
        StartCoroutine(CheckAlive());
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
        /*
        Credit = 40 + (Wave * 10);
        enemiesKilled = 0;
        maxEnemies = 0;
        Wave++;
        WaveHUD.GetComponent<Text>().text = "Wave " + Wave;
        StartCoroutine(Spawning()); 
        */
        LoadNextStage();
    }

    public void LoadNextStage()
    {
        int stage = GameManager.instance.Stage;
        Map.SetActive(false);
        if (stage >= 1 && stage <= 3)
        {
            //If on stage 1-3, open Battle + 2 others
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/MapComplete3"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Battle");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
            map.transform.GetChild(2).GetComponent<StageTeleport>().SetTeleport("Random", map.transform.GetChild(1).GetComponent<StageTeleport>().Stage);
        }
        else if (stage == 4)
        {
            //If on stage 4, open Elite
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/MapComplete1"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Elite");
        }
        else if (stage == 7)
        {
            //If on stage 7, open Battle + 2 others
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/MapComplete3"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Battle");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
            map.transform.GetChild(2).GetComponent<StageTeleport>().SetTeleport("Random", map.transform.GetChild(1).GetComponent<StageTeleport>().Stage);
        }
        else if (stage == 8)
        {
            //If on stage 8, open Shop + 1 other
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/MapComplete2"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Shop");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
        }

        if (stage == 10)
        {
            GameManager.instance.Stage = 1;
            GameManager.instance.Floor++;
        }
        else
            GameManager.instance.Stage++;
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
}
 