using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

}
