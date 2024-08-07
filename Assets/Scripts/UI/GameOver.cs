using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public void ToMainMenu()
	{
		SceneLoader sceneManager = GameObject.Find("SceneManager").GetComponent<SceneLoader>();
		sceneManager.SceneTransition("MainMenu");
	}
}
