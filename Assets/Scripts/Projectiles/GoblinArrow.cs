using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : MonoBehaviour
{
    public float life = 3f;
    public float rotationSpeed = 15f;
    public float damage = 5f;
    public bool destroyOnCollision = true;

    void Awake()
    {
        Destroy(gameObject, life);
    }

    private void Update()
    {
        Quaternion newRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0) * transform.rotation;

        // Apply the new rotation to the object
        transform.rotation = newRotation;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage, transform.position, 1);

            if (destroyOnCollision)
                Destroy(gameObject);
        }
    }

    public void EditArrow(float Life, float Damage, bool destroy)
    {
        life = Life;
        damage = Damage;
        destroyOnCollision = destroy;
        Destroy(gameObject, life);
    }
}
