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
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)damage);
            else
                collision.gameObject.GetComponent<IDamageable>().Damaged((int)-damage);

            //collision.gameObject.GetComponent<IEffectable>()
            //    .ApplyBuff(new IncreaseDamageTaken(20, 10f, "Throwing Knife - Ability 1", collision.gameObject));
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
