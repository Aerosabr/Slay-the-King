using System.Collections;
using UnityEngine;

public class Rock : Entity, IDamageable
{
    public Rigidbody2D rb;

    public GameObject goldPrefab;

    private float explosionTimer = 10f;
    public float explosionRadius = 15f;
    private bool isExploded = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(StartExplosionCountdown());
    }

    private IEnumerator StartExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionTimer);

        if (!isExploded && currentHealth > 0)
        {
            Explode();
        }
    }

    //IDamageable Components
    public int Damaged(int amount)
    {
        int damage = (Mathf.Abs(amount) - Defense > 0) ? Mathf.Abs(amount) - Defense : 1;

        if (currentHealth - damage > 0)
            currentHealth -= damage;
        else
        {
            damage = currentHealth;
            currentHealth = 0;
        }

        if (currentHealth <= 0)
            Die();

        DamagePopup.Create(rb.transform.position, (int)Mathf.Abs(damage), false);
        return damage;
    }

    public int Healed(int amount)
    {
        return 0;
    }

    private void Die()
    {
        if (!isExploded)
        {
            DropGold();
        }
        Destroy(gameObject);
    }

    private void DropGold()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }

    private void Explode()
    {
        isExploded = true;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player playerHealth = hitCollider.GetComponent<Player>();
                if (playerHealth != null)
                {
                    playerHealth.Damaged(5);
                }
            }
        }

        Destroy(gameObject);
    }
}
