using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //Needs to take in the global mode later
    public int PlayerNum = 1;
    public int doorNumber = 0;
    public RectTransform queuePrompt;
    List<Interaction> player = new List<Interaction>();
    bool promptedList = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void QueuePlayer(Interaction Player)
    {
        bool hasQueued = false;
        foreach(var checker in player)
        {
            if(checker == Player)
            {
                hasQueued = true;
            }
        }
        if(!hasQueued)
        {
            player.Add(Player);
        }
    }
    public void DequeuePlayer(Interaction Player)
    {
        foreach(var checker in player)
        {
            if(checker == Player)
            {
                player.Remove(Player);
            }
        }

    }

    public void Update()
    {
        if(player.Count == PlayerNum)
        {
            //Prompt the thing
            if(!promptedList)
            {
                DoorQueueManager.instance.UpdateQueueUI(doorNumber, player.Count, PlayerNum);
                promptedList = true;
            }
        }
        else
        {
            promptedList = false;
            DoorQueueManager.instance.UpdateQueueUI(doorNumber, player.Count, PlayerNum);
        }
    }

}
