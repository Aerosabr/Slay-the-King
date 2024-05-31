using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneSphere : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public int Damage;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged(Damage, transform.position, 3);
        }
    }

    public void StartSphere(int damage, float life)
    {
        Damage = damage;
        Destroy(gameObject, life);
    }
}
