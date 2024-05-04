using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandLaser : MonoBehaviour
{
    public LineRenderer line;
    public EdgeCollider2D edgeCollider;
    public bool Firing;
    public int Damage;
    public List<GameObject> hitEnemies = new List<GameObject>();

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        line.positionCount = 2;
        line.useWorldSpace = true;
    }

    private void FixedUpdate()
    {
        if (Firing)
        {
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SetLaserCollider();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            hitEnemies.Add(collision.gameObject);    
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            hitEnemies.Remove(collision.gameObject);
    }

    public IEnumerator DamageEnemies()
    {
        for (int i = 0; i < hitEnemies.Count; i++)
            hitEnemies[i].GetComponent<IDamageable>().Damaged(Damage);
        
        yield return new WaitForSeconds(.25f);
        StartCoroutine(DamageEnemies());
    }

    public void StartLaser(Vector3 pos, int damage)
    {
        line.SetPosition(0, pos);
        Firing = true;
        Damage = damage;
        Destroy(gameObject, 10f);
        StartCoroutine(DamageEnemies());
    }

    public void SetLaserCollider()
    {
        List<Vector2> edges = new List<Vector2>();
        edges.Add(Vector2.zero);
        edges.Add(line.transform.InverseTransformPoint(line.GetPosition(1)));

        edgeCollider.SetPoints(edges);
    }
}
