using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTeleport : MonoBehaviour
{
    public SceneLoader SceneLoader;
    public string Stage;

    public void Awake()
    {
        SceneLoader = GameObject.Find("SceneManager").GetComponent<SceneLoader>();
    }

    public void SetTeleport(string stage)
    {
        if (stage == "Random")
        {
            string[] Stages = { "Boulder", "Mining", "Rats", "Scavenger", "Tree", "Chests" };
            Stage = Stages[Random.Range(0, Stages.Length)];

        }
        else
            Stage = stage;

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Icons/StageIcons/" + Stage);
    }

    public void SetTeleport(string stage, string exception)
    {
        if (stage == "Random")
        {
            string[] Stages = { "Boulder", "Mining", "Rats", "Scavenger", "Tree", "Chests" };
            List<string> temp = new List<string>(Stages);
            temp.Remove(exception);
            Stage = temp[Random.Range(0, temp.Count)];
        }
        else
            Stage = stage;

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Icons/StageIcons/" + Stage);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            InventoryManager.instance.BeginTransfer();
            SceneLoader.SceneTransition(Stage);
        }
    }

}
