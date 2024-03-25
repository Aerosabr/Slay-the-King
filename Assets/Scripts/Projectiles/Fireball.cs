using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float life = 3f;

    void Awake()
    {
        //life = GameObject.FindGameObjectWithTag("Player").GetComponent<Mage>().fireballLife;
        Destroy(gameObject, life);
    }

    private void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.SendMessage("Damaged", 5);
            else
                collision.gameObject.SendMessage("Damaged", -5);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Environment")
            Destroy(gameObject);
        /*
         * if (collision.gameObject.tag == "Enemy")
            Destroy(collision.gameObject);
         */

    }
}
