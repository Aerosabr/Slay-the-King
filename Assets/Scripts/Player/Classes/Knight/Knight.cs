using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Transform attackHitBoxPos;
    public float attackRadius;
    public LayerMask Damageable;

    public float baseAttackCD = 0.1f;
    public float AttackCD = 0;

    public float baseAbility1CD = 2f;
    public float Ability1CD = 0;

    public float baseAbility2CD = 3f;
    public float Ability2CD = 0;

    public float baseUltimateCD = 5f;
    public float UltimateCD = 0;

    public float baseMovementCD = 5f;
    public float MovementCD = 0;

    public float dashDistance;

    public void Awake()
    {
        PSC = gameObject.GetComponent<PlayerSpriteController>();
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player Player = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>();
        Player.AttackCD = baseAttackCD;
        Player.Ability1CD = baseAbility1CD;
        Player.Ability2CD = baseAbility2CD;
        Player.UltimateCD = baseUltimateCD;
        Player.MovementCD = baseMovementCD;
    }

    public void OnDash()
    {
        if (!PSC.isAttacking && MovementCD == 0)
        {
            PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
            StartCoroutine(Dashing());
        }
    }

    public IEnumerator Dashing()
    {
        PSC.Movable = false;
        yield return new WaitForSeconds(0.25f);
        Cooldowns[4].SetActive(true);
        MovementCD = baseMovementCD;
        PSC.Movable = true;
    }

    public float GetMovementCooldown()
    {
        return baseMovementCD;
    }

    public void ResetMovementCooldown()
    {
        MovementCD = 0;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }

    public float circleRadius; // Radius of the circle

    public Vector2 MapPoint(Vector2 point)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        Vector2 temp = new Vector2(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);
        temp.y -= 1.5f;
        return temp;
    }

    //Player Attack
    public void OnAttack()
    {
        if (!PSC.isAttacking && AttackCD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("DSlash", 2);
        Cooldowns[0].SetActive(true);
        AttackCD = baseAttackCD;
        Attack();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attackRadius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.SendMessage("Damaged", 4);

        }
    }

    public float GetAttackCooldown()
    {
        return baseAttackCD;
    }

    public void ResetAttackCooldown()
    {
        AttackCD = 0;
    }

    //Player Ability1
    public void OnAbility1()
    {
        if (!PSC.isAttacking && Ability1CD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability1Cast());
        }
    }

    private IEnumerator Ability1Cast()
    {
        yield return new WaitForSeconds(.5f);
        Cooldowns[1].SetActive(true);
        Ability1CD = baseAbility1CD;
        PSC.isAttacking = false;
    }

    public void Ability1()
    {

    }

    public float GetAbility1Cooldown()
    {
        return baseAbility1CD;
    }

    public void ResetAbility1Cooldown()
    {
        Ability1CD = 0;
    }

    //Player Ability2
    public void OnAbility2()
    {
        if (!PSC.isAttacking && Ability2CD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        yield return null;
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility2CD;
        PSC.isAttacking = false;
    }

    public void Ability2()
    {

    }

    public float GetAbility2Cooldown()
    {
        return baseAbility2CD;
    }

    public void ResetAbility2Cooldown()
    {
        Ability2CD = 0;
    }

    //Player Ultimate
    public void OnUltimate()
    {
        if (!PSC.isAttacking && UltimateCD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        yield return new WaitForSeconds(.5f);
        Cooldowns[3].SetActive(true);
        UltimateCD = baseUltimateCD;
        PSC.isAttacking = false;
    }

    public IEnumerator Ultimate()
    {
        yield return null;
    }

    public float GetUltimateCooldown()
    {
        return baseUltimateCD;
    }

    public void ResetUltimateCooldown()
    {
        UltimateCD = 0;
    }
}
