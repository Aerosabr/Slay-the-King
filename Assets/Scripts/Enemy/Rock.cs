using System.Collections;
using UnityEngine;

public class Rock : Entity, IDamageable
{
    public Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        loadStats();
    }

    private void loadStats()
    {
        int multiplier = GameManager.instance.Floor;
        baseMaxHealth *= multiplier;
        maxHealth *= multiplier;
        currentHealth *= multiplier;
    }

    #region IDamageable Components
    public int Damaged(int amount, Vector3 origin, float kb)
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

        DamagePopup.Create(transform.position, Mathf.Abs(damage), false);

        if (currentHealth <= 0)
        {
            anim.Play("Broken");
            MiningStage.instance.rocksBroken++;
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

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
