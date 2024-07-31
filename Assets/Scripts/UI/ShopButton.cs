using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public int Amount;
    [SerializeField] private Button button;
    [SerializeField] private Text text;
    void FixedUpdate()
    {
        if (PlayerManager.instance.Players[0].GetComponent<Player>().Money >= Amount)
        {
            button.enabled = true;
            text.color = Color.white;
        }
        else
        {
            button.enabled = false;
            text.color = Color.red;
        }
    }
}
