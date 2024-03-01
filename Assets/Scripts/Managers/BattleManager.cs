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
    public ActionEnergy ActionEnergy;

    public List<GameObject> Enemies = new List<GameObject>();

    public bool temp;

    private void Awake()
    {
        PlayerPositions = new BattlePosition[3, 3];
        EnemyPositions = new BattlePosition[3, 3];
        instance = this;
        TurnNum = 1;
        PlayerAction = true;
        ActionEnergy = new ActionEnergy(6, 3);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                EnemyPositions[i, j] = new BattlePosition();
            }
        }
    }

    private void Start()
    {
        GenerateRandomEnemies();
    }

    void Update()
    {
        if (Actions.Count == 4 && !temp)
        {
            StartCoroutine(ExecuteActions());
        }    
    }

    public void GenerateRandomEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Random.Range(0, 3) == 1)
                {
                    int temp = (i * 3) + j;
                    EnableSprite(temp);
                    EnemyPositions[i, j].hasCharacter = true;
                }
                else
                    EnemyPositions[i, j].hasCharacter = false;
            }
        }
    }

    public void EnableSprite(int index)
    {
        Enemies[index].SetActive(true);
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


