using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knives : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public GameObject knifePrefab;
    public float knifeSpeed = 10f;
    public float knifeLife = 3f;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Player = GetComponent<Player>();
        knifePrefab = Resources.Load<GameObject>("Prefabs/Knife");
		PSC.twoHanded = false;
	}

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public Vector2 MapPointFromDegree(int degree, float radius)
    {
        float angle = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    #region Movement ability
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
            AttackCD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Player.Cooldowns[0].SetActive(true);
        Player.Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        GameObject knife = Instantiate(knifePrefab, Player.transform.position, Player.transform.rotation);
        knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        knife.GetComponent<Knife>().EditKnife(3f, Player.Attack, false, false);
        knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection, 1);
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Player.Cooldowns[1].SetActive(true);
        Player.Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        Ability1();
        PSC.isAttacking = false;
    }

    public void Ability1()
    {
        GameObject knife = Instantiate(knifePrefab, Player.transform.position, Player.transform.rotation);
        knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        knife.GetComponent<Knife>().EditKnife(3f, Player.Attack, false, true);
        knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection, 1);
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
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.2f);
        PSC._rigidbody.velocity = Vector2.zero;
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Player.Cooldowns[2].SetActive(true);
        Player.Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        Ability2();
        PSC.isAttacking = false;
    }

    public void Ability2()
    {
        for (int i = -2; i < 3; i++)
        {
            knifeLife = 3f;
            GameObject knife = Instantiate(knifePrefab, new Vector3(Player.transform.position.x + MapPointFromDegree(72 * i, 1).x, Player.transform.position.y + MapPointFromDegree(72 * i, 1).y), Player.transform.rotation);
            knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - (-i * 10 + 90));
            knife.GetComponent<Knife>().EditKnife(3f, Player.Attack, true, false);
        }
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
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
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
        int waves = (int)(Player.attackSpeed / .1f);
        for (int h = 0; h < waves; h++)
        {
            for (int i = -2; i < 3; i++)
            {
                GameObject knife = Instantiate(knifePrefab, new Vector3(Player.transform.position.x + MapPointFromDegree(72 * i, 1).x, Player.transform.position.y + MapPointFromDegree(72 * i, 1).y), Player.transform.rotation);
                knife.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
                knife.GetComponent<Knife>().EditKnife(3f, Player.Attack, false, false);
                knife.GetComponent<Rigidbody2D>().velocity = knifeSpeed * MapPoint(PSC.currentDirection, 1);
            }
            yield return new WaitForSeconds(.2f);
        }
        Player.Cooldowns[3].SetActive(true);
        Player.Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(10f * ((100 - Player.CDR) / 100));
        PSC.isAttacking = false;
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
