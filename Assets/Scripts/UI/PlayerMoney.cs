using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoney : MonoBehaviour
{
	public Text moneyText;
	public Player player;

	void Awake()
	{
		player = PlayerManager.instance.Players[0].GetComponent<Player>();
	}

	void FixedUpdate()
	{
		moneyText.text = player.Money.ToString();
	}
}
