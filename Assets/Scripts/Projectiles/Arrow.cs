using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float life = 3f;

    void Awake()
    {
        life = GameObject.FindGameObjectWithTag("Player").GetComponent<Ranger>().arrowLife;
        Destroy(gameObject, life);
    }
}
