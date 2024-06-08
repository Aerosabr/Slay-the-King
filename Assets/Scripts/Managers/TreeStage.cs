using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeStage : MonoBehaviour
{
    public static TreeStage instance;
    public Animator axe;
    public float timeElapsed;
    public bool treeCut;
    [SerializeField] private Text Timer;
    public Slider starRating;
    private bool Active = false;
    private List<RuntimeAnimatorController> RAC = new List<RuntimeAnimatorController>();
    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
        loadAxes();
    }

    public void FixedUpdate()
    {
        if (!treeCut && Active)
        {
            timeElapsed += Time.deltaTime;
            Timer.text = "Time Elapsed: " + timeElapsed.ToString("F2") + "s";
            UpdateTreeRating();
        }
    }

    public void loadAxes()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            player.GetComponent<Class>().unequipWeapon(player.GetComponent<Player>().Weapon);
            player.AddComponent<Axe>();
            RAC.Add(player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController);
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = axe.runtimeAnimatorController;
        }
        CooldownManager.instance.LoadCooldowns("Axe");
    }

    public void unequipAxes()
    {
        int increment = 0;
        foreach (GameObject player in PlayerManager.instance.Players)
        {
            Destroy(player.GetComponent<Axe>());
            player.GetComponent<Class>().equipCurrent();
            player.GetComponent<PlayerSpriteController>().Sprites[5].GetComponent<Animator>().runtimeAnimatorController = RAC[increment];
            increment++;
        }
        CooldownManager.instance.LoadCooldowns();
    }

    public IEnumerator treeFelled()
    {
        treeCut = true;
        yield return new WaitForSeconds(1f);
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoomOutSlow(8);
        TeleportManager.instance.LoadNextStage("Tree");
        unequipAxes();
    }

    public void UpdateTreeRating()
    {
        if (timeElapsed < 10.0f)
            starRating.value = 1f;
        else if (timeElapsed < 20.0f)
            starRating.value = 0.7f;
        else if (timeElapsed < 30.0f)
            starRating.value = 0.4f;
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            Preround.SetActive(false);
            StageActive.SetActive(true);
            Active = true;
        }
    }
}
