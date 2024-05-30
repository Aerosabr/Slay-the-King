using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity, IDamageable
{
    public Rigidbody2D rb;
    public Animator anim;

    #region IDamageable Components
    public int Damaged(int amount)
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
        anim.Play("Hit");
        DamagePopup.Create(rb.transform.position, Mathf.Abs(damage), false);

        if (currentHealth <= 0)
        {
            anim.SetBool("Felled", true);
            TreeStage.instance.treeFelled();
        }

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

    public int Healed(int amount)
    {
        return 0;
    }
    #endregion
}
