using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStage : MonoBehaviour
{
    public static BattleStage instance;
    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private LayerMask unspawnableLayers;
    //Spawner
    public Transform spawnArea;
    public float spawnRadius;
    public int maxEnemies = 0;
    public int enemiesKilled = 0;
    public int Credit = 50;
    public int Wave = 1;
    private int maxWaves;
    public GameObject WaveHUD;
    public Text EnemyCountText;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> EliteEnemies = new List<GameObject>();
    public GameObject Boss;
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
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
    }

    private void StartStage()
    {
        if (Spawnable)
        {
            if (GameManager.instance.Stage == 10)
            {
                //Boss
                maxWaves = 5;
                StartCoroutine(SpawnElites());
            }
            else if (GameManager.instance.Stage == 5)
            {
                //Elite
                maxWaves = 4;
                StartCoroutine(SpawnElites());
            }
            else
            {
                //Normal
                maxWaves = 3;
            }
            StartCoroutine(Spawning());
            StartCoroutine(CheckAlive());
        }
        else
        {
            foreach (GameObject player in PlayerManager.instance.Players)
                player.GetComponent<Player>().CameraZoomOutSlow(8);

            TeleportManager.instance.LoadNextStage("Battle");
        }
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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(SpawnArea, 2f);
            foreach (Collider2D collider in colliders)
            {
                if (((1 << collider.gameObject.layer) & unspawnableLayers) != 0)
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

        if (Wave == maxWaves)
        {
            foreach (GameObject player in PlayerManager.instance.Players)
                player.GetComponent<Player>().CameraZoomOutSlow(8);
            
            TeleportManager.instance.LoadNextStage("Battle");
        }
        else
        {
            Credit = 50 + (Wave * 10);
            enemiesKilled = 0;
            maxEnemies = 0;
            Wave++;
            if (Wave == 5)
                SpawnEnemy(Boss);
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
    public IEnumerator SpawnElites()
    {
        int counter = 0;
        bool temp = true;
        while (temp)
        {
            yield return new WaitUntil(() => maxEnemies % 5 == 0 && maxEnemies != counter);
            counter = maxEnemies;
            SpawnEnemy(EliteEnemies[Random.Range(0, EliteEnemies.Count)]);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            Preround.SetActive(false);
            StageActive.SetActive(true);
            StartStage();
        }
    }
}
 