using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestPlayerAnimation : MonoBehaviour
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
	public Text animationText;
	public Text weaponText;
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
		DisplayWeaponText();
	}

	void FixedUpdate()
	{
		if (!isAttacking && Movable)
		{
			_smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _movementInputSmoothVelocity, 0.1f);
			keyboardDirection = _movementInput;
			UpdateSpriteParameters();
		}
	}
	public void NextWeapon()
	{
		counter = (counter + 1) % weaponSO.Length;
		selectedWeaponSO = weaponSO[counter];
		for (int i = 0; i < 8; i++)
		{
			Sprites[i].runtimeAnimatorController = selectedWeaponSO.animatorBody[i].runtimeAnimatorController;
		}
		DisplayWeaponText();
		PlayAnimation("Idle");
	}
	public void PrevWeapon()
	{
		counter = (counter - 1 + weaponSO.Length) % weaponSO.Length;
		selectedWeaponSO = weaponSO[counter];
		for (int i = 0; i < 8; i++)
		{
			Sprites[i].runtimeAnimatorController = selectedWeaponSO.animatorBody[i].runtimeAnimatorController;
		}
		DisplayWeaponText();
		PlayAnimation("Idle");
	}

	public void DisplayWeaponText()
	{
		weaponText.text = selectedWeaponSO.name;
	}

	public void OnAttack()
	{
		isAttacking = true;
		StartCoroutine(timer());
		Attack(selectedWeaponSO.abilityNames[0], 1.0f);
		animationText.text = "'" + selectedWeaponSO.abilityNames[0] + "'";
	}
	public void OnAbility1()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("attackSpeed", 1.0f);
		isAttacking = true;
		StartCoroutine(timer());
		PlayAnimation(selectedWeaponSO.abilityNames[1]);
		animationText.text = "'" + selectedWeaponSO.abilityNames[1] + "'";
	}
	public void OnAbility2()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("attackSpeed", 1.0f);
		isAttacking = true;
		StartCoroutine(timer());
		PlayAnimation(selectedWeaponSO.abilityNames[2]);
		animationText.text = "'" + selectedWeaponSO.abilityNames[2] + "'";
	}
	public void OnUltimate()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("attackSpeed", 1.0f);
		isAttacking = true;
		StartCoroutine(timer());
		PlayAnimation(selectedWeaponSO.abilityNames[3]);
		animationText.text = "'" + selectedWeaponSO.abilityNames[3] + "'";
	}

	public void Attack(string animation, float spriteSpeed)
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("attackSpeed", spriteSpeed);
		PlayAnimation(animation);
		
	}
	IEnumerator timer()
	{
		yield return new WaitForSeconds(2); // Wait for 2 seconds
		isAttacking = false;

	}
	//Detecting when player is sprinting
	public void OnSprintStart()
	{
		sprintMultiplier = 2f;
		isSprinting = true;
	}

	public void OnSprintFinish()
	{
		sprintMultiplier = 1f;
		isSprinting = false;
	}

	//Player Movement 
	public void OnMovement(InputValue inputValue)
	{
		_movementInput = inputValue.Get<Vector2>();

		if (_movementInput.x != 0 || _movementInput.y != 0)
		{
			keyboardDirection = _movementInput;
		}

		if (_movementInput.x == 0 && _movementInput.y == 0)
			isMoving = false;
		else
			isMoving = true;
	}

	private void UpdateSpriteParameters()
	{
		foreach (Animator sprite in Sprites)
			sprite.SetFloat("speed", walkSpeed);

		if (isMoving)
		{
			PlayAnimation(selectedWeaponSO.run);
			currentDirection = keyboardDirection;
		}
		else if (!isMoving)
			PlayAnimation(selectedWeaponSO.idle);
	}

	public void PlayAnimation(string Name)
	{
		float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
		switch (angle)
		{
			case float _ when angle > -11.25f && angle <= 11.25f:
				Name += "E";
				break;
			case float _ when angle > 11.25f && angle <= 33.75f:
				Name += "ENE";
				break;
			case float _ when angle > 33.75f && angle <= 56.25f:
				Name += "NE";
				break;
			case float _ when angle > 56.25f && angle <= 78.75f:
				Name += "NNE";
				break;
			case float _ when angle > 78.75f && angle <= 101.25f:
				Name += "N";
				break;
			case float _ when angle > 101.25f && angle <= 123.75f:
				Name += "NNW";
				break;
			case float _ when angle > 123.75f && angle <= 146.25f:
				Name += "NW";
				break;
			case float _ when angle > 146.25f && angle <= 168.75f:
				Name += "WNW";
				break;
			case float _ when angle > 168.75f || angle <= -168.75f:
				Name += "W";
				break;
			case float _ when angle > -168.75f && angle <= -146.25f:
				Name += "WSW";
				break;
			case float _ when angle > -146.25f && angle <= -123.75f:
				Name += "SW";
				break;
			case float _ when angle > -123.75f && angle <= -101.25f:
				Name += "SSW";
				break;
			case float _ when angle > -101.25f && angle <= -78.75f:
				Name += "S";
				break;
			case float _ when angle > -78.75f && angle <= -56.25f:
				Name += "SSE";
				break;
			case float _ when angle > -56.25f && angle <= -33.75f:
				Name += "SE";
				break;
			case float _ when angle > -33.75f && angle <= -11.25f:
				Name += "ESE";
				break;
		}

		foreach (Animator sprite in Sprites)
			sprite.Play(Name);
	}
}
