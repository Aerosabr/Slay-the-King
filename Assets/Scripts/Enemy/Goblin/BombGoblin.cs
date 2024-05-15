using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BombGoblin : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 4.0f;
    public int Cost;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public bool Attackable;
    public LayerMask Damageable;

    void Awake()
    {
        player = GameObject.Find("PlayerManager").transform.GetChild(0).transform.GetChild(0).gameObject;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        ESC = GetComponent<EnemySpriteController>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (!isStunned && currentHealth > 0)
        {
            if (currentHealth > 0 && isMovable)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                ProcessDirection(player.transform.position);
                ESC.isMoving = true;
                ESC.isAttacking = false;
                ESC.PlayAnimation("Explosive");
            }
            else if (Attackable && !ESC.isAttacking)
            {
                ESC.isMoving = false;
                Attacking();
            }
        }
        else if (isStunned)
            ESC.PlayAnimation("Idle");

        if (Buffs.Count > 0)
            HandleBuff();
    }

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
        ESC.currentDir = new Vector2(Mathf.Cos(angle) * 1f, Mathf.Sin(angle) * 1f);
    }

    #region IEffectable Components
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();

    public void ApplyBuff(Buff buff)
    {
        if (Buffs.ContainsKey(buff.Source))
            RemoveBuff(Buffs[buff.Source]);

        Buffs.Add(buff.Source, buff);
        Buffs[buff.Source].ApplyEffect();
    }

    public void RemoveBuff(Buff buff)
    {
        buff.RemoveEffect();
        Buffs.Remove(buff.Source);
    }

    public void HandleBuff()
    {
        List<string> keys = Buffs.Keys.ToList();
        foreach (string key in keys)
        {
            if (Buffs[key].HandleEffect())
                RemoveBuff(Buffs[key]);
        }
    }
    #endregion

    #region IDamageable Components
    public int Damaged(int amount)
    {
        amount = Mathf.Abs(amount);
        int damage = (amount - Defense > 0) ? amount - Defense : 1;

        if (currentHealth - damage > 0)
            currentHealth -= damage;
        else
        {
            damage = currentHealth;
            currentHealth = 0;
        }

        if (currentHealth <= 0)
        {
            ESC.PlayAnimation("Death");
            Destroy(rb);
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponent<CircleCollider2D>());
            StartCoroutine(Death(2f));
        }
        else
            //anim.SetTrigger("Damaged");

            DamagePopup.Create(rb.transform.position, Mathf.Abs(damage), false);
        return damage;
    }

    public int Healed(int amount)
    {
        return 0;
    }

    public IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Gold"), transform.position, Quaternion.identity);
        //EnemySpawner.instance.enemiesKilled++;
    }
    #endregion

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.transform.position.x - transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged(Attack);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
            Destroy(gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isMovable = true;
            Attackable = false;
        }
    }

    public void Attacking()
    {
        ESC.isAttacking = true;
        ESC.PlayAnimation("Shoot");
    }

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public IEnumerator BasicAttack()
    {
        yield return new WaitForSeconds(.1f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        ESC.isAttacking = false;
    }
}
