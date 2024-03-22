using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public List<Player> Players = new List<Player>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    
    public void AddPlayer()
    {
        if (Players.Count == 4)
            return;
      
        Player player = new Player(Random.Range(1, 100));
        Players.Add(player);
        GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Character"), transform);
        temp.name = "Player" + Players.Count;
    }

    public void RemovePlayer(int slot)
    {
        Players.RemoveAt(slot);
        Destroy(transform.GetChild(slot));
    }

}
