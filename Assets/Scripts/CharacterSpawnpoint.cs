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
            PlayerManager.instance.Players[i].transform.position = Spawnpoints[i].transform.position;
        }
    }

}
