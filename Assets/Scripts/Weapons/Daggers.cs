using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Transform attackHitBoxPos;
    public float attackRadius = 0.5f;
    public LayerMask Damageable;
    public GameObject dashEnemy;
    public bool dashing = false;
    public bool ability1Hit = false;
    public IEnumerator ability1Coroutine;
    public IEnumerator ultimateCoroutine;
    public int ultimateCasts = 0;

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

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = gameObject.GetComponent<PlayerSpriteController>();
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player Player = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        baseAttackCD = 1 / Player.attackSpeed;
        AttackCD = baseAttackCD;
        Player.AttackCD = baseAttackCD;
        Player.Ability1CD = baseAbility1CD;
        Player.Ability2CD = baseAbility2CD;
        Player.UltimateCD = baseUltimateCD;
        Player.MovementCD = baseMovementCD;
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
                Ability1CD = baseAbility1CD;
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
    /*
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }
    */
    public float circleRadius = 3f; // Radius of the circle

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        Vector2 temp = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        return temp;
    }

    //Player Attack
    public void OnAttack()
    {
        if (!PSC.isAttacking && AttackCD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("Stab", 2);
        Cooldowns[0].SetActive(true);
        AttackCD = baseAttackCD;
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
                collider.gameObject.GetComponent<IDamageable>().Damaged(10);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-10);

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
        Ability1CD = baseAbility1CD;
        Ability1();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Ability1()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        GameObject dagger = Instantiate(Resources.Load<GameObject>("Prefabs/Dagger"), player.transform.position, player.transform.rotation);
        dagger.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        dagger.GetComponent<Dagger>().EditDagger(2f, 11f, true);
        dagger.GetComponent<Rigidbody2D>().velocity = 7f * MapPoint(PSC.currentDirection, 3f);
    }

    public void Ability1TargetHit(GameObject enemy)
    {
        ability1Hit = true;
        dashEnemy = enemy;
        Cooldowns[1].SetActive(false);
        Ability1CD = 0;
        ability1Coroutine = Ability1DashTimer();
        StartCoroutine(ability1Coroutine);
    }

    public IEnumerator Ability1DashTimer()
    {
        yield return new WaitForSeconds(5f);
        Cooldowns[1].SetActive(true);
        Ability1CD = baseAbility1CD;
        ability1Hit = false;
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
            GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
            GameObject dagger = Instantiate(Resources.Load<GameObject>("Prefabs/Dagger"), player.transform.position, player.transform.rotation);
            dagger.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
            dagger.GetComponent<Dagger>().EditDagger(2f, 11f, false);
            dagger.GetComponent<Rigidbody2D>().velocity = 15f * MapPoint(PSC.currentDirection, 1f);
            yield return new WaitForSeconds(.1f);
        }
        PSC._rigidbody.velocity = Vector2.zero;
        PSC.Attack("Stab", 2);
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility2CD;
        PSC.isAttacking = false;
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
            ultimateCasts++;
            if (ultimateCasts == 1)
            {
                ultimateCoroutine = UltimateCastTimer();
                StartCoroutine(ultimateCoroutine);
            }
            PSC.Movable = false;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
        PSC.PlayAnimation("Run");
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(0.25f);
        Ability1CD = 0;
        Ability2CD = 0;
        Cooldowns[1].SetActive(false);
        Cooldowns[2].SetActive(false);
        if (ultimateCasts == 3)
        {
            Cooldowns[3].SetActive(true);
            UltimateCD = baseUltimateCD;
            ultimateCasts = 0;
            StopCoroutine(ultimateCoroutine);
        }
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        PSC.Movable = true;
    }

    public IEnumerator UltimateCastTimer()
    {
        yield return new WaitForSeconds(10f);
        Cooldowns[3].SetActive(true);
        UltimateCD = baseUltimateCD;
        ultimateCasts = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged(10);
        }
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
