using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public int TurnNum;
    public bool PlayerAction;
    public bool CurrentAction;
    public BattlePosition[,] PlayerPositions;
    public BattlePosition[,] EnemyPositions;
    public List<BattleAction> Actions = new List<BattleAction>();

    public bool temp;

    void Awake()
    {
        PlayerPositions = new BattlePosition[3, 3];
        EnemyPositions = new BattlePosition[3, 3];
        instance = this;
        TurnNum = 1;
        PlayerAction = true;
    }

    void Update()
    {
        if (Actions.Count == 4 && !temp)
        {
            StartCoroutine(ExecuteActions());
        }    
    }

    IEnumerator ExecuteActions()
    {
        temp = true;
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(2f);
            Actions.RemoveAt(0);
            ActionBar.instance.UpdateActionBar();
        }
        PlayerUI.instance.PlayersTurn();
        temp = false;
        /*foreach (BattleAction action in Actions)
        {
            yield return new WaitUntil(() => CurrentAction);
        }
        */
    }

}

public class BattleAction : MonoBehaviour
{
    public int PlayerNum;
    public string Action;

    public BattleAction(int playerNum, string action)
    {
        PlayerNum = playerNum;
        Action = action;
    }
}
