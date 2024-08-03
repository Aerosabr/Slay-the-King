using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUp : MonoBehaviour
{
	public Transform PlayerHUD;
	public Transform PlayerManager;
	
	private void Awake()
	{
		GameObject playerHUDObject = GameObject.Find("PlayerHUD");
		if (playerHUDObject != null)
		{
			PlayerHUD = playerHUDObject.transform;
			Destroy(PlayerHUD.gameObject);
		}

		// Find PlayerManager and its children if it exists
		GameObject playerManagerObject = GameObject.Find("PlayerManager");
		if (playerManagerObject != null)
		{
			PlayerManager = playerManagerObject.transform;
			if (PlayerManager.GetChild(0).childCount > 0)
				Destroy(PlayerManager.GetChild(0).GetChild(0).gameObject);
			if (PlayerManager.GetChild(1).childCount > 0)
				Destroy(PlayerManager.GetChild(1).GetChild(0).gameObject);
			if (PlayerManager.GetChild(2).childCount > 0)
				Destroy(PlayerManager.GetChild(2).GetChild(0).gameObject);
			if (PlayerManager.GetChild(3).childCount > 0)
				Destroy(PlayerManager.GetChild(3).GetChild(0).gameObject);
			PlayerManager manager = PlayerManager.GetComponent<PlayerManager>();
			manager.NumPlayers = 0;
			manager.Players.Clear();
			manager.player1Weapon = "";
			
		}
	}
}
