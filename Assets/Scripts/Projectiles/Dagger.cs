using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    public float life;
    public float rotationSpeed = 15f;
    public float damage;
    public bool isDebuff = false;

    void Awake()
    {
        Destroy(gameObject, life);
    }

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

            if (isDebuff)
            {
                //collision.gameObject.GetComponent<IEffectable>().ApplyBuff(new IncreaseDamageTaken(20, 10f, "Throwing Knife - Ability 1", collision.gameObject));
                Daggers player = GameObject.Find("Player1").transform.GetChild(0).GetComponent<Daggers>();
                player.Ability1TargetHit(collision.gameObject);
            }
        }

        Destroy(gameObject);
    }

    public void EditDagger(float Life, float Damage, bool debuff)
    {
        life = Life;
        damage = Damage;
        isDebuff = debuff;
        Destroy(gameObject, life);
    }
}
