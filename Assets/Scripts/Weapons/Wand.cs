using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public GameObject Bolttarget;
    public GameObject Laserbeam;
    public GameObject Shockbolt;
    public GameObject Energycrystal;
    public GameObject Wandlaser;
    public Collider2D Collider;
    public Vector3 boltEnd;

    public bool AttackCD = true;
    public bool Ability1CD = true;
    public bool Ability2CD = true;
    public bool UltimateCD = true;
    public bool MovementCD = true;

    public float dashDistance = 150f;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        Player = GetComponent<Player>();
        Bolttarget = Resources.Load<GameObject>("Prefabs/BoltTarget");
        Laserbeam = Resources.Load<GameObject>("Prefabs/LaserBeam");
        Shockbolt = Resources.Load<GameObject>("Prefabs/ShockBolt");
        Energycrystal = Resources.Load<GameObject>("Prefabs/EnergyCrystal");
        Wandlaser = Resources.Load<GameObject>("Prefabs/WandLaser");
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
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
            PSC._rigidbody.velocity = new Vector2(PSC.currentDirection.x * dashDistance, PSC.currentDirection.y * dashDistance);
            StartCoroutine(Dashing());
        }
    }

    public IEnumerator Dashing()
    {
        PSC.Movable = false;
        yield return new WaitForSeconds(0.025f);
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        Player.Cooldowns[4].SetActive(true);
        Player.Cooldowns[4].GetComponent<CooldownUI>().StartCooldown(5f);
        PSC.Movable = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
        }
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AttackCD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f);
            StartCoroutine(AttackCast(mousePosition));
        }
    }

    private IEnumerator AttackCast(Vector3 mousePosition)
    {
        Collider = null;
        PSC.Attack("HandCast", 2);
        boltEnd = mousePosition;
        GameObject laser = Instantiate(Bolttarget, Player.transform.position, Player.transform.rotation);
        laser.GetComponent<BoltTarget>().EditTarget(this, mousePosition);

        yield return new WaitForSeconds(.25f);
        LineRenderer lineRenderer = Instantiate(Laserbeam, transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, boltEnd);
        if (Collider != null)
        {
            if (Collider.transform.position.x - transform.position.x >= 0)
                Collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            else
                Collider.gameObject.GetComponent<IDamageable>().Damaged(-Player.Attack);
        }
        Destroy(lineRenderer.gameObject, .5f);
        Player.Cooldowns[0].SetActive(true);
        Player.Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
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
            StartCoroutine(Ability1Cast());
        }
    }

    private IEnumerator Ability1Cast()
    {
        PSC.Attack("HandCast", 2);
        yield return new WaitForSeconds(.25f);
        GameObject bolt = Instantiate(Shockbolt, Player.transform.position, Player.transform.rotation);
        bolt.GetComponent<ShockBolt>().EditBolt(8f, Player.Attack);
        bolt.GetComponent<Rigidbody2D>().velocity = 10f * MapPoint(PSC.currentDirection, 1);
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ability2CD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast(mousePosition));
        }
    }

    private IEnumerator Ability2Cast(Vector3 mousePos)
    {
        PSC.Attack("HandCast", 16);
        yield return new WaitForSeconds(.2f);
        mousePos.z = 0;
        GameObject crystal = Instantiate(Energycrystal, mousePos, Player.transform.rotation);
        crystal.GetComponent<EnergyCrystal>().EditCrystal(Player.Attack);
        Player.Cooldowns[2].SetActive(true);
        Player.Cooldowns[2].GetComponent<CooldownUI>().StartCooldown(3f * ((100 - Player.CDR) / 100));
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
        PSC.Attack("HandCast", 2);
        yield return new WaitForSeconds(.25f);
        GameObject laser = Instantiate(Wandlaser, transform.position, Quaternion.identity);
        laser.GetComponent<WandLaser>().StartLaser(transform.position, Player.Attack);
        yield return new WaitForSeconds(10f);
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
