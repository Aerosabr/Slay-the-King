using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour
{
    public List<GameObject> Spawnpoints = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < PlayerManager.instance.NumPlayers; i++)
        {
            GameObject temp = PlayerManager.instance.Players[i].transform.GetChild(0).gameObject;
            Transform position = Spawnpoints[i].transform;
            temp.transform.position = position.transform.position;
            temp.transform.localScale = position.transform.localScale;
        }
    }

}
