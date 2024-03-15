using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorQueueManager : MonoBehaviour
{
    public static DoorQueueManager instance;

    public List<Transform> doors = new List<Transform>();
    public RectTransform doorParent;
    //Timer Variables
    public float timerDuration = 5f;
    private float elapsedTime = 0f;
    private bool timerActive = false;

    void Start()
    {
        instance = this;
    }

    public void UpdateQueueUI(int index, int playerCount, int maxPlayer)
    {
        doors[index].transform.GetChild(1).GetComponent<Text>().text = playerCount.ToString() + " / " + maxPlayer.ToString();
        if(playerCount == maxPlayer && !timerActive)
        {
            StartTimer();
            doors[index].transform.GetChild(2).GetComponent<ProgressBar>().ProgressWithTime(5);
        }
        else
        {
            ResetTimer();
            doors[index].transform.GetChild(2).GetComponent<ProgressBar>().StopProgress();
        }
    }
    void Update()
    {
        if (timerActive)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timerDuration)
            {

                // Optionally, perform actions when the timer reaches its duration
                // ...

                // Reset the timer
                ResetTimer();
            }
        }
    }
    public void StartTimer()
    {
        timerActive = true;
        elapsedTime = 0f;
    }
    public void ResetTimer()
    {
        timerActive = false;
        elapsedTime = 0f;
    }
}
