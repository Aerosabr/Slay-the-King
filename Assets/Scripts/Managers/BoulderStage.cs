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
    private bool Active = false;
    public bool Spawning;
    private float timeInterval = 6f;
    private float dropOnPlayer = 2f;
    [SerializeField] private float timeElapsed;
    [SerializeField] private int numRocksSpawned = 1;
    [SerializeField] private float spawnDelay = 0.5f;
    private List<int> maxHP = new List<int>();
    private List<int> currentHP = new List<int>();      

    public Text timerText;
    public Slider starRating;

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
            UpdateScoreBoard(timeElapsed);
            if (timeElapsed > timeInterval && spawnDelay > 0.15f)
            {
                timeInterval += 10f;
                spawnDelay -= 0.1f;
                numRocksSpawned++;
            }
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
        int increment = 0;
        TeleportManager.instance.LoadNextStage("Boulder");
        GameManager.instance.canEquip = true;
        yield return new WaitForSeconds(3f);
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Player temp = player.GetComponent<Player>();
            player.GetComponent<Class>().equipCurrent();
            player.GetComponent<PlayerSpriteController>().Sprintable = true;
            temp.CameraZoomOutSlow(8);
            temp.maxHealth = maxHP[increment];
            temp.Revive(currentHP[increment]);
            increment++;
        }
        if (timeElapsed >= 40)
            ItemCreation.instance.ThreeStarLoot();
        else if (timeElapsed >= 25)
            ItemCreation.instance.TwoStarLoot();
        else if (timeElapsed >= 10)
            ItemCreation.instance.OneStarLoot();
        CooldownManager.instance.LoadCooldowns();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);

            CooldownManager.instance.LoadCooldowns("None");
            Preround.SetActive(false);
            StageActive.SetActive(true);
            Active = true;
            foreach (GameObject player in PlayerManager.instance.Players)
            {
                Player temp = player.GetComponent<Player>();
                player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
                player.GetComponent<PlayerSpriteController>().Sprintable = false;
                GameManager.instance.canEquip = false;
                maxHP.Add(temp.maxHealth);
                currentHP.Add(temp.currentHealth);
                temp.maxHealth = (temp.Defense / 4) + 1;
                temp.currentHealth = temp.maxHealth;
            }
            StartCoroutine(SpawnBoulders());
        }
    }

    public void UpdateScoreBoard(float timer)
    {
		timerText.text = "Time Elapsed: " + timer.ToString("#.00") + "s";
        if (timer >= 40)
            starRating.value = 1f;
        else if (timer >= 25)
            starRating.value = 0.7f;
        else if (timer >= 10)
            starRating.value = 0.4f;
        
	}
}

