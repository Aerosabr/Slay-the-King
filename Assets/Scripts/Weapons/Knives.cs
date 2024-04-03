using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knives : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public GameObject knifePrefab;
    public float knifeSpeed = 10f;
    public float knifeLife = 3f;
    public List<GameObject> Cooldowns = new List<GameObject>();

    public float baseAttackCD;
    public float AttackCD;

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
        knifePrefab = Resources.Load<GameObject>("Prefabs/Knife");
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player Player = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Player>();
        baseAttackCD = 1 / Player.attackSpeed;
        AttackCD = baseAttackCD;
        Player.AttackCD = baseAttackCD;
        Player.Ability1CD = baseAbility1CD;
        Player.Ability2CD = baseAbility2CD;
        Player.UltimateCD = baseUltimateCD;
        Player.MovementCD = baseMovementCD;
    }

    public void Update()
    {

    }

    //Movement ability
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

    //Knife direction
    public float circleRadius = 1f; // Radius of the circle

    public Vector2 MapPoint(Vector2 point)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);
    }

    public Vector2 MapPointFromDegree(int degree)
    {
        float angle = degree * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);
    }

    //Basic Attack
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[0].SetActive(true);
        AttackCD = baseAttackCD;
        Attack();
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        GameObject knife = Instantiate(knifePrefab, player.transform.position, player.transform.rotation);
        knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        knife.GetComponent<Knife>().EditKnife(3f, 11f, false, false);
        knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection);
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
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance * 4, PSC.currentDirection.y * dashDistance * 4);
        yield return new WaitForSeconds(.1f);
        PSC._rigidbody.velocity = Vector2.zero;
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[1].SetActive(true);
        Ability1CD = baseAbility1CD;
        Ability1();
        PSC.isAttacking = false;
    }

    public void Ability1()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        for (int i = -2; i < 3; i++)
        {
            knifeLife = 3f;
            GameObject knife = Instantiate(knifePrefab, new Vector3(player.transform.position.x + MapPointFromDegree(72 * i).x, player.transform.position.y + MapPointFromDegree(72 * i).y), player.transform.rotation);
            knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - (-i * 10 + 90));
            float currentAngleRadians = ((Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg) + 10 * i) * Mathf.Deg2Rad;
            Vector2 currentVector = new Vector2(Mathf.Cos(currentAngleRadians), Mathf.Sin(currentAngleRadians)) * PSC.currentDirection.magnitude;
            knife.GetComponent<Knife>().EditKnife(3f, 11f, true, false);
            //knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(currentVector);

        }

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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility1CD;
        Ability2();
        PSC.isAttacking = false;
    }

    public void Ability2()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        GameObject knife = Instantiate(knifePrefab, player.transform.position, player.transform.rotation);
        knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        knife.GetComponent<Knife>().EditKnife(3f, 11f, false, true);
        knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection);
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        
        StartCoroutine(Ultimate());
    }

    public IEnumerator Ultimate()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        int waves = (int)(player.GetComponent<Player>().attackSpeed / .1f);
        for (int h = 0; h < waves; h++)
        {
            for (int i = -2; i < 3; i++)
            {
                GameObject knife = Instantiate(knifePrefab, new Vector3(player.transform.position.x + MapPointFromDegree(72 * i).x, player.transform.position.y + MapPointFromDegree(72 * i).y), player.transform.rotation);
                knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
                knife.GetComponent<Knife>().EditKnife(3f, 11f, false, false);
                knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection);
            }
            yield return new WaitForSeconds(.2f);
        }
        Cooldowns[3].SetActive(true);
        UltimateCD = baseUltimateCD;
        PSC.isAttacking = false;
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
