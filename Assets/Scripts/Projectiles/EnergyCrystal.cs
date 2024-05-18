using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCrystal : MonoBehaviour
{
    public int Damage;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged(Damage);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged(-Damage);
            Explosion();
        }
    }

    public void EditCrystal(int damage)
    {
        Damage = damage;
        GetComponent<EdgeCollider2D>().isTrigger = true;
    }

    public IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(10f);
        Explosion();
    }

    public void Explosion()
    {
        LayerMask Damageable = LayerMask.GetMask("Enemy");
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, 2f, Damageable);
        foreach (Collider2D collision in detectedObjects)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                    collision.gameObject.GetComponent<IDamageable>().Damaged(Damage);
                else
                    collision.gameObject.GetComponent<IDamageable>().Damaged(-Damage);
            }
        }
        Destroy(gameObject);
    }
}
