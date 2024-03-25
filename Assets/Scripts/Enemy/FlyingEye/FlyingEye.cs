using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Animator anim;
    public Rigidbody2D rb;
    public float speed = 1.0f;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        if (currentHealth > 0)
            transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("Player1").transform.GetChild(0).transform.position, step);
    }

    public void Damaged(int amount)
    {
        currentHealth -= Mathf.Abs(amount);
        if (currentHealth <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(rb);
            Destroy(GetComponent<BoxCollider2D>());
            StartCoroutine(Death(2f));
        }
        else
            anim.SetTrigger("Damaged");
            
        DamagePopup.Create(rb.transform.position, Mathf.Abs(amount), false);
    }

    public IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Gold"), transform.position, Quaternion.identity);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("Damaged", 1);
        }
    }
}
