using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheDomain : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 10f);
    }

    private void FixedUpdate()
    {
        if (transform.localScale.x < 10)
        {
            transform.localScale += new Vector3(.2f, .2f);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<IEffectable>().ApplyBuff(new ScytheDomainBuff(5, 50, .5f, 1f, "Scythe - Ultimate", collision.gameObject));
        }
    }
}
