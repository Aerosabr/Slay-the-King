using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float life = 3f;

    void Awake()
    {
        life = GameObject.FindGameObjectWithTag("Player").GetComponent<Ranger>().arrowLife;
        Destroy(gameObject, life);
    }

    private void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if(collision.transform.position.x - gameObject.transform.position.x >= 0)
                collision.gameObject.SendMessage("Damaged", 5);
            else
                collision.gameObject.SendMessage("Damaged", -5);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Wall")
            Destroy(gameObject);
        /*
         * if (collision.gameObject.tag == "Enemy")
            Destroy(collision.gameObject);
         */

    }
}
