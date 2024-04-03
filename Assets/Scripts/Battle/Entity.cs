using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //Stats
    //Base is internal values and used for buffs/debuffs
    public int baseMaxHealth;
    public int maxHealth;
    public int currentHealth;

    public int baseAttack;
    public int Attack;

    public int baseDefense;
    public int Defense;

    public int baseDexterity;
    public int Dexterity;

    public float baseCDR;
    public float CDR;

    public float baseAttackSpeed;
    public float attackSpeed;

    public int baseLuck;
    public int Luck;

    public float baseMovementSpeed;
    public float movementSpeed;

    public bool isStunned;
}
