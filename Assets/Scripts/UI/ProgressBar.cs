using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider; //Part that moves across the progress bar
    public float FillSpeed = 0.01f; //Speed at which the progress bar moves
    private float targetProgress = 0; //Usually set to 1, basically max number of progress where it is completed
    private bool Running = false;
    //public GameObject floatingText; //This is where the animation for text spawning is held

    private void Awake()
    {
        
        
    }
    
    void Update()
    {
        if (Running)
        {
            if (slider.value < targetProgress)
                slider.value += FillSpeed * Time.deltaTime; //Every certain number of ticks moves the bar according to fill speed
            if (slider.value == 1)
            {
                //Resets slider progress
                targetProgress = 0;
                slider.value = 0;

                Running = false;
            }
        }
    }

    public void ProgressWithTime(int time) //Sets time bar will take to fill
    {
        slider = gameObject.GetComponent<Slider>();
        FillSpeed = (float) 1 / time;
        targetProgress = 1;
        slider.value = 0;
        Running = true;
    }

    public void StopProgress()
    {
        Running = false;
        targetProgress = 0;
        slider.value = 0;
    }
}
