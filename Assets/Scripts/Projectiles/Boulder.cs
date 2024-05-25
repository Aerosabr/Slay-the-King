using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    public Animator anim;
    public float yPos;

    public void Awake()
    {
        yPos = transform.position.y;
        transform.position = new Vector2(transform.position.x, transform.position.y + 10);
    }

    private void FixedUpdate()
    {
        if (transform.position.y > yPos)
            transform.position += new Vector3(0, -0.3f);
        else
            anim.Play("Breaking");
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
