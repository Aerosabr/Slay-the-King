using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string Name;
    public int maxHealth;
    public int currentHealth;
    public float damageAmp = 1f;
    public abstract int Damaged(int amount);

}
