using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatStage : MonoBehaviour
{
    public GameObject Rat;
    public float xPos;
    public float yPos;
    public Transform spawnArea;

    public void Awake()
    {
        StartCoroutine(SpawnRats());
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(xPos * 2, yPos * 2));
    }

    public IEnumerator SpawnRats()
    {
        for (int i = 0; i < 30; i++)
        {
            Instantiate(Rat, FindSpawnPos(), Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }    
    }

    public Vector2 FindSpawnPos()
    {
        return new Vector2(Random.Range(-xPos, xPos), Random.Range(-yPos, yPos));
    }
}
