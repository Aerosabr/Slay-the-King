using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();

    void Awake()
    {
        instance = this;
    }

    public void AddPlayer(int slot)
    {
        if (Players.Count == 4)
            return;
      
        Player player = new Player(Random.Range(1, 100));
        Players.Add(slot, player);
        PlayerSlots.instance.updateUI();
    }

    public void RemovePlayer(int slot)
    {
        Players.Remove(slot);
        PlayerSlots.instance.updateUI();
    }

}
