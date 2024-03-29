using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public float remainingTime;
    public float baseCD;
    public Text cooldown;
    public string Action;

    private void Awake()
    {
        Action = gameObject.name;
    }

    private void Start()
    {
        switch (Action)
        {
            case "AttackCooldown":
                baseCD = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>().AttackCD;
                break;
            case "Ability1Cooldown":
                baseCD = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>().Ability1CD;
                break;
            case "Ability2Cooldown":
                baseCD = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>().Ability2CD;
                break;
            case "UltimateCooldown":
                baseCD = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>().UltimateCD;
                break;
            case "MovementCooldown":
                baseCD = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>().MovementCD;
                break;
        }
        remainingTime = baseCD;
    }

    public void OnEnable()
    {
        remainingTime = baseCD;
    }

    public void Update()
    {
        remainingTime -= Time.deltaTime;
        cooldown.text = remainingTime.ToString("#.0");
        if (remainingTime < 0)
        {
            gameObject.SetActive(false);
            GameObject.Find("Player1").transform.GetChild(0).gameObject.SendMessage("Reset" + Action);
        }
    }
}
