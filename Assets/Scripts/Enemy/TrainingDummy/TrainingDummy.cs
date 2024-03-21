using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrainingDummy : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Rigidbody2D rb;
    public Animator anim;

    /*
    public float strength;
    public float delay;
    public UnityEvent OnBegin, OnDone;
    */
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Damaged(float amount)
    {
        currentHealth -= Mathf.Abs(amount);
        if (amount > 0)
            anim.SetTrigger("DamagedLeft");
        else
            anim.SetTrigger("DamagedRight");

        DamagePopup.Create(rb.transform.position, (int)Mathf.Abs(amount), false);
    }
}
