using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

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
    public bool isDashing = false;

    public void Awake()
    {
        PSC = GetComponent<PlayerSpriteController>();
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Arrows");
        Cooldowns = PlayerManager.instance.Cooldowns;
        Player = GetComponent<Player>();
    }

    private void Start()
    {
        string[] icons = { "Bow/Attack", "Bow/Ability1", "Bow/Ability2", "Bow/Ultimate", "Movement" };
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
    public void CreateGhost()
    {
        transform.GetChild(4).GetChild(3).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[0].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[1].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(1).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[2].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(2).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[3].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(3).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[4].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(4).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[5].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(5).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[6].GetComponent<SpriteRenderer>().sprite;
		transform.GetChild(4).GetChild(3).GetChild(6).GetComponent<SpriteRenderer>().sprite = PSC.Sprites[7].GetComponent<SpriteRenderer>().sprite;
        var clone = Instantiate(transform.GetChild(4).GetChild(3), transform.position, transform.rotation);
        clone.gameObject.SetActive(true);
		Destroy(clone.gameObject, 0.2f);
	}
    public IEnumerator GenerateGhost()
    {
        while(isDashing)
        {
            CreateGhost();
            yield return null;
        }
    }    
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
        isDashing = true;
        StartCoroutine(GenerateGhost());
        yield return new WaitForSeconds(0.25f);
        isDashing = false;
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
		PSC.Attack("DashBackShoot", 1);
		isDashing = true;
		StartCoroutine(GenerateGhost());
		PSC._rigidbody.velocity = new Vector2(-PSC.currentDirection.x * dashDistance, -PSC.currentDirection.y * dashDistance);
        yield return new WaitForSeconds(.2f);
        isDashing = false;
        PSC._rigidbody.velocity = Vector2.zero;
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

        PSC.Attack("ChargeArrow", 1);
		ParticleSystem particleSystem = PSC.Sprites[5].transform.GetChild(0).GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startRotation3D = true;

            float angle = Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg;
            switch (angle)
            {
                case float _ when angle > -11.25f && angle <= 11.25f:
                    PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(0f, 0f, 0f);
                    PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(0f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(80f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > 11.25f && angle <= 33.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(0f, 1f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					break;
                case float _ when angle > 33.75f && angle <= 56.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(0f, 1f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > 56.25f && angle <= 78.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(0f, 1f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					break;
                case float _ when angle > 78.75f && angle <= 101.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-1f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-1f, 1f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(80f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(0f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > 101.25f && angle <= 123.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, 1f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					break;
                case float _ when angle > 123.75f && angle <= 146.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, 1f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > 146.25f && angle <= 168.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, 1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, 1f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					break;
                case float _ when angle > 168.75f || angle <= -168.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2.5f, 0f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, 0f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(0f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(80f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > -168.75f && angle <= -146.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					break;
                case float _ when angle > -146.25f && angle <= -123.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > -123.75f && angle <= -101.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-2f, -0.5f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(135f * Mathf.Deg2Rad);
					break;
                case float _ when angle > -101.25f && angle <= -78.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-1.2f, -1f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-1f, -0.5f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(80f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(0f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > -78.75f && angle <= -56.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-0.5f, -0.5f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					break;
                case float _ when angle > -56.25f && angle <= -33.75f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-0.5f, -0.5f, 0f);
                    main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
                    main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
                    break;
                case float _ when angle > -33.75f && angle <= -11.25f:
					PSC.Sprites[5].transform.GetChild(1).transform.localPosition = new Vector3(-0.2f, -0.5f, 0f);
					PSC.Sprites[5].transform.GetChild(0).transform.localPosition = new Vector3(-0.5f, -0.5f, 0f);
					main.startRotationX = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					main.startRotationY = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);
					break;
            }
        }
		PSC.Sprites[5].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
		PSC.Sprites[5].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
		yield return new WaitForSeconds(1f);
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
        PSC.Attack("ShootUp", 1);
        yield return new WaitForSeconds(1f);
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
