using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDome : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 10f);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Projectile")
        {
            Destroy(collider.gameObject);
        }
    }
}
