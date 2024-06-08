using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopStage : MonoBehaviour
{
    public static ShopStage instance;
    
    [SerializeField] private GameObject StageActive;
    [SerializeField] private GameObject Preround;
    [SerializeField] private BoxCollider2D box;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject player in PlayerManager.instance.Players)
            player.GetComponent<Player>().CameraZoom(5);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(box);
            GameManager.instance.canEquip = false;
            Preround.SetActive(false);
            StageActive.SetActive(true);
            TeleportManager.instance.LoadNextStage("Shop");
        }
    }
}
