using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float life = 3f;
    public int Damage;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged(Damage);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged(-Damage);
            Destroy(gameObject);
        }
    }

    public void StartFireball(int damage)
    {
        Damage = damage;
        Destroy(gameObject, life);
    }
}
