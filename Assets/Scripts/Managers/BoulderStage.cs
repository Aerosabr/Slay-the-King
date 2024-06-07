using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool Active = false;

    public float Timer;
    public float timeInterval = 10f;

    public bool Spawning;
    public float spawnDelay = 0.5f;

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
            Timer += Time.deltaTime;
            if (Timer > timeInterval && spawnDelay > 0.15f)
            {
                timeInterval += 10f;
                spawnDelay -= 0.1f;
            }
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
        bool searchLocation = true;
        Vector3 SpawnArea = Vector3.zero;
        while (searchLocation)
        {
            searchLocation = false;
            SpawnArea = new Vector3(Random.Range(-spawnX / 2, spawnX / 2), Random.Range(-spawnY / 2, spawnY/ 2));
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

        GameObject tc = Instantiate(TargetCircle, SpawnArea, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(1f, 1f);
        yield return new WaitForSeconds(1f);
        Instantiate(Boulder, SpawnArea, Quaternion.identity);
    }

    private void EndStage()
    {
        Active = false;
        Spawning = false;
        TeleportManager.instance.LoadNextStage("Boulder");
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(10);
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

