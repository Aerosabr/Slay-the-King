using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public int Health;
    public int Strength;
    public int MagicalPower;
    public int Armor;
    public int Resistance;
    public int Dexterity;
}
