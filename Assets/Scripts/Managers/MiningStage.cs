using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningStage : MonoBehaviour
{
    public static MiningStage instance;
    public float timeRemaining = 30f;
    public int rocksBroken = 0;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();
    [SerializeField] private GameObject Timer;
    [SerializeField] private Animator pickaxe;
    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private GameObject Rocks;
    private bool Active = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
        loadPickaxes();
    }

    public void FixedUpdate()
    {
        if(Active)
        {
            if (rocksBroken == 6)
            {

                StartCoroutine(EndStage());
            }
            else if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                Timer.GetComponent<Text>().text = timeRemaining.ToString("#.00");
            }
            else
                StartCoroutine(EndStage());
        }
    }

    public void loadPickaxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Pickaxe>();
            RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = pickaxe.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Axe");
    }

    public void unequipPickaxes()
    {
        Active = false;
        int increment = 0;
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Pickaxe>());
            player.GetComponent<Class>().equipCurrent();
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
            increment++;
        }
        CooldownManager.instance.LoadCooldowns();
    }

    private IEnumerator EndStage()
    {
        Active = false;
        yield return new WaitForSeconds(1f);
        unequipPickaxes();
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(obj);

        TeleportManager.instance.LoadNextStage("Mining");
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
            Rocks.SetActive(true);
        }
    }
}
