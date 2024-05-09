using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
	public List<Animator> hairList = new List<Animator>();
	public string hairColor = "3A3A3A";
	public string skinColor = "FFE1AA";
	public List<PlayerAssetLibrary> classList = new List<PlayerAssetLibrary>();

	public List<Animator> Sprites = new List<Animator>();
	public float walkSpeed = 1;

	private void Awake()
	{
	}

	private void Start()
	{
		Sprites.Add(gameObject.GetComponent<Animator>());
		for (int i = 0; i < 7; i++)
			Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
	}

	void FixedUpdate()
	{
		UpdateSpriteParameters();
	}

	private void UpdateSpriteParameters()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("speed", walkSpeed);

		foreach (Animator sprite in Sprites)
			sprite.Play("IdleS");
	}
}
