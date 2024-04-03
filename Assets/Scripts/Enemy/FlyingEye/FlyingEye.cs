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
    public bool isMovable = true;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        if (currentHealth > 0 && isMovable)
            transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("Player1").transform.GetChild(0).transform.position, step);

        if (Buffs.Count > 0)
            HandleBuff();

    }

    //IEffectable Components
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();

    public void ApplyBuff(Buff buff)
    {
        if (Buffs.ContainsKey(buff.Source))
            Buffs.Remove(buff.Source);
        else
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

    //IDamageable Components
    public int Damaged(int amount)
    {
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

    public int Healed(int amount)
    {
        return 0;
    }

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
            StartCoroutine(Attack(collision));
        }
    }

    public IEnumerator Attack(Collision2D collision)
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(.75f);
        collision.gameObject.SendMessage("Damaged", 1);
        isAttacking = false;
    }
}
