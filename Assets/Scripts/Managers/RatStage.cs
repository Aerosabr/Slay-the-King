using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatStage : MonoBehaviour
{
    public static RatStage instance;
    public GameObject Rat;
    public float xPos;
    public float yPos;
    public Transform spawnArea;

    [SerializeField] private Animator net;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();

    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private GameObject Timer;
    private bool Active = false;
    private float timeElapsed = 30f;
    public int ratsCaught = 0;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(xPos, yPos));
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
        loadNets();
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            if (timeElapsed >= 0)
            {
                timeElapsed -= Time.deltaTime;
                Timer.GetComponent<Text>().text = timeElapsed.ToString("#.00") + "  Rats Caught " + ratsCaught;
            }
            else
                StartCoroutine(EndStage());
        }
    }

    public void loadNets()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Net>();
            //RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            //player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = net.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Net");
    }

    public void unequipNets()
    {
        int increment = 0;
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Net>());
            player.GetComponent<Class>().equipCurrent();
            //player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
            increment++;
        }
        CooldownManager.instance.LoadCooldowns();
    }

    public IEnumerator SpawnRats()
    {
        for (int i = 0; i < 30; i++)
        {
            Instantiate(Rat, FindSpawnPos(), Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }    
    }

    public Vector2 FindSpawnPos()
    {
        return new Vector2(Random.Range(-xPos, xPos), Random.Range(-yPos, yPos));
    }

    private IEnumerator EndStage()
    {
        yield return new WaitForSeconds(1f);
        unequipNets();
        Active = false;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(obj);

        TeleportManager.instance.LoadNextStage("Rats");
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
            StartCoroutine(SpawnRats());
        }
    }
}
