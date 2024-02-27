using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSlots : MonoBehaviour
{
    public static PlayerSlots instance;
    public List<GameObject> Slots = new List<GameObject>();

    void Awake()
    {
        instance = this;
        foreach(GameObject i in Slots)
        {
            i.gameObject.GetComponent<Text>().text = "Add Player";
        }
    }

    public void updateUI()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (PlayerManager.instance.Players.ContainsKey(i + 1))
            {
                Slots[i].gameObject.GetComponent<Text>().text = "Player " + (i + 1) + ": " + PlayerManager.instance.Players[i + 1].Health.ToString() + "hp";
            }
            else
                Slots[i].gameObject.GetComponent<Text>().text = "Add Player";
        }
    }
}
