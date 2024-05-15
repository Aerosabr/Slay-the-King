using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyLight : MonoBehaviour
{
    public LayerMask Damageable;

    public void DamageEnemy(int damage)
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.2f, Damageable);
        
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                collider.gameObject.GetComponent<IDamageable>().Damaged(damage);
                collider.gameObject.GetComponent<IEffectable>().ApplyBuff(new IncreaseAttack(0, -.25f, 5f, "Tome - Ability1", collider.gameObject));
            }
        }
    }

    public void EditHolyLight(int damage)
    {
        Destroy(gameObject, 1.5f);
        DamageEnemy(damage);
    }
}
