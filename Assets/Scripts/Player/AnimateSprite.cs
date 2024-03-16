using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    //public SpriteRenderer spriteRenderer;
    public float idleTime;
    public Vector2 idleDirection;
    public Animator animator;
    public float walkSpeed;
    public Vector2 mouseDirection;
    public Vector2 keyboardDirection;

    public bool isAttacking = false;

    private void Update()
    {
        // Calculate the direction from the character to the mouse cursor

        if(Input.GetMouseButtonDown(1) && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Shoot());
        }
        if(!isAttacking)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDirection = (mousePosition - transform.position).normalized;
            keyboardDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            UpdateAnimatorParameters();
        }

        // HandleSpriteFlip();
    }

    private IEnumerator Shoot()
    {
        //Shoot!
        animator.SetFloat("attackSpeed", 1);
        PlayAnimation("Shoot");
        yield return new WaitForSeconds(1.1f);
        isAttacking = false;
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetFloat("speed", walkSpeed);

        bool isWalking = keyboardDirection.magnitude > 0;
        if(isWalking)
        {
            PlayAnimation("Run");
            idleDirection = mouseDirection;
        }
        else if(!isWalking)
        {
            PlayAnimation("Idle");
        }
    }

    public void PlayAnimation(string Name)
    {
        float angle = Mathf.Atan2(keyboardDirection.y, keyboardDirection.x) * Mathf.Rad2Deg;
        if (angle > -11.25f && angle <= 11.25f)
        {
            animator.Play(Name + "E");
        }
        else if (angle > 11.25f && angle <= 33.75f)
        {
            animator.Play(Name + "ENE");
        }
        else if (angle > 33.75f && angle <= 56.25f)
        {
            animator.Play(Name + "NE");
        }
        else if (angle > 56.25f && angle <= 78.75f)
        {
            animator.Play(Name + "NNE");
        }
        else if (angle > 78.75f && angle <= 101.25f)
        {
            animator.Play(Name + "N");
        }
        else if (angle > 101.25f && angle <= 123.75f)
        {
            animator.Play(Name + "NNW");
        }
        else if (angle > 123.75f && angle <= 146.25f)
        {
            animator.Play(Name + "NW");
        }
        else if (angle > 146.25f && angle <= 168.75f)
        {
            animator.Play(Name + "WNW");
        }
        else if (angle > 168.75f || angle <= -168.75f)
        {
            animator.Play(Name + "W");
        }
        else if (angle > -168.75f && angle <= -146.25f)
        {
            animator.Play(Name + "WSW");
        }
        else if (angle > -146.25f && angle <= -123.75f)
        {
            animator.Play(Name + "SW");
        }
        else if (angle > -123.75f && angle <= -101.25f)
        {
            animator.Play(Name + "SSW");
        }
        else if (angle > -101.25f && angle <= -78.75f)
        {
            animator.Play(Name + "S");
        }
        else if (angle > -78.75f && angle <= -56.25f)
        {
            animator.Play(Name + "SSE");
        }
        else if (angle > -56.25f && angle <= -33.75f)
        {
            animator.Play(Name + "SE");
        }
        else if (angle > -33.75f && angle <= -11.25f)
        {
            animator.Play(Name + "ESE");
        }
    }
}
