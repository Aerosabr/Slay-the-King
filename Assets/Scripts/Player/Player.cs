using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        HealthBar = GameObject.Find("PlayerHealth").transform.GetChild(0).gameObject;
        currentHealth = maxHealth;
    }

    public Player(int health)
    {
        maxHealth = health;
    }

    public void Damaged(int damage) 
    {
        currentHealth -= damage;
        HealthBar.GetComponent<Slider>().value = (float)currentHealth / (float)maxHealth;
        DamagePopup.Create(rb.transform.position, damage, false);
    }
}
