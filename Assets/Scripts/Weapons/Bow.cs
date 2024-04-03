using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowLife = 3f;
    public List<GameObject> Cooldowns = new List<GameObject>();
    public Transform dashEffect;

    public int passiveCounter = 0;
    public int damageMultiplier = 1;

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
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Arrows");
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
        if (passiveCounter == 5)
        {
            damageMultiplier = 2;
            passiveCounter = 0;
        }
        
        if (passiveCounter == 1)
            damageMultiplier = 1;

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
        dashEffect.gameObject.SetActive(true);
        StartCoroutine(DashEffect(PSC));
        yield return new WaitForSeconds(0.25f);
        Cooldowns[4].SetActive(true);
        MovementCD = baseMovementCD;
        dashEffect.gameObject.SetActive(false);
        PSC.Movable = true;
    }

    public IEnumerator DashEffect(PlayerSpriteController PSC)
    {
        while(!PSC.Movable)
        {
            foreach(Animator sprite in PSC.Sprites)
            {
                GameObject character = new GameObject("Character");

                SpriteRenderer originalSpriteRenderer = sprite.GetComponent<SpriteRenderer>();

                // Add a new SpriteRenderer component to the new GameObject
                SpriteRenderer characterSpriteRenderer = character.AddComponent<SpriteRenderer>();

                // Copy properties from the original SpriteRenderer to the new one
                characterSpriteRenderer.sprite = originalSpriteRenderer.sprite;
                Color newColor = originalSpriteRenderer.color;
                newColor.a = 155f / 255f; // Set alpha value to approximately 0.6078
                characterSpriteRenderer.color = newColor;
                characterSpriteRenderer.flipX = originalSpriteRenderer.flipX;
                characterSpriteRenderer.flipY = originalSpriteRenderer.flipY;
                characterSpriteRenderer.material = originalSpriteRenderer.material;
                characterSpriteRenderer.sortingLayerID = originalSpriteRenderer.sortingLayerID;
                characterSpriteRenderer.sortingOrder = originalSpriteRenderer.sortingOrder;

                characterSpriteRenderer.sprite = sprite.transform.GetComponent<SpriteRenderer>().sprite;
                character.AddComponent<Ghost>();
                // Set the position and rotation of the new GameObject
                character.transform.position = sprite.transform.position;
                character.transform.rotation = sprite.transform.rotation;
                character.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                
            }
            yield return null;
        }
    }
    
    public float GetMovementCooldown()
    {
        return baseMovementCD;
    }

    public void ResetMovementCooldown()
    {
        MovementCD = 0;
    }

    //Arrow direction
    public float circleRadius = 1f; // Radius of the circle

    public Vector2 MapPoint(Vector2 point)
    {
        float angle = Mathf.Atan2(point.y, point.x);
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
        passiveCounter++;
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
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg + 180);
        arrow.GetComponent<Arrow>().EditArrow(3f, 5f * damageMultiplier, true);
        arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(PSC.currentDirection);
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
        passiveCounter++;
        PSC._rigidbody.velocity = new Vector2(-PSC.currentDirection.x * dashDistance * 4, -PSC.currentDirection.y * dashDistance * 4);
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
            arrowLife = 3f;
            GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
            arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - (-i * 10 - 180));
            float currentAngleRadians = ((Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg) + 10 * i) * Mathf.Deg2Rad;
            Vector2 currentVector = new Vector2(Mathf.Cos(currentAngleRadians), Mathf.Sin(currentAngleRadians)) * PSC.currentDirection.magnitude;
            arrow.GetComponent<Arrow>().EditArrow(3f, 10f * damageMultiplier, true);
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
        if (!PSC.isAttacking && Ability2CD == 0 && PSC.Movable)
        {
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(Ability2Cast());
        }
    }

    private IEnumerator Ability2Cast()
    {
        passiveCounter++;
        PSC.Attack("Shoot", 16);
        yield return new WaitForSeconds(.2f);
        Ability2();
        Cooldowns[2].SetActive(true);
        Ability2CD = baseAbility2CD; 
        PSC.isAttacking = false;
    }

    public void Ability2()
    {
        GameObject player = GameObject.Find("Player1").transform.GetChild(0).gameObject;
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg + 180);
        arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * MapPoint(PSC.currentDirection);
        arrow.GetComponent<Arrow>().EditArrow(3f, 10f * damageMultiplier, false);
        arrow.transform.localScale = new Vector3(.14f, .2f, .2f);
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mousePosition.y += 5;
            PSC.isAttacking = true;
            PSC.currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            StartCoroutine(UltimateCast(mousePosition));
        }
    }

    private IEnumerator UltimateCast(Vector3 mousePosition)
    {
        passiveCounter++;
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
        
        for (int i = 0; i < 40; i++)
        {
            temp.x = mousePosition.x + Random.Range(-1.6f, 1.6f);
            GameObject arrow = Instantiate(arrowPrefab, temp, Quaternion.identity);
            arrow.GetComponent<Arrow>().EditArrow(0.6f, 10f * damageMultiplier, false);
            arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * Vector2.down;
            yield return new WaitForSeconds(.05f);
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
