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
    public Text ratText;
    public Slider starRating;
    public Text Timer;
    [SerializeField] private Animator net;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();

    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;

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
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            UpdateRatUI();
			if (timeElapsed >= 0)
            {
                timeElapsed -= Time.deltaTime;
                Timer.text = timeElapsed.ToString("F2") + "s";
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
            RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = net.runtimeAnimatorController;
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
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
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
        return new Vector2(Random.Range(-xPos / 2, xPos / 2), Random.Range(-yPos / 2, yPos / 2));
    }

    public void UpdateRatUI()
    {
        ratText.text = "Rats caught: " + ratsCaught.ToString();
        if (ratsCaught == 24)
            starRating.value = 1f;
        else if (ratsCaught == 16)
            starRating.value = 0.7f;
        else if (ratsCaught == 8)
            starRating.value = 0.4f;
    }

    private IEnumerator EndStage()
    {
        if (Active)
        {
            Active = false;
            foreach (GameObject player in PlayerManager.instance.Players)
                player.GetComponent<Net>().StageEnded = true;
            yield return new WaitForSeconds(1f);

            if (ratsCaught >= 24)
                ItemCreation.instance.ThreeStarLoot();
            else if (ratsCaught >= 16)
                ItemCreation.instance.TwoStarLoot();
            else if (ratsCaught >= 8)
                ItemCreation.instance.OneStarLoot();

            GameManager.instance.canEquip = true;
            unequipNets();
            Active = false;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
                Destroy(obj);

            TeleportManager.instance.LoadNextStage("Rats");
            foreach (GameObject player in PlayerManager.instance.Players)
                player.GetComponent<Player>().CameraZoomOutSlow(8);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            GameManager.instance.canEquip = false;
            loadNets();
            Preround.SetActive(false);
            StageActive.SetActive(true);
            Active = true;
            StartCoroutine(SpawnRats());
        }
    }
}
