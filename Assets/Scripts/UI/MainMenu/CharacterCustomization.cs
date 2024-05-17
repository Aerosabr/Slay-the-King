using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
    public static CharacterCustomization instance;
	public List<Animator> hairList = new List<Animator>();
	public int hairCounter = 0;
	public Color hairColor = Color.black;
	public Color skinColor = Color.white;
	public List<PlayerAssetLibrary> classList = new List<PlayerAssetLibrary>();
	public int counter = 0;
	public int equippedWeapon = 1;
	public bool twoHanded = false;
    public string WeaponName;

	public List<Animator> Sprites = new List<Animator>();
	public float walkSpeed = 1;

	public Transform playerParent;

	public Button weapon1;
	public Button weapon2;
	public Image weapon1Icon;
	public Image weapon2Icon;
	public Text className;
	public Text hairName;

	private void Awake()
	{
        instance = this;
	}

	private void Start()
	{
		Sprites.Add(gameObject.GetComponent<Animator>());
		for (int i = 0; i < 7; i++)
			Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
		ApplyHairColor(hairColor);
		ApplySkinColor(skinColor);
		ApplyClass(classList[counter]);
		ApplyHairStyle();
		UpdateSpriteParameters();
	}

	void FixedUpdate()
	{
		if (equippedWeapon == 1)
		{
			weapon1.Select();
		}
		else if (equippedWeapon == 2)
		{
			weapon2.Select();
		}
	}

	private void UpdateSpriteParameters()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("speed", walkSpeed);
		if (equippedWeapon == 1)
		{
			if (classList[counter].weapon1twoHanded)
			{
				foreach (Animator sprite in Sprites)
					sprite.Play("2HIdleS", -1, 0f);
			}
			else
			{
				foreach (Animator sprite in Sprites)
					sprite.Play("IdleS", -1, 0f);
			}
		}
		else if (equippedWeapon == 2)
		{
			if (classList[counter].weapon2twoHanded)
			{
				foreach (Animator sprite in Sprites)
					sprite.Play("2HIdleS", -1, 0f);
			}
			else
			{
				foreach (Animator sprite in Sprites)
					sprite.Play("IdleS", -1, 0f);
			}
		}
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
	
	public void ApplyClass(PlayerAssetLibrary playerClass)
	{
		Sprites[3].runtimeAnimatorController = playerClass.shirt.runtimeAnimatorController;
		Sprites[4].runtimeAnimatorController = playerClass.glove.runtimeAnimatorController;
		Sprites[6].runtimeAnimatorController = playerClass.pant.runtimeAnimatorController;
		Sprites[7].runtimeAnimatorController = playerClass.boot.runtimeAnimatorController;
		ApplyWeapon(equippedWeapon);
		className.text = playerClass.name;
		weapon1Icon.sprite = playerClass.weapon1Icon;
		weapon2Icon.sprite = playerClass.weapon2Icon;

	}

	public void ApplyWeapon(int weapon)
	{
		equippedWeapon = weapon;
		if (equippedWeapon == 1)
		{
			Sprites[5].runtimeAnimatorController = classList[counter].weapon1.runtimeAnimatorController;
			weapon1.Select();
            WeaponName = classList[counter].weapon1Name;

        }
		else if (equippedWeapon == 2)
		{
			Sprites[5].runtimeAnimatorController = classList[counter].weapon2.runtimeAnimatorController;
			weapon2.Select();
            WeaponName = classList[counter].weapon2Name;
        }
		
		UpdateSpriteParameters();
	}

	public void ApplyHairStyle()
	{
		Sprites[1].runtimeAnimatorController = hairList[hairCounter].runtimeAnimatorController;
		UpdateSpriteParameters();
		hairName.text = hairList[hairCounter].name;
	}
	public void NextHair()
	{
		hairCounter = (hairCounter + 1) % hairList.Count;
		ApplyHairStyle();
	}

	public void PrevHair()
	{
		hairCounter = (hairCounter - 1 + classList.Count) % hairList.Count;
		ApplyHairStyle();
	}
	public void NextClass()
	{
		counter = (counter + 1) % classList.Count;
		ApplyClass(classList[counter]);
	}

	public void PrevClass()
	{
		counter = (counter - 1 + classList.Count) % classList.Count;
		ApplyClass(classList[counter]);
	}

}
