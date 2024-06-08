using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
	public static GameOver instance;

	public GameObject inventory;
	public GameObject gameOver;

	private void Awake()
	{
		instance = this;
	}

	public void PopUp()
	{
		inventory.SetActive(false);
		gameOver.SetActive(true);
	}
}
