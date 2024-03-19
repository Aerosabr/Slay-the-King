using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Entity
{
    //Class that represents each individual player's stats and inventory
    //Equipment
    public Helmet Helmet;
    public Chestplate Chestplate;
    public Leggings Leggings;
    public Boots Boots;
    public Weapon Weapon;

    //Player Class
    public Class Class;

    //Stats
    public int Health;
    public int Strength;
    public int MagicalPower;
    public int Armor;
    public int Resistance;
    public int Dexterity;
    
    public Player(int health)
    {
        Health = health;
    }

    public void OnAttack(InputValue inputValue)
    {

    }

    public void Damaged(int damage) 
    { 
    
    }
}
