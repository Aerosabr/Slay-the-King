using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour
{
    public static Movement instance;

    public float _speed;
    public Rigidbody2D _rigidbody;
    public Vector2 _movementInput;
    public Vector2 _smoothedMovementInput;
    public Vector2 _movementInputSmoothVelocity;
    //public Animator anim;
    public bool isMoving;
    public bool isSprinting;
    public LayerMask interactableLayer;
    public bool Movable = true;
    void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _movementInputSmoothVelocity, 0.1f);
        _rigidbody.velocity = _smoothedMovementInput * _speed;
        //anim.SetBool("isMoving", isMoving);
        //anim.SetBool("isSprinting", isSprinting);
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
            //anim.SetFloat("moveX", _movementInput.x);
            //anim.SetFloat("moveY", _movementInput.y);
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
}
