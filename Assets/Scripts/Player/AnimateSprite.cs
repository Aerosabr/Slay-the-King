using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public float walkSpeed;
    public float idleTime;
    public Vector2 direction;
    public Vector2 idleDirection;

    private void Update()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        UpdateAnimatorParameters();

        // HandleSpriteFlip();
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetFloat("speed", direction.magnitude * walkSpeed);

        bool isWalking = direction.magnitude > 0;
        if(isWalking)
        {
            PlayAnimation("Run");
            idleDirection = direction;
        }
        else if(!isWalking)
        {
            direction = idleDirection;
            PlayAnimation("Idle");
        }
    }

    public void PlayAnimation(string Name)
    {
        if(direction.y > 0)
        {
            if(direction.x > 0)
            {
                animator.Play(Name + "NE");
            }
            else if(direction.x == 0)
            {
                animator.Play(Name + "N");
            }
            else if(direction.x < 0)
            {
                animator.Play(Name + "NW");
            }
        }
        else if(direction.y < 0)
        {
            if(direction.x > 0)
            {
                animator.Play(Name + "SE");
            }
            else if(direction.x == 0)
            {
                animator.Play(Name + "S");
            }
            else if(direction.x < 0)
            {
                animator.Play(Name + "SW");
            }
        }
        else
        {
            if(direction.x > 0)
            {
                animator.Play(Name + "E");
            }
            else 
            {
                animator.Play(Name + "W");
            }
        }
    }
}
