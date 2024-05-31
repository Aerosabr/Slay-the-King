using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int Damaged(int damage, Vector3 origin, float kb);
    public int trueDamaged(int damage);
    public int Healed(int amount);
}
