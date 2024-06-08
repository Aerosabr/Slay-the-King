using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExp : MonoBehaviour
{
	public Slider slider;
	public Text levelText;
	public Player player;
	void Awake()
	{
		player = PlayerManager.instance.Players[0].GetComponent<Player>();
	}

	void FixedUpdate()
	{
		levelText.text = player.Level.ToString();
		slider.value = (float)player.levelExp / 1000f;
	}
}
