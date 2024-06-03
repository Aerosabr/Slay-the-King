using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;

    public bool AttackCD = true;
    private float AttackRadius = .75f;
    public bool Ability1CD = true;
    private float Ability1Radius = 1.5f;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    private float UltimateRadius = 4f;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Player = GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Enemy");
		PSC.twoHanded = true;
	}

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        Vector2 temp = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        return temp;
    }

    #region Player Movement
    public void OnDash()
    {
        if (!PSC.isAttacking && MovementCD)
        {
            MovementCD = false;
            PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
            StartCoroutine(Dashing());
        }
    }

    public IEnumerator Dashing()
    {
        PSC.Movable = false;
        yield return new WaitForSeconds(0.25f);
        Player.Cooldowns[4].SetActive(true);
        Player.Cooldowns[4].GetComponent<CooldownUI>().StartCooldown(5f);
        PSC.Movable = true;
    }

    public float GetMovementCooldown()
    {
        return 5f;
    }

    public void ResetMovementCooldown()
    {
        MovementCD = true;
    }
    #endregion

    #region Player Attack
    public void OnAttack()
    {
        if (!PSC.isAttacking && AttackCD && PSC.Movable)
        {
            PSC.isAttacking = true;
            AttackCD = false;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - (transform.position - new Vector3(0.04f, 0.3f)), 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("Stab", 2);
        Player.Cooldowns[0].SetActive(true);
        Player.Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, AttackRadius);
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = AttackRadius;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, AttackRadius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);  
        }
    }

    public float GetAttackCooldown()
    {
        return 1 / Player.attackSpeed;
    }

    public void ResetAttackCooldown()
    {
        AttackCD = true;
    }
    #endregion

    #region Player Ability1
    public void OnAbility1()
    {
        if (!PSC.isAttacking && Ability1CD && PSC.Movable)
        {
            Ability1CD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - (transform.position - new Vector3(0.04f, 0.3f)), 1f);
            StartCoroutine(Ability1Cast());
        }
    }

    private IEnumerator Ability1Cast()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, Ability1Radius);
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = Ability1Radius;
        PSC.Attack("Stab", 2);
        yield return new WaitForSeconds(1.5f);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, Ability1Radius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
                if (collider.gameObject.GetComponent<Entity>().currentHealth > 0)
                    collider.gameObject.GetComponent<IEffectable>().ApplyBuff(new MaceStun(3f, "Hammer - Ability1", collider.gameObject));
            }
        }

        Player.Cooldowns[1].SetActive(true);
        Player.Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(6f * ((100 - Player.CDR) / 100));
        PSC.isAttacking = false;
    }

    public float GetAbility1Cooldown()
    {
        return 3f * ((100 - Player.CDR) / 100);
    }

    public void ResetAbility1Cooldown()
    {
        Ability1CD = true;
    }
    #endregion

    #region Player Ability2
    public void OnAbility2()
    {
        if (!PSC.isAttacking && Ability2CD && PSC.Movable)
        {
            Ability2CD = false;
            PSC.isAttacking = true;

            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC.PlayAnimation("Stab");
        gameObject.GetComponent<IEffectable>().ApplyBuff(new Shield(gameObject.GetComponent<Entity>().maxHealth / 3, 25f, "Hammer - Ability 2", gameObject));
        if (Player.Cooldowns[1].activeSelf)
            Player.Cooldowns[1].GetComponent<CooldownUI>().remainingTime -= (6f * ((100 - Player.CDR) / 100)) / 2;
        yield return new WaitForSeconds(.25f);
        PSC.isAttacking = false;
        Player.Cooldowns[2].SetActive(true);
        Player.Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(6f * ((100 - Player.CDR) / 100));
    }

    public float GetAbility2Cooldown()
    {
        return 4f * ((100 - Player.CDR) / 100);
    }

    public void ResetAbility2Cooldown()
    {
        Ability2CD = true;
    }
    #endregion

    #region Player Ultimate
    public void OnUltimate()
    {
        if (!PSC.isAttacking && UltimateCD && PSC.Movable)
        {
            PSC.isAttacking = true;
            UltimateCD = false;
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        float cd = 10f * ((100 - Player.CDR) / 100);
        PSC.Attack("Stab", 2);
        yield return new WaitForSeconds(.25f);
        attackHitBoxPos.localPosition = Vector2.zero;
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = UltimateRadius;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, UltimateRadius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 5);
                if (collider.GetComponent<Entity>().currentHealth > 0)
                {
                    StartCoroutine(KnockCoroutine(collider.GetComponent<Rigidbody2D>()));
                    collider.gameObject.GetComponent<IEffectable>().ApplyBuff(new MaceStun(5f, "Hammer - Ultimate", collider.gameObject));
                }
            }
        }
        PSC.isAttacking = false;
        Player.Cooldowns[3].SetActive(true);
        Player.Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(cd);
    }

    public IEnumerator KnockCoroutine(Rigidbody2D enemy)
    {
        Vector2 force = (enemy.transform.position - transform.position).normalized * 8f;
        enemy.GetComponent<Entity>().isMovable = false;
        enemy.velocity = force;
        yield return new WaitForSeconds(.3f);
        enemy.GetComponent<Entity>().isMovable = true;
        enemy.velocity = new Vector2();
    }

    public float GetUltimateCooldown()
    {
        return 10f * ((100 - Player.CDR) / 100);
    }

    public void ResetUltimateCooldown()
    {
        UltimateCD = true;
    }
    #endregion
}
