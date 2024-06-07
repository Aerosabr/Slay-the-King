using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerStage : MonoBehaviour
{
    public static ScavengerStage instance;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float spawnRadius;
    [SerializeField] private GameObject Coin;
    [SerializeField] private int spawnRate;
    [SerializeField] private LayerMask unspawnableLayers;
    [SerializeField] private BoxCollider2D box;
    public int numSpawned;
    public int numCollected;
    private bool Active = false;
    [SerializeField] private GameObject Timer;
    private float timeElapsed = 30f;

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
        for (int i = 0; i < 70; i++)
        {
            bool searchLocation = true;
            Vector3 SpawnArea = Vector3.zero;
            while (searchLocation)
            {
                searchLocation = false;
                SpawnArea = new Vector3(Random.Range(-spawnRadius / 2, spawnRadius / 2), Random.Range(-spawnRadius / 2, spawnRadius / 2));
                Collider2D[] colliders = Physics2D.OverlapCircleAll(SpawnArea, 1f);
                foreach (Collider2D collider in colliders)
                {
                    if (((1 << collider.gameObject.layer) & unspawnableLayers) != 0)
                    {
                        searchLocation = true;
                        break;
                    }
                }
            }
            GameObject temp = Instantiate(Coin, SpawnArea, Quaternion.identity);
            temp.SetActive(true);
            numSpawned++;
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
            Destroy(obj);

        foreach(GameObject player in PlayerManager.instance.Players)
        {
            if (player.transform.position.y >= 15)
                player.transform.position = new Vector2(player.transform.position.x, 14);
        }

        TeleportManager.instance.LoadNextStage("Scavenger");
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(10);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            Active = true;
            SpawnCoins();
        }
    }
}
