using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatStage : MonoBehaviour
{
    public GameObject Rat;
    public float xPos;
    public float yPos;
    public Transform spawnArea;
    public int captures;
    public Text ratText;
    public Slider starRating;

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

    public void UpdateRatUI()
    {
        ratText.text = "Rats caught: " + captures.ToString();
        if (captures == 24)
            starRating.value = 1f;
        else if (captures == 16)
            starRating.value = 0.7f;
        else if (captures == 8)
            starRating.value = 0.4f;
    }
}
