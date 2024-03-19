using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ranger : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float arrowLife = 3f;
    public void Awake()
    {
        arrowPrefab = Resources.Load<GameObject>("Prefabs/Arrow");
    }

    //Player Attack
    public void OnAttack()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking)
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
        Attack(PSC);
        PSC.isAttacking = false;
    }

    public void Attack(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        rigidbody.velocity = arrowSpeed * PSC.currentDirection;
    }

    //Player Ability1
    public void OnAbility1()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking)
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
            arrow.GetComponent<Rigidbody2D>().velocity = arrowSpeed * currentVector;
        }
    }

    //Player Ability2
    public void OnAbility2()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking)
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
        
        PSC.isAttacking = false;
    }

    public void Ability2(PlayerSpriteController PSC)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        arrowLife = 3f;
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, player.transform.rotation);
        Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(PSC.currentDirection.y, PSC.currentDirection.x) * Mathf.Rad2Deg - 90);
        rigidbody.velocity = arrowSpeed * PSC.currentDirection;
    }

    //Player Ultimate
    public void OnUltimate()
    {
        PlayerSpriteController PSC = gameObject.GetComponent<PlayerSpriteController>();
        if (!PSC.isAttacking)
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
}
