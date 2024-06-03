using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBurst : MonoBehaviour
{
    public int Damage;
    public bool Active;

    private void FixedUpdate()
    {
        if (Active)
        {
            if (transform.localScale.x < 5)            
                transform.localScale += new Vector3(.1f, .1f);            
            else
                Destroy(gameObject);
        }   
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged(Damage, transform.position, 3);
        }
    }

    public void StartBurst(int damage)
    {
        Damage = damage;
        Active = true;
    }
}
