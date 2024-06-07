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
            Player player = collider.gameObject.GetComponent<Player>();
            int damage;
            if (player.baseDefense > 9)
            {
                damage = (player.maxHealth / (player.baseDefense / 5)) + 1;
                collider.gameObject.GetComponent<IDamageable>().trueDamaged(damage);
            }
            else
                collider.gameObject.GetComponent<IDamageable>().trueDamaged(player.maxHealth);
        }
    }
}
