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
    void FixedUpdate()
    {
        UpdatePlayerToUI();
	}

    public void UpdatePlayerToUI()
    {
        if (player.childCount == 0)
            return;
        else
        {
            transform.gameObject.SetActive(true);
            storedPlayer.GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetComponent<Animator>().runtimeAnimatorController;
            for (int i = 0; i < maxBodyParts; ++i)
            {
                storedPlayer.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController;
            }

            if (player.GetChild(0).GetComponent<PlayerSpriteController>().twoHanded)
            {
                storedPlayer.GetComponent<Animator>().Play("2HIdleS");
                for (int i = 0; i < maxBodyParts; ++i)
                {
                    if (player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController != null)
                    {
                        storedPlayer.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController;
                        storedPlayer.GetChild(i).GetComponent<Animator>().Play("2HIdleS");
                    }
                }
            }
            else
            {
                storedPlayer.GetComponent<Animator>().Play("IdleS");
                for (int i = 0; i < maxBodyParts; ++i)
                {
                    if (player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController != null)
                    {
                        storedPlayer.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = player.GetChild(0).GetChild(i).GetComponent<Animator>().runtimeAnimatorController;
                        storedPlayer.GetChild(i).GetComponent<Animator>().Play("IdleS");
                    }
                }
            }
        }
	}
}
