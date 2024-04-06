using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //Stats
    //Base is internal values and used for buffs/debuffs
    public int baseMaxHealth;
    public float healthMultiplier = 1.0f;
    public int maxHealth;
    public int currentHealth;

    public int baseAttack;
    public float attackMultiplier = 1.0f;
    public int Attack;

    public int baseDefense;
    public float defenseMultiplier = 1.0f;
    public int Defense;

    public int baseDexterity;
    public float dexterityMultiplier = 1.0f;
    public int Dexterity;

    public float baseAttackSpeed;
    public float asMultiplier = 1.0f;
    public float attackSpeed;

    public float baseMovementSpeed;
    public float msMultiplier = 1.0f;
    public float movementSpeed;

    public float CDR;
    public int Luck;

    public bool isStunned;

    public void changeHealth(int flat, float percentage)
    {
        baseMaxHealth += flat;
        healthMultiplier += percentage;
        maxHealth = (int)(baseMaxHealth * healthMultiplier);
    }

    public void changeAttack(int flat, float percentage)
    {
        baseAttack += flat;
        attackMultiplier += percentage;
        Attack = (int)(baseAttack * attackMultiplier);
    }

    public void changeDefense(int flat, float percentage)
    {
        baseDefense += flat;
        defenseMultiplier += percentage;
        Defense = (int)(baseDefense * defenseMultiplier);
    }

    public void changeDexterity(int flat, float percentage)
    {
        baseDexterity += flat;
        dexterityMultiplier += percentage;
        Dexterity = (int)(baseDexterity * dexterityMultiplier);
    }

    public void changeMovementSpeed(int flat, float percentage)
    {
        baseMovementSpeed += flat;
        msMultiplier += percentage;
        movementSpeed = baseMovementSpeed * msMultiplier;
    }

    public void changeAttackSpeed(int flat, float percentage)
    {
        baseAttackSpeed += flat;
        asMultiplier += percentage;
        attackSpeed = baseAttackSpeed * asMultiplier;
    }
}
