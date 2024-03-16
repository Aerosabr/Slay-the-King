using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpriteController : MonoBehaviour
{
    //Sprite Movement
    public Rigidbody2D _rigidbody;
    public float _speed; 
    public Vector2 _movementInput;
    public Vector2 _smoothedMovementInput;
    public Vector2 _movementInputSmoothVelocity;

    //Sprite Animations
    public List<Animator> Sprites = new List<Animator>();
    public float walkSpeed;
    public Vector2 mouseDirection;
    public Vector2 keyboardDirection;
    public Vector2 idleDirection;

    public bool isMoving;
    public bool isSprinting;
    public bool isAttacking;
    public LayerMask interactableLayer;
    public bool Movable = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        Sprites.Add(gameObject.GetComponent<Animator>());
        for (int i = 0; i < 7; i++)
            Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());

    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _movementInputSmoothVelocity, 0.1f);
            _rigidbody.velocity = _smoothedMovementInput * _speed;
            keyboardDirection = _movementInput;
            UpdateSpriteParameters();
        }
    }

    //Player Attack
    public void OnAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDirection = (mousePosition - transform.position).normalized;
            keyboardDirection = mouseDirection;
            idleDirection = keyboardDirection;
            StartCoroutine(Attack());
        }
    }
    
    private IEnumerator Attack()
    {
        //Shoot!
        foreach (Animator sprite in Sprites)
            sprite.SetFloat("attackSpeed", 2);

        _rigidbody.velocity = Vector2.zero;
        PlayAnimation("Shoot");
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }
    
    //Detecting when player is sprinting
    public void OnSprintStart()
    {
        _speed *= 2;
        isSprinting = true;
    }

    public void OnSprintFinish()
    {
        _speed /= 2;
        isSprinting = false;
    }

    //Player Movement 
    public void OnMovement(InputValue inputValue)
    {
        if (!Movable)
            return;
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

    public void RestrictMovement()
    {
        _movementInput = new Vector2(0, 0);
        Movable = false;
        isMoving = false;
        isSprinting = false;
    }

    private void UpdateSpriteParameters()
    {
        foreach (Animator sprite in Sprites)
            sprite.SetFloat("speed", walkSpeed);


        if (isMoving)
        {
            PlayAnimation("Run");
            idleDirection = keyboardDirection;
        }
        else if (!isMoving)
        {
            keyboardDirection = idleDirection;
            PlayAnimation("Idle");
        }
    }

    public void PlayAnimation(string Name)
    {
        float angle = Mathf.Atan2(keyboardDirection.y, keyboardDirection.x) * Mathf.Rad2Deg;
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
