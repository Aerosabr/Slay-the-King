using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Class that represents each individual player's stats and inventory
    //Equipment
    public Helmet Helmet;
    public Chestplate Chestplate;
    public Leggings Leggings;
    public Boots Boots;
    public Weapon Weapon;

    //Player Class
    public string Class;

    //Stats
    public int maxHealth;
    public int currentHealth;
    public int Strength;
    public int MagicalPower;
    public int Armor;
    public int Resistance;
    public int Dexterity;

    //Ability Cooldowns
    public float AttackCD;
    public float Ability1CD;
    public float Ability2CD;
    public float UltimateCD;

    public GameObject HealthBar;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public Player(int health)
    {
        maxHealth = health;
    }

    public void Damaged(int damage) 
    {
        currentHealth -= damage;
        GameObject.Find("PlayerHealth").GetComponent<RectTransform>().sizeDelta = new Vector2(((float)currentHealth / (float)maxHealth) * 280, 70);
        DamagePopup.Create(rb.transform.position, damage, false);
    }
}
