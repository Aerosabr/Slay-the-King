using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinKingCollider : MonoBehaviour
{
    public GoblinKing GK;

    private void Awake()
    {
        GK = transform.parent.gameObject.GetComponent<GoblinKing>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && GK.Charging)
        {
            GK.EnemyCharged(collision);
        }
    }
}
