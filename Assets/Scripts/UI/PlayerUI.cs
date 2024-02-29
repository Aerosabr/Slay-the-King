using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
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
        }
    }

    public void PlayersTurn()
    {
        foreach (GameObject a in Slots)
        {
            a.transform.GetChild(1).gameObject.SetActive(true);
            a.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void PlayerCancel(int player)
    {
        Slots[player - 1].transform.GetChild(1).gameObject.SetActive(true);
        Slots[player - 1].transform.GetChild(2).gameObject.SetActive(false);
    }

    public void PlayerAction(int player)
    {
        Slots[player - 1].transform.GetChild(1).gameObject.SetActive(false);
        Slots[player - 1].transform.GetChild(2).gameObject.SetActive(true);
    }
}
