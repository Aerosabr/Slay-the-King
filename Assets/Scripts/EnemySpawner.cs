using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnArea;
    public float spawnRadius;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnRadius, spawnRadius));
    }

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
        Vector3 SpawnArea = new Vector3(Random.Range(-spawnRadius / 2, spawnRadius / 2), Random.Range(-spawnRadius / 2, spawnRadius / 2));
        Instantiate(Enemy, SpawnArea, Quaternion.identity);
        StartCoroutine(Spawning(Random.Range(2f, 3f)));
    }
}
 