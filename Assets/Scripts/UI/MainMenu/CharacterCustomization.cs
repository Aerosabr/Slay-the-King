using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
	public List<Animator> hairList = new List<Animator>();
	public Color hairColor = Color.black;
	public Color skinColor = Color.white;
	public List<PlayerAssetLibrary> classList = new List<PlayerAssetLibrary>();

	public List<Animator> Sprites = new List<Animator>();
	public float walkSpeed = 1;

	public Transform playerParent;

	private void Awake()
	{
	}

	private void Start()
	{
		Sprites.Add(gameObject.GetComponent<Animator>());
		for (int i = 0; i < 7; i++)
			Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
		ApplyHairColor(hairColor);
		ApplySkinColor(skinColor);
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

	public void ApplySkinColor(Color skinColor)
	{
		this.skinColor = skinColor;
		Sprites[0].transform.GetComponent<SpriteRenderer>().color = skinColor;
		Sprites[2].transform.GetComponent<SpriteRenderer>().color = skinColor;
	}

	public void ApplyHairColor(Color hairColor)
	{
		this.hairColor = hairColor;
		Sprites[1].transform.GetComponent<SpriteRenderer>().color = hairColor;
	}
}
