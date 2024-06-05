using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneVortex : MonoBehaviour
{
    public List<GameObject> Enemies = new List<GameObject>();
    public int Damage;

    void FixedUpdate()
    {
        transform.Rotate(0, 0, -1);
        foreach (GameObject enemy in Enemies)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, transform.position, .75f * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered " + collision.name);
        Enemies.Add(collision.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exited " + collision.name);
        Enemies.Remove(collision.gameObject);
    }

    public IEnumerator DamageEnemies()
    {
        foreach(GameObject enemy in Enemies)
        {
            if (enemy.tag == "Enemy")
                enemy.GetComponent<IDamageable>().Damaged(Damage, transform.position, 5);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(DamageEnemies());
    }

    public void StartVortex(int damage, float duration)
    {
        Damage = damage;
        Destroy(gameObject, duration);
        StartCoroutine(DamageEnemies());
    }
}
