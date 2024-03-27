using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ranger : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowLife = 3f;
    public List<GameObject> Cooldowns = new List<GameObject>();

    public float baseAttackCD = 0.1f;
    public float AttackCD = 0;

    public float baseAbility1CD = 2f;
    public float Ability1CD = 0;

    public float baseAbility2CD = 3f;
    public float Ability2CD = 0;

    public float baseUltimateCD = 5f;
    public float UltimateCD = 0;

    public void Awake()
    {
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Arrows");
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[0].SetActive(true);
        AttackCD = baseAttackCD;
        Attack(PSC);
        PSC.isAttacking = false;
    }

    public void Attack(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90 + 270);

        rigidbody.velocity = arrowSpeed * MapPoint(PSC.currentDirection);
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[1].SetActive(true);
        Ability1CD = baseAbility1CD;
        Ability1(PSC);
        PSC.isAttacking = false;
    }

    public void Ability1(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        for (int i = -1; i < 2; i++)
        {
            arrowLife = 3f;
            GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
            arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - (-i * 20 + 90));
            float currentAngleRadians = ((Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg) + 20 * i) * Mathf.Deg2Rad;
            Vector2 currentVector = new Vector2(Mathf.Cos(currentAngleRadians), Mathf.Sin(currentAngleRadians)) * PSC.currentDirection.magnitude;

            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(currentVector);
  
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
        for (int i = 0; i < 5; i++)
        {
            PSC.Attack("Shoot", 16);
            yield return new WaitForSeconds(.2f);
            Ability2(PSC);
        }
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility2CD; 
        PSC.isAttacking = false;
    }

    public void Ability2(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        rigidbody.velocity = arrowSpeed * MapPoint(PSC.currentDirection);
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += 3;
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(UltimateCast(PSC, mousePosition));
        }
    }

    private IEnumerator UltimateCast(PlayerSpriteController PSC, Vector3 mousePosition)
    {
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[3].SetActive(true);
        UltimateCD = baseUltimateCD;
        StartCoroutine(Ultimate(mousePosition));
        PSC.isAttacking = false;
    }

    public IEnumerator Ultimate(Vector3 mousePosition)
    {
        Vector3 temp = mousePosition;
        for (int i = 0; i < 20; i++)
        {
            temp.x = mousePosition.x + Random.Range(-0.8f, 0.8f);
            arrowLife = .3f;
            GameObject arrow = Instantiate(arrowPrefab, temp, Quaternion.identity);
            arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * Vector2.down;
            yield return new WaitForSeconds(.1f);
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
