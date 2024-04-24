using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public Tome Tome;
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
            Tome.Collider = collision;
            Tome.laserEnd = collision.transform.position;
            Destroy(gameObject);
        }   
    }

    public void EditTarget(Tome tome, Vector3 pos)
    {
        Tome = tome;
        Target = pos;
        Moving = true;
    }
}
