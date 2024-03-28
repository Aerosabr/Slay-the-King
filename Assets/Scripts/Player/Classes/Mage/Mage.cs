using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    public List<GameObject> Cooldowns = new List<GameObject>();

    public float fireballSpeed = 5f;
    public float fireballLife = 3.0f;
    public GameObject fireballPrefab;

    public float baseAttackCD = 1f;
    public float AttackCD = 0;

    public float baseAbility1CD = 2f;
    public float Ability1CD = 0;

    public float baseAbility2CD = 3f;
    public float Ability2CD = 0;

    public float baseUltimateCD = 5f;
    public float UltimateCD = 0;

    public float teleportDistance;

    public void Awake()
    {
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Player Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Player.AttackCD = baseAttackCD;
        Player.Ability1CD = baseAbility1CD;
        Player.Ability2CD = baseAbility2CD;
        Player.UltimateCD = baseUltimateCD;
    }

    public void OnDash()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * teleportDistance, PSC.currentDirection.y * teleportDistance);
        StartCoroutine(Dashing(PSC));
    }

    public IEnumerator Dashing(PlayerSpriteController PSC)
    {
        PSC.Movable = false;
        yield return new WaitForSeconds(0.025f);
        PSC.Movable = true;
    }

    public float circleRadius = 1f; // Radius of the circle

    public Vector2 MapPoint(Vector2 point)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);
    }

    //Player Attack
    public void OnAttack()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking && AttackCD == 0)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(AttackCast(PSC));
        }
    }

    private IEnumerator AttackCast(PlayerSpriteController PSC)
    {
        PSC.Attack("HandCast", 1f);
        yield return new WaitForSeconds(.5f);
        Attack(PSC);
        Cooldowns[0].SetActive(true);
        AttackCD = baseAttackCD;
        PSC.isAttacking = false;
    }

    public void Attack(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        fireballLife = 3f;
        GameObject arrow = Instantiate(fireballPrefab, player.transform.position, player.transform.rotation);
        Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);

        rigidbody.velocity = fireballSpeed * MapPoint(PSC.currentDirection);
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
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking && Ability1CD == 0)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability1Cast(PSC));
        }
    }

    private IEnumerator Ability1Cast(PlayerSpriteController PSC)
    {
        PSC.Attack("CastCircle", 1);
        yield return new WaitForSeconds(.5f);
        Cooldowns[1].SetActive(true);
        Ability1CD = baseAbility1CD;
        PSC.isAttacking = false;
    }

    public void Ability1(PlayerSpriteController PSC)
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
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking && Ability2CD == 0)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast(PSC));
        }
    }

    private IEnumerator Ability2Cast(PlayerSpriteController PSC)
    {
        yield return null;
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility2CD;
        PSC.isAttacking = false;
    }

    public void Ability2(PlayerSpriteController PSC)
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
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking && UltimateCD == 0)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability1Cast(PSC));
        }
    }

    private IEnumerator UltimateCast(PlayerSpriteController PSC, Vector3 mousePosition)
    {
        yield return new WaitForSeconds(.5f);
        Cooldowns[3].SetActive(true);
        UltimateCD = baseUltimateCD;
        PSC.isAttacking = false;
    }

    public IEnumerator Ultimate(Vector3 mousePosition)
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
