using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider;
    public Player player;
    void Awake()
    {
        player = PlayerManager.instance.Players[0].GetComponent<Player>();
    }

    void FixedUpdate()
    {
        slider.value = (float)player.currentHealth / (float)player.maxHealth;
    }
}
