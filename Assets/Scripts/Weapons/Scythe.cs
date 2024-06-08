using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;

    public bool AttackCD = true;
    private float AttackRadius = 1f;
    public bool Ability1CD = true;
    private float Ability1Radius = 1.5f;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;
    public bool dashing;
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
    public void CreateGhost()
	{
		transform.GetChild(4).GetChild(3).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[0].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[1].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[2].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(2).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[3].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(3).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[4].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(4).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[5].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(5).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[6].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(6).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[7].GetComponent<SpriteRenderer>().sprite;
		var clone = Instantiate(transform.GetChild(4).GetChild(3), transform.position, transform.rotation);
		clone.gameObject.SetActive(true);
		Destroy(clone.gameObject, 0.2f);
	}
	public IEnumerator GenerateGhost()
	{
		while (dashing)
		{
			CreateGhost();
			yield return null;
		}
	}
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
        dashing = true;
        StartCoroutine(GenerateGhost());
        yield return new WaitForSeconds(0.25f);
        Player.Cooldowns[4].SetActive(true);
        Player.Cooldowns[4].GetComponent<CooldownUI>().StartCooldown(5f);
        dashing = false;
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
        PSC.Attack("ScytheSlash", 2);
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
        for (int i = 0; i < 2; i++)
        {
            PSC.Attack("ScytheCross", 1);
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, Ability1Radius, Damageable);
            foreach (Collider2D collider in detectedObjects)
            {
                if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                    collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, i * 3);
            }
            yield return new WaitForSeconds(.5f);
        }
        
        Player.Cooldowns[1].SetActive(true);
        Player.Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
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
            dashing = true;
            StartCoroutine(GenerateGhost());
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - (transform.position - new Vector3(0.04f, 0.3f)), 1f);
        PSC.PlayAnimation("ScytheDash");
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.25f);       
        Player.Cooldowns[2].SetActive(true);
        Player.Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        dashing = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        PSC.isAttacking = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (dashing)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
            }
            else if (collision.gameObject.tag == "Environment")
            {
                dashing = false;
                GetComponent<CapsuleCollider2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
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
        PSC.Attack("ScytheDash", 2);
        attackHitBoxPos.localPosition = Vector2.zero;
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = 5f;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 5f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
        }
        Instantiate(Resources.Load<GameObject>("Prefabs/ScytheDomain"), transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
        yield return new WaitForSeconds(10f);
        Player.Cooldowns[3].SetActive(true);
        Player.Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(cd);
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
