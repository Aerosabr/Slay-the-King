using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownAxe : MonoBehaviour
{
    public float life;
    public float rotationSpeed = 15f;
    public float damage;
    public DualAxes Player;
    public Vector2 position;
    public bool isActive;

    private void Update()
    {
        if (Vector2.Distance(transform.position, position) > 5 && isActive)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(GetComponent<EdgeCollider2D>());
        }
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)-damage);
        }
    }

    public void EditAxe(Vector2 pos, float Life, float Damage, DualAxes player)
    {
        life = Life;
        damage = Damage;
        Player = player;
        position = pos;
        isActive = true;
        Destroy(gameObject, life);
    }
}
