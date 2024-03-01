using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    //UI script that manages player actions bar
    public static PlayerUI instance;
    public List<GameObject> Slots = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public void updateUI()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (PlayerManager.instance.Players.ContainsKey(i + 1))
            {
                Slots[i].transform.GetChild(0).GetComponent<Text>().text = "Player " + (i + 1) + ": " + PlayerManager.instance.Players[i + 1].Health.ToString() + "hp";
            }
            else
                Slots[i].transform.GetChild(0).GetComponent<Text>().text = "Add Player";
            Slots[i].SetActive(false);
        }
    }

    public void PlayersTurn()
    {
        foreach (GameObject a in Slots)
        {
            a.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void PlayerAction(int player)
    {
        Slots[player - 1].transform.GetChild(1).gameObject.SetActive(false);
    }

    public void SwitchPlayer(int index)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (i + 1 == index)
            {
                Slots[i].SetActive(true);
            }
            else
                Slots[i].SetActive(false);
        }
    }
}
