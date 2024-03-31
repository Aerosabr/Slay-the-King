using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
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

    public void Damaged(float amount)
    {
        if (isExploded) return;

        currentHealth -= Mathf.Abs(amount);
        DamagePopup.Create(rb.transform.position, (int)Mathf.Abs(amount), false);

        if (currentHealth <= 0)
        {
            Die();
        }
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
