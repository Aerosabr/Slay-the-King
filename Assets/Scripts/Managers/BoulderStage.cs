using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoulderStage : MonoBehaviour
{
    public static BoulderStage instance;
    public Transform spawnArea;
    public float spawnX;
    public float spawnY;
    public GameObject Boulder;
    public GameObject TargetCircle;

    public float Timer;
    public float timeInterval = 10f;

    public bool Spawning;
    public float spawnDelay = 0.5f;

    public Text timerText;
    public Slider starRating;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnArea.position, new Vector3(spawnX, spawnY));
    }

    private void Awake()
    {
        instance = this;
        Spawning = true;
        StartCoroutine(SpawnBoulders());
        foreach(GameObject player in PlayerManager.instance.Players)
        {
            player.transform.position = Vector2.zero;
        }
    }

    public void FixedUpdate()
    {
        Timer += Time.deltaTime;
		UpdateScoreBoard(Timer);
		if (Timer > timeInterval && spawnDelay > 0.15f)
        {
            timeInterval += 10f;
            spawnDelay -= 0.1f;
        }
        
    }

    public IEnumerator SpawnBoulders()
    {
        while (Spawning)
        {
            StartCoroutine(Spawn());
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public IEnumerator Spawn()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-spawnX / 2, spawnX / 2), Random.Range(-spawnY / 2, spawnY / 2));
        GameObject tc = Instantiate(TargetCircle, spawnPos, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(1f, 1f);
        yield return new WaitForSeconds(1f);
        GameObject boulder = Instantiate(Boulder, spawnPos, Quaternion.identity);
    }

    public void UpdateScoreBoard(float timer)
    {
        int second = Mathf.FloorToInt(timer);
		timerText.text = "Time Elasped: " + second + "s";
        if (second <= 30)
            starRating.value = 1f;
        else if (second < 20)
            starRating.value = 0.7f;
        else if (second < 10)
            starRating.value = 0.4f;
        
	}
}

