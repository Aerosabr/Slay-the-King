using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    public float speed;
    public float range;
    public float xPos;
    public float yPos;
    public Vector2 point;
    public Animator anim;

    void Awake()
    {
        setDestination();
    }

    void FixedUpdate()
    {
        if (transform.position.x - point.x > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        transform.position = Vector2.MoveTowards(transform.position, point, speed * Time.deltaTime);
        anim.Play("Walk");
        if (Vector2.Distance(transform.position, point) < range)
            setDestination();
    }

    public void setDestination()
    {
        point = new Vector2(Random.Range(-xPos, xPos), Random.Range(-yPos, yPos));
    }

    public void Caught()
    {
        RatStage.instance.ratsCaught++;
        Destroy(gameObject);
    }
}
