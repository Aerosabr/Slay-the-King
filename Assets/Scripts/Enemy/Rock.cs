using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Rigidbody2D rb;

    // Reference to the Gold prefab you want to drop
    public GameObject goldPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Damaged(float amount)
    {
        currentHealth -= Mathf.Abs(amount);
        // Assuming DamagePopup.Create is a method you've defined elsewhere and works as intended
        DamagePopup.Create(rb.transform.position, (int)Mathf.Abs(amount), false);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DropGold();
        Destroy(gameObject); // This will remove the rock from the scene
    }

    private void DropGold()
    {
        if (goldPrefab != null)
        {
            // Instantiate the Gold at the rock's position and no rotation
            Instantiate(goldPrefab, transform.position, Quaternion.identity);
        }
    }
}
