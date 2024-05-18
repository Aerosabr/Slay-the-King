using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slashwave : MonoBehaviour
{
    public float life;
    public float rotationSpeed = 15f;
    public float damage;

    private void Update()
    {
        Quaternion newRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0) * transform.rotation;
        transform.rotation = newRotation;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.position.x - gameObject.transform.position.x >= 0)
            collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage);
        else
            collision.gameObject.GetComponent<IDamageable>().Damaged((int)-damage);
    }

    public void EditSlashwave(float Life, float Damage)
    {
        life = Life;
        damage = Damage;
        Destroy(gameObject, life);
    }
}
