using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;
    public GameObject Slashwave;

    public bool AttackCD = true;
    private float AttackRadius = 1.25f;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    private float Ability2Radius = 2.5f;
    public bool UltimateCD = true;
    private float UltimateRadius = 3f;
    public bool MovementCD = true;

    public float dashDistance = 15f;
    public bool isDashing = false;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Player = GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Enemy");
        Slashwave = Resources.Load<GameObject>("Prefabs/Slashwave");
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
		while (isDashing)
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
        isDashing = true;
        StartCoroutine(GenerateGhost());
        yield return new WaitForSeconds(0.25f);
        Player.Cooldowns[4].SetActive(true);
        Player.Cooldowns[4].GetComponent<CooldownUI>().StartCooldown(5f);
        isDashing = false;
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
        PSC.Attack("2HSlash", 2);
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
            {
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
            }
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
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(Ability1Cast());
        }
    }

    private IEnumerator Ability1Cast()
    {
        PSC.Attack("2HThrust", 2);
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.1f);
        PSC._rigidbody.velocity = Vector2.zero;
        GameObject slashwave = Instantiate(Slashwave, Player.transform.position, Player.transform.rotation);
        slashwave.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg);
        slashwave.GetComponent<Slashwave>().EditSlashwave(3f, Player.Attack);
        slashwave.GetComponent<Rigidbody2D>().velocity = 10f * MapPoint(PSC.currentDirection, 1);
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
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
		PSC.Attack("2HSpin", 2);
		attackHitBoxPos.localPosition = Vector3.zero;
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = Ability2Radius;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, Ability2Radius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
        }
        yield return new WaitForSeconds(.25f);
        Player.Cooldowns[2].SetActive(true);
        Player.Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        PSC.isAttacking = false;
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
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        float cd = 10f * ((100 - Player.CDR) / 100);
        PSC.Attack("2HSlam", 2);
        yield return new WaitForSeconds(.5f);
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, UltimateRadius);
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = UltimateRadius;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 3f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack, transform.position, 3);
                if (collider.gameObject.GetComponent<Entity>().currentHealth > 0)
                    collider.gameObject.GetComponent<IEffectable>().ApplyBuff(new MaceStun(3f, "Greatsword - Ultimate", collider.gameObject));
            }
        }
        
        PSC.isAttacking = false;
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
