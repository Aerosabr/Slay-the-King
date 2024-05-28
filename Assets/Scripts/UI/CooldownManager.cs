using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public List<GameObject> Cooldowns = new List<GameObject>();

    void Awake()
    {
        PlayerManager.instance.Players[0].GetComponent<Player>().Cooldowns = Cooldowns;
    }

    private void Start()
    {
        string[] icons = { "/Attack", "/Ability1", "/Ability2", "/Ultimate", "/Movement" };
        for (int i = 0; i < icons.Length; i++)
        {
            Cooldowns[i].GetComponent<CooldownUI>().InitiateCooldown(Resources.Load<Sprite>("Icons/" + PlayerManager.instance.player1Weapon + icons[i]), PlayerManager.instance.gameObject.transform.GetChild(0).transform.GetChild(0).gameObject);
            Cooldowns[i].SetActive(false);
        }
    }
}
