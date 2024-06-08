using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    public Animator anim;
    public Transform attackHitbox;
    public LayerMask Damageable;
    public float yPos;

    public void Awake()
    {
        yPos = transform.position.y;
        transform.position = new Vector2(transform.position.x, transform.position.y + 10);
        Damageable = LayerMask.GetMask("Player");
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

    public void Contact()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitbox.position, 2f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                collider.gameObject.GetComponent<IDamageable>().trueDamaged(1);
        }
    }
}
