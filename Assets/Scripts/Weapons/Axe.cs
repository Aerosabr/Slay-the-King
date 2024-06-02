using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public PlayerSpriteController PSC;
    public Player Player;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;

    public bool AttackCD = true;

    public float dashDistance = 15f;

    public void Awake()
    {
        PSC = gameObject.GetComponent<PlayerSpriteController>();
        Player = gameObject.GetComponent<Player>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Enemy");
        PSC.twoHanded = false;
    }

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        Vector2 temp = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        return temp;
    }

    #region Player Attack
    public void OnAttack()
    {
        if (!PSC.isAttacking && AttackCD && PSC.Movable)
        {
            AttackCD = false;
            PSC.isAttacking = true;
            PSC.currentDirection = MapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) - (transform.position - new Vector3(0.04f, 0.3f)), 1f);
            StartCoroutine(AttackCast());
        }
    }

    private IEnumerator AttackCast()
    {
        PSC.Attack("Chop", 2);
        Player.Cooldowns[0].SetActive(true);
        Player.Cooldowns[0].GetComponent<CooldownUI>().StartCooldown(1 / Player.attackSpeed);
        Attack();
        yield return new WaitForSeconds(.5f);
        PSC.isAttacking = false;
    }

    public void Attack()
    {
        attackHitBoxPos.localPosition = MapPoint(PSC.currentDirection, .5f);
        attackHitBoxPos.gameObject.GetComponent<CircleCollider2D>().radius = .5f;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, .5f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                if (collider.transform.position.x - gameObject.transform.position.x >= 0)
                    collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
                else
                    collider.gameObject.GetComponent<IDamageable>().Damaged(Player.Attack);
            }
        }
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
}
