using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Player Player;
    public GameObject Fireball;
    public GameObject Vortex;
    public GameObject Sphere;
    public GameObject Burst;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Cooldowns = PlayerManager.instance.Cooldowns;
        Player = GetComponent<Player>();
        Fireball = Resources.Load<GameObject>("Prefabs/Fireball");
        Vortex = Resources.Load<GameObject>("Prefabs/ArcaneVortex");
        Sphere = Resources.Load<GameObject>("Prefabs/ArcaneSphere");
        Burst = Resources.Load<GameObject>("Prefabs/ArcaneBurst");
    }

    private void Start()
    {
        //string[] icons = { "Bow/Attack", "Bow/Ability1", "Bow/Ability2", "Bow/Ultimate", "Movement" };
        string[] icons = { "Movement", "Movement", "Movement", "Movement", "Movement" };
        for (int i = 0; i < icons.Length; i++)
        {
            Cooldowns[i].GetComponent<CooldownUI>().InitiateCooldown(Resources.Load<Sprite>("Icons/" + icons[i]), gameObject);
            Cooldowns[i].SetActive(false);
        }
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
        yield return new WaitForSeconds(.2f);
        GameObject fireball = Instantiate(Fireball, Player.transform.position, Player.transform.rotation);
        fireball.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg + 270);
        fireball.GetComponent<Fireball>().StartFireball(Player.Attack);
        fireball.GetComponent<Rigidbody2D>().velocity = 7f * MapPoint(PSC.currentDirection, 1);
        Cooldowns[0].SetActive(true);
        Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        PSC.isAttacking = false;
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartCoroutine(Ability1Cast(mousePosition));
        }
    }

    private IEnumerator Ability1Cast(Vector3 mousePosition)
    {
        PSC.Attack("HandCast", 1);
        mousePosition.z = 0;
        GameObject arcaneVortex = Instantiate(Vortex, mousePosition, Quaternion.identity);
        arcaneVortex.GetComponent<ArcaneVortex>().StartVortex(Player.Attack / 10, 10f);
        yield return new WaitForSeconds(.5f);
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
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        PSC.Attack("HandCast", 1);
        yield return new WaitForSeconds(.2f);
        GameObject arcaneSphere = Instantiate(Sphere, Player.transform.position, Player.transform.rotation);
        arcaneSphere.GetComponent<ArcaneSphere>().StartSphere(Player.Attack, 3f);
        arcaneSphere.GetComponent<Rigidbody2D>().velocity = 5f * MapPoint(PSC.currentDirection, 1);
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
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(UltimateCast());
        }
    }

    private IEnumerator UltimateCast()
    {
        PSC.Attack("HandCast", 1);
        yield return new WaitForSeconds(.2f);
        GameObject arcaneBurst = Instantiate(Burst, Player.transform.position, Player.transform.rotation);
        arcaneBurst.GetComponent<ArcaneBurst>().StartBurst(Player.Attack * 2);
        Cooldowns[3].SetActive(true);
        Cooldowns[3].GetComponent<CooldownUI>().StartCooldown(10f * ((100 - Player.CDR) / 100));
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
