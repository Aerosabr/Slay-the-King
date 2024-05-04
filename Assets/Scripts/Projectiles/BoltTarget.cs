using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltTarget : MonoBehaviour
{
    public Wand Wand;
    public Vector3 Target;
    public bool Moving = false;

    public void FixedUpdate()
    {
        if (Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target, 50f * Time.deltaTime);
            if (transform.position == Target)
                Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Wand.Collider = collision;
            Wand.boltEnd = collision.transform.position;
            Destroy(gameObject);
        }
    }

    public void EditTarget(Wand wand, Vector3 pos)
    {
        Wand = wand;
        Target = pos;
        Moving = true;
    }
}
