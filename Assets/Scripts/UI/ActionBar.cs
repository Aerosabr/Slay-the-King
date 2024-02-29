using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionBar : MonoBehaviour
{
    public static ActionBar instance;
    public List<GameObject> Actions = new List<GameObject>();

    void Awake()
    {
        instance = this;
        foreach (GameObject action in Actions)
        {
            action.SetActive(false);
        }
    }

    public void UpdateActionBar()
    {
        int num = 0;
        foreach (GameObject action in Actions)
        {
            if (num >= BattleManager.instance.Actions.Count)
                Actions[num].gameObject.SetActive(false);
            else
            {
                Actions[num].gameObject.SetActive(true);
                Actions[num].transform.GetChild(0).gameObject.GetComponent<Text>().text = 
                    "Player " + BattleManager.instance.Actions[num].PlayerNum + ": " + BattleManager.instance.Actions[num].Action;
            }
            num++;
        }
    }
}
