using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockBolt : MonoBehaviour
{
    public int damage;
    public int ricochetCount;
    public GameObject trackedEnemy;
    public bool trackingEnemy;
    public List<GameObject> hitEnemies = new List<GameObject>();
    void FixedUpdate()
    {
        if (trackingEnemy)
            transform.position = Vector2.MoveTowards(transform.position, trackedEnemy.transform.position, 15f * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!hitEnemies.Contains(collision.gameObject))
            {
                collision.gameObject.GetComponent<IDamageable>().Damaged(damage, transform.position, 0);
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                hitEnemies.Add(collision.gameObject);
                ricochetCount++;
            }
            if (ricochetCount == 4)
                Destroy(gameObject);
            else
                FindEnemy();
        }
    }

    public void EditBolt(float life, int Damage)
    {
        damage = Damage;
        Destroy(gameObject, life);
    }

    public void FindEnemy()
    {
        trackedEnemy = null;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float oldDistance = 999999;
        foreach (GameObject g in Enemies)
        {
            if (!hitEnemies.Contains(g))
            {
                float dist = Vector3.Distance(gameObject.transform.position, g.transform.position);
                if (dist < oldDistance && g.GetComponent<BoxCollider2D>() != null)
                {
                    trackedEnemy = g;
                    oldDistance = dist;
                }
            }
        }
        if (trackedEnemy == null)
            Destroy(gameObject);
        trackingEnemy = true;
    }
}
