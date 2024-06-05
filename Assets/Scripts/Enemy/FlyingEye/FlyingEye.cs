using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlyingEye : Entity, IDamageable, IEffectable
{
    public Animator anim;
    public Rigidbody2D rb;
    public float speed = 1.0f;
    public bool isAttacking;
    public GameObject player;
    public EnemySpriteController ESC;

    void Awake()
    {
        player = GameObject.Find("PlayerManager").transform.GetChild(0).transform.GetChild(0).gameObject;
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ESC = GetComponent<EnemySpriteController>();
    }

    void Update()
    {
        if (!isStunned)
        {
            float step = speed * Time.deltaTime;
            if (currentHealth > 0 && isMovable)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
                Debug.Log(Vector2.SignedAngle(transform.position, player.transform.position));
                

            }
        }

        if (Buffs.Count > 0)
            HandleBuff();
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
    public int Damaged(int amount, Vector3 origin, float kb)
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

        if (kbResistance < kb)
            StartCoroutine(KnockCoroutine(origin, kb - kbResistance));

        if (currentHealth <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(rb);
            Destroy(GetComponent<BoxCollider2D>());
            StartCoroutine(Death(2f));
        }
        else
            anim.SetTrigger("Damaged");
            
        DamagePopup.Create(rb.transform.position, Mathf.Abs(damage), false);
        return damage;
    }

    public int trueDamaged(int amount)
    {
        int damage;
        if (amount > currentHealth)
        {
            damage = currentHealth;
            currentHealth = 0;
        }
        else
        {
            damage = currentHealth - amount;
            currentHealth -= amount;
        }

        return damage;
    }

    public IEnumerator KnockCoroutine(Vector3 origin, float kb)
    {
        Vector2 force = (transform.position - origin).normalized * kb;
        isMovable = false;
        rb.velocity = force;
        yield return new WaitForSeconds(.3f);
        isMovable = true;
        rb.velocity = new Vector2();
    }

    public int Healed(int amount)
    {
        return 0;
    }
    #endregion

    public IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Gold"), transform.position, Quaternion.identity);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isMovable = false;   
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isMovable = true;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isAttacking)
        {
            StartCoroutine(Attacking(collision));
        }
    }

    public IEnumerator Attacking(Collision2D collision)
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(.75f);
        collision.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 0);
        isAttacking = false;
    }
}
