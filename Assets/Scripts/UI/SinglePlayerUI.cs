using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerUI : MonoBehaviour
{
	public PlayerManager pManager;
	public ItemCreation iCreator;
	public GameObject cam;

	private void Awake()
	{

		// Find PlayerManager and its children if it exists
		pManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		iCreator = GameObject.Find("GameManager").GetComponent<ItemCreation>();
		cam = GameObject.Find("Main Camera");
	}

	public void StartGame()
	{
		pManager.StartGame();
		cam.SetActive(false);
		iCreator.LoadPossibleWeapons();
	}

	public void AddPlayer(int slot)
	{
		pManager.AddPlayer(slot);
	}

}
