using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Player Player;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player = GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        string[] icons = { "Scythe/Attack", "Scythe/Ability1", "Scythe/Ability2", "Scythe/Ultimate", "Movement" };
        for (int i = 0; i < icons.Length; i++)
        {
            Cooldowns[i].GetComponent<CooldownUI>().InitiateCooldown(Resources.Load<Sprite>("Icons/" + icons[i]), gameObject);
            Cooldowns[i].SetActive(false);
        }
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
        Cooldowns[4].SetActive(true);
        Cooldowns[4].GetComponent<CooldownUI>().StartCooldown(5f);
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
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("Stab", 2);
        Cooldowns[0].SetActive(true);
        Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, 3f);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 1f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Player.Attack);
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
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, 3f);
        for (int i = 0; i < 2; i++)
        {
            PSC.Attack("Stab", 2);
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 1.5f, Damageable);
            foreach (Collider2D collider in detectedObjects)
            {
                if (collider.transform.position.x - transform.position.x >= 0)
                    collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
                else
                    collider.gameObject.GetComponent<IDamageable>().Damaged(-Player.Attack);
            }
            yield return new WaitForSeconds(.5f);
        }
        
        Cooldowns[1].SetActive(true);
        Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
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
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
        PSC.PlayAnimation("Run");
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.25f);       
        Cooldowns[2].SetActive(true);
        Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        PSC.isAttacking = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged(-Player.Attack);
        }
        else if (collision.gameObject.tag == "Environment")
        {
            
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
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
        PSC.Attack("Stab", 2);
        attackHitBoxPos.localPosition = Player.transform.localPosition;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 5f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Player.Attack);
        }
        Instantiate(Resources.Load<GameObject>("Prefabs/ScytheDomain"), transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
        yield return new WaitForSeconds(10f);
        Cooldowns[3].SetActive(true);
        Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(cd);
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
