using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
public class TrainingDummy : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public Animator anim;

    /*
    public float strength;
    public float delay;
    public UnityEvent OnBegin, OnDone;
    */
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Buffs.Count > 0)
            HandleBuff();
    }

    //IDamageable Components
    public int Damaged(int amount)
    {
        int damage = (Mathf.Abs(amount) - Defense > 0) ? Mathf.Abs(amount) - Defense : 1;

        if (currentHealth - damage > 0)
            currentHealth -= damage;
        else
        {
            damage = currentHealth;
            currentHealth = 0;
        }

        if (amount > 0)
            anim.SetTrigger("DamagedLeft");
        else
            anim.SetTrigger("DamagedRight");

        DamagePopup.Create(rb.transform.position, (int)Mathf.Abs(damage), false);
        return damage;
    }

    public int Healed(int amount)
    {
        return 0;
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
            if(Buffs[key].HandleEffect())
                RemoveBuff(Buffs[key]);
        }
    }
}
