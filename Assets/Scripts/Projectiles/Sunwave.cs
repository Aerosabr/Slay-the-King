using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunwave : MonoBehaviour
{
    public float life;
    public float rotationSpeed = 15f;
    public float damage;
    public Tome Player;

    private void Update()
    {
        Quaternion newRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0) * transform.rotation;
        transform.rotation = newRotation;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage, transform.position, 3);

            collision.gameObject.GetComponent<IEffectable>().ApplyBuff(new IncreaseDamageTaken(20, 10f, "Tome - Ability 2", collision.gameObject));
        }
    }

    public void EditSunwave(float Life, float Damage, Tome player)
    {
        life = Life;
        damage = Damage;
        Player = player;
        Destroy(gameObject, life);
    }
}
