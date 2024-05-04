using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Player Player;
    public GameObject daggerPrefab;
    public Transform attackHitBoxPos;
    public float attackRadius = 0.5f;
    public LayerMask Damageable;
    public GameObject dashEnemy;
    public bool dashing = false;
    public bool ability1Hit = false;
    public IEnumerator ability1Coroutine;
    public IEnumerator ultimateCoroutine;
    public int ultimateCasts = 0;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = gameObject.GetComponent<PlayerSpriteController>();
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player = gameObject.GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        daggerPrefab = Resources.Load<GameObject>("Prefabs/Dagger");
        Damageable = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        string[] icons = { "Daggers/Attack", "Daggers/Ability1", "Daggers/Ability2", "Daggers/Ultimate", "Movement" };
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
            AttackCD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("HSlash", 2);
        Cooldowns[0].SetActive(true);
        Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, 3f);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, attackRadius, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.transform.position.x - gameObject.transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
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
            if (ability1Hit)
            {
                PSC.currentDirection = MapPoint(dashEnemy.transform.position - gameObject.transform.position, 1f);
                PSC.PlayAnimation("Run");
                dashing = true;
                return;
            }
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(Ability1Cast());
        }
    }

    private IEnumerator Ability1Cast()
    {
        PSC.Attack("Stab", 2);
        Cooldowns[1].SetActive(true);
        Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        Ability1();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Ability1()
    {
        GameObject dagger = Instantiate(daggerPrefab, Player.transform.position, Player.transform.rotation);
        dagger.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        dagger.GetComponent<Dagger>().EditDagger(2f, Player.Attack, true, this);
        dagger.GetComponent<Rigidbody2D>().velocity = 7f * MapPoint(PSC.currentDirection, 3f);
    }

    public void Ability1TargetHit(GameObject enemy)
    {
        ability1Hit = true;
        dashEnemy = enemy;
        Cooldowns[1].SetActive(false);
        Ability1CD = true;
        ability1Coroutine = Ability1DashTimer();
        StartCoroutine(ability1Coroutine);
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            transform.position = Vector2.MoveTowards(transform.position, dashEnemy.transform.position, 10f * Time.deltaTime);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if (gameObject.transform.position == dashEnemy.transform.position)
            {
                dashing = false;
                PSC.isAttacking = false;
                Cooldowns[1].SetActive(true);
                Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
                ability1Hit = false;
                StopCoroutine(ability1Coroutine);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                if (dashEnemy.transform.position.x - gameObject.transform.position.x >= 0)
                    dashEnemy.GetComponent<IDamageable>().Damaged(10);
                else
                    dashEnemy.GetComponent<IDamageable>().Damaged(-10);
            }
        }
    }

    public IEnumerator Ability1DashTimer()
    {
        yield return new WaitForSeconds(5f);
        Ability1CD = false;
        Cooldowns[1].SetActive(true);
        Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        ability1Hit = false;
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
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            PSC.PlayAnimation("Run");
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC._rigidbody.velocity = new Vector2(-PSC.currentDirection.x * (dashDistance / 2), -PSC.currentDirection.y * (dashDistance / 2));
        for (int i = 0; i < 3; i++)
        {
            GameObject dagger = Instantiate(daggerPrefab, Player.transform.position, Player.transform.rotation);
            dagger.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
            dagger.GetComponent<Dagger>().EditDagger(2f, Player.Attack, false, null);
            dagger.GetComponent<Rigidbody2D>().velocity = 15f * MapPoint(PSC.currentDirection, 1f);
            yield return new WaitForSeconds(.1f);
        }
        PSC._rigidbody.velocity = Vector2.zero;
        PSC.Attack("Stab", 2);
        Cooldowns[2].SetActive(true);
        Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        PSC.isAttacking = false;
    }

    public float GetAbility2Cooldown()
    {
        return 3f * ((100 - Player.CDR) / 100);
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
            UltimateCD = false;
            ultimateCasts++;
            if (ultimateCasts == 1)
            {
                ultimateCoroutine = UltimateCastTimer();
                StartCoroutine(ultimateCoroutine);
            }
            PSC.Movable = false;
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
        PSC.PlayAnimation("Run");
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance * 1.5f, PSC.currentDirection.y * dashDistance * 1.5f);
        yield return new WaitForSeconds(0.2f);
        Ability1CD = true;
        Ability2CD = true;
        Cooldowns[1].SetActive(false);
        Cooldowns[2].SetActive(false);
        if (ultimateCasts == 3)
        {
            Cooldowns[3].SetActive(true);
            Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(10f * ((100 - Player.CDR) / 100));
            ultimateCasts = 0;
            StopCoroutine(ultimateCoroutine);
        }
        else
            UltimateCD = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        PSC.Movable = true;
    }

    public IEnumerator UltimateCastTimer()
    {
        yield return new WaitForSeconds(10f);
        Cooldowns[3].SetActive(true);
        Cooldowns[3].SetActive(true);
        Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(10f * ((100 - Player.CDR) / 100));
        ultimateCasts = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged(10);
        }
        else if (collision.gameObject.tag == "Environment")
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
            PSC._rigidbody.velocity = Vector2.zero;
        }
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
