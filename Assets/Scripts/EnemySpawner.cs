using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private void Awake()
    {
        SpawnEnemy(Resources.Load<GameObject>("Prefabs/FlyingEye"));
    }

    public IEnumerator Spawning(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemy(Resources.Load<GameObject>("Prefabs/FlyingEye"));
    }

    public void SpawnEnemy(GameObject Enemy)
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
        StartCoroutine(Spawning(Random.Range(50f, 50f)));
    }
}
