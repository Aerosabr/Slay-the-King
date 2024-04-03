using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public float life = 2f;
    public float rotationSpeed = 15f;
    public float damage = 5f;
    public bool trackingEnemy = false;
    public GameObject trackedEnemy;
    public bool isDebuff = false;

    void Awake()
    {
        Destroy(gameObject, life);
    }

    private void Update()
    {
        Quaternion newRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0) * transform.rotation;
        // Apply the new rotation to the object
        transform.rotation = newRotation;
        if (trackingEnemy)
            transform.position = Vector2.MoveTowards(transform.position, trackedEnemy.transform.position, 10f * Time.deltaTime);
            
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
            {
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage);
            }
            else
            {
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)-damage);
            }
            if (isDebuff)
                collision.gameObject.GetComponent<IEffectable>().ApplyBuff(new IncreaseDamageTaken(20, 10f, "Throwing Knife - Ability 1", collision.gameObject));
        }

        Destroy(gameObject);
    }

    public void EditKnife(float Life, float Damage, bool tracking, bool debuff)
    {
        life = Life;
        damage = Damage;
        trackingEnemy = tracking;
        isDebuff = debuff;
        if (tracking)
        {
            FindEnemy();
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        Destroy(gameObject, life);
    }

    public void FindEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float oldDistance = 999999;
        foreach (GameObject g in Enemies)
        {
            float dist = Vector3.Distance(gameObject.transform.position, g.transform.position);
            if (dist < oldDistance && g.GetComponent<BoxCollider2D>() != null)
            {
                trackedEnemy = g;
                oldDistance = dist;
            }
        }
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(trackedEnemy.transform.position.y - gameObject.transform.position.y, trackedEnemy.transform.position.x - gameObject.transform.position.x) - 90);
    }
}
