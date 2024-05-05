using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
	//Sprite Movement
	public Vector2 _movementInput;
	public Vector2 _smoothedMovementInput;
	public Vector2 _movementInputSmoothVelocity;
	public float sprintMultiplier = 1f;

	//Sprite Animations
	public WeaponAbilitySO[] weaponSO;
	public int counter = 0;
	public WeaponAbilitySO selectedWeaponSO;

	public List<Animator> Sprites = new List<Animator>();
	public float walkSpeed = 1;
	public Vector2 keyboardDirection;
	public Vector2 currentDirection;

	public bool isMoving;
	public bool isSprinting;
	public bool isAttacking;
	public LayerMask interactableLayer;
	public bool Movable = true;

	private void Awake()
	{
	}

	private void Start()
	{
		Sprites.Add(gameObject.GetComponent<Animator>());
		for (int i = 0; i < 7; i++)
			Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
		for (int i = 0; i < 8; i++)
		{
			Sprites[i].runtimeAnimatorController = selectedWeaponSO.animatorBody[i].runtimeAnimatorController;
		}
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
