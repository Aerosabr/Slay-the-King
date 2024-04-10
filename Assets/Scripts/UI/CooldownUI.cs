using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public float remainingTime;
    public bool ready;
    public Text cooldown;
    public string Action;
    public GameObject Player;

    private void Awake()
    {
        Action = gameObject.name;
        ready = false;
    }

    public void Update()
    {
        if (ready)
        {
            remainingTime -= Time.deltaTime;
            cooldown.text = remainingTime.ToString("#.0");
            if (remainingTime <= 0)
            {
                ready = false;
                //GameObject.Find("Player1").transform.GetChild(0).gameObject.SendMessage("Reset" + Action);
                Player.SendMessage("Reset" + Action);
                gameObject.SetActive(false);
            }
        }
    }

    public void StartCooldown(float time)
    {
        remainingTime = time;
        ready = true;
    }
}
