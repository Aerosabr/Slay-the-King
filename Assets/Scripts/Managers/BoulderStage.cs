using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoulderStage : MonoBehaviour
{
    public static BoulderStage instance;
    public Transform spawnArea;
    public float spawnX;
    public float spawnY;
    public GameObject Boulder;
    public GameObject TargetCircle;
    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private LayerMask unspawnableLayers;
    [SerializeField] private GameObject Timer;
    private bool Active = false;
    public bool Spawning;
    private float timeInterval = 6f;
    private float dropOnPlayer = 2f;
    [SerializeField] private float timeElapsed;
    [SerializeField] private int numRocksSpawned = 1;
    [SerializeField] private float spawnDelay = 0.5f;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnX, spawnY));
    }

    private void Awake()
    {
        instance = this;
        Spawning = true;

    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
    }

    public void FixedUpdate()
    {
        if (Active)
        {
            if (dropOnPlayer > 0)
                dropOnPlayer -= Time.deltaTime;

            timeElapsed += Time.deltaTime;
            if (timeElapsed > timeInterval && spawnDelay > 0.15f)
            {
                timeInterval += 10f;
                spawnDelay -= 0.1f;
                numRocksSpawned++;
            }
            Timer.GetComponent<Text>().text = timeElapsed.ToString("#.00");
            bool alive = true;
            List<GameObject> temp = PlayerManager.instance.Players;
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].GetComponent<Player>().currentHealth > 0)
                    alive = false;
            }

            if (alive && Active)
                StartCoroutine(EndStage());
        }  
    }

    public IEnumerator SpawnBoulders()
    {
        while (Spawning)
        {
            StartCoroutine(Spawn());
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public IEnumerator Spawn()
    {
        for (int i = 0; i < numRocksSpawned; i++)
        {
            if (Active)
            {
                bool searchLocation = true;
                Vector3 SpawnArea = Vector3.zero;
                while (searchLocation)
                {
                    if (dropOnPlayer <= 0)
                    {
                        SpawnArea = PlayerManager.instance.Players[0].transform.position;
                        dropOnPlayer = 2f;
                        searchLocation = false;
                    }
                    else
                    {
                        searchLocation = false;
                        SpawnArea = new Vector3(Random.Range(-spawnX / 2, spawnX / 2), Random.Range(-spawnY / 2, spawnY / 2));
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
                }

                GameObject tc = Instantiate(TargetCircle, SpawnArea, Quaternion.identity);
                tc.GetComponent<TargetCircle>().InitiateTarget(2f, 1f);
                yield return new WaitForSeconds(1f);
                GameObject boulder = Instantiate(Boulder, SpawnArea, Quaternion.identity);
                boulder.name = "BoulderOnPlayer";
            }
        }
    }

    private IEnumerator EndStage()
    {
        Active = false;
        Spawning = false;
        TeleportManager.instance.LoadNextStage("Boulder");
        yield return new WaitForSeconds(2f);
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Player>().CameraZoom(10);
            player.GetComponent<Player>().Revive(player.GetComponent<Player>().maxHealth);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            Preround.SetActive(false);
            StageActive.SetActive(true);
            Active = true;
            StartCoroutine(SpawnBoulders());
        }
    }
}

