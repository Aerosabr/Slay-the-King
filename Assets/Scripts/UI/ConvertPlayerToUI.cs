using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertPlayerToUI : MonoBehaviour
{
    public Transform player;
    public Transform storedPlayer;
    public int maxBodyParts;

    // Start is called before the first frame update
    private void OnEnable()
    {
        UpdatePlayerToUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerToUI()
    {
        if(player.childCount == 0)
            transform.gameObject.SetActive(false);
        else
        {
            transform.gameObject.SetActive(true);
            storedPlayer.GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetComponent<Animator>().runtimeAnimatorController;
            for(int i = 0; i < maxBodyParts; ++i)
            {
                storedPlayer.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController;
            }
        }
    }
}
