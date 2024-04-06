using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public GameObject arrowPrefab;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Player Player;
    public float arrowSpeed = 10f;
    public float arrowLife = 3f;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = gameObject.GetComponent<PlayerSpriteController>();
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Arrows");
        Cooldowns.Add(GameObject.Find("AttackCooldown"));
        Cooldowns.Add(GameObject.Find("Ability1Cooldown"));
        Cooldowns.Add(GameObject.Find("Ability2Cooldown"));
        Cooldowns.Add(GameObject.Find("UltimateCooldown"));
        Cooldowns.Add(GameObject.Find("MovementCooldown"));
        Player = gameObject.GetComponent<Player>();
    }

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
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
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[0].SetActive(true);
        Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        GameObject arrow = Instantiate(arrowPrefab, Player.transform.position, Player.transform.rotation);
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg + 180);
        arrow.GetComponent<Arrow>().EditArrow(3f, Player.Attack, true);
        arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(PSC.currentDirection, 1);
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
        PSC._rigidbody.velocity = new Vector2(-PSC.currentDirection.x * dashDistance, -PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.2f);
        PSC._rigidbody.velocity = Vector2.zero;
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[1].SetActive(true);
        Cooldowns[1].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        Ability1();
        PSC.isAttacking = false;
    }

    public void Ability1()
    {
        for (int i = -2; i < 3; i++)
        {
            arrowLife = 3f;
            GameObject arrow = Instantiate(arrowPrefab, Player.transform.position, Player.transform.rotation);
            arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - (-i * 10 - 180));
            float currentAngleRadians = ((Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg) + 10 * i) * Mathf.Deg2Rad;
            Vector2 currentVector = new Vector2(Mathf.Cos(currentAngleRadians), Mathf.Sin(currentAngleRadians)) * PSC.currentDirection.magnitude;
            arrow.GetComponent<Arrow>().EditArrow(3f, Player.Attack, true);
            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(currentVector, 1);
        }
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
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC.Attack("Shoot", 16);
        yield return new WaitForSeconds(.2f);
        Ability2();
        Cooldowns[2].SetActive(true);
        Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
        PSC.isAttacking = false;
    }

    public void Ability2()
    {
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, Player.transform.position, Player.transform.rotation);
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg + 180);
        arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(PSC.currentDirection, 1);
        arrow.GetComponent<Arrow>().EditArrow(3f, Player.Attack, false);
        arrow.transform.localScale = new Vector3(.14f, .2f, .2f);
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += 5;
            UltimateCD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(UltimateCast(mousePosition));
        }
    }

    private IEnumerator UltimateCast(Vector3 mousePosition)
    {
        PSC.Attack("Shoot", 2);
        yield return new WaitForSeconds(.5f);
        Cooldowns[3].SetActive(true);
        Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(10f * ((100 - Player.CDR) / 100));
        StartCoroutine(Ultimate(mousePosition));
        PSC.isAttacking = false;
    }

    public IEnumerator Ultimate(Vector3 mousePosition)
    {
        Vector3 temp = mousePosition;
        
        for (int i = 0; i < 40; i++)
        {
            temp.x = mousePosition.x + Random.Range(-1.6f, 1.6f);
            GameObject arrow = Instantiate(arrowPrefab, temp, Quaternion.identity);
            arrow.GetComponent<Arrow>().EditArrow(0.6f, Player.Attack, false);
            arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * Vector2.down;
            yield return new WaitForSeconds(.05f);
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
