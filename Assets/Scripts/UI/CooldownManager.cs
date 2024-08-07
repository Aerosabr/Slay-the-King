using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public static CooldownManager instance;
    public List<GameObject> Cooldowns = new List<GameObject>();
	public ActivateConsumables[] activeSlot;

	void Awake()
    {
        instance = this;
        Player player = PlayerManager.instance.Players[0].GetComponent<Player>();
        player.Cooldowns = Cooldowns;
        player.consumableSlot = activeSlot;
}

    public void LoadCooldowns()
    {
        string[] icons = { "/Attack", "/Ability1", "/Ability2", "/Ultimate", "/Movement" };
        for (int i = 0; i < icons.Length; i++)
        {
            Cooldowns[i].GetComponent<CooldownUI>().InitiateCooldown(Resources.Load<Sprite>("Icons/" + PlayerManager.instance.Players[0].GetComponent<Player>().Weapon + icons[i]), PlayerManager.instance.Players[0]);
            Cooldowns[i].SetActive(false);
        }
    }

    public void LoadCooldowns(string weapon)
    {
        string[] icons = { "/Attack", "/Ability1", "/Ability2", "/Ultimate", "/Movement" };
        for (int i = 0; i < icons.Length; i++)
        {
            Cooldowns[i].GetComponent<CooldownUI>().InitiateCooldown(Resources.Load<Sprite>("Icons/" + weapon + icons[i]), PlayerManager.instance.Players[0]);
            Cooldowns[i].SetActive(false);
        }
    }
}
